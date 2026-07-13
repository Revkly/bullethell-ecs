using Unity.Entities;

public struct WhipHitbox : IComponentData
{
    public float Radius;
    public float Damage;
    public float Lifetime;
    public bool  HasDamaged; // true setelah damage diterapkan (hit 1x per aktivasi)
}
