using UnityEngine;
using TMPro;
using Unity.Entities;

public class ECSCounterUI : MonoBehaviour
{
    public TextMeshProUGUI enemyText;
    public TextMeshProUGUI totalText;

    EntityManager em;

    EntityQuery enemyQuery;
    EntityQuery allQuery;

    void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;

        enemyQuery = em.CreateEntityQuery(typeof(EnemyTag));
        allQuery = em.UniversalQuery;
    }

    void Update()
    {
        int enemyCount = enemyQuery.CalculateEntityCount();
        int totalCount = allQuery.CalculateEntityCount();

        enemyText.text = "Enemy: " + enemyCount;
        totalText.text = "Total Entity: " + totalCount;
    }
}