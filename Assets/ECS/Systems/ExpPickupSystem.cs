using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public partial struct ExpPickupSystem : ISystem
{
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

        var ecb = new EntityCommandBuffer(Allocator.Temp);

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
                foreach (var playerExp in
                    SystemAPI.Query<RefRW<PlayerExp>>()
                             .WithAll<PlayerTag>())
                {
                    playerExp.ValueRW.Current += value.ValueRO.Value;
                }

                ecb.DestroyEntity(entity);
            }
        }

        ecb.Playback(state.EntityManager);
    }
}
