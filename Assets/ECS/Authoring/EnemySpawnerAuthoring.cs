using Unity.Entities;
using UnityEngine;

/// <summary>
/// Authoring untuk spawner enemy.
///
/// enemyScale → isi sesuai Transform.Scale prefab enemy di Unity Inspector.
/// Nilai ini dipakai oleh EnemySpawnSystem dan MassSpawnSystem saat
/// ecb.SetComponent(LocalTransform) agar scale konsisten dengan prefab.
/// </summary>
public class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float      spawnInterval = 1f;

    [Tooltip("Harus sama dengan Transform Scale prefab enemy di scene/inspector")]
    public float      enemyScale    = 0.1f;

    class Baker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring a)
        {
            Entity e = GetEntity(TransformUsageFlags.None);

            AddComponent(e, new EnemySpawner
            {
                EnemyPrefab   = GetEntity(a.enemyPrefab, TransformUsageFlags.Dynamic),
                SpawnInterval = a.spawnInterval,
                Timer         = 0f,
                EnemyScale    = a.enemyScale
            });
        }
    }
}