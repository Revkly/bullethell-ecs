using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

/// <summary>
/// Gerakkan enemy yang sedang kena knockback, lalu hapus komponen saat selesai.
///
/// OPTIMASI: [BurstCompile] — ECB Singleton sudah benar sebelumnya.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct EnemyKnockbackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (transform, knockback, entity) in
            SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<EnemyKnockback>>()
            .WithEntityAccess())
        {
            transform.ValueRW.Position +=
                new float3(
                    knockback.ValueRO.Direction.x,
                    knockback.ValueRO.Direction.y,
                    0f) * knockback.ValueRO.Force * dt;

            knockback.ValueRW.Timer -= dt;

            if (knockback.ValueRO.Timer <= 0f)
                ecb.RemoveComponent<EnemyKnockback>(entity);
        }
    }
}