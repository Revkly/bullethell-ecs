using Unity.Entities;

// ─── Weapon base ─────────────────────────────────────────────────────────────
public struct Weapon              : IComponentData {}

public struct WeaponOwner : IComponentData
{
    public Entity Player;
}

public struct WeaponLevel : IComponentData
{
    public int Value;
}

public struct WeaponCooldown : IComponentData
{
    public float Value;   // interval
    public float Timer;   // countdown
}

public struct WeaponTypeComponent : IComponentData
{
    public WeaponType Value;
}

public struct WeaponSlot : IComponentData
{
    public int MaxSlot;
}

// ─── Weapon types ─────────────────────────────────────────────────────────────
public enum WeaponType
{
    Whip,
    MagicWand,
    FireWand,
    Knife,
    Axe,
    Fireball,
    SpreadShot,
    LightningRing,
    Lightning,
    Magnet
}

// NOTE: EnemyKnockback sudah dipindah ke EnemyComponents.cs — jangan duplikasi