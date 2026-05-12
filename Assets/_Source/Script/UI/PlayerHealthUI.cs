using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.UI;

/// <summary>
/// UI health bar yang mengikuti posisi player di layar.
///
/// FIX: EntityQuery dibuat dengan ComponentType.ReadOnly untuk menandai
/// kita tidak menulis ke komponen — lebih aman untuk job scheduling.
/// </summary>
public class PlayerHealthUI : MonoBehaviour
{
    public Image  healthFill;
    public Vector3 offset = new Vector3(0, -0.7f, 0);

    private EntityManager _em;
    private Entity        _player;

    void Start()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;

        var query = _em.CreateEntityQuery(typeof(PlayerTag));
        if (query.CalculateEntityCount() > 0)
            _player = query.GetSingletonEntity();
        query.Dispose();
    }

    void Update()
    {
        if (!_em.Exists(_player)) return;

        var t      = _em.GetComponentData<LocalTransform>(_player);
        var health = _em.GetComponentData<PlayerHealth>(_player);

        transform.position = new Vector3(t.Position.x, t.Position.y, 0f) + offset;

        healthFill.fillAmount = Mathf.Clamp01(health.Current / health.Max);
    }
}