using TMPro;
using UnityEngine;

public class EconomyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI incomeText;
    private EconomySystem _economy;
    public void Initialize(EconomySystem economy)
    {
        _economy = economy;
        _economy.OnCurrencyChanged += UpdateCurrency;
        _economy.OnIncomePerMinuteChanged += UpdateIncome;
        UpdateCurrency(_economy.Currency);
        UpdateIncome(_economy.IncomePerMinute);
    }
    private void UpdateCurrency(float value)
    {
        if (currencyText != null)
            currencyText.text = $"Валюта: {Mathf.FloorToInt(value)}";
    }
    private void UpdateIncome(float value)
    {
        if (incomeText != null)
            incomeText.text = $"Доход: {Mathf.FloorToInt(value)} / мин";
    }
    private void OnDestroy()
    {
        if (_economy == null) return;
        _economy.OnCurrencyChanged -= UpdateCurrency;
        _economy.OnIncomePerMinuteChanged -= UpdateIncome;
    }
}