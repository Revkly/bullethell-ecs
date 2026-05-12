using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;

/// <summary>
/// Scale health & speed enemy berdasarkan waktu bermain.
///
/// OPTIMASI:
/// - [BurstCompile] untuk SIMD / JIT native code
/// - Hanya update enemy yang baru spawn (via InitializedTag trick) ATAU
///   cukup jalankan sekali per N detik, bukan setiap frame.
///   Implementasi ini menggunakan throttle 1 detik agar tidak
///   membebani setiap frame saat ada ribuan enemy.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct EnemyStatScaleSystem : ISystem
{
    private float _updateTimer;
    private const float UPDATE_INTERVAL = 1f; // update setiap 1 detik

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<GameTime>())
            return;

        float dt = SystemAPI.Time.DeltaTime;
        _updateTimer += dt;

        // ✅ OPTIMASI: Throttle — tidak perlu scale tiap frame
        if (_updateTimer < UPDATE_INTERVAL)
            return;

        _updateTimer = 0f;

        float time       = SystemAPI.GetSingleton<GameTime>().Elapsed;
        float difficulty = 1f + time * 0.015f;

        new EnemyStatScaleJob { Difficulty = difficulty }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct EnemyStatScaleJob : IJobEntity
{
    public float Difficulty;

    public void Execute(
        ref EnemyHealth  health,
        ref EnemyMove    move,
        in  EnemyBaseStats baseStats,
        in  EnemyTag     _)
    {
        // Update health hanya jika masih "fresh" (belum kena damage)
        if (math.abs(health.Value - baseStats.BaseHealth) < 0.01f)
            health.Value = baseStats.BaseHealth * Difficulty;

        move.Speed = baseStats.BaseSpeed * (1f + Difficulty * 0.2f);
    }
}