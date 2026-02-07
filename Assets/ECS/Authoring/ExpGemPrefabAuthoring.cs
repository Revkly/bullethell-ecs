using Unity.Entities;
using UnityEngine;

public class ExpGemPrefabAuthoring : MonoBehaviour
{
    public GameObject expPrefab;

    class Baker : Baker<ExpGemPrefabAuthoring>
    {
        public override void Bake(ExpGemPrefabAuthoring a)
        {
            Entity e = GetEntity(TransformUsageFlags.None);

            AddComponent(e, new ExpGemPrefab
            {
                Value = GetEntity(a.expPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}
