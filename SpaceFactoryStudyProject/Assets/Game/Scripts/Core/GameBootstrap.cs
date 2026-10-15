using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] private EconomyConfig economyConfig;
    [SerializeField] private UpgradeConfig[] upgradeConfigs;

    private void Awake()
    {
        Debug.Log("Bootstrap Awake — все системы будут инициализироваться здесь");
    }
}