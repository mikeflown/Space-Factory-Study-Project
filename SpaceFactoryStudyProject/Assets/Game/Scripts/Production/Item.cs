using UnityEngine;

public class Item
{
    public ResourceConfig Resource { get; }
    public Vector2Int GridPosition { get; set; }
    public Item(ResourceConfig resource, Vector2Int position)
    {
        Resource = resource;
        GridPosition = position;
    }
}