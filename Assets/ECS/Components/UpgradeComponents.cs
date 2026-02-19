using Unity.Entities;

public struct PendingUpgrade : IComponentData
{
    public WeaponType OptionA;
    public WeaponType OptionB;
    public WeaponType OptionC;
}

public struct LevelUpEvent : IComponentData {}
