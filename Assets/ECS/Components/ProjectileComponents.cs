using Unity.Entities;
using Unity.Mathematics;

// ─── Tags ─────────────────────────────────────────────────────────────────────
public struct ProjectileTag : IComponentData {}
public struct KnifeTag      : IComponentData {}

// ─── Visual ───────────────────────────────────────────────────────────────────
/// <summary>
/// Scale asli prefab, di-bake oleh ProjectileAuthoring dari Transform Inspector.
/// Weapon system membaca ini saat Instantiate alih-alih hardcode angka.
/// </summary>
public struct ProjectileScale : IComponentData
{
    public float Value;
}

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