using Unity.Entities;

public struct EnemySpawner : IComponentData
{
    public Entity EnemyPrefab;
    public float  SpawnInterval;
    public float  Timer;
    /// <summary>
    /// Scale visual enemy — harus sama dengan Transform.Scale prefab di Unity.
    /// Diisi dari EnemySpawnerAuthoring.enemyScale.
    /// </summary>
    public float  EnemyScale;
}

public struct SpawnRequest : IComponentData
{
    public int   Total;
    public int   Spawned;
    public int   BatchSize;
    public float EnemyScale; // diteruskan dari EnemySpawner saat request dibuat
}