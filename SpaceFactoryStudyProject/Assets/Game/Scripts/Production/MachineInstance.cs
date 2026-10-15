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
        Behaviour = config.type switch
        {
            MachineType.Dispenser => new DispenserBehaviour(),
            MachineType.Conveyor  => new ConveyorBehaviour(),
            MachineType.Seller    => new SellerBehaviour(),
            _                     => new EmptyBehaviour()
        };
        Behaviour.Bind(this);
    }
}