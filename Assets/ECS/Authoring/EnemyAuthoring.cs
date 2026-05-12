using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Authoring untuk Enemy prefab.
///
/// FIX: Scale di-bake langsung ke LocalTransform dari nilai Transform
/// GameObject di scene/prefab. Ini memastikan scale yang di-bake
/// konsisten dengan apa yang terlihat di Inspector (0.1, 0.1, 0.1).
///
/// EnemySpawnSystem dan MassSpawnSystem kemudian pakai EnemyScale
/// dari EnemySpawner (juga 0.1) agar konsisten saat spawn runtime.
/// </summary>
public class EnemyAuthoring : MonoBehaviour
{
    [Header("Stats")]
    public float speed  = 2f;
    public float health = 10f;

    class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring a)
        {
            Entity e = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<EnemyTag>(e);

            AddComponent(e, new EnemyMove
            {
                Speed = a.speed
            });

            AddComponent(e, new EnemyHealth
            {
                Value = a.health
            });

            AddComponent(e, new EnemyBaseStats
            {
                BaseHealth = a.health,
                BaseSpeed  = a.speed
            });

            // Bake Scale dari Transform GameObject di Inspector
            // Ini yang menentukan ukuran prefab saat di-instantiate via ECB
            var transform = a.GetComponent<Transform>();
            AddComponent(e, new LocalTransform
            {
                Position = Unity.Mathematics.float3.zero,
                Rotation = Unity.Mathematics.quaternion.identity,
                Scale    = transform.localScale.x  // ambil dari prefab Inspector (0.1)
            });
        }
    }
}