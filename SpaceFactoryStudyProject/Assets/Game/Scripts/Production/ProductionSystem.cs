using System.Collections.Generic;
using UnityEngine;

public class ProductionSystem
{
    private readonly GridSystem _grid;
    private readonly EconomySystem _economy;
    private readonly GameEvents _events;
    private readonly List<MachineInstance> _machines = new();
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
            OnItemProduced = HandleItemProduced
        };
        foreach (var machine in _machines)
        {
            machine.Behaviour?.Tick(deltaTime, context);
        }
    }
    private void HandleItemProduced(MachineInstance machine, ResourceConfig resource)
    {
        Vector2Int pos = machine.GridPosition;
        if (_grid.HasItem(pos))
        {
            Debug.Log("[Production] Клетка занята предметом, ресурс не создан");
            return;
        }
        var item = new Item(resource, pos);
        if (_grid.TryPlaceItem(pos, item))
        {
            Debug.Log($"[Production] Создан предмет {resource.displayName} в {pos}");
        }
    }
}