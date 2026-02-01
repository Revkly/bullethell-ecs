using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 1f;

    class Baker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring a)
        {
            Entity e = GetEntity(TransformUsageFlags.None);

            AddComponent(e, new EnemySpawner
            {
                EnemyPrefab = GetEntity(a.enemyPrefab, TransformUsageFlags.Dynamic),
                SpawnInterval = a.spawnInterval,
                Timer = 0f
            });
        }
    }
}
