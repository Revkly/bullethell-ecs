using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;

/// <summary>
/// Berikan senjata awal kepada player secara random saat game mulai.
///
/// OPTIMASI: Ganti UnityEngine.Random dengan Unity.Mathematics.Random
/// agar Burst-compatible. Sistem ini hanya berjalan SEKALI (state.Enabled = false).
/// </summary>
[BurstCompile]
public partial struct GameStartWeaponSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<WeaponPoolElement>();
        state.RequireForUpdate<WeaponPrefabElement>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var pool     = SystemAPI.GetSingletonBuffer<WeaponPoolElement>();
        var registry = SystemAPI.GetSingletonBuffer<WeaponPrefabElement>();

        if (pool.Length == 0) return;

        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // Unity.Mathematics.Random — Burst-safe
        var rng = Unity.Mathematics.Random.CreateFromIndex(
            (uint)SystemAPI.Time.ElapsedTime.GetHashCode());

        foreach (var (slot, buffer, player) in
            SystemAPI.Query<
                RefRO<WeaponSlot>,
                DynamicBuffer<OwnedWeapon>>()
            .WithAll<PlayerTag>()
            .WithEntityAccess())
        {
            if (buffer.Length > 0) continue;

            int        r           = rng.NextInt(0, pool.Length);
            WeaponType startWeapon = pool[r].Value;

            Entity weapon = ecb.CreateEntity();

            ecb.AddComponent<Weapon>(weapon);
            ecb.AddComponent(weapon, new WeaponOwner        { Player = player });
            ecb.AddComponent(weapon, new WeaponLevel        { Value  = 1      });
            ecb.AddComponent(weapon, new WeaponCooldown     { Value  = 1f, Timer = 0f });
            ecb.AddComponent(weapon, new WeaponTypeComponent{ Value  = startWeapon  });

            foreach (var entry in registry)
            {
                if (entry.Type == startWeapon)
                {
                    ecb.AddComponent(weapon, new WeaponProjectilePrefab
                    {
                        Value = entry.Prefab
                    });
                    break;
                }
            }

            ecb.AppendToBuffer(player, new OwnedWeapon { WeaponEntity = weapon });
        }

        state.Enabled = false; // hanya sekali
    }
}