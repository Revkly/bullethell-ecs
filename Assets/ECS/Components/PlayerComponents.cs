using Unity.Entities;
using Unity.Mathematics;

public struct PlayerTag : IComponentData { }

public struct MoveSpeed : IComponentData
{
    public float Value;
}

public struct PlayerInput : IComponentData
{
    public float2 Move;
}
