using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public partial struct EnemyDropSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (transform, entity) in
            SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<EnemyTag, DeadTag>()
                     .WithEntityAccess())
        {
            // Spawn XP
            Entity xp = ecb.Instantiate(
                SystemAPI.GetSingleton<ExpGemPrefab>().Value
            );

            ecb.SetComponent(xp, new LocalTransform
            {
                Position = transform.ValueRO.Position,
                Rotation = quaternion.identity,
                Scale = 0.3f
            });

            // Hapus enemy SETELAH XP spawn
            ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}
