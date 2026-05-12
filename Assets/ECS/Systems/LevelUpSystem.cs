using Unity.Entities;
using Unity.Burst;

/// <summary>
/// Level up player saat XP cukup.
///
/// OPTIMASI: [BurstCompile] — hilangkan Debug.Log di produksi
/// (Debug.Log tidak Burst-compatible dan sangat lambat jika sering dipanggil).
/// RequireForUpdate agar tidak jalan jika tidak ada player.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct LevelUpSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (exp, level, nextExp, entity) in
            SystemAPI.Query<
                RefRW<PlayerExp>,
                RefRW<PlayerLevel>,
                RefRW<ExpToNextLevel>>()
            .WithAll<PlayerTag>()
            .WithNone<LevelUpEvent>()
            .WithEntityAccess())
        {
            if (exp.ValueRO.Current < nextExp.ValueRO.Value)
                continue;

            exp.ValueRW.Current     -= nextExp.ValueRO.Value;
            level.ValueRW.Value     += 1;
            nextExp.ValueRW.Value   += 5;

            ecb.AddComponent<LevelUpEvent>(entity);
        }
    }
}