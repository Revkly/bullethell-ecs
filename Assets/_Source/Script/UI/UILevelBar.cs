using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;

public class UILevelBar : MonoBehaviour
{
    public Slider xpBar;

    EntityManager em;
    Entity player;

    void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;

        var query = em.CreateEntityQuery(typeof(PlayerTag));

        if (query.CalculateEntityCount() > 0)
        {
            player = query.GetSingletonEntity();
        }
    }

    void Update()
    {
        if (!em.Exists(player))
            return;

        if (!em.HasComponent<PlayerExp>(player))
            return;

        var exp = em.GetComponentData<PlayerExp>(player);
        var next = em.GetComponentData<ExpToNextLevel>(player);

        xpBar.maxValue = next.Value;
        xpBar.value = exp.Current;
    }
}