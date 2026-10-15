using System;

public class ProductionContext
{
    public EconomySystem Economy;
    public GridSystem Grid;
    public GameEvents Events;
    public Action<MachineInstance, ResourceConfig> OnItemProduced;
}