using Unity.Entities;

public struct WeaponProjectilePrefab : IComponentData
{
    public Entity Value;
}


public struct WeaponPrefabRegistry : IComponentData
{
    public Entity Knife;
}

// public struct KnifePierce : IComponentData
// {
//     public float Radius;
// }