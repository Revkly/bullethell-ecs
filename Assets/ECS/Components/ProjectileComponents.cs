using Unity.Entities;
using Unity.Mathematics;

public struct ProjectileTag : IComponentData {}

public struct ProjectileData : IComponentData
{
    public float Speed;
    public float Damage;
    public float2 Direction;
    public float Scale;
}

public struct ProjectileDamage : IComponentData
{
    public float Value;
}
