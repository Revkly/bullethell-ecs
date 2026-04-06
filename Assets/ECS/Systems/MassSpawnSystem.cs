using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct MassSpawnSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<EnemySpawner>())
            return;

        var spawner = SystemAPI.GetSingleton<EnemySpawner>();

        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (req, entity) in
            SystemAPI.Query<RefRW<SpawnRequest>>().WithEntityAccess())
        {
            int remaining = req.ValueRO.Total - req.ValueRO.Spawned;

            if (remaining <= 0)
            {
                Debug.Log("Spawn selesai");
                ecb.DestroyEntity(entity);
                continue;
            }

            int batch = math.min(req.ValueRO.BatchSize, remaining);

            for (int i = 0; i < batch; i++)
            {
                float2 pos = UnityEngine.Random.insideUnitCircle * 20f;

                Entity enemy = ecb.Instantiate(spawner.EnemyPrefab);

                ecb.SetComponent(enemy, new LocalTransform
                {
                    Position = new float3(pos.x, pos.y, 0),
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
            }

            req.ValueRW.Spawned += batch;

            // debug progress
            Debug.Log($"Spawn progress: {req.ValueRW.Spawned}/{req.ValueRO.Total}");
        }
    }
}