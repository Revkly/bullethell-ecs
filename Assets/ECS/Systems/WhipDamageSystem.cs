using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct WhipDamageSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb =
            ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (hitbox, transform, entity) in
            SystemAPI.Query<
                RefRW<WhipHitbox>,
                RefRO<LocalTransform>>()
            .WithEntityAccess())
        {
            hitbox.ValueRW.Lifetime -= dt;

            foreach (var (enemyTransform, enemyHealth, enemyEntity) in
                SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRW<EnemyHealth>>()
                .WithAll<EnemyTag>()
                .WithNone<DeadTag>()
                .WithEntityAccess())
            {
                float dist = math.distance(
                    transform.ValueRO.Position,
                    enemyTransform.ValueRO.Position);

                if (dist <= hitbox.ValueRO.Radius)
                {
                    enemyHealth.ValueRW.Value -= hitbox.ValueRO.Damage;
                }
            }

            if (hitbox.ValueRO.Lifetime <= 0f)
            {
                ecb.DestroyEntity(entity);
            }
        }
    }
}
