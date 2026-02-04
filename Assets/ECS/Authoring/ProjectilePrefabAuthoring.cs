using Unity.Entities;
using UnityEngine;

public class ProjectilePrefabAuthoring : MonoBehaviour
{
    public GameObject projectilePrefab;

    class Baker : Baker<ProjectilePrefabAuthoring>
    {
        public override void Bake(ProjectilePrefabAuthoring a)
        {
            Entity e = GetEntity(TransformUsageFlags.None);

            AddComponent(e, new ProjectilePrefab
            {
                Value = GetEntity(a.projectilePrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}
