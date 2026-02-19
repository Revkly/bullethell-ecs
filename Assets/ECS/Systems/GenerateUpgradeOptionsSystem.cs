using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public partial struct GenerateUpgradeOptionsSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb =
            ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var pool = SystemAPI.GetSingletonBuffer<WeaponPoolElement>();

        foreach (var (slot, buffer, entity) in
            SystemAPI.Query<
                RefRO<WeaponSlot>,
                DynamicBuffer<OwnedWeapon>>()
            .WithAll<PlayerTag, LevelUpEvent>()
            .WithNone<PendingUpgrade>()
            .WithNone<SelectedUpgrade>()
            .WithEntityAccess())
        {
            var validWeapons = new NativeList<WeaponType>(Allocator.Temp);

            foreach (var w in pool)
            {
                bool owned = false;
                int currentLevel = 0;

                foreach (var ownedWeapon in buffer)
                {
                    if (!state.EntityManager.Exists(ownedWeapon.WeaponEntity))
                        continue;

                    var type = state.EntityManager
                        .GetComponentData<WeaponTypeComponent>(ownedWeapon.WeaponEntity);

                    if (type.Value == w.Value)
                    {
                        owned = true;
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

            // Shuffle
            for (int i = 0; i < validWeapons.Length; i++)
            {
                int rand = Random.Range(i, validWeapons.Length);
                var temp = validWeapons[i];
                validWeapons[i] = validWeapons[rand];
                validWeapons[rand] = temp;
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
