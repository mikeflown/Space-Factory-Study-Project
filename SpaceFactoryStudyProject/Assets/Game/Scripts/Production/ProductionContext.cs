using System;
using UnityEngine;

public class ProductionContext
{
    public EconomySystem Economy;
    public GridSystem Grid;
    public GameEvents Events;
    public Action<MachineInstance, ResourceConfig> OnItemProduced;
    public Action<Vector2Int> OnItemSold;
}