using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Entities;

public class UpgradeUI : MonoBehaviour
{
    public GameObject panel;

    public Button buttonA;
    public Button buttonB;
    public Button buttonC;

    public TMP_Text textA;
    public TMP_Text textB;
    public TMP_Text textC;

    EntityManager em;
    Entity player;

    bool isOpen = false;

    void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;

        foreach (var e in em.GetAllEntities())
        {
            if (em.HasComponent<PlayerTag>(e))
            {
                player = e;
                break;
            }
        }

        panel.SetActive(false);

        buttonA.onClick.AddListener(() => Select(0));
        buttonB.onClick.AddListener(() => Select(1));
        buttonC.onClick.AddListener(() => Select(2));
    }

    void Update()
    {
        if (!em.Exists(player))
            return;

        bool hasUpgrade = em.HasComponent<PendingUpgrade>(player);

        if (hasUpgrade && !isOpen)
        {
            Open();
        }
        else if (!hasUpgrade && isOpen)
        {
            Close();
        }
    }

    void Open()
    {
        var upgrade = em.GetComponentData<PendingUpgrade>(player);

        textA.text = GetDisplay(upgrade.OptionA);
        textB.text = GetDisplay(upgrade.OptionB);
        textC.text = GetDisplay(upgrade.OptionC);

        panel.SetActive(true);
        Time.timeScale = 0f;
        isOpen = true;
    }

    void Close()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
        isOpen = false;
    }

    string GetDisplay(WeaponType type)
    {
        var buffer = em.GetBuffer<OwnedWeapon>(player);

        foreach (var owned in buffer)
        {
            var weaponType = em.GetComponentData<WeaponTypeComponent>(owned.WeaponEntity);

            if (weaponType.Value == type)
            {
                int level = em.GetComponentData<WeaponLevel>(owned.WeaponEntity).Value;
                int nextLevel = Mathf.Min(level + 1, 3);
                return type + "  Lv." + nextLevel;
            }
        }

        return type + "  Lv.1";
    }

    void Select(int index)
    {
        var upgrade = em.GetComponentData<PendingUpgrade>(player);

        WeaponType chosen = upgrade.OptionA;

        if (index == 1) chosen = upgrade.OptionB;
        if (index == 2) chosen = upgrade.OptionC;

        if (!em.HasComponent<SelectedUpgrade>(player))
        {
            em.AddComponentData(player,
                new SelectedUpgrade { Value = chosen });
        }

        em.RemoveComponent<PendingUpgrade>(player);
    }
}
