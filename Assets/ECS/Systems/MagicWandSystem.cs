using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public partial struct MagicWandSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Loop semua weapon entity bertipe MagicWand
        foreach (var (cooldown, owner, level, entity) in
            SystemAPI.Query<
                RefRW<WeaponCooldown>,
                RefRO<WeaponOwner>,
                RefRO<WeaponLevel>>()
            .WithAll<Weapon, WeaponTypeComponent>()
            .WithEntityAccess())
        {
            // cek type
            var type = state.EntityManager.GetComponentData<WeaponTypeComponent>(entity);
            if (type.Value != WeaponType.MagicWand)
                continue;

            // cooldown tick
            cooldown.ValueRW.Timer -= dt;
            if (cooldown.ValueRO.Timer > 0f)
                continue;

            // reset cooldown
            cooldown.ValueRW.Timer = cooldown.ValueRO.Value;

            // ambil posisi player
            if (!state.EntityManager.Exists(owner.ValueRO.Player))
                continue;

            float3 playerPos =
                state.EntityManager.GetComponentData<LocalTransform>(
                    owner.ValueRO.Player).Position;

            // cari enemy terdekat
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

            float3 dir = math.normalize(enemyPos - playerPos);

            // spawn projectile
            Entity proj = ecb.Instantiate(
                SystemAPI.GetSingleton<ProjectilePrefab>().Value
            );

            ecb.SetComponent(proj, new LocalTransform
            {
                Position = playerPos,
                Rotation = quaternion.identity,
                Scale = 0.2f
            });

            ecb.AddComponent(proj, new ProjectileData
            {
                Speed = 8f,
                Direction = new float2(dir.x, dir.y)
            });

            ecb.AddComponent(proj, new ProjectileDamage
            {
                Value = 5f + level.ValueRO.Value * 2f
            });

            ecb.AddComponent(proj, new ProjectileLifetime
            {
                Value = 2f
            });
        }

        ecb.Playback(state.EntityManager);
    }
}
