using System;
using UnityEngine;

public class EconomySystem
{
    private float _currency;
    private float _incomePerMinute;
    private readonly EconomyConfig _config;
    private float _salesAccumulator;
    private float _salesWindowTimer;
    private const float SalesWindowSeconds = 4f;
    public float Currency => _currency;
    public float IncomePerMinute => _incomePerMinute;
    public event Action<float> OnCurrencyChanged;
    public event Action<float> OnIncomePerMinuteChanged;
    public EconomySystem(EconomyConfig config)
    {
        _config = config;
    }
    public void Initialize(float startingCurrency, float startingIncomePerMinute)
    {
        _currency = startingCurrency;
        _incomePerMinute = startingIncomePerMinute;
        OnCurrencyChanged?.Invoke(_currency);
        OnIncomePerMinuteChanged?.Invoke(_incomePerMinute);
    }
    public void AddCurrency(float amount)
    {
        if (amount == 0f) return;
        _currency += amount;
        _currency = Mathf.Floor(_currency);
        OnCurrencyChanged?.Invoke(_currency);
    }
    public bool TrySpend(float amount)
    {
        if (_currency < amount)
            return false;
        _currency -= amount;
        OnCurrencyChanged?.Invoke(_currency);
        return true;
    }
    public void RecordSale(float amount)
    {
        if (amount <= 0f) return;
        AddCurrency(amount);
        _salesAccumulator += amount;
    }
    public void Tick(float deltaTime)
    {
        _salesWindowTimer += deltaTime;
        if (_salesWindowTimer >= SalesWindowSeconds)
        {
            float incomeInWindow = _salesAccumulator;
            float newIncomePerMinute = (incomeInWindow / SalesWindowSeconds) * 60f;
            float smoothSpeed = _config != null ? _config.incomeSmoothSpeed : 1f;
            _incomePerMinute = Mathf.Lerp(_incomePerMinute, newIncomePerMinute, Mathf.Clamp01(smoothSpeed * SalesWindowSeconds));
            _salesAccumulator = 0f;
            _salesWindowTimer = 0f;
            OnIncomePerMinuteChanged?.Invoke(_incomePerMinute);
        }
    }
    public void SetIncomePerMinute(float value)
    {
        _incomePerMinute = Mathf.Max(0f, value);
        OnIncomePerMinuteChanged?.Invoke(_incomePerMinute);
    }
    public void ApplyOfflineIncome(DateTime lastExitTime)
    {
        if (_incomePerMinute <= 0f) return;
        double offlineSeconds = (DateTime.UtcNow - lastExitTime).TotalSeconds;
        float maxSeconds = _config.maxOfflineHours * 3600f;
        offlineSeconds = Math.Min(offlineSeconds, maxSeconds);
        float offlineCurrency = (float)((_incomePerMinute / 60f) * offlineSeconds);
        if (offlineCurrency > 0f)
        {
            AddCurrency(offlineCurrency);
            Debug.Log($"[Economy] Оффлайн-доход: +{offlineCurrency:F1} (за {offlineSeconds:F0} сек.)");
        }
    }
    public void ManualClick()
    {
        AddCurrency(_config.manualClickAmount);
    }
}