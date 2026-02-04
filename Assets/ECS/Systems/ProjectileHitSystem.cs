using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public partial struct ProjectileHitSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (projTransform, damage, projEntity) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRO<ProjectileDamage>>()
            .WithEntityAccess())
        {
            foreach (var (enemyTransform, enemyHealth, enemyEntity) in
                SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRW<EnemyHealth>>()
                .WithAll<EnemyTag>()
                .WithEntityAccess())
            {
                float dist = math.distance(
                    projTransform.ValueRO.Position,
                    enemyTransform.ValueRO.Position
                );

                if (dist < 0.5f)
                {
                    // Damage enemy
                    enemyHealth.ValueRW.Value -= damage.ValueRO.Value;

                    // Destroy projectile
                    ecb.DestroyEntity(projEntity);

                    break; // projectile hanya hit satu enemy
                }
            }
        }

        ecb.Playback(state.EntityManager);
    }
}
