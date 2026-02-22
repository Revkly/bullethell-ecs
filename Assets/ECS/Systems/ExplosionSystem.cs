using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct ExplosionSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        var ecb =
            ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (transform, damage, explosion, knockback, hit, entity) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRO<ProjectileDamage>,
                RefRO<ExplosionData>,
                RefRO<KnockbackData>,
                RefRO<ProjectileHit>>()
            .WithEntityAccess())
        {
            float3 pos = transform.ValueRO.Position;
            float radius = explosion.ValueRO.Radius;
            float dmg = damage.ValueRO.Value;
            float force = knockback.ValueRO.Force;

            foreach (var (enemyTransform, health, enemyEntity) in
                SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRW<EnemyHealth>>()
                .WithAll<EnemyTag>()
                .WithNone<DeadTag>()
                .WithEntityAccess())
            {
                float dist = math.distance(pos, enemyTransform.ValueRO.Position);

                if (dist <= radius)
                {
                    health.ValueRW.Value -= dmg;

                    float3 dir = math.normalize(
                        enemyTransform.ValueRO.Position - pos);

                    ecb.AddComponent(enemyEntity,
                        new EnemyKnockback
                        {
                            Direction = new float2(dir.x, dir.y),
                            Force = force,
                            Timer = 0.2f
                        });
                }
            }

            ecb.DestroyEntity(entity);
        }
    }
}