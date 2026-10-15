using UnityEngine;

[CreateAssetMenu(fileName = "ClickerConfig", menuName = "Game/Clicker Config")]
public class ClickerConfig : ScriptableObject
{
    public string id = "ManualClicker";
    public string displayName = "Ручной станок";
    public float currencyPerClick = 1f;
    public Sprite icon;
}