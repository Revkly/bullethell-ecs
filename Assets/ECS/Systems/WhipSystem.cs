using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct WhipSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb =
            ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (cooldown, owner, level, weaponEntity) in
            SystemAPI.Query<
                RefRW<WeaponCooldown>,
                RefRO<WeaponOwner>,
                RefRO<WeaponLevel>>()
            .WithAll<Weapon>()
            .WithEntityAccess())
        {
            var type = state.EntityManager
                .GetComponentData<WeaponTypeComponent>(weaponEntity);

            if (type.Value != WeaponType.Whip)
                continue;

            cooldown.ValueRW.Timer -= dt;
            if (cooldown.ValueRO.Timer > 0f)
                continue;

            cooldown.ValueRW.Timer = cooldown.ValueRO.Value;

            float3 playerPos =
                state.EntityManager.GetComponentData<LocalTransform>(
                    owner.ValueRO.Player).Position;

            // scaling
            float radius = 1.5f + level.ValueRO.Value * 0.5f;
            float damage = 10f + level.ValueRO.Value * 5f;

            Entity hitbox = ecb.CreateEntity();

            ecb.AddComponent(hitbox, new LocalTransform
            {
                Position = playerPos,
                Rotation = quaternion.identity,
                Scale = 1f
            });

            ecb.AddComponent(hitbox, new WhipHitbox
            {
                Radius = radius,
                Damage = damage,
                Lifetime = 0.15f
            });
        }
    }
}
    