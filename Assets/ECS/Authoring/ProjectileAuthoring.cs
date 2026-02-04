using Unity.Entities;
using UnityEngine;

public class ProjectileAuthoring : MonoBehaviour
{
    public float speed = 8f;
    public float damage = 10f;

    class Baker : Baker<ProjectileAuthoring>
    {
        public override void Bake(ProjectileAuthoring a)
        {
            Entity e = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<ProjectileTag>(e);

            AddComponent(e, new ProjectileData
            {
                Speed = a.speed,
                Damage = a.damage,
                Direction = new Unity.Mathematics.float2(0, 1)
                
            });
        }
    }
}
