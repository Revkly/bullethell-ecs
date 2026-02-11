using Unity.Entities;
using Unity.Collections;

public partial struct ProjectileLifetimeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (life, entity) in
            SystemAPI.Query<RefRW<ProjectileLifetime>>()
                     .WithEntityAccess())
        {
            life.ValueRW.Value -= dt;
            if (life.ValueRO.Value <= 0f)
                ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}
