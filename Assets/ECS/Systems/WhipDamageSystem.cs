using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

/// <summary>
/// Whip area damage — aktif selama 0.15 detik.
///
/// OPTIMASI:
/// - [BurstCompile]
/// - ECB Singleton sudah dipakai (sudah benar sebelumnya)
/// - Nested loop O(n²) ini tidak bisa dihindari sepenuhnya untuk AoE,
///   tapi dibatasi oleh lifetime pendek (0.15s) sehingga hanya jalan ~9 frame.
///   Untuk 5000 enemy ini masih bisa berat — pertimbangkan spatial hashing
///   jika perlu lebih dari 2000 enemy aktif.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct WhipDamageSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (hitbox, transform, entity) in
            SystemAPI.Query<
                RefRW<WhipHitbox>,
                RefRO<LocalTransform>>()
            .WithEntityAccess())
        {
            hitbox.ValueRW.Lifetime -= dt;

            foreach (var (enemyTransform, enemyHealth) in
                SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRW<EnemyHealth>>()
                .WithAll<EnemyTag>()
                .WithNone<DeadTag>())
            {
                float dist = math.distance(
                    transform.ValueRO.Position,
                    enemyTransform.ValueRO.Position);

                if (dist <= hitbox.ValueRO.Radius)
                    enemyHealth.ValueRW.Value -= hitbox.ValueRO.Damage;
            }

            if (hitbox.ValueRO.Lifetime <= 0f)
                ecb.DestroyEntity(entity);
        }
    }
}