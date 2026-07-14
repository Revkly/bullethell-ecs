using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Entities;

public class GameOverController : MonoBehaviour
{
    public GameObject gameOverPanel;
    private EntityManager _em;
    private Entity _player;
    private bool _isGameOver = false;
    private bool _initialized = false;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (_isGameOver) return;

        if (World.DefaultGameObjectInjectionWorld == null || !World.DefaultGameObjectInjectionWorld.IsCreated)
            return;

        if (!_initialized)
        {
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        // Lazy find player
        if (!_initialized || !_em.Exists(_player))
        {
            var query = _em.CreateEntityQuery(typeof(PlayerTag));
            if (query.CalculateEntityCount() > 0)
            {
                _player = query.GetSingletonEntity();
                _initialized = true;
            }
            query.Dispose();
        }

        if (!_initialized || !_em.Exists(_player)) return;

        var health = _em.GetComponentData<PlayerHealth>(_player);
        if (health.Current <= 0f)
        {
            TriggerGameOver();
        }
    }

    void TriggerGameOver()
    {
        _isGameOver = true;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RetryGame()
    {
        ECSWorldResetter.ResetAndLoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        ECSWorldResetter.ResetAndLoadScene("MainMenu");
    }
}
