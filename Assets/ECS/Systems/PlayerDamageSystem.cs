using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

/// <summary>
/// Player menerima damage saat enemy menyentuhnya, dengan i-frame cooldown.
///
/// OPTIMASI: [BurstCompile]
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct PlayerDamageSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        float3 playerPos = float3.zero;

        foreach (var t in
            SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PlayerTag>())
        {
            playerPos = t.ValueRO.Position;
            break;
        }

        foreach (var (health, cooldown) in
            SystemAPI.Query<RefRW<PlayerHealth>, RefRW<PlayerDamageCooldown>>()
                     .WithAll<PlayerTag>())
        {
            if (cooldown.ValueRW.Timer > 0f)
            {
                cooldown.ValueRW.Timer -= dt;
                return; // masih i-frame
            }

            foreach (var enemyTransform in
                SystemAPI.Query<RefRO<LocalTransform>>()
                         .WithAll<EnemyTag>()
                         .WithNone<DeadTag>())
            {
                float dist = math.distance(playerPos, enemyTransform.ValueRO.Position);

                if (dist < 0.6f)
                {
                    health.ValueRW.Current    -= 10f;
                    cooldown.ValueRW.Timer     = cooldown.ValueRO.Cooldown;
                    break;
                }
            }
        }
    }
}