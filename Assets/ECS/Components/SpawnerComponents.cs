using Unity.Entities;

public struct EnemySpawner : IComponentData
{
    public Entity EnemyPrefab;
    public float SpawnInterval;
    public float Timer;
}

public struct SpawnRequest : IComponentData
{
    public int Count;
}