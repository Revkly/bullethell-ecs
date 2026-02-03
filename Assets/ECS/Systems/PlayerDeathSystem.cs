using Unity.Entities;
using UnityEngine;

public partial struct PlayerDeathSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var health in
            SystemAPI.Query<RefRO<PlayerHealth>>()
                     .WithAll<PlayerTag>())
        {
            if (health.ValueRO.Current <= 0f)
            {
                Debug.Log("GAME OVER");
                state.Enabled = false;
            }
        }
    }
}
