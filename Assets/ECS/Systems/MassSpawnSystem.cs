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
            SystemAPI.Query<RefRO<SpawnRequest>>().WithEntityAccess())
        {
            int count = req.ValueRO.Count;

            Debug.Log("Spawn request: " + count);

            for (int i = 0; i < count; i++)
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

            ecb.DestroyEntity(entity);
        }
    }
}