using Unity.Entities;
using Unity.Mathematics;

// ─── Tags ────────────────────────────────────────────────────────────────────
public struct EnemyTag      : IComponentData {}
public struct DeadTag       : IComponentData {}

// ─── Movement & Health ───────────────────────────────────────────────────────
public struct EnemyMove : IComponentData
{
    public float Speed;
}

public struct EnemyHealth : IComponentData
{
    public float Value;
}

// public struct EnemyBaseStats : IComponentData
// {
//     public float BaseHealth;
//     public float BaseSpeed;
// }

// ─── Knockback ────────────────────────────────────────────────────────────────
// PINDAHKAN dari WeaponComponents.cs ke sini — definisi tunggal
public struct EnemyKnockback : IComponentData
{
    public float2 Direction;
    public float  Force;
    public float  Timer;
}

// ─── Nearest-enemy cache (singleton, diupdate 1x/frame oleh NearestEnemySystem)
// Dipakai oleh FireWandSystem & MagicWandSystem agar tidak O(n²) ─────────────
public struct NearestEnemyCache : IComponentData
{
    public Entity Value;
    public float3 Position;
}