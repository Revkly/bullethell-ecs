using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct PlayerDamageSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        // 1. Ambil posisi player
        float3 playerPos = float3.zero;

        foreach (var t in
            SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<PlayerTag>())
        {
            playerPos = t.ValueRO.Position;
            break;
        }

        // 2. Update damage cooldown + cek damage
        foreach (var (health, cooldown) in
            SystemAPI.Query<RefRW<PlayerHealth>, RefRW<PlayerDamageCooldown>>()
                     .WithAll<PlayerTag>())
        {
            // Kurangi timer
            if (cooldown.ValueRW.Timer > 0f)
                cooldown.ValueRW.Timer -= dt;

            // Masih i-frame → tidak bisa kena damage
            if (cooldown.ValueRO.Timer > 0f)
                return;

            // 3. Cek enemy terdekat
            foreach (var enemyTransform in
                SystemAPI.Query<RefRO<LocalTransform>>()
                         .WithAll<EnemyTag>())
            {
                float dist = math.distance(
                    playerPos,
                    enemyTransform.ValueRO.Position);

                if (dist < 0.6f)
                {
                    health.ValueRW.Current -= 10f;
                    cooldown.ValueRW.Timer = cooldown.ValueRO.Cooldown;
                    break;
                }
            }
        }
    }
}
