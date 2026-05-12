using Unity.Entities;
using Unity.Burst;

/// <summary>
/// Update waktu game yang telah berjalan.
///
/// OPTIMASI: Pakai SetSingleton — lebih cache-friendly dari foreach query
/// saat hanya ada satu instance. [BurstCompile].
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct GameTimeSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<GameTime>())
            return;

        float dt      = SystemAPI.Time.DeltaTime;
        var   current = SystemAPI.GetSingleton<GameTime>();

        SystemAPI.SetSingleton(new GameTime
        {
            Elapsed = current.Elapsed + dt
        });
    }
}