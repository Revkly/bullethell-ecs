using Unity.Entities;

public struct WhipHitbox : IComponentData
{
    public float Radius;
    public float Damage;
    public float Lifetime;
}
