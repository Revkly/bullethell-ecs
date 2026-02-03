using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;

public partial struct EnemySpawnSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var spawner in
            SystemAPI.Query<RefRW<EnemySpawner>>())
        {
            spawner.ValueRW.Timer += dt;

            if (spawner.ValueRO.Timer >= spawner.ValueRO.SpawnInterval)
            {
                spawner.ValueRW.Timer = 0f;

                Entity enemy = ecb.Instantiate(spawner.ValueRO.EnemyPrefab);

                float2 randomPos = UnityEngine.Random.insideUnitCircle * 10f;

                ecb.SetComponent(enemy, new LocalTransform
                {
                    Position = new float3(randomPos.x, randomPos.y, 0),
                    Rotation = quaternion.identity,
                    Scale = 0.1f
                });
            }
        }

        ecb.Playback(state.EntityManager);
    }
}
