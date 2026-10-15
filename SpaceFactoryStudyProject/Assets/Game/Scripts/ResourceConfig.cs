using UnityEngine;

[CreateAssetMenu(fileName = "NewResource", menuName = "Game/Resource Config")]
public class ResourceConfig : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public float baseSellPrice;
    public ResourceType type;

    public enum ResourceType
    {
        Raw,
        Processed,
        Complex
    }
}