using Unity.Entities;

public struct WeaponPoolElement : IBufferElementData
{
    public WeaponType Value;
}

public struct WeaponPrefabElement : IBufferElementData
{
    public WeaponType Type;
    public Entity Prefab;
}