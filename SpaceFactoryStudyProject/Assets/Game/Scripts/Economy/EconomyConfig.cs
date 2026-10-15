using UnityEngine;

[CreateAssetMenu(fileName = "EconomyConfig", menuName = "Game/Economy Config")]
public class EconomyConfig : ScriptableObject
{
    public float manualClickAmount = 1f;
    public float maxOfflineHours = 8f;
    public float incomeSmoothSpeed = 1f;
}