using UnityEngine;
using TMPro;
using Unity.Entities;

public class GameTimeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private EntityManager entityManager;
    private bool _initialized;

    void Update()
    {
        if (World.DefaultGameObjectInjectionWorld == null || !World.DefaultGameObjectInjectionWorld.IsCreated)
            return;

        if (!_initialized)
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _initialized = true;
        }

        var gameTimeQuery = entityManager.CreateEntityQuery(typeof(GameTime));

        if (gameTimeQuery.IsEmpty)
        {
            gameTimeQuery.Dispose();
            return;
        }

        GameTime gameTime = gameTimeQuery.GetSingleton<GameTime>();
        gameTimeQuery.Dispose();

        float elapsed = gameTime.Elapsed;

        int minutes = Mathf.FloorToInt(elapsed / 60f);
        int seconds = Mathf.FloorToInt(elapsed % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}