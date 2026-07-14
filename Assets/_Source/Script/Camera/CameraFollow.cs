using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

/// <summary>
/// Kamera mengikuti player dengan smooth lerp.
///
/// FIX: Lazy initialization — World dan Player dicari di LateUpdate
/// agar tidak crash saat SubScene belum selesai load.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public float  smoothSpeed = 5f;
    public Vector3 offset     = new Vector3(0, 0, -10);

    private EntityManager _entityManager;
    private Entity        _playerEntity;
    private bool          _initialized;

    void LateUpdate()
    {
        if (World.DefaultGameObjectInjectionWorld == null || !World.DefaultGameObjectInjectionWorld.IsCreated)
            return;

        if (!_initialized)
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        // Lazy find player
        if (!_initialized || !_entityManager.Exists(_playerEntity))
        {
            var query = _entityManager.CreateEntityQuery(typeof(PlayerTag), typeof(LocalTransform));
            if (query.CalculateEntityCount() > 0)
            {
                _playerEntity = query.GetSingletonEntity();
                _initialized = true;
            }
            query.Dispose();
        }

        if (!_initialized || !_entityManager.Exists(_playerEntity)) return;

        float3 playerPos = _entityManager
            .GetComponentData<LocalTransform>(_playerEntity).Position;

        Vector3 target = new Vector3(playerPos.x, playerPos.y, 0f) + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            target,
            smoothSpeed * Time.deltaTime);
    }
}