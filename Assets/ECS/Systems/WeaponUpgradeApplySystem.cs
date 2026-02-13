using Unity.Entities;
using Unity.Collections;

public partial struct WeaponUpgradeApplySystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (selected, slot, buffer, entity) in
            SystemAPI.Query<
                RefRO<SelectedUpgrade>,
                RefRO<WeaponSlot>,
                DynamicBuffer<OwnedWeapon>>()
            .WithAll<PlayerTag>()
            .WithEntityAccess())
        {
            WeaponType chosen = selected.ValueRO.Value;
            bool found = false;

            foreach (var owned in buffer)
            {
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

                    found = true;
                    break;
                }
            }

            if (!found && buffer.Length < slot.ValueRO.MaxSlot)
            {
                Entity weapon = ecb.CreateEntity();

                ecb.AddComponent<Weapon>(weapon);
                ecb.AddComponent(weapon, new WeaponOwner { Player = entity });
                ecb.AddComponent(weapon, new WeaponLevel { Value = 1 });
                ecb.AddComponent(weapon, new WeaponCooldown { Value = 1f, Timer = 0f });
                ecb.AddComponent(weapon, new WeaponTypeComponent { Value = chosen });

                buffer.Add(new OwnedWeapon { WeaponEntity = weapon });
            }

            ecb.RemoveComponent<SelectedUpgrade>(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}
