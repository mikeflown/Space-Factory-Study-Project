using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public float currency;
    public float lastIncomePerMinute;
    public long lastExitTicks;
    public List<UpgradeSave> upgrades = new();
    public List<GridCellSave> grid = new();
}

[Serializable]
public class UpgradeSave
{
    public string id;
    public int level;
}

[Serializable]
public class GridCellSave
{
    public int x;
    public int y;
    public string machineId;
    public string recipeId;
}