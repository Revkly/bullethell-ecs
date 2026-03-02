using Unity.Entities;
using UnityEngine;

public class WeaponPrefabRegistryAuthoring : MonoBehaviour
{
    [System.Serializable]
    public struct Entry
    {
        public WeaponType type;
        public GameObject prefab;
    }

    public Entry[] weapons;

    class Baker : Baker<WeaponPrefabRegistryAuthoring>
    {
        public override void Bake(WeaponPrefabRegistryAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            var buffer = AddBuffer<WeaponPrefabElement>(entity);

            foreach (var entry in authoring.weapons)
            {
                if (entry.prefab == null)
                    continue;

                buffer.Add(new WeaponPrefabElement
                {
                    Type = entry.type,
                    Prefab = GetEntity(entry.prefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}