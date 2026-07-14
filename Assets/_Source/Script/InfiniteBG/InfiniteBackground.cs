using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class InfiniteBackground : MonoBehaviour
{
    public float scrollSpeed = 0.05f;

    private Material mat;
    private EntityManager em;
    private Entity player;
    private bool playerFound = false;
    private bool _initialized = false;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        if (World.DefaultGameObjectInjectionWorld == null || !World.DefaultGameObjectInjectionWorld.IsCreated)
            return;

        if (!_initialized)
        {
            em = World.DefaultGameObjectInjectionWorld.EntityManager;
            _initialized = true;
        }

        // cari player entity sekali saja
        if (!playerFound)
        {
            var query = em.CreateEntityQuery(typeof(PlayerTag), typeof(LocalTransform));

            if (query.CalculateEntityCount() > 0)
            {
                player = query.GetSingletonEntity();
                playerFound = true;
            }
            query.Dispose();
            return;
        }

        if (!em.Exists(player)) return;

        var transformData = em.GetComponentData<LocalTransform>(player);

        Vector3 playerPos = new Vector3(
            transformData.Position.x,
            transformData.Position.y,
            0
        );

        // offset texture
        Vector2 offset = new Vector2(
            playerPos.x * scrollSpeed,
            playerPos.y * scrollSpeed
        );

        mat.mainTextureOffset = offset;
    }
}