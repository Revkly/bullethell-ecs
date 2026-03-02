using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct KnifeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        var ecb =
            ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (cooldown, owner, level, prefab, type, weaponEntity) in
            SystemAPI.Query<
                RefRW<WeaponCooldown>,
                RefRO<WeaponOwner>,
                RefRO<WeaponLevel>,
                RefRO<WeaponProjectilePrefab>,
                RefRO<WeaponTypeComponent>>()
            .WithAll<Weapon>()
            .WithEntityAccess())
        {
            if (type.ValueRO.Value != WeaponType.Knife)
                continue;

            cooldown.ValueRW.Timer -= dt;
            if (cooldown.ValueRO.Timer > 0f)
                continue;

            cooldown.ValueRW.Timer = cooldown.ValueRO.Value;

            float3 playerPos =
                state.EntityManager
                .GetComponentData<LocalTransform>(owner.ValueRO.Player).Position;

            var facing =
                state.EntityManager
                .GetComponentData<PlayerFacing>(owner.ValueRO.Player);

            float2 baseDir = facing.Direction;

            if (math.lengthsq(baseDir) < 0.001f)
                continue;

            int count = level.ValueRO.Value;
            float spread = 15f * (count - 1);

            for (int i = 0; i < count; i++)
            {
                float offset = 0f;

                if (count > 1)
                {
                    float t = (float)i / (count - 1);
                    offset = math.lerp(-spread, spread, t);
                }

                float rad = math.radians(offset);

                float2 dir = new float2(
                    baseDir.x * math.cos(rad) - baseDir.y * math.sin(rad),
                    baseDir.x * math.sin(rad) + baseDir.y * math.cos(rad)
                );

                Entity proj = ecb.Instantiate(prefab.ValueRO.Value);

                ecb.SetComponent(proj, new LocalTransform
                {
                    Position = playerPos,
                    Rotation = quaternion.RotateZ(math.atan2(dir.y, dir.x)),
                    Scale = 1f
                });

                ecb.AddComponent(proj, new ProjectileData
                {
                    Speed = 12f,
                    Direction = dir
                });

                ecb.AddComponent(proj, new ProjectileDamage
                {
                    Value = 4f + level.ValueRO.Value * 2f
                });

                ecb.AddComponent(proj, new ProjectileLifetime
                {
                    Value = 0.6f
                });

                // 🔥 Tandai ini sebagai Knife projectile
                ecb.AddComponent<KnifeTag>(proj);
            }
        }
    }
}