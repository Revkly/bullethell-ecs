using UnityEngine;
using Unity.Entities;

public class SpawnShortcutInput : MonoBehaviour
{
    EntityManager em;

    void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            CreateRequest(1000);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            CreateRequest(5000);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            CreateRequest(10000);
    }

    void CreateRequest(int count)
    {
        Entity e = em.CreateEntity();
        em.AddComponentData(e, new SpawnRequest { Count = count });
    }
}