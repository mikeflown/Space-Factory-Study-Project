using System;
using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] private EconomyConfig economyConfig;
    [Header("Machine Configs")]
    [SerializeField] private MachineConfig[] allMachineConfigs;
    [Header("Resources")]
    [SerializeField] private ResourceConfig ironResource;
    [Header("UI")]
    [SerializeField] private EconomyUI economyUI;
    [SerializeField] private BuildMenuUI buildMenuUI;
    [Header("Clicker")]
    [SerializeField] private ManualClicker manualClickerPrefab;
    [Header("Grid")]
    [SerializeField] private int gridWidth = 12;
    [SerializeField] private int gridHeight = 8;
    [Header("Grid View")]
    [SerializeField] private GridView gridView;
    private SaveSystem _saveSystem;
    private SaveData _currentSave;
    private EconomySystem _economy;
    private GameEvents _events;
    private GridSystem _grid;
    private ProductionSystem _production;
    private void Awake()
    {
        _saveSystem = new SaveSystem();
        _events = new GameEvents();
        _economy = new EconomySystem(economyConfig);
        _currentSave = _saveSystem.Load();
        if (_currentSave == null)
        {
            _currentSave = new SaveData
            {
                currency = 0f,
                lastIncomePerMinute = 0f,
                lastExitTicks = DateTime.UtcNow.Ticks
            };
            _economy.Initialize(0f, 0f);
            Debug.Log("[Bootstrap] Создан новый сейв");
        }
        else
        {
            _economy.Initialize(_currentSave.currency, _currentSave.lastIncomePerMinute);
            DateTime lastExit = new DateTime(_currentSave.lastExitTicks, DateTimeKind.Utc);
            _economy.ApplyOfflineIncome(lastExit);
            Debug.Log($"[Bootstrap] Сейв загружен. Валюта: {_economy.Currency:F1}");
        }
        _grid = new GridSystem(gridWidth, gridHeight);
        Debug.Log($"[Bootstrap] Сетка создана: {_grid.Width}x{_grid.Height}");
        _production = new ProductionSystem(_grid, _economy, _events);
        _production.GridView = gridView;
        if (gridView != null)
            gridView.Initialize(_grid, _events, _economy);
        if (buildMenuUI != null && gridView != null)
            buildMenuUI.Initialize(gridView);
        if (_currentSave != null && _currentSave.grid != null)
            RestoreGrid(_currentSave.grid);
        if (economyUI != null)
            economyUI.Initialize(_economy);
        if (manualClickerPrefab != null)
        {
            ManualClicker clicker = Instantiate(manualClickerPrefab);
            clicker.Initialize(_economy);
        }
        _events.OnMachinePlaced += OnMachinePlaced;
        _events.OnMachineRemoved += OnMachineRemoved;
    }
    private void Update()
    {
        _economy.Tick(Time.deltaTime);
        _production?.Tick(Time.deltaTime);
    }
    private void OnMachinePlaced(Vector2Int pos, MachineInstance machine)
    {
        if (machine.Config.type == MachineType.Dispenser && machine.Behaviour is DispenserBehaviour dispenser)
        {
            dispenser.Setup(ironResource, 2f, 1f);
        }
        _production.Register(machine);
        Debug.Log($"[Bootstrap] Машина зарегистрирована: {machine.Config.displayName}");
    }
    private void OnMachineRemoved(Vector2Int pos, MachineInstance machine)
    {
        _production.Unregister(machine);
        Debug.Log($"[Bootstrap] Машина разрегистрирована: {machine.Config.displayName}");
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    private void OnDestroy()
    {
        if (_events != null)
        {
            _events.OnMachinePlaced -= OnMachinePlaced;
            _events.OnMachineRemoved -= OnMachineRemoved;
        }
    }
    private void SaveGame()
    {
        if (_currentSave == null || _economy == null) return;
        _currentSave.currency = _economy.Currency;
        _currentSave.lastIncomePerMinute = _economy.IncomePerMinute;
        _currentSave.grid = CollectGridSave();
        _saveSystem.Save(_currentSave);
        Debug.Log($"[Bootstrap] Игра сохранена. Машин на сетке: {_currentSave.grid.Count}");
    }
    private List<GridCellSave> CollectGridSave()
    {
        var list = new List<GridCellSave>();
        if (_grid == null) return list;
        foreach (var machine in _grid.GetAllMachines())
        {
            list.Add(new GridCellSave
            {
                x = machine.GridPosition.x,
                y = machine.GridPosition.y,
                machineId = machine.Config.id,
                recipeId = machine.SelectedRecipeId
            });
        }
        return list;
    }
    private void RestoreGrid(List<GridCellSave> savedCells)
    {
        if (savedCells == null || savedCells.Count == 0) return;
        if (gridView == null) return;
        foreach (var cellSave in savedCells)
        {
            MachineConfig config = null;
            foreach (var cfg in allMachineConfigs)
            {
                if (cfg != null && cfg.id == cellSave.machineId)
                {
                    config = cfg;
                    break;
                }
            }
            if (config == null)
            {
                Debug.LogWarning($"[Bootstrap] Не найден MachineConfig с id: {cellSave.machineId}");
                continue;
            }
            Vector2Int pos = new Vector2Int(cellSave.x, cellSave.y);
            var machineInstance = new MachineInstance(config);
            machineInstance.SelectedRecipeId = cellSave.recipeId;
            if (!_grid.TryPlace(pos, machineInstance))
            {
                Debug.LogWarning($"[Bootstrap] Не удалось восстановить машину в {pos}");
                continue;
            }
            if (config.type == MachineType.Dispenser && machineInstance.Behaviour is DispenserBehaviour dispenser)
            {
                dispenser.Setup(ironResource, 2f, 1f);
            }
            _production.Register(machineInstance);
            gridView.SpawnMachineVisual(machineInstance, pos);
        }
        Debug.Log($"[Bootstrap] Восстановлено машин: {savedCells.Count}");
    }
    [ContextMenu("Delete Save")]
    private void DeleteSaveFromContext()
    {
        if (_saveSystem == null)
            _saveSystem = new SaveSystem();
        _saveSystem.DeleteSave();
        Debug.Log("Сейв удалён через контекстное меню");
    }
    [ContextMenu("Add 100 Currency")]
    private void AddTestCurrency()
    {
        _economy?.AddCurrency(100f);
    }
    [ContextMenu("Save Game Now")]
    private void SaveGameFromContext()
    {
        SaveGame();
    }
}