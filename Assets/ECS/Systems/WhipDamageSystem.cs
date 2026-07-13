using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

/// <summary>
/// Whip area damage — hit 1x per aktivasi, visual tetap tampil selama lifetime.
///
/// BALANCE:
/// - Damage hanya diterapkan 1x (frame pertama) menggunakan HasDamaged flag.
///   Sebelumnya damage diterapkan setiap frame selama 0.15s (~9 frame),
///   menghasilkan damage 9x lipat dari yang seharusnya.
/// - Lifetime sekarang 0.4s agar visual jelas terlihat (bukan kedip).
///
/// OPTIMASI:
/// - [BurstCompile]
/// - ECB Singleton untuk destroy entity
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

            // BALANCE: Hanya damage 1x per aktivasi
            if (!hitbox.ValueRO.HasDamaged)
            {
                hitbox.ValueRW.HasDamaged = true;

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
            }

            // Destroy setelah lifetime habis (visual sudah selesai tampil)
            if (hitbox.ValueRO.Lifetime <= 0f)
                ecb.DestroyEntity(entity);
        }
    }
}