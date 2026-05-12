using Unity.Entities;
using Unity.Mathematics;

// ─── Tags ─────────────────────────────────────────────────────────────────────
public struct ProjectileTag : IComponentData {}

// KnifeTag dipindah ke sini dari WeaponProjectilePrefab.cs — satu tempat
public struct KnifeTag      : IComponentData {}

// ─── Movement ─────────────────────────────────────────────────────────────────
public struct ProjectileData : IComponentData
{
    public float  Speed;
    public float2 Direction;
}

// ─── Combat ───────────────────────────────────────────────────────────────────
public struct ProjectileDamage : IComponentData
{
    public float Value;
}

public struct ProjectileLifetime : IComponentData
{
    public float Value;
}

// ─── Special behaviours ───────────────────────────────────────────────────────
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