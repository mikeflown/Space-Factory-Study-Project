using System;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] private EconomyConfig economyConfig;
    [Header("UI")]
    [SerializeField] private EconomyUI economyUI;
    [Header("Clicker")]
    [SerializeField] private ManualClicker manualClickerPrefab;
    private SaveSystem _saveSystem;
    private SaveData _currentSave;
    private EconomySystem _economy;
    private void Awake()
    {
        _saveSystem = new SaveSystem();
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
        if (economyUI != null)
            economyUI.Initialize(_economy);
        if (manualClickerPrefab != null)
        {
            ManualClicker clicker = Instantiate(manualClickerPrefab);
            clicker.Initialize(_economy);
        }
    }
    private void Update()
    {
        _economy.Tick(Time.deltaTime);
    }
    private void OnApplicationQuit()
    {
        if (_currentSave == null || _economy == null) return;
        _currentSave.currency = _economy.Currency;
        _currentSave.lastIncomePerMinute = _economy.IncomePerMinute;
        _saveSystem.Save(_currentSave);
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
}