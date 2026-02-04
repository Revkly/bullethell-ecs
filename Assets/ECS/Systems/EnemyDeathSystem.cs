using Unity.Entities;
using Unity.Collections;

public partial struct EnemyDeathSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (health, entity) in
            SystemAPI.Query<RefRO<EnemyHealth>>()
                     .WithAll<EnemyTag>()
                     .WithEntityAccess())
        {
            if (health.ValueRO.Value <= 0f)
            {
                ecb.DestroyEntity(entity);
            }
        }

        ecb.Playback(state.EntityManager);
    }
}
