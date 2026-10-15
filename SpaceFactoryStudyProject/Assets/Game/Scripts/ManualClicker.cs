using UnityEngine;

public class ManualClicker : MonoBehaviour
{
    [SerializeField] private ClickerConfig config;
    private EconomySystem _economy;
    public void Initialize(EconomySystem economy)
    {
        _economy = economy;
    }
    private void OnMouseDown()
    {
        if (_economy == null || config == null) return;
        _economy.AddCurrency(config.currencyPerClick);
        Debug.Log($"Клик! +{config.currencyPerClick}");
    }
}