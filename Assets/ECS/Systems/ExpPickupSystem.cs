using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

/// <summary>
/// Pickup XP gem saat player berada dalam radius.
///
/// OPTIMASI: ECB Singleton menggantikan Allocator.Temp.
/// [BurstCompile] untuk query yang lebih cepat.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct ExpPickupSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float3 playerPos = float3.zero;

        foreach (var t in
            SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<PlayerTag>())
        {
            playerPos = t.ValueRO.Position;
            break;
        }

        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // Cache ref PlayerExp agar tidak query di dalam loop gem
        RefRW<PlayerExp> playerExpRef = default;
        bool hasPlayer = false;

        foreach (var playerExp in SystemAPI.Query<RefRW<PlayerExp>>().WithAll<PlayerTag>())
        {
            playerExpRef = playerExp;
            hasPlayer = true;
            break;
        }

        if (!hasPlayer) return;

        foreach (var (transform, value, radius, entity) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRO<ExpValue>,
                RefRO<PickupRadius>>()
            .WithAll<ExpGem>()
            .WithEntityAccess())
        {
            float dist = math.distance(playerPos, transform.ValueRO.Position);

            if (dist <= radius.ValueRO.Value)
            {
                playerExpRef.ValueRW.Current += value.ValueRO.Value;
                ecb.DestroyEntity(entity);
            }
        }
    }
}