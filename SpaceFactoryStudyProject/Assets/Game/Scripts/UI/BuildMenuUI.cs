using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildMenuUI : MonoBehaviour
{
    [System.Serializable]
    public class BuildButton
    {
        public Button button;
        public MachineConfig config;
        public TextMeshProUGUI label;
    }
    [SerializeField] private BuildButton[] buttons;
    [SerializeField] private TextMeshProUGUI selectedText;
    private MachineConfig _selectedConfig;
    private GridView _gridView;
    public MachineConfig SelectedConfig => _selectedConfig;
    public void Initialize(GridView gridView)
    {
        _gridView = gridView;
        foreach (var btn in buttons)
        {
            if (btn.button == null || btn.config == null) continue;
            var config = btn.config;
            btn.button.onClick.AddListener(() => SelectMachine(config));
            if (btn.label != null)
                btn.label.text = $"{btn.config.displayName}\n{btn.config.baseCost}";
        }
        if (buttons.Length > 0 && buttons[0].config != null)
            SelectMachine(buttons[0].config);
    }
    private void SelectMachine(MachineConfig config)
    {
        _selectedConfig = config;
        if (selectedText != null)
            selectedText.text = $"Выбрано: {config.displayName}";
        if (_gridView != null)
            _gridView.SetSelectedMachine(config);
        Debug.Log($"[BuildMenu] Выбрана машина: {config.displayName}");
    }
}