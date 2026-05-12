using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

/// <summary>
/// Spawn enemy berkala secara otomatis.
/// OPTIMASI: ECB Singleton + Unity.Mathematics.Random + spawn di luar layar.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct EnemySpawnSystem : ISystem
{
    private uint _seed;

    public void OnCreate(ref SystemState state)
    {
        _seed = 1;
        state.RequireForUpdate<EnemySpawner>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var spawner in SystemAPI.Query<RefRW<EnemySpawner>>())
        {
            spawner.ValueRW.Timer += dt;

            if (spawner.ValueRO.Timer < spawner.ValueRO.SpawnInterval) continue;

            spawner.ValueRW.Timer = 0f;

            var rng   = Unity.Mathematics.Random.CreateFromIndex(_seed++);
            float angle = rng.NextFloat(0f, math.PI2);
            float dist  = rng.NextFloat(12f, 18f);
            float2 pos  = new float2(math.cos(angle), math.sin(angle)) * dist;

            Entity enemy = ecb.Instantiate(spawner.ValueRO.EnemyPrefab);

            ecb.SetComponent(enemy, new LocalTransform
            {
                Position = new float3(pos.x, pos.y, 0f),
                Rotation = quaternion.identity,
                // Scale diambil dari Inspector — tidak hardcode di sini
                Scale    = spawner.ValueRO.EnemyScale
            });
        }
    }
}