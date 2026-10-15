using System;
using UnityEngine;

public class MachineInstance
{
    public string InstanceId { get; } = Guid.NewGuid().ToString();
    public MachineConfig Config { get; }
    public IMachineBehaviour Behaviour { get; }
    public Vector2Int GridPosition { get; set; }
    public string SelectedRecipeId { get; set; }
    public MachineInstance(MachineConfig config)
    {
        Config = config;
        if (config.type == MachineType.Dispenser)
            Behaviour = new DispenserBehaviour();
        else
            Behaviour = new EmptyBehaviour();
        Behaviour.Bind(this);
    }
}