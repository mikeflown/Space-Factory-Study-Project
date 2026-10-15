using UnityEngine;

[CreateAssetMenu(fileName = "NewMachine", menuName = "Game/Machine Config")]
public class MachineConfig : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public MachineType type;
    public int baseCost = 10;
    public float costMultiplier = 1.5f;
    public int unlockAtCurrency = 0;
    public Vector2Int size = Vector2Int.one;
    [Header("Conveyor Settings")]
    public ConveyorDirection direction = ConveyorDirection.Right;
}
public enum MachineType
{
    Dispenser,
    Conveyor,
    Smelter,
    Cutter,
    Press,
    Assembler,
    Seller
}
public enum ConveyorDirection
{
    Right,
    Left,
    Up,
    Down
}