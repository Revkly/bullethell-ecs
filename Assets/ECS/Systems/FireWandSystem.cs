using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct FireWandSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb =
            ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (cooldown, owner, level, entity) in
            SystemAPI.Query<
                RefRW<WeaponCooldown>,
                RefRO<WeaponOwner>,
                RefRO<WeaponLevel>>()
            .WithAll<Weapon>()
            .WithEntityAccess())
        {
            var type = state.EntityManager
                .GetComponentData<WeaponTypeComponent>(entity);

            if (type.Value != WeaponType.FireWand)
                continue;

            cooldown.ValueRW.Timer -= dt;
            if (cooldown.ValueRO.Timer > 0f)
                continue;

            cooldown.ValueRW.Timer = cooldown.ValueRO.Value;

            if (!state.EntityManager.Exists(owner.ValueRO.Player))
                continue;

            float3 playerPos =
                state.EntityManager
                .GetComponentData<LocalTransform>(
                    owner.ValueRO.Player).Position;

            Entity nearest = Entity.Null;
            float minDist = float.MaxValue;

            foreach (var (enemyTransform, enemyEntity) in
                SystemAPI.Query<RefRO<LocalTransform>>()
                .WithAll<EnemyTag>()
                .WithNone<DeadTag>()
                .WithEntityAccess())
            {
                float dist = math.distance(playerPos, enemyTransform.ValueRO.Position);

                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = enemyEntity;
                }
            }

            if (nearest == Entity.Null)
                continue;

            float3 enemyPos =
                state.EntityManager
                .GetComponentData<LocalTransform>(nearest).Position;

            float3 dir = math.normalize(enemyPos - playerPos);

            Entity proj = ecb.Instantiate(
                SystemAPI.GetSingleton<ProjectilePrefab>().Value
            );

            ecb.SetComponent(proj, new LocalTransform
            {
                Position = playerPos,
                Rotation = quaternion.identity,
                Scale = 0.3f
            });

            ecb.AddComponent(proj, new ProjectileData
            {
                Speed = 6f,
                Direction = new float2(dir.x, dir.y)
            });

            ecb.AddComponent(proj, new ProjectileDamage
            {
                Value = 8f + level.ValueRO.Value * 4f
            });

            ecb.AddComponent(proj, new ProjectileLifetime
            {
                Value = 2.5f
            });

            ecb.AddComponent(proj, new ExplosionData
            {
                Radius = 1.5f + level.ValueRO.Value * 0.5f
            });

            ecb.AddComponent(proj, new KnockbackData
            {
                Force = 4f + level.ValueRO.Value * 1.5f
            });
        }
    }
}
