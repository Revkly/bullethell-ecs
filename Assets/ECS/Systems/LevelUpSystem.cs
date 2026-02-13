using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public partial struct LevelUpSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (exp, level, nextExp, entity) in
            SystemAPI.Query<
                RefRW<PlayerExp>,
                RefRW<PlayerLevel>,
                RefRW<ExpToNextLevel>>()
            .WithAll<PlayerTag>()
            .WithNone<PendingUpgrade>()
            .WithEntityAccess())
        {
            if (exp.ValueRO.Current < nextExp.ValueRO.Value)
                continue;

            exp.ValueRW.Current -= nextExp.ValueRO.Value;
            level.ValueRW.Value += 1;
            nextExp.ValueRW.Value += 5;

            // Cukup tandai bahwa player siap generate upgrade
            ecb.AddComponent<PendingUpgrade>(entity);

            Debug.Log($"LEVEL UP! Level {level.ValueRO.Value}");
        }

        ecb.Playback(state.EntityManager);
    }
}
