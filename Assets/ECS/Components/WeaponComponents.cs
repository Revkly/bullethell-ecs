using Unity.Entities;

public struct Weapon : IComponentData
{
    public float Damage;
    public float Cooldown;
    public int ProjectileCount;
}

public struct WeaponTimer : IComponentData
{
    public float Timer;
}
