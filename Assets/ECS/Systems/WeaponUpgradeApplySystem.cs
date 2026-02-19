using Unity.Entities;

public partial struct WeaponUpgradeApplySystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        var ecb =
            ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (selected, slot, buffer, player) in
            SystemAPI.Query<
                RefRO<SelectedUpgrade>,
                RefRO<WeaponSlot>,
                DynamicBuffer<OwnedWeapon>>()
            .WithAll<PlayerTag>()
            .WithEntityAccess())
        {
            WeaponType chosen = selected.ValueRO.Value;

            bool alreadyOwned = false;

            foreach (var owned in buffer)
            {
                if (!state.EntityManager.Exists(owned.WeaponEntity))
                    continue;

                var type = state.EntityManager
                    .GetComponentData<WeaponTypeComponent>(owned.WeaponEntity);

                if (type.Value == chosen)
                {
                    var level = state.EntityManager
                        .GetComponentData<WeaponLevel>(owned.WeaponEntity);

                    if (level.Value < 3)
                    {
                        level.Value += 1;
                        state.EntityManager
                            .SetComponentData(owned.WeaponEntity, level);
                    }

                    alreadyOwned = true;
                    break;
                }
            }

            // Jika belum punya dan slot masih ada → buat weapon baru
            if (!alreadyOwned && buffer.Length < slot.ValueRO.MaxSlot)
            {
                Entity weapon = ecb.CreateEntity();

                ecb.AddComponent<Weapon>(weapon);
                ecb.AddComponent(weapon, new WeaponOwner { Player = player });
                ecb.AddComponent(weapon, new WeaponLevel { Value = 1 });
                ecb.AddComponent(weapon, new WeaponCooldown
                {
                    Value = 1f,
                    Timer = 0f
                });
                ecb.AddComponent(weapon, new WeaponTypeComponent
                {
                    Value = chosen
                });

                ecb.AppendToBuffer(player, new OwnedWeapon
                {
                    WeaponEntity = weapon
                });
            }

            ecb.RemoveComponent<SelectedUpgrade>(player);
        }
    }
}
