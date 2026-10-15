using UnityEngine;

public class GridCell
{
    public Vector2Int Position { get; }
    public MachineInstance Machine { get; private set; }
    public bool IsEmpty => Machine == null;
    public GridCell(Vector2Int position)
    {
        Position = position;
    }
    public bool TryPlace(MachineInstance machine)
    {
        if (!IsEmpty) return false;
        Machine = machine;
        return true;
    }
    public MachineInstance Remove()
    {
        var machine = Machine;
        Machine = null;
        return machine;
    }
}