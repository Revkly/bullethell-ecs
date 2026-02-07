using Unity.Entities;

public struct ExpGem : IComponentData {}

public struct ExpValue : IComponentData
{
    public int Value;
}

public struct PickupRadius : IComponentData
{
    public float Value;
}
