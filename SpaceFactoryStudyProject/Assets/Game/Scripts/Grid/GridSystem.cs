using System.Collections.Generic;
using UnityEngine;

public class GridSystem
{
    private readonly GridCell[,] _cells;
    private readonly Dictionary<Vector2Int, Item> _items = new();
    public int Width { get; }
    public int Height { get; }
    public GridSystem(int width, int height)
    {
        Width = width;
        Height = height;
        _cells = new GridCell[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                _cells[x, y] = new GridCell(new Vector2Int(x, y));
    }
    public bool IsInside(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height;
    }
    public bool CanPlace(Vector2Int pos, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                var checkPos = pos + new Vector2Int(x, y);
                if (!IsInside(checkPos) || !_cells[checkPos.x, checkPos.y].IsEmpty)
                    return false;
            }
        }
        return true;
    }
    public bool TryPlace(Vector2Int pos, MachineInstance machine)
    {
        if (!CanPlace(pos, machine.Config.size))
            return false;
        for (int x = 0; x < machine.Config.size.x; x++)
            for (int y = 0; y < machine.Config.size.y; y++)
                _cells[pos.x + x, pos.y + y].TryPlace(machine);
        machine.GridPosition = pos;
        return true;
    }
    public MachineInstance RemoveAt(Vector2Int pos)
    {
        if (!IsInside(pos)) return null;
        return _cells[pos.x, pos.y].Remove();
    }
    public GridCell GetCell(Vector2Int pos)
    {
        return IsInside(pos) ? _cells[pos.x, pos.y] : null;
    }
    public IEnumerable<MachineInstance> GetAllMachines()
    {
        var seen = new HashSet<MachineInstance>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var machine = _cells[x, y].Machine;
                if (machine != null && seen.Add(machine))
                    yield return machine;
            }
        }
    }
    public bool HasItem(Vector2Int pos)
    {
        return _items.ContainsKey(pos);
    }
    public Item GetItem(Vector2Int pos)
    {
        _items.TryGetValue(pos, out var item);
        return item;
    }
    public bool TryPlaceItem(Vector2Int pos, Item item)
    {
        if (!IsInside(pos)) return false;
        if (_items.ContainsKey(pos)) return false;

        item.GridPosition = pos;
        _items[pos] = item;
        return true;
    }
    public Item RemoveItem(Vector2Int pos)
    {
        if (!_items.TryGetValue(pos, out var item))
            return null;

        _items.Remove(pos);
        return item;
    }
    public IEnumerable<Item> GetAllItems()
    {
        return _items.Values;
    }
}