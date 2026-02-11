using Unity.Entities;
using Unity.Collections;

public partial struct UpgradeApplySystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (upgrade, entity) in
            SystemAPI.Query<RefRO<PendingUpgrade>>()
                     .WithAll<PlayerTag>()
                     .WithEntityAccess())
        {
            // ApplyUpgrade(upgrade.ValueRO.OptionA, entity, ref state);

            // ❗ pakai ECB
            ecb.RemoveComponent<PendingUpgrade>(entity);
        }

        ecb.Playback(state.EntityManager);
    }

    // void ApplyUpgrade(UpgradeType type, Entity player, ref SystemState state)
    // {
    //     switch (type)
    //     {
    //         case UpgradeType.DamageUp:
    //             var weapon = state.EntityManager.GetComponentData<Weapon>(player);
    //             weapon.Damage += 5f;
    //             state.EntityManager.SetComponentData(player, weapon);
    //             break;

    //         case UpgradeType.AttackSpeedUp:
    //             var weapon2 = state.EntityManager.GetComponentData<Weapon>(player);
    //             weapon2.Cooldown *= 0.85f;
    //             state.EntityManager.SetComponentData(player, weapon2);
    //             break;

    //         case UpgradeType.MoveSpeedUp:
    //             var move = state.EntityManager.GetComponentData<MoveSpeed>(player);
    //             move.Value += 1f;
    //             state.EntityManager.SetComponentData(player, move);
    //             break;
    //     }
    // }
}
