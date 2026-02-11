using Unity.Entities;
using UnityEngine;

public class ProjectileAuthoring : MonoBehaviour
{
    class Baker : Baker<ProjectileAuthoring>
    {
        public override void Bake(ProjectileAuthoring authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.Dynamic);

            // TAG SAJA
            AddComponent<ProjectileTag>(e);
        }
    }
}
