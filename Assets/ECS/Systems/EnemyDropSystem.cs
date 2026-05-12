using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct EnemyDropSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<ExpGemPrefab>()) return;

        var gemPrefab = SystemAPI.GetSingleton<ExpGemPrefab>().Value;

        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (transform, entity) in
            SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<EnemyTag, DeadTag>()
                     .WithEntityAccess())
        {
            Entity xp = ecb.Instantiate(gemPrefab);
            ecb.SetComponent(xp, new LocalTransform
            {
                Position = transform.ValueRO.Position,
                Rotation = quaternion.identity,
                Scale    = 0.1f
            });
            ecb.DestroyEntity(entity);
        }
    }
}