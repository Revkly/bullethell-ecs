using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct ProjectileMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (transform, projectile) in
            SystemAPI.Query<RefRW<LocalTransform>, RefRO<ProjectileData>>())
        {
            float3 move = new float3(
                projectile.ValueRO.Direction.x,
                projectile.ValueRO.Direction.y,
                0f);

            transform.ValueRW.Position += move * projectile.ValueRO.Speed * dt;
            transform.ValueRW.Scale = projectile.ValueRO.Scale;
        }
    }
}
