using UnityEngine;
using TMPro;
using Unity.Entities;

public class GameTimeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private EntityQuery gameTimeQuery;
    private EntityManager entityManager;

    void Start()
    {
        entityManager =
            World.DefaultGameObjectInjectionWorld.EntityManager;

        gameTimeQuery =
            entityManager.CreateEntityQuery(typeof(GameTime));
    }

    void Update()
    {
        if (gameTimeQuery.IsEmpty)
            return;

        GameTime gameTime =
            gameTimeQuery.GetSingleton<GameTime>();

        float elapsed = gameTime.Elapsed;

        int minutes = Mathf.FloorToInt(elapsed / 60f);
        int seconds = Mathf.FloorToInt(elapsed % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}