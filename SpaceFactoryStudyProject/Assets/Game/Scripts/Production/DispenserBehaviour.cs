using UnityEngine;

public class DispenserBehaviour : IMachineBehaviour
{
    private MachineInstance _owner;
    private float _timer;
    private float _interval = 2f;
    private float _costPerItem = 1f;
    private ResourceConfig _outputResource;
    public void Bind(MachineInstance owner)
    {
        _owner = owner;
    }
    public void Setup(ResourceConfig outputResource, float interval = 2f, float costPerItem = 1f)
    {
        _outputResource = outputResource;
        _interval = interval;
        _costPerItem = costPerItem;
    }
    public void Tick(float deltaTime, ProductionContext context)
    {
        if (_outputResource == null || _owner == null || _owner.Config == null)
            return;
        _timer += deltaTime;
        if (_timer < _interval) return;
        _timer = 0f;
        Vector2Int outputPos = GetOutputPosition(_owner.GridPosition, _owner.Config.direction);
        if (!context.Grid.IsInside(outputPos))
        {
            Debug.Log("[Dispenser] Выход за пределы сетки");
            return;
        }
        if (context.Grid.HasItem(outputPos))
            return;
        if (!context.Economy.TrySpend(_costPerItem))
        {
            Debug.Log("[Dispenser] Недостаточно валюты для производства");
            return;
        }
        Debug.Log($"[Dispenser] Потратил {_costPerItem} и выдал {_outputResource.displayName} в {outputPos}");
        context.OnItemProduced?.Invoke(_owner, _outputResource);
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
}