using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Image healthFill;
    public Vector3 offset = new Vector3(0, -0.7f, 0);

    EntityManager entityManager;
    Entity player;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var query = entityManager.CreateEntityQuery(typeof(PlayerTag));

        if (query.CalculateEntityCount() > 0)
        {
            player = query.GetSingletonEntity();
        }
    }

    void Update()
    {
        if (!entityManager.Exists(player))
            return;

        var transformData = entityManager.GetComponentData<LocalTransform>(player);
        var health = entityManager.GetComponentData<PlayerHealth>(player);

        Vector3 pos = new Vector3(
            transformData.Position.x,
            transformData.Position.y,
            0
        );

        transform.position = pos + offset;

        float normalized = health.Current / health.Max;
        normalized = Mathf.Clamp01(normalized);

        healthFill.fillAmount = normalized;
    }
}