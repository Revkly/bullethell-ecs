using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public partial struct WeaponFireSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // float dt = SystemAPI.Time.DeltaTime;
        // var ecb = new EntityCommandBuffer(Allocator.Temp);

        // foreach (var (weapon, timer, transform) in
        //     SystemAPI.Query<
        //         RefRO<Weapon>,
        //         RefRW<WeaponTimer>,
        //         RefRO<LocalTransform>>()
        //     .WithAll<PlayerTag>())
        // {
        //     timer.ValueRW.Timer -= dt;

        //     if (timer.ValueRO.Timer > 0f)
        //         continue;

        //     timer.ValueRW.Timer = weapon.ValueRO.Cooldown;

        //     // Spawn projectile
        //     Entity projectile = ecb.Instantiate(
        //         SystemAPI.GetSingleton<ProjectilePrefab>().Value
        //     );

        //     ecb.SetComponent(projectile, new LocalTransform
        //     {
        //         Position = transform.ValueRO.Position,
        //         Rotation = quaternion.identity,
        //         // Scale = 1f
        //     });

        //     ecb.SetComponent(projectile, new ProjectileData
        //     {
        //         Speed = 8f,
        //         Damage = weapon.ValueRO.Damage,
        //         Direction = new float2(0, 1),
        //         Scale = 0.5f
        //     });
            
        //     ecb.AddComponent(projectile, new ProjectileDamage
        //     {
        //         Value = weapon.ValueRO.Damage
        //     });
        // }

        // ecb.Playback(state.EntityManager);
    }
}
