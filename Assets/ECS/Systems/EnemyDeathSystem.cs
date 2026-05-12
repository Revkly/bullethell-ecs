using Unity.Entities;
using Unity.Burst;

/// <summary>
/// Tandai enemy yang HP-nya habis dengan DeadTag.
///
/// OPTIMASI: [BurstCompile] untuk eksekusi lebih cepat.
/// ECB Singleton menghindari alokasi per-frame.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct EnemyDeathSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (health, entity) in
            SystemAPI.Query<RefRO<EnemyHealth>>()
                     .WithAll<EnemyTag>()
                     .WithNone<DeadTag>()
                     .WithEntityAccess())
        {
            if (health.ValueRO.Value <= 0f)
                ecb.AddComponent<DeadTag>(entity);
        }
    }
}