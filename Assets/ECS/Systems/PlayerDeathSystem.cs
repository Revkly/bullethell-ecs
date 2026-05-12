using Unity.Entities;
using Unity.Burst;

/// <summary>
/// Deteksi kematian player dan hentikan semua sistem.
///
/// OPTIMASI: [BurstCompile] + RequireForUpdate agar tidak jalan
/// sama sekali jika tidak ada PlayerTag di dunia.
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct PlayerDeathSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var health in
            SystemAPI.Query<RefRO<PlayerHealth>>().WithAll<PlayerTag>())
        {
            if (health.ValueRO.Current <= 0f)
            {
                // Semua sistem ECS berhenti — scene reload ditangani MonoBehaviour
                state.Enabled = false;
            }
        }
    }
}