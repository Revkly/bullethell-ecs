using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class CameraFollow : MonoBehaviour
{
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    private EntityManager entityManager;
    private Entity playerEntity;
    private bool playerFound = false;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    void LateUpdate()
    {
        if (!playerFound)
        {
            var query = entityManager.CreateEntityQuery(typeof(PlayerTag), typeof(LocalTransform));

            if (query.CalculateEntityCount() > 0)
            {
                playerEntity = query.GetSingletonEntity();
                playerFound = true;
            }
            return;
        }

        if (!entityManager.Exists(playerEntity))
            return;

        float3 playerPos = entityManager
            .GetComponentData<LocalTransform>(playerEntity).Position;

        Vector3 targetPosition = new Vector3(playerPos.x, playerPos.y, 0) + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}