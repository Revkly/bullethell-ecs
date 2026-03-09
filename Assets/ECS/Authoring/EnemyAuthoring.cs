using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    public float speed = 2f;
    public float health = 10f;
    public float scale = 0.3f;

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

            // Base stats untuk difficulty scaling
            AddComponent(e, new EnemyBaseStats
            {
                BaseHealth = a.health,
                BaseSpeed = a.speed
            });
        }
    }
}