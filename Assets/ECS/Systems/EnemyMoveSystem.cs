using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct EnemyMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        float3 playerPos = float3.zero;

        foreach (var t in
            SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<PlayerTag>())
        {
            playerPos = t.ValueRO.Position;
            break;
        }

        foreach (var (transform, move) in
            SystemAPI.Query<RefRW<LocalTransform>, RefRO<EnemyMove>>()
                     .WithAll<EnemyTag>())
        {
            float3 dir = math.normalize(playerPos - transform.ValueRO.Position);
            transform.ValueRW.Position += dir * move.ValueRO.Speed * dt;
        }
    }
}
