using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct MagicWandSystem : ISystem
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

            if (type.Value != WeaponType.MagicWand)
                continue;

            cooldown.ValueRW.Timer -= dt;
            if (cooldown.ValueRO.Timer > 0f)
                continue;

            cooldown.ValueRW.Timer = cooldown.ValueRO.Value;

            if (!state.EntityManager.Exists(owner.ValueRO.Player))
                continue;

            float3 playerPos =
                state.EntityManager.GetComponentData<LocalTransform>(
                    owner.ValueRO.Player).Position;

            // ===============================
            // CARI ENEMY TERDEKAT
            // ===============================

            Entity nearestEnemy = Entity.Null;
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
                    nearestEnemy = enemyEntity;
                }
            }

            if (nearestEnemy == Entity.Null)
                continue;

            float3 enemyPos =
                state.EntityManager.GetComponentData<LocalTransform>(
                    nearestEnemy).Position;

            float3 dir3 = math.normalize(enemyPos - playerPos);
            float2 dir = new float2(dir3.x, dir3.y);

            // ===============================
            // SPAWN PROJECTILE
            // ===============================

            Entity proj = ecb.Instantiate(
                SystemAPI.GetSingleton<ProjectilePrefab>().Value);

            ecb.SetComponent(proj, new LocalTransform
            {
                Position = playerPos,
                Rotation = quaternion.identity,
                Scale = 0.2f
            });

            ecb.AddComponent(proj, new ProjectileData
            {
                Speed = 8f,
                Direction = dir
            });

            ecb.AddComponent(proj, new ProjectileDamage
            {
                Value = 8f + level.ValueRO.Value * 2f
            });

            ecb.AddComponent(proj, new ProjectileLifetime
            {
                Value = 2f
            });
        }
    }
}
