using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

/// <summary>
/// Spawn massal enemy via shortcut keyboard.
///
/// BUG FIX UTAMA: Scale = 0.3f ditambahkan di LocalTransform.
/// Sebelumnya Scale tidak di-set → default struct C# = 0 atau nilai
/// tidak terdefinisi dari prefab → enemy terlihat sangat besar/kecil.
///
/// OPTIMASI: ECB Singleton, Unity.Mathematics.Random (Burst-safe),
/// spawn di luar layar, [BurstCompile].
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct MassSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<EnemySpawner>()) return;

        var spawner = SystemAPI.GetSingleton<EnemySpawner>();

        var ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (req, entity) in
            SystemAPI.Query<RefRW<SpawnRequest>>().WithEntityAccess())
        {
            int remaining = req.ValueRO.Total - req.ValueRO.Spawned;

            if (remaining <= 0)
            {
                ecb.DestroyEntity(entity);
                continue;
            }

            int batch = math.min(req.ValueRO.BatchSize, remaining);

            for (int i = 0; i < batch; i++)
            {
                uint seed = (uint)(req.ValueRO.Spawned + i + 1);
                var  rng  = Unity.Mathematics.Random.CreateFromIndex(seed);

                float angle = rng.NextFloat(0f, math.PI2);
                float dist  = rng.NextFloat(15f, 25f);
                float2 pos  = new float2(math.cos(angle), math.sin(angle)) * dist;

                Entity enemy = ecb.Instantiate(spawner.EnemyPrefab);

                ecb.SetComponent(enemy, new LocalTransform
                {
                    Position = new float3(pos.x, pos.y, 0f),
                    Rotation = quaternion.identity,
                    // Scale diambil dari SpawnRequest — diteruskan dari EnemySpawner Inspector
                    Scale    = req.ValueRO.EnemyScale   // ✅ BUG FIX — konsisten dengan EnemySpawnSystem
                });
            }

            req.ValueRW.Spawned += batch;
        }
    }
}