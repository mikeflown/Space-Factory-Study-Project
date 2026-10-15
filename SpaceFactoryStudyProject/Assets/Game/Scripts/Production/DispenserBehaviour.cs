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
        if (_outputResource == null) return;
        _timer += deltaTime;
        if (_timer < _interval) return;
        _timer = 0f;
        if (!context.Economy.TrySpend(_costPerItem))
        {
            Debug.Log("[Dispenser] Недостаточно валюты для производства");
            return;
        }
        Debug.Log($"[Dispenser] Потратил {_costPerItem} валюты и выдал {_outputResource.displayName}");
        context.OnItemProduced?.Invoke(_owner, _outputResource);
    }
}