using Unity.Entities;
using Unity.Mathematics;

public partial struct EnemyStatScaleSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<GameTime>())
            return;

        float time = SystemAPI.GetSingleton<GameTime>().Elapsed;

        float difficulty = 1f + time * 0.015f;

        foreach (var (health, move, baseStats) in
            SystemAPI.Query<
                RefRW<EnemyHealth>,
                RefRW<EnemyMove>,
                RefRO<EnemyBaseStats>>()
            .WithAll<EnemyTag>()
            .WithNone<DeadTag>())
        {
            health.ValueRW.Value = baseStats.ValueRO.BaseHealth * difficulty;

            move.ValueRW.Speed = baseStats.ValueRO.BaseSpeed * (1f + difficulty * 0.2f);
        }
    }
}