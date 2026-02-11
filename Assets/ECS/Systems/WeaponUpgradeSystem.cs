using Unity.Entities;
using Unity.Collections;

public partial struct WeaponUpgradeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (upgrade, slot, buffer, player) in
            SystemAPI.Query<
                RefRO<PendingUpgrade>,
                RefRO<WeaponSlot>,
                DynamicBuffer<OwnedWeapon>>()
            .WithAll<PlayerTag>()
            .WithEntityAccess())
        {
            WeaponType chosen = WeaponType.MagicWand; // nanti ganti pakai pilihan UI

            bool alreadyOwned = false;

            foreach (var owned in buffer)
            {
                var type = state.EntityManager
                    .GetComponentData<WeaponTypeComponent>(owned.WeaponEntity);

                if (type.Value == chosen)
                {
                    var level = state.EntityManager
                        .GetComponentData<WeaponLevel>(owned.WeaponEntity);

                    level.Value += 1;
                    state.EntityManager
                        .SetComponentData(owned.WeaponEntity, level);

                    alreadyOwned = true;
                    break;
                }
            }

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

                buffer.Add(new OwnedWeapon
                {
                    WeaponEntity = weapon
                });
            }

            ecb.RemoveComponent<PendingUpgrade>(player);
        }

        ecb.Playback(state.EntityManager);
    }
}
