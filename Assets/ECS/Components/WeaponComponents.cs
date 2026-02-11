using Unity.Entities;

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