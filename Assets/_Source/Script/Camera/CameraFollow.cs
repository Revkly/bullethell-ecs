using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

/// <summary>
/// Kamera mengikuti player dengan smooth lerp.
///
/// FIX: CreateEntityQuery() dipindah ke Awake — sebelumnya dipanggil
/// berulang di LateUpdate() setiap frame sampai player ditemukan.
/// Membuat query di dalam Update adalah alokasi yang tidak perlu.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public float  smoothSpeed = 5f;
    public Vector3 offset     = new Vector3(0, 0, -10);

    private EntityManager _entityManager;
    private EntityQuery   _playerQuery;
    private Entity        _playerEntity;
    private bool          _playerFound;

    void Awake()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Buat query SEKALI di Awake, bukan berulang di LateUpdate
        _playerQuery = _entityManager.CreateEntityQuery(
            typeof(PlayerTag),
            typeof(LocalTransform));
    }

    void LateUpdate()
    {
        if (!_playerFound)
        {
            if (_playerQuery.CalculateEntityCount() == 0) return;

            _playerEntity = _playerQuery.GetSingletonEntity();
            _playerFound  = true;
        }

        if (!_entityManager.Exists(_playerEntity)) return;

        float3 playerPos = _entityManager
            .GetComponentData<LocalTransform>(_playerEntity).Position;

        Vector3 target = new Vector3(playerPos.x, playerPos.y, 0f) + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            target,
            smoothSpeed * Time.deltaTime);
    }

    void OnDestroy()
    {
        _playerQuery.Dispose();
    }
}