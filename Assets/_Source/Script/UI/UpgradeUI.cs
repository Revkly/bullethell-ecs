using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;

/// <summary>
/// Upgrade card UI — menampilkan gambar weapon, level bintang, dan nama.
/// Setiap card punya: Image weapon, level indicator (bintang), nama weapon.
/// </summary>
public class UpgradeUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("Cards")]
    public UpgradeCard cardA;
    public UpgradeCard cardB;
    public UpgradeCard cardC;

    [Header("Weapon Icons")]
    public Sprite iconWhip;
    public Sprite iconMagicWand;
    public Sprite iconFireWand;
    public Sprite iconKnife;
    public Sprite iconAxe;
    public Sprite iconFireball;
    public Sprite iconSpreadShot;
    public Sprite iconLightningRing;
    public Sprite iconLightning;
    public Sprite iconMagnet;
    public Sprite iconDefault; // fallback jika tidak ada icon

    private EntityManager _em;
    private Entity        _player;
    private bool          _isOpen;
    private bool          _initialized;

    void Start()
    {
        panel.SetActive(false);

        cardA.button.onClick.AddListener(() => Select(0));
        cardB.button.onClick.AddListener(() => Select(1));
        cardC.button.onClick.AddListener(() => Select(2));
    }

    void Update()
    {
        if (World.DefaultGameObjectInjectionWorld == null || !World.DefaultGameObjectInjectionWorld.IsCreated)
            return;

        if (!_initialized)
        {
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        // Lazy find player — SubScene mungkin belum selesai load saat Start()
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

        bool hasUpgrade = _em.HasComponent<PendingUpgrade>(_player);

        if (hasUpgrade && !_isOpen)       Open();
        else if (!hasUpgrade && _isOpen)  Close();
    }

    void Open()
    {
        var upgrade = _em.GetComponentData<PendingUpgrade>(_player);

        SetupCard(cardA, upgrade.OptionA);
        SetupCard(cardB, upgrade.OptionB);
        SetupCard(cardC, upgrade.OptionC);

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

    void SetupCard(UpgradeCard card, WeaponType type)
    {
        // Set icon
        card.weaponImage.sprite = GetIcon(type);
        card.weaponImage.color  = Color.white;

        // Set nama weapon
        card.weaponName.text = GetWeaponName(type);

        // Set level bintang
        int currentLevel = GetCurrentLevel(type);
        int nextLevel    = Mathf.Min(currentLevel + 1, 3);

        // Update bintang — filled = sudah punya, outline = akan didapat
        for (int i = 0; i < card.stars.Length; i++)
        {
            if (i < currentLevel)
                card.stars[i].color = new Color(1f, 0.85f, 0f); // kuning = sudah punya
            else if (i < nextLevel)
                card.stars[i].color = new Color(1f, 1f, 1f);    // putih = akan naik
            else
                card.stars[i].color = new Color(0.3f, 0.3f, 0.3f); // abu = kosong
        }

        // Label NEW jika senjata belum dimiliki
        if (card.newLabel != null)
            card.newLabel.SetActive(currentLevel == 0);
    }

    int GetCurrentLevel(WeaponType type)
    {
        var buffer = _em.GetBuffer<OwnedWeapon>(_player);
        foreach (var owned in buffer)
        {
            if (!_em.Exists(owned.WeaponEntity)) continue;
            var t = _em.GetComponentData<WeaponTypeComponent>(owned.WeaponEntity);
            if (t.Value == type)
                return _em.GetComponentData<WeaponLevel>(owned.WeaponEntity).Value;
        }
        return 0;
    }

    Sprite GetIcon(WeaponType type) => type switch
    {
        WeaponType.Whip          => iconWhip          ? iconWhip          : iconDefault,
        WeaponType.MagicWand     => iconMagicWand     ? iconMagicWand     : iconDefault,
        WeaponType.FireWand      => iconFireWand      ? iconFireWand      : iconDefault,
        WeaponType.Knife         => iconKnife         ? iconKnife         : iconDefault,
        WeaponType.Axe           => iconAxe           ? iconAxe           : iconDefault,
        WeaponType.Fireball      => iconFireball      ? iconFireball      : iconDefault,
        WeaponType.SpreadShot    => iconSpreadShot    ? iconSpreadShot    : iconDefault,
        WeaponType.LightningRing => iconLightningRing ? iconLightningRing : iconDefault,
        WeaponType.Lightning     => iconLightning     ? iconLightning     : iconDefault,
        WeaponType.Magnet        => iconMagnet        ? iconMagnet        : iconDefault,
        _                        => iconDefault
    };

    string GetWeaponName(WeaponType type) => type switch
    {
        WeaponType.Whip          => "Whip",
        WeaponType.MagicWand     => "Magic Wand",
        WeaponType.FireWand      => "Fire Wand",
        WeaponType.Knife         => "Knife",
        WeaponType.Axe           => "Axe",
        WeaponType.Fireball      => "Fireball",
        WeaponType.SpreadShot    => "Spread Shot",
        WeaponType.LightningRing => "Lightning Ring",
        WeaponType.Lightning     => "Lightning",
        WeaponType.Magnet        => "Magnet",
        _                        => type.ToString()
    };

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

/// <summary>
/// Data tiap card upgrade — assign di Inspector.
/// </summary>
[System.Serializable]
public class UpgradeCard
{
    public Button      button;
    public Image       weaponImage;  // gambar weapon
    public TMPro.TMP_Text weaponName;   // nama weapon (boleh dikosongkan di Inspector jika tidak mau teks)
    public Image[]     stars;        // 3 bintang level indicator
    public GameObject  newLabel;     // badge "NEW" — optional
}