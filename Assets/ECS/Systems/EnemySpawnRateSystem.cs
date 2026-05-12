using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;

/// <summary>
/// Menyesuaikan spawn interval seiring berjalannya waktu game.
///
/// OPTIMASI: Throttle setiap 2 detik — tidak perlu menghitung ulang
/// spawn interval 60x/detik karena nilainya berubah sangat lambat.
/// [BurstCompile] untuk eksekusi native.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct EnemySpawnRateSystem : ISystem
{
    private float _timer;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<GameTime>())
            return;

        _timer += SystemAPI.Time.DeltaTime;

        // Update hanya setiap 2 detik — nilai berubah sangat lambat
        if (_timer < 2f) return;
        _timer = 0f;

        float time       = SystemAPI.GetSingleton<GameTime>().Elapsed;
        float difficulty = 1f + time * 0.02f;
        float newInterval = math.max(0.35f, 2f / difficulty);

        foreach (var spawner in SystemAPI.Query<RefRW<EnemySpawner>>())
            spawner.ValueRW.SpawnInterval = newInterval;
    }
}