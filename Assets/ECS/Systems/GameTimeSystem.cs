using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct GameTimeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var time in SystemAPI.Query<RefRW<GameTime>>())
        {
            time.ValueRW.Elapsed += dt;
        }
    }
}