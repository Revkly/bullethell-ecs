using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial struct ProjectileMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (transform, data) in
            SystemAPI.Query<RefRW<LocalTransform>, RefRO<ProjectileData>>())
        {
            transform.ValueRW.Position +=
                new float3(data.ValueRO.Direction, 0f) * data.ValueRO.Speed * dt;
        }
    }
}
