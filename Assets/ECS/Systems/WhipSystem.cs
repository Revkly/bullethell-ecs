using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

/// <summary>
/// Senjata Whip: serangan area di sekeliling player.
///
/// OPTIMASI:
/// - Hapus state.EntityManager.GetComponentData di dalam foreach loop.
///   Ganti dengan query filter WithAll + komponen langsung di query.
/// - [BurstCompile]
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

        // Query langsung sertakan WeaponTypeComponent — tidak perlu GetComponentData
        foreach (var (cooldown, owner, level, type) in
            SystemAPI.Query<
                RefRW<WeaponCooldown>,
                RefRO<WeaponOwner>,
                RefRO<WeaponLevel>,
                RefRO<WeaponTypeComponent>>()
            .WithAll<Weapon>())
        {
            if (type.ValueRO.Value != WeaponType.Whip)
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

            float radius = 1.5f + level.ValueRO.Value * 0.5f;
            float damage = 10f  + level.ValueRO.Value * 5f;

            Entity hitbox = ecb.CreateEntity();

            ecb.AddComponent(hitbox, new LocalTransform
            {
                Position = playerPos,
                Rotation = quaternion.identity,
                Scale    = 0.1f
            });

            ecb.AddComponent(hitbox, new WhipHitbox
            {
                Radius   = radius,
                Damage   = damage,
                Lifetime = 0.15f
            });
        }
    }
}