using UnityEngine;

public class ConveyorBehaviour : IMachineBehaviour
{
    private MachineInstance _owner;
    private float _timer;
    private float _moveInterval = 0.4f;
    public void Bind(MachineInstance owner)
    {
        _owner = owner;
    }
    public void Tick(float deltaTime, ProductionContext context)
    {
        if (_owner == null || _owner.Config == null) return;
        _timer += deltaTime;
        if (_timer < _moveInterval) return;
        _timer = 0f;
        Vector2Int currentPos = _owner.GridPosition;
        if (!context.Grid.HasItem(currentPos))
            return;
        Vector2Int nextPos = GetNextPosition(currentPos, _owner.Config.direction);
        if (context.Grid.TryMoveItem(currentPos, nextPos))
        {
            var item = context.Grid.GetItem(nextPos);
            context.OnItemMoved?.Invoke(currentPos, nextPos, item);
            Debug.Log($"[Conveyor] Перенёс предмет с {currentPos} → {nextPos}");
        }
    }
    private Vector2Int GetNextPosition(Vector2Int pos, ConveyorDirection dir)
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