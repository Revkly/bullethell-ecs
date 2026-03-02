using Unity.Entities;
using UnityEngine;

public partial struct GameStartWeaponSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<WeaponPoolElement>();
        state.RequireForUpdate<WeaponPrefabElement>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var pool = SystemAPI.GetSingletonBuffer<WeaponPoolElement>();
        var registry = SystemAPI.GetSingletonBuffer<WeaponPrefabElement>();

        if (pool.Length == 0)
            return;

        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        var ecb =
            ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (slot, buffer, player) in
            SystemAPI.Query<
                RefRO<WeaponSlot>,
                DynamicBuffer<OwnedWeapon>>()
            .WithAll<PlayerTag>()
            .WithEntityAccess())
        {
            if (buffer.Length > 0)
                continue;

            int r = UnityEngine.Random.Range(0, pool.Length);
            WeaponType startWeapon = pool[r].Value;

            Entity weapon = ecb.CreateEntity();

            ecb.AddComponent<Weapon>(weapon);
            ecb.AddComponent(weapon, new WeaponOwner { Player = player });
            ecb.AddComponent(weapon, new WeaponLevel { Value = 1 });
            ecb.AddComponent(weapon, new WeaponCooldown { Value = 1f, Timer = 0f });
            ecb.AddComponent(weapon, new WeaponTypeComponent { Value = startWeapon });

            // 🔥 ATTACH PREFAB
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

            ecb.AppendToBuffer(player, new OwnedWeapon
            {
                WeaponEntity = weapon
            });
        }

        state.Enabled = false; // hanya jalan sekali
    }
}