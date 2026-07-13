using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

/// <summary>
/// Senjata Whip: serangan area di sekeliling player.
///
/// BALANCE:
/// - Cooldown scale berdasarkan level (3.0s di lvl 1 → 1.5s di lvl 5+)
/// - Lifetime 0.4s agar visual jelas terlihat, bukan kedip
/// - Damage diterapkan 1x per aktivasi (lihat WhipDamageSystem + HasDamaged flag)
///
/// OPTIMASI:
/// - [BurstCompile]
/// - Spawn dari prefab (ecb.Instantiate) agar visual terlihat in-game
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct WhipSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
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
            if (type.ValueRO.Value != WeaponType.Whip)
                continue;

            cooldown.ValueRW.Timer -= dt;
            if (cooldown.ValueRO.Timer > 0f)
                continue;

            // BALANCE: Cooldown scale berdasarkan level
            // Lvl 1 = 1.0s, Lvl 2 = 0.8s, Lvl 3+ = 0.5s
            int   lvl         = level.ValueRO.Value;
            float baseCooldown = lvl >= 3 ? 0.5f : lvl >= 2 ? 0.8f : 1.0f;
            cooldown.ValueRW.Timer = baseCooldown;

            if (!state.EntityManager.Exists(owner.ValueRO.Player))
                continue;

            float3 playerPos =
                state.EntityManager
                    .GetComponentData<LocalTransform>(owner.ValueRO.Player)
                    .Position;

            // BALANCE: radius dan damage scale per level
            float radius   = 1.5f + lvl * 0.5f;
            float damage   = 20f  + lvl * 10f;
            float lifetime = 0.4f; // cukup lama agar visual jelas

            // Instantiate dari prefab agar visual (sprite) terlihat
            Entity hitbox = ecb.Instantiate(prefab.ValueRO.Value);

            // Baca scale asli prefab dari Inspector, kalikan dengan radius
            float prefabScale = state.EntityManager
                .GetComponentData<LocalTransform>(prefab.ValueRO.Value).Scale;

            ecb.SetComponent(hitbox, new LocalTransform
            {
                Position = playerPos,
                Rotation = quaternion.identity,
                Scale    = prefabScale * radius
            });

            ecb.AddComponent(hitbox, new WhipHitbox
            {
                Radius     = radius,
                Damage     = damage,
                Lifetime   = lifetime,
                HasDamaged = false
            });
        }
    }
}
