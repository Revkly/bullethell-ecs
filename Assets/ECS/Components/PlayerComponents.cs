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

public struct PlayerHealth : IComponentData
{
    public float Current;
    public float Max;
}

public struct PlayerDamageCooldown : IComponentData
{
    public float Cooldown;
    public float Timer;
}

public struct PlayerExp : IComponentData
{
    public int Current;
}

public struct PlayerLevel : IComponentData
{
    public int Value;
}

public struct ExpToNextLevel : IComponentData
{
    public int Value;
}

