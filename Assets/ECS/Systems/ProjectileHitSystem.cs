using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial struct ProjectileHitSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        var ecb =
            ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (projTransform, damage, projEntity) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRO<ProjectileDamage>>()
            .WithAll<ProjectileTag>()
            .WithNone<ProjectileHit>()
            .WithEntityAccess())
        {
            foreach (var (enemyTransform, enemyHealth, enemyEntity) in
                SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRW<EnemyHealth>>()
                .WithAll<EnemyTag>()
                .WithNone<DeadTag>()
                .WithEntityAccess())
            {
                float dist = math.distance(
                    projTransform.ValueRO.Position,
                    enemyTransform.ValueRO.Position);

                if (dist < 0.5f)
                {
                    // Jika projectile tidak punya explosion → direct damage
                    if (!state.EntityManager.HasComponent<ExplosionData>(projEntity))
                    {
                        enemyHealth.ValueRW.Value -= damage.ValueRO.Value;
                        ecb.DestroyEntity(projEntity);
                    }
                    else
                    {
                        // Tandai untuk explosion system
                        ecb.AddComponent(projEntity, new ProjectileHit
                        {
                            HitEntity = enemyEntity
                        });
                    }

                    break;
                }
            }
        }
    }
}
