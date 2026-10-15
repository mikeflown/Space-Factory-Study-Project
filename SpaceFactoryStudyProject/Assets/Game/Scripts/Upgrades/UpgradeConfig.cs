using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Game/Upgrade Config")]
public class UpgradeConfig : ScriptableObject
{
    public string id;
    public string displayName;
    [Min(0)] public int baseCost = 10;
    [Min(1f)] public float costMultiplier = 1.5f;
    public float baseIncomePerMinute = 1f;
    public float incomeMultiplier = 1.2f;
    public int unlockAtCurrency = 0;
    public UpgradeConfig prerequisite;
}