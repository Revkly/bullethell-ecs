using UnityEngine;
using TMPro;
using Unity.Entities;

/// <summary>
/// Menampilkan jumlah enemy dan total entity di layar.
///
/// FIX: Lazy initialization — World dan query dicari di Update
/// agar tidak crash saat SubScene belum selesai load.
/// </summary>
public class ECSCounterUI : MonoBehaviour
{
    public TextMeshProUGUI enemyText;
    public TextMeshProUGUI totalText;

    private EntityManager _em;
    private EntityQuery   _enemyQuery;
    private bool          _initialized;

    private float _timer;
    private const float UPDATE_INTERVAL = 0.1f;

    void Update()
    {
        if (World.DefaultGameObjectInjectionWorld == null || !World.DefaultGameObjectInjectionWorld.IsCreated)
            return;

        if (!_initialized)
        {
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
            _enemyQuery = _em.CreateEntityQuery(typeof(EnemyTag));
            _initialized = true;
        }

        _timer += Time.deltaTime;
        if (_timer < UPDATE_INTERVAL) return;
        _timer = 0f;

        int enemyCount = _enemyQuery.CalculateEntityCount();
        int totalCount = _em.UniversalQuery.CalculateEntityCount();

        enemyText.text = "Enemy: "        + enemyCount;
        totalText.text = "Total Entity: " + totalCount;
    }

    void OnDestroy()
    {
        if (_initialized && World.DefaultGameObjectInjectionWorld != null && World.DefaultGameObjectInjectionWorld.IsCreated)
        {
            _enemyQuery.Dispose();
        }
    }
}