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

    EntityManager entityManager;
    Entity playerEntity;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        foreach (var entity in entityManager.GetAllEntities())
        {
            if (entityManager.HasComponent<PlayerTag>(entity))
            {
                playerEntity = entity;
                break;
            }
        }

        panel.SetActive(false);
    }

    void Update()
    {
        if (!entityManager.Exists(playerEntity))
            return;

        if (entityManager.HasComponent<PendingUpgrade>(playerEntity))
        {
            var upgrade = entityManager.GetComponentData<PendingUpgrade>(playerEntity);

            textA.text = upgrade.OptionA.ToString();
            textB.text = upgrade.OptionB.ToString();
            textC.text = upgrade.OptionC.ToString();

            panel.SetActive(true);

            buttonA.onClick.RemoveAllListeners();
            buttonB.onClick.RemoveAllListeners();
            buttonC.onClick.RemoveAllListeners();

            buttonA.onClick.AddListener(() => Select(upgrade.OptionA));
            buttonB.onClick.AddListener(() => Select(upgrade.OptionB));
            buttonC.onClick.AddListener(() => Select(upgrade.OptionC));
        }
        else
        {
            panel.SetActive(false);
        }
    }

    void Select(WeaponType type)
    {
        if (!entityManager.HasComponent<SelectedUpgrade>(playerEntity))
        {
            entityManager.AddComponentData(playerEntity,
                new SelectedUpgrade { Value = type });
        }

        entityManager.RemoveComponent<PendingUpgrade>(playerEntity);
        panel.SetActive(false);
    }
}
