using UnityEngine;
using Unity.Entities;

/// <summary>
/// Shortcut keyboard untuk spawn massal enemy (testing/demo).
/// 1 → 100 enemy, 2 → 1000 enemy, 3 → 5000 enemy.
///
/// EnemyScale dibaca otomatis dari EnemySpawner singleton —
/// tidak perlu hardcode di sini.
/// </summary>
public class SpawnShortcutInput : MonoBehaviour
{
    [SerializeField] private int batchSize = 50;

    private EntityManager _em;

    void Start()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) CreateRequest(100);
        if (Input.GetKeyDown(KeyCode.Alpha2)) CreateRequest(500);
        if (Input.GetKeyDown(KeyCode.Alpha3)) CreateRequest(1000);
    }

    void CreateRequest(int count)
    {
        // Baca EnemyScale dari spawner agar konsisten
        float enemyScale = 0.1f;
        var spawnerQuery = _em.CreateEntityQuery(typeof(EnemySpawner));
        if (spawnerQuery.CalculateEntityCount() > 0)
        {
            var spawner = spawnerQuery.GetSingleton<EnemySpawner>();
            enemyScale  = spawner.EnemyScale;
        }
        spawnerQuery.Dispose();

        Entity e = _em.CreateEntity();
        _em.AddComponentData(e, new SpawnRequest
        {
            Total      = count,
            Spawned    = 0,
            BatchSize  = batchSize,
            EnemyScale = enemyScale
        });
    }
}