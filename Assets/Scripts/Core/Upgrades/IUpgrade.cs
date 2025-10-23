namespace OverBang.GameName.Core.Upgrades
{
    public interface IUpgrade
    {
        UpgradeTarget Target { get; }
        UpgradeValueType ValueType { get; }
        
        float GetValue(float baseValue);
    }
}