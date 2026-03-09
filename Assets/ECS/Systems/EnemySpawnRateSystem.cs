using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct EnemySpawnRateSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<GameTime>())
            return;

        float time = SystemAPI.GetSingleton<GameTime>().Elapsed;

        float difficulty = 1f + time * 0.02f;

        foreach (var spawner in SystemAPI.Query<RefRW<EnemySpawner>>())
        {
            float newInterval = math.max(0.35f, 2f / difficulty);

            spawner.ValueRW.SpawnInterval = newInterval;
        }
    }
}