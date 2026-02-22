using Unity.Entities;
using Unity.Mathematics;


public struct Weapon : IComponentData {}

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
    public float Value;
    public float Timer;
}

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

public struct WeaponTypeComponent : IComponentData
{
    public WeaponType Value;
}

public struct WeaponSlot : IComponentData
{
    public int MaxSlot;
}

public struct EnemyKnockback : IComponentData
{
    public float2 Direction;
    public float Force;
    public float Timer;
}