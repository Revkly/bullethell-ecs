using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;

/// <summary>
/// Mencegah enemy numpuk di satu titik dengan mendorong mereka
/// menjauh satu sama lain (separation/avoidance behavior).
///
/// Cara kerja:
/// - Setiap enemy cek semua enemy lain dalam radius separationRadius
/// - Jika terlalu dekat, enemy didorong menjauh
/// - Hasilnya enemy menyebar membentuk kerumunan alami
///
/// CATATAN: Sistem ini O(n²) — untuk jumlah enemy sangat besar (>2000)
/// pertimbangkan spatial hashing. Untuk tugas akhir dengan 1000-5000 enemy
/// ini sudah cukup karena [BurstCompile] membuatnya sangat cepat.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(EnemyMoveSystem))]
[BurstCompile]
public partial struct EnemySeparationSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Kumpulkan semua posisi enemy ke NativeArray dulu
        // agar bisa diakses paralel tanpa race condition
        var enemyQuery = SystemAPI.QueryBuilder()
            .WithAll<EnemyTag>()
            .WithNone<DeadTag>()
            .Build();

        int count = enemyQuery.CalculateEntityCount();
        if (count < 2) return;

        var positions = new NativeArray<float3>(count, Allocator.TempJob);

        // Isi positions dari query
        int idx = 0;
        foreach (var transform in
            SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<EnemyTag>()
                     .WithNone<DeadTag>())
        {
            positions[idx++] = transform.ValueRO.Position;
        }

        // Jalankan separation job secara paralel
        var job = new EnemySeparationJob
        {
            Positions        = positions,
            SeparationRadius = 1f,  // sesuaikan dengan ukuran enemy (scale 0.1)
            SeparationForce  = 2.5f,
            DeltaTime        = SystemAPI.Time.DeltaTime
        };

        var handle = job.ScheduleParallel(state.Dependency);
        state.Dependency = handle;

        // Dispose setelah job selesai
        positions.Dispose(state.Dependency);
    }
}

[BurstCompile]
public partial struct EnemySeparationJob : IJobEntity
{
    [ReadOnly] public NativeArray<float3> Positions;
    public float SeparationRadius;
    public float SeparationForce;
    public float DeltaTime;

    public void Execute(ref LocalTransform transform, in EnemyTag _, [EntityIndexInQuery] int index)
    {
        float3 push = float3.zero;

        for (int i = 0; i < Positions.Length; i++)
        {
            if (i == index) continue;

            float3 diff = transform.Position - Positions[i];
            float  dist = math.length(diff);

            if (dist < SeparationRadius && dist > 0.001f)
            {
                // Semakin dekat → semakin kuat dorongannya
                float strength = (SeparationRadius - dist) / SeparationRadius;
                push += math.normalize(diff) * strength;
            }
        }

        if (math.lengthsq(push) > 0.001f)
            transform.Position += math.normalize(push) * SeparationForce * DeltaTime;
    }
}