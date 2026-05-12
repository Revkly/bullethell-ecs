using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Entities;

/// <summary>
/// UI untuk memilih upgrade saat player level up.
///
/// FIX: Ganti em.GetAllEntities() di Start dengan CreateEntityQuery.
/// GetAllEntities() mengalokasikan NativeArray berisi SEMUA entity —
/// sangat mahal dan langsung dibuang setelah satu iterasi.
/// </summary>
public class UpgradeUI : MonoBehaviour
{
    public GameObject panel;
    public Button     buttonA, buttonB, buttonC;
    public TMP_Text   textA,   textB,   textC;

    private EntityManager _em;
    private Entity        _player;
    private bool          _isOpen;

    void Start()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Gunakan EntityQuery — efisien, tidak alokasi semua entity
        var query = _em.CreateEntityQuery(typeof(PlayerTag));
        if (query.CalculateEntityCount() > 0)
            _player = query.GetSingletonEntity();
        query.Dispose();

        panel.SetActive(false);

        buttonA.onClick.AddListener(() => Select(0));
        buttonB.onClick.AddListener(() => Select(1));
        buttonC.onClick.AddListener(() => Select(2));
    }

    void Update()
    {
        if (!_em.Exists(_player)) return;

        bool hasUpgrade = _em.HasComponent<PendingUpgrade>(_player);

        if (hasUpgrade && !_isOpen)  Open();
        else if (!hasUpgrade && _isOpen) Close();
    }

    void Open()
    {
        var upgrade = _em.GetComponentData<PendingUpgrade>(_player);

        textA.text = GetDisplay(upgrade.OptionA);
        textB.text = GetDisplay(upgrade.OptionB);
        textC.text = GetDisplay(upgrade.OptionC);

        panel.SetActive(true);
        Time.timeScale = 0f;
        _isOpen = true;
    }

    void Close()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
        _isOpen = false;
    }

    string GetDisplay(WeaponType type)
    {
        var buffer = _em.GetBuffer<OwnedWeapon>(_player);

        foreach (var owned in buffer)
        {
            if (!_em.Exists(owned.WeaponEntity)) continue;

            var weaponType = _em.GetComponentData<WeaponTypeComponent>(owned.WeaponEntity);

            if (weaponType.Value == type)
            {
                int level     = _em.GetComponentData<WeaponLevel>(owned.WeaponEntity).Value;
                int nextLevel = Mathf.Min(level + 1, 3);
                return type + "  Lv." + nextLevel;
            }
        }

        return type + "  Lv.1";
    }

    void Select(int index)
    {
        var upgrade = _em.GetComponentData<PendingUpgrade>(_player);

        WeaponType chosen = index switch
        {
            1 => upgrade.OptionB,
            2 => upgrade.OptionC,
            _ => upgrade.OptionA
        };

        if (!_em.HasComponent<SelectedUpgrade>(_player))
            _em.AddComponentData(_player, new SelectedUpgrade { Value = chosen });

        _em.RemoveComponent<PendingUpgrade>(_player);
    }
}