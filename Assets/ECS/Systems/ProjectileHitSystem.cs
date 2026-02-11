using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
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
            .WithAll<ProjectileTag>()
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
                    enemyHealth.ValueRW.Value -= damage.ValueRO.Value;
                    ecb.DestroyEntity(projEntity);
                    break;
                }
            }
        }

        ecb.Playback(state.EntityManager);
    }
}
