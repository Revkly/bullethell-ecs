using Unity.Entities;
using Unity.Mathematics;

public struct ProjectileTag : IComponentData {}

public struct ProjectileData : IComponentData
{
    public float Speed;
    public float2 Direction;
}

public struct ProjectileDamage : IComponentData
{
    public float Value;
}

public struct ProjectileLifetime : IComponentData
{
    public float Value;
}

public struct ExplosionData : IComponentData
{
    public float Radius;
}

public struct KnockbackData : IComponentData
{
    public float Force;
}

public struct ProjectileHit : IComponentData
{
    public Entity HitEntity;
}
