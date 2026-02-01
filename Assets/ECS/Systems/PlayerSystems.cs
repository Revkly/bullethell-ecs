using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.InputSystem;
using UnityEngine;


#region INPUT
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class PlayerInputSystem : SystemBase
{
    private PlayerInputActions input;

    protected override void OnCreate()
    {
        input = new PlayerInputActions();
        input.Player.Enable();
    }

    protected override void OnUpdate()
    {
        float2 move = input.Player.Move.ReadValue<Vector2>();

        foreach (var inputData in SystemAPI.Query<RefRW<PlayerInput>>())
        {
            inputData.ValueRW.Move = math.normalizesafe(move);
        }
    }
}
#endregion

#region MOVEMENT
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class PlayerMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (transform, input, speed) in
            SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRO<PlayerInput>,
                RefRO<MoveSpeed>>()
            .WithAll<PlayerTag>())
        {
            float3 delta = new float3(
                input.ValueRO.Move.x * speed.ValueRO.Value * dt,
                input.ValueRO.Move.y * speed.ValueRO.Value * dt,
                0f
            );

            transform.ValueRW.Position += delta;
        }
    }
}
#endregion
