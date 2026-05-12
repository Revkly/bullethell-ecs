using Unity.Entities;

/// <summary>
/// Terapkan pilihan upgrade: upgrade senjata yang sudah dimiliki, atau tambah senjata baru.
/// Berjalan hanya saat player memiliki SelectedUpgrade.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct WeaponUpgradeApplySystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WeaponPrefabElement>();
        state.RequireForUpdate<PlayerTag>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var registryBuffer = SystemAPI.GetSingletonBuffer<WeaponPrefabElement>();

        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (selected, slot, buffer, player) in
            SystemAPI.Query<
                RefRO<SelectedUpgrade>,
                RefRO<WeaponSlot>,
                DynamicBuffer<OwnedWeapon>>()
            .WithAll<PlayerTag>()
            .WithEntityAccess())
        {
            WeaponType chosen      = selected.ValueRO.Value;
            bool       alreadyOwned = false;

            foreach (var owned in buffer)
            {
                if (!state.EntityManager.Exists(owned.WeaponEntity)) continue;

                var type = state.EntityManager
                    .GetComponentData<WeaponTypeComponent>(owned.WeaponEntity);

                if (type.Value == chosen)
                {
                    var level = state.EntityManager
                        .GetComponentData<WeaponLevel>(owned.WeaponEntity);

                    if (level.Value < 3)
                    {
                        level.Value += 1;
                        state.EntityManager.SetComponentData(owned.WeaponEntity, level);
                    }

                    alreadyOwned = true;
                    break;
                }
            }

            if (!alreadyOwned && buffer.Length < slot.ValueRO.MaxSlot)
            {
                Entity weapon = ecb.CreateEntity();

                ecb.AddComponent<Weapon>(weapon);
                ecb.AddComponent(weapon, new WeaponOwner        { Player = player  });
                ecb.AddComponent(weapon, new WeaponLevel        { Value  = 1       });
                ecb.AddComponent(weapon, new WeaponCooldown     { Value  = 0.25f, Timer = 0f });
                ecb.AddComponent(weapon, new WeaponTypeComponent{ Value  = chosen  });

                foreach (var entry in registryBuffer)
                {
                    if (entry.Type == chosen)
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

            ecb.RemoveComponent<SelectedUpgrade>(player);
        }
    }
}