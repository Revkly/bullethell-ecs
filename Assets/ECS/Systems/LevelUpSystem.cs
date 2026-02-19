using Unity.Entities;
using UnityEngine;

public partial struct LevelUpSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb =
            ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (exp, level, nextExp, entity) in
            SystemAPI.Query<
                RefRW<PlayerExp>,
                RefRW<PlayerLevel>,
                RefRW<ExpToNextLevel>>()
            .WithAll<PlayerTag>()
            .WithNone<LevelUpEvent>()
            .WithEntityAccess())
        {
            if (exp.ValueRO.Current < nextExp.ValueRO.Value)
                continue;

            exp.ValueRW.Current -= nextExp.ValueRO.Value;
            level.ValueRW.Value += 1;
            nextExp.ValueRW.Value += 5;

            ecb.AddComponent<LevelUpEvent>(entity);

            Debug.Log($"LEVEL UP! Level {level.ValueRO.Value}");
        }
    }
}
