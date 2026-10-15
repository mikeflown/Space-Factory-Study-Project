using System.Collections.Generic;
using UnityEngine;

public class ProductionSystem
{
    private readonly GridSystem _grid;
    private readonly EconomySystem _economy;
    private readonly GameEvents _events;
    private readonly List<MachineInstance> _machines = new();
    public GridView GridView;
    public ProductionSystem(GridSystem grid, EconomySystem economy, GameEvents events)
    {
        _grid = grid;
        _economy = economy;
        _events = events;
    }
    public void Register(MachineInstance machine)
    {
        if (!_machines.Contains(machine))
            _machines.Add(machine);
    }
    public void Unregister(MachineInstance machine)
    {
        _machines.Remove(machine);
    }
    public void Tick(float deltaTime)
    {
        var context = new ProductionContext
        {
            Economy = _economy,
            Grid = _grid,
            Events = _events,
            OnItemProduced = HandleItemProduced,
            OnItemSold = HandleItemSold,
            OnItemMoved = HandleItemMoved
        };
        foreach (var machine in _machines)
        {
            machine.Behaviour?.Tick(deltaTime, context);
        }
    }
    private void HandleItemMoved(Vector2Int from, Vector2Int to, Item item)
    {
        GridView?.RemoveItemVisual(from);
        GridView?.SpawnItemVisual(item);
    }
    private void HandleItemProduced(MachineInstance machine, ResourceConfig resource)
    {
        Vector2Int outputPos = machine.GridPosition;
        if (machine.Config.type == MachineType.Dispenser)
        {
            outputPos = GetOutputPosition(machine.GridPosition, machine.Config.direction);
        }
        if (_grid.HasItem(outputPos))
            return;
        var item = new Item(resource, outputPos);
        if (_grid.TryPlaceItem(outputPos, item))
        {
            Debug.Log($"[Production] Создан предмет {resource.displayName} в {outputPos}");
            GridView?.SpawnItemVisual(item);
        }
    }
    private Vector2Int GetOutputPosition(Vector2Int pos, ConveyorDirection dir)
    {
        return dir switch
        {
            ConveyorDirection.Right => pos + Vector2Int.right,
            ConveyorDirection.Left  => pos + Vector2Int.left,
            ConveyorDirection.Up    => pos + Vector2Int.up,
            ConveyorDirection.Down  => pos + Vector2Int.down,
            _ => pos
        };
    }
    private void HandleItemSold(Vector2Int pos)
    {
        GridView?.RemoveItemVisual(pos);
    }
}