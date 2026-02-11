using Unity.Entities;

public struct PendingUpgrade : IComponentData
{
    public UpgradeType OptionA;
    public UpgradeType OptionB;
    public UpgradeType OptionC;
}
