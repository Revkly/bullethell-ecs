using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

/// <summary>
/// Senjata MagicWand: menembak ke enemy terdekat dengan proyektil lurus.
///
/// OPTIMASI: Pakai NearestEnemyCache — tidak ada loop enemy di sini.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct MagicWandSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<NearestEnemyCache>())
            return;

        var cache = SystemAPI.GetSingleton<NearestEnemyCache>();

        if (cache.Value == Entity.Null)
            return;

        float dt = SystemAPI.Time.DeltaTime;

        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (cooldown, owner, level, prefab, type) in
            SystemAPI.Query<
                RefRW<WeaponCooldown>,
                RefRO<WeaponOwner>,
                RefRO<WeaponLevel>,
                RefRO<WeaponProjectilePrefab>,
                RefRO<WeaponTypeComponent>>()
            .WithAll<Weapon>())
        {
            if (type.ValueRO.Value != WeaponType.MagicWand)
                continue;

            cooldown.ValueRW.Timer -= dt;
            if (cooldown.ValueRO.Timer > 0f)
                continue;

            cooldown.ValueRW.Timer = cooldown.ValueRO.Value;

            if (!state.EntityManager.Exists(owner.ValueRO.Player))
                continue;

            float3 playerPos =
                state.EntityManager
                    .GetComponentData<LocalTransform>(owner.ValueRO.Player)
                    .Position;

            float3 dir3 = math.normalizesafe(cache.Position - playerPos);
            float2 dir  = new float2(dir3.x, dir3.y);

            Entity proj = ecb.Instantiate(prefab.ValueRO.Value);

            ecb.SetComponent(proj, new LocalTransform
            {
                Position = playerPos,
                Rotation = quaternion.identity,
                Scale    = 0.1f
            });

            ecb.AddComponent(proj, new ProjectileData
            {
                Speed     = 8f,
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