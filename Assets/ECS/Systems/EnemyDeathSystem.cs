using Unity.Entities;

public partial struct EnemyDeathSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb =
            ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (health, entity) in
            SystemAPI.Query<RefRO<EnemyHealth>>()
                     .WithAll<EnemyTag>()
                     .WithNone<DeadTag>()
                     .WithEntityAccess())
        {
            if (health.ValueRO.Value <= 0f)
            {
                ecb.AddComponent<DeadTag>(entity);
            }
        }
    }
}
