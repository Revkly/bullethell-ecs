using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

/// <summary>
/// Gerakkan semua enemy ke arah player.
///
/// OPTIMASI: Menggunakan IJobEntity + [BurstCompile] sehingga loop
/// dijalankan oleh Job System (multi-thread, SIMD). Jauh lebih cepat
/// saat ada ribuan enemy.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct EnemyMoveSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float3 playerPos = float3.zero;

        foreach (var t in
            SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<PlayerTag>())
        {
            playerPos = t.ValueRO.Position;
            break;
        }

        float dt = SystemAPI.Time.DeltaTime;

        // ✅ OPTIMASI: IJobEntity — dieksekusi paralel oleh Unity Job System
        new EnemyMoveJob
        {
            PlayerPos = playerPos,
            DeltaTime = dt
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct EnemyMoveJob : IJobEntity
{
    public float3 PlayerPos;
    public float  DeltaTime;

    public void Execute(ref LocalTransform transform, in EnemyMove move,
                        in EnemyTag _)
    {
        float3 dir = math.normalizesafe(PlayerPos - transform.Position);
        transform.Position += dir * move.Speed * DeltaTime;
    }
}