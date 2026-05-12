using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

/// <summary>
/// Saat player level up, hasilkan 3 pilihan upgrade acak.
///
/// OPTIMASI:
/// - Ganti UnityEngine.Random.Range dengan Unity.Mathematics.Random
///   berbasis seed dari ElapsedTime + level — deterministik dan Burst-safe.
/// - NativeList tetap dengan Allocator.Temp — sesuai karena dibuat dan
///   dibuang dalam satu frame.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct GenerateUpgradeOptionsSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var pool = SystemAPI.GetSingletonBuffer<WeaponPoolElement>();

        foreach (var (slot, buffer, level, entity) in
            SystemAPI.Query<
                RefRO<WeaponSlot>,
                DynamicBuffer<OwnedWeapon>,
                RefRO<PlayerLevel>>()
            .WithAll<PlayerTag, LevelUpEvent>()
            .WithNone<PendingUpgrade, SelectedUpgrade>()
            .WithEntityAccess())
        {
            var validWeapons = new NativeList<WeaponType>(Allocator.Temp);

            foreach (var w in pool)
            {
                bool owned        = false;
                int  currentLevel = 0;

                foreach (var ownedWeapon in buffer)
                {
                    if (!state.EntityManager.Exists(ownedWeapon.WeaponEntity)) continue;

                    var type = state.EntityManager
                        .GetComponentData<WeaponTypeComponent>(ownedWeapon.WeaponEntity);

                    if (type.Value == w.Value)
                    {
                        owned        = true;
                        currentLevel = state.EntityManager
                            .GetComponentData<WeaponLevel>(ownedWeapon.WeaponEntity).Value;
                        break;
                    }
                }

                if (owned && currentLevel < 3)
                    validWeapons.Add(w.Value);
                else if (!owned && buffer.Length < slot.ValueRO.MaxSlot)
                    validWeapons.Add(w.Value);
            }

            if (validWeapons.Length == 0)
            {
                validWeapons.Dispose();
                ecb.RemoveComponent<LevelUpEvent>(entity);
                continue;
            }

            // Shuffle dengan seed deterministik
            var rng = Unity.Mathematics.Random.CreateFromIndex(
                (uint)(level.ValueRO.Value * 1000 + (int)(SystemAPI.Time.ElapsedTime * 100)));

            for (int i = validWeapons.Length - 1; i > 0; i--)
            {
                int j    = rng.NextInt(0, i + 1);
                var temp = validWeapons[i];
                validWeapons[i] = validWeapons[j];
                validWeapons[j] = temp;
            }

            WeaponType a = validWeapons[0];
            WeaponType b = validWeapons.Length > 1 ? validWeapons[1] : validWeapons[0];
            WeaponType c = validWeapons.Length > 2 ? validWeapons[2] : validWeapons[0];

            ecb.AddComponent(entity, new PendingUpgrade
            {
                OptionA = a,
                OptionB = b,
                OptionC = c
            });

            ecb.RemoveComponent<LevelUpEvent>(entity);
            validWeapons.Dispose();
        }
    }
}