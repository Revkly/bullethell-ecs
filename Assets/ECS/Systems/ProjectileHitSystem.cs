using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

/// <summary>
/// Deteksi tabrakan projectile dengan enemy.
///
/// OPTIMASI: Sebelumnya ada dua masalah besar:
/// 1. HasComponent<KnifeTag>(projEntity) dipanggil di dalam loop O(n×m)
///    — ini structural query per entitas, sangat mahal.
/// 2. Nested loop proyektil × enemy = O(n²).
///
/// Fix:
/// - Pisahkan Knife dan non-Knife menjadi DUA loop query terpisah
///   menggunakan WithAll/WithNone. Unity akan filter di level archetype,
///   bukan per-entity di runtime.
/// - Tambahkan [BurstCompile] untuk eksekusi native.
///
/// NOTE: Deteksi tabrakan radius-based ini adalah pendekatan yang mudah
/// dan cukup untuk ECS tutorial/tugas akhir. Untuk produksi, gunakan
/// Unity Physics atau spatial hashing untuk performa optimal di 10k+ entity.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct ProjectileHitSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // ── LOOP 1: Knife projectile (pierce, tidak di-destroy saat hit) ──────
        foreach (var (projTransform, damage) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRO<ProjectileDamage>>()
            .WithAll<ProjectileTag, KnifeTag>()
            .WithNone<ProjectileHit>())
        {
            foreach (var (enemyTransform, enemyHealth) in
                SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRW<EnemyHealth>>()
                .WithAll<EnemyTag>()
                .WithNone<DeadTag>())
            {
                float dist = math.distance(
                    projTransform.ValueRO.Position,
                    enemyTransform.ValueRO.Position);

                if (dist < 0.5f)
                    enemyHealth.ValueRW.Value -= damage.ValueRO.Value;
                    // Knife tidak di-destroy — intentional pierce
            }
        }

        // ── LOOP 2: Projectile eksplosif (FireWand) ───────────────────────────
        foreach (var (projTransform, damage, projEntity) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRO<ProjectileDamage>>()
            .WithAll<ProjectileTag, ExplosionData>()
            .WithNone<KnifeTag, ProjectileHit>()
            .WithEntityAccess())
        {
            foreach (var (enemyTransform, enemyEntity) in
                SystemAPI.Query<RefRO<LocalTransform>>()
                .WithAll<EnemyTag>()
                .WithNone<DeadTag>()
                .WithEntityAccess())
            {
                float dist = math.distance(
                    projTransform.ValueRO.Position,
                    enemyTransform.ValueRO.Position);

                if (dist < 0.5f)
                {
                    // Tandai untuk diproses ExplosionSystem
                    ecb.AddComponent(projEntity, new ProjectileHit
                    {
                        HitEntity = enemyEntity
                    });
                    break;
                }
            }
        }

        // ── LOOP 3: Projectile normal (MagicWand, dll) — satu hit lalu destroy ─
        foreach (var (projTransform, damage, projEntity) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRO<ProjectileDamage>>()
            .WithAll<ProjectileTag>()
            .WithNone<KnifeTag, ExplosionData, ProjectileHit>()
            .WithEntityAccess())
        {
            foreach (var (enemyTransform, enemyHealth) in
                SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRW<EnemyHealth>>()
                .WithAll<EnemyTag>()
                .WithNone<DeadTag>())
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
    }
}