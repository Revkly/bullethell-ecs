using Unity.Entities;
using UnityEngine;

public class WeaponPoolAuthoring : MonoBehaviour
{
    public WeaponType[] availableWeapons;

    class Baker : Baker<WeaponPoolAuthoring>
{
    public override void Bake(WeaponPoolAuthoring a)
    {
        Entity e = GetEntity(TransformUsageFlags.None);

        var buffer = AddBuffer<WeaponPoolElement>(e);

        foreach (var weapon in a.availableWeapons)
        {
            buffer.Add(new WeaponPoolElement { Value = weapon });
        }
    }
}
}
