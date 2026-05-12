using Unity.Entities;
using Unity.Burst;

/// <summary>
/// Hapus projectile yang lifetime-nya habis.
///
/// OPTIMASI: [BurstCompile] + ECB Singleton menggantikan Allocator.Temp.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct ProjectileLifetimeSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (life, entity) in
            SystemAPI.Query<RefRW<ProjectileLifetime>>()
                     .WithEntityAccess())
        {
            life.ValueRW.Value -= dt;
            if (life.ValueRO.Value <= 0f)
                ecb.DestroyEntity(entity);
        }
    }
}