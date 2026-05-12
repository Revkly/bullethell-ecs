using Unity.Entities;

public struct WeaponProjectilePrefab : IComponentData
{
    public Entity Value;
}

public struct WeaponPrefabRegistry : IComponentData
{
    public Entity Knife;
}

// NOTE: KnifeTag sudah dipindah ke ProjectileComponents.cs — jangan duplikasi