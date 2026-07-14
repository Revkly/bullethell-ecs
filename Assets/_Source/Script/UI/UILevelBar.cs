using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;

public class UILevelBar : MonoBehaviour
{
    public Slider xpBar;

    EntityManager em;
    Entity player;
    bool _initialized;

    void Start()
    {
        if (World.DefaultGameObjectInjectionWorld != null && World.DefaultGameObjectInjectionWorld.IsCreated)
        {
            em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }
    }

    void Update()
    {
        // Cek ulang World jika belum ready saat Start
        if (World.DefaultGameObjectInjectionWorld == null || !World.DefaultGameObjectInjectionWorld.IsCreated)
            return;

        if (!_initialized)
        {
            em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        // Lazy find player — SubScene mungkin belum selesai load saat Start()
        if (!_initialized || !em.Exists(player))
        {
            var query = em.CreateEntityQuery(typeof(PlayerTag));
            if (query.CalculateEntityCount() > 0)
            {
                player = query.GetSingletonEntity();
                _initialized = true;
            }
            query.Dispose();
        }

        if (!_initialized || !em.Exists(player))
            return;

        if (!em.HasComponent<PlayerExp>(player))
            return;

        var exp = em.GetComponentData<PlayerExp>(player);
        var next = em.GetComponentData<ExpToNextLevel>(player);

        xpBar.maxValue = next.Value;
        xpBar.value = exp.Current;
    }
}