using UnityEngine;
using TMPro;
using Unity.Entities;

/// <summary>
/// Menampilkan jumlah enemy dan total entity di layar.
///
/// FIX:
/// - EntityQuery dibuat SEKALI di Start, bukan dibuat ulang tiap frame.
/// - Update di-throttle setiap 0.1 detik — UI counter tidak perlu refresh 60fps.
///   CalculateEntityCount() tiap frame bisa terasa saat ada 5000+ entity.
/// </summary>
public class ECSCounterUI : MonoBehaviour
{
    public TextMeshProUGUI enemyText;
    public TextMeshProUGUI totalText;

    private EntityManager _em;
    private EntityQuery   _enemyQuery;
    private EntityQuery   _allQuery;

    private float _timer;
    private const float UPDATE_INTERVAL = 0.1f;

    void Start()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Buat query sekali — jangan buat di Update
        _enemyQuery = _em.CreateEntityQuery(typeof(EnemyTag));
        _allQuery   = _em.UniversalQuery;
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < UPDATE_INTERVAL) return;
        _timer = 0f;

        int enemyCount = _enemyQuery.CalculateEntityCount();
        int totalCount = _allQuery.CalculateEntityCount();

        enemyText.text = "Enemy: "        + enemyCount;
        totalText.text = "Total Entity: " + totalCount;
    }

    void OnDestroy()
    {
        _enemyQuery.Dispose();
        // _allQuery adalah UniversalQuery milik EntityManager — jangan di-dispose
    }
}