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

        foreach (var (transform, input, speed, facing) in
            SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRO<PlayerInput>,
                RefRO<MoveSpeed>,
                RefRW<PlayerFacing>>()
            .WithAll<PlayerTag>())
        {
            float2 moveDir = input.ValueRO.Move;

            // =========================
            // UPDATE FACING DIRECTION
            // =========================
            if (math.lengthsq(moveDir) > 0.0001f)
            {
                facing.ValueRW.Direction = math.normalize(moveDir);
            }

            // =========================
            // MOVEMENT
            // =========================
            float3 delta = new float3(
                moveDir.x * speed.ValueRO.Value * dt,
                moveDir.y * speed.ValueRO.Value * dt,
                0f
            );

            transform.ValueRW.Position += delta;
        }
    }
}
#endregion