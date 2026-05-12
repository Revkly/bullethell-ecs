using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

/// <summary>
/// Menghitung enemy terdekat dari player SEKALI per frame dan menyimpannya
/// di singleton NearestEnemyCache.
///
/// Sebelumnya FireWandSystem dan MagicWandSystem masing-masing loop seluruh
/// enemy setiap kali mau menembak → O(n²) per weapon per fire.
/// Dengan sistem ini biayanya O(n) per frame, dipakai bersama oleh semua senjata.
///
/// Singleton NearestEnemyCache harus di-bake di scene (entity kosong +
/// NearestEnemyAuthoring) atau dibuat sekali di OnCreate.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(FireWandSystem))]
[UpdateBefore(typeof(MagicWandSystem))]
[BurstCompile]
public partial struct NearestEnemySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Buat singleton cache jika belum ada
        if (!SystemAPI.HasSingleton<NearestEnemyCache>())
        {
            var e = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(e, new NearestEnemyCache
            {
                Value    = Entity.Null,
                Position = float3.zero
            });
        }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Ambil posisi player
        float3 playerPos = float3.zero;
        bool   hasPlayer = false;

        foreach (var t in
            SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PlayerTag>())
        {
            playerPos = t.ValueRO.Position;
            hasPlayer = true;
            break;
        }

        if (!hasPlayer) return;

        // Cari enemy terdekat — O(n), sekali per frame
        Entity nearestEntity = Entity.Null;
        float3 nearestPos    = float3.zero;
        float  minDistSq     = float.MaxValue;

        foreach (var (transform, entity) in
            SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<EnemyTag>()
                     .WithNone<DeadTag>()
                     .WithEntityAccess())
        {
            float distSq = math.distancesq(playerPos, transform.ValueRO.Position);
            if (distSq < minDistSq)
            {
                minDistSq     = distSq;
                nearestEntity = entity;
                nearestPos    = transform.ValueRO.Position;
            }
        }

        // Tulis ke singleton
        SystemAPI.SetSingleton(new NearestEnemyCache
        {
            Value    = nearestEntity,
            Position = nearestPos
        });
    }
}