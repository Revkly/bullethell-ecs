using Unity.Entities;

public struct EnemyTag : IComponentData {}

public struct EnemyMove : IComponentData
{
    public float Speed;
}

public struct EnemyHealth : IComponentData
{
    public float Value;
}
