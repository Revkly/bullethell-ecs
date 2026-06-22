using Unity.Entities;
using Unity.Mathematics;

public struct EnemyTag      : IComponentData {}
public struct DeadTag       : IComponentData {}

public struct EnemyMove : IComponentData
{
    public float Speed;
}

public struct EnemyHealth : IComponentData
{
    public float Value;
}

public struct EnemyKnockback : IComponentData
{
    public float2 Direction;
    public float  Force;
    public float  Timer;
}

public struct NearestEnemyCache : IComponentData
{
    public Entity Value;
    public float3 Position;
}