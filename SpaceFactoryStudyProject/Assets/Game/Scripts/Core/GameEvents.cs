using System;
using UnityEngine;

public class GameEvents
{
    public event Action<float> OnCurrencyChanged;
    public event Action<float> OnIncomePerMinuteChanged;
    public event Action<Vector2Int, MachineInstance> OnMachinePlaced;
    public event Action<Vector2Int> OnMachineRemoved;
    public event Action<ResourceConfig, float> OnResourceSold;
    public event Action<string, int> OnUpgradeLevelChanged;
    public event Action OnSaveRequested;
    public event Action OnProgressReset;
    public void RaiseCurrencyChanged(float value) => OnCurrencyChanged?.Invoke(value);
    public void RaiseIncomeChanged(float value) => OnIncomePerMinuteChanged?.Invoke(value);
    public void RaiseMachinePlaced(Vector2Int pos, MachineInstance machine) => OnMachinePlaced?.Invoke(pos, machine);
    public void RaiseMachineRemoved(Vector2Int pos) => OnMachineRemoved?.Invoke(pos);
    public void RaiseResourceSold(ResourceConfig resource, float amount) => OnResourceSold?.Invoke(resource, amount);
    public void RaiseUpgradeChanged(string id, int level) => OnUpgradeLevelChanged?.Invoke(id, level);
    public void RaiseSaveRequested() => OnSaveRequested?.Invoke();
    public void RaiseProgressReset() => OnProgressReset?.Invoke();
}