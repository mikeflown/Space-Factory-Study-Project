using System;
using UnityEngine;

public class MachineInstance
{
    public string InstanceId { get; } = Guid.NewGuid().ToString();
    public MachineConfig Config { get; }
    public Vector2Int GridPosition { get; set; }
    public string SelectedRecipeId { get; set; }
    public MachineInstance(MachineConfig config)
    {
        Config = config;
    }
}