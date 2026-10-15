using UnityEngine;
using UnityEngine.InputSystem;

public class GridView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Color gridColor = new Color(1f, 1f, 1f, 0.15f);
    [SerializeField] private Color hoverColor = new Color(0.3f, 0.8f, 1f, 0.35f);
    [Header("References")]
    [SerializeField] private Transform cellsParent;
    [SerializeField] private GameObject cellPrefab;
    [Header("Building")]
    [SerializeField] private GameObject machinePrefab;
    private MachineConfig _selectedMachineConfig;
    private GridSystem _grid;
    private GameEvents _events;
    private EconomySystem _economy;
    private Camera _camera;
    private GameObject[,] _visualCells;
    private Vector2Int? _hoveredCell;
    private Transform _machinesParent;
    public void Initialize(GridSystem grid, GameEvents events, EconomySystem economy)
    {
        _grid = grid;
        _events = events;
        _economy = economy;
        _camera = Camera.main;
        CreateVisualGrid();
        var machinesGo = new GameObject("Machines");
        machinesGo.transform.SetParent(transform);
        _machinesParent = machinesGo.transform;
    }
    public void SpawnMachineVisual(MachineInstance machine, Vector2Int pos)
    {
        if (machinePrefab == null) return;
        GameObject visual = Instantiate(machinePrefab, _machinesParent);
        visual.name = $"Machine_{machine.Config.id}_{pos.x}_{pos.y}";
        Vector3 offset = new Vector3(
            -(_grid.Width * cellSize) / 2f + cellSize / 2f,
            -(_grid.Height * cellSize) / 2f + cellSize / 2f, -0.1f);
        visual.transform.position = offset + new Vector3(pos.x * cellSize, pos.y * cellSize, 0f);
        visual.transform.localScale = Vector3.one * (cellSize * 0.9f);
        var renderer = visual.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            if (machine.Config.icon != null)
                renderer.sprite = machine.Config.icon;
            renderer.color = GetColorByType(machine.Config.type);
        }
        var marker = visual.AddComponent<MachineVisualMarker>();
        marker.GridPosition = pos;
    }
    public void SetSelectedMachine(MachineConfig config)
    {
        _selectedMachineConfig = config;
    }
    private void CreateVisualGrid()
    {
        if (cellsParent == null)
        {
            var parentGo = new GameObject("GridCells");
            parentGo.transform.SetParent(transform);
            cellsParent = parentGo.transform;
        }
        _visualCells = new GameObject[_grid.Width, _grid.Height];
        Vector3 offset = new Vector3(
            -(_grid.Width * cellSize) / 2f + cellSize / 2f,
            -(_grid.Height * cellSize) / 2f + cellSize / 2f, 0f);
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                GameObject cell;
                if (cellPrefab != null)
                {
                    cell = Instantiate(cellPrefab, cellsParent);
                }
                else
                {
                    cell = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    cell.transform.SetParent(cellsParent);
                    var col = cell.GetComponent<Collider>();
                    if (col != null) Destroy(col);
                }
                cell.name = $"Cell_{x}_{y}";
                cell.transform.localPosition = offset + new Vector3(x * cellSize, y * cellSize, 0f);
                cell.transform.localScale = Vector3.one * cellSize;
                var renderer = cell.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = gridColor;
                }
                _visualCells[x, y] = cell;
            }
        }
        Debug.Log($"[GridView] Визуальная сетка создана: {_grid.Width}x{_grid.Height}");
    }
    private void Update()
    {
        if (_grid == null || _camera == null) return;
        HandleHover();
        HandleClick();
    }
    private void HandleHover()
    {
        Vector2Int? cell = GetCellUnderMouse();
        if (_hoveredCell.HasValue && _hoveredCell != cell)
        {
            SetCellColor(_hoveredCell.Value, gridColor);
        }
        if (cell.HasValue)
        {
            SetCellColor(cell.Value, hoverColor);
        }
        _hoveredCell = cell;
    }
    private void HandleClick()
    {
        if (Mouse.current == null) return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!_hoveredCell.HasValue) return;
            TryPlaceTestMachine(_hoveredCell.Value);
            return;
        }
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (!_hoveredCell.HasValue) return;
            TryRemoveMachine(_hoveredCell.Value);
        }
    }
    private Vector2Int? GetCellUnderMouse()
    {
        if (Mouse.current == null) return null;
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = _camera.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, 0f));
        mouseWorld.z = 0f;
        Vector3 offset = new Vector3(-(_grid.Width * cellSize) / 2f + cellSize / 2f, -(_grid.Height * cellSize) / 2f + cellSize / 2f, 0f);
        Vector3 local = mouseWorld - offset;
        int x = Mathf.RoundToInt(local.x / cellSize);
        int y = Mathf.RoundToInt(local.y / cellSize);
        if (_grid.IsInside(new Vector2Int(x, y)))
            return new Vector2Int(x, y);
        return null;
    }
    private void SetCellColor(Vector2Int pos, Color color)
    {
        if (_visualCells == null) return;
        if (pos.x < 0 || pos.x >= _grid.Width || pos.y < 0 || pos.y >= _grid.Height) return;
        var renderer = _visualCells[pos.x, pos.y].GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = color;
    }
    private void TryPlaceTestMachine(Vector2Int pos)
{
    if (_selectedMachineConfig == null || machinePrefab == null)
    {
        Debug.LogWarning("[GridView] Не выбрана машина или не назначен MachinePrefab");
        return;
    }
    if (_economy == null)
    {
        Debug.LogWarning("[GridView] EconomySystem не передан");
        return;
    }
    var cell = _grid.GetCell(pos);
    if (cell == null || !cell.IsEmpty)
    {
        Debug.Log("[GridView] Клетка занята");
        return;
    }
    float cost = _selectedMachineConfig.baseCost;
    if (!_economy.TrySpend(cost))
    {
        Debug.Log($"[GridView] Недостаточно валюты (нужно {cost})");
        return;
    }
    var machineInstance = new MachineInstance(_selectedMachineConfig);
    if (!_grid.TryPlace(pos, machineInstance))
    {
        _economy.AddCurrency(cost);
        Debug.Log("[GridView] Не удалось разместить машину");
        return;
    }
    GameObject visual = Instantiate(machinePrefab, _machinesParent);
    visual.name = $"Machine_{_selectedMachineConfig.id}_{pos.x}_{pos.y}";
    Vector3 offset = new Vector3(
        -(_grid.Width * cellSize) / 2f + cellSize / 2f,
        -(_grid.Height * cellSize) / 2f + cellSize / 2f, -0.1f);
    visual.transform.position = offset + new Vector3(pos.x * cellSize, pos.y * cellSize, 0f);
    visual.transform.localScale = Vector3.one * (cellSize * 0.9f);
    var renderer = visual.GetComponent<SpriteRenderer>();
    if (renderer != null)
    {
        if (_selectedMachineConfig.icon != null)
            renderer.sprite = _selectedMachineConfig.icon;
        renderer.color = GetColorByType(_selectedMachineConfig.type);
    }
    var marker = visual.AddComponent<MachineVisualMarker>();
    marker.GridPosition = pos;
    _events.RaiseMachinePlaced(pos, machineInstance);
    Debug.Log($"[GridView] Машина {_selectedMachineConfig.displayName} установлена в {pos} за {cost}");
}
private Color GetColorByType(MachineType type)
{
    return type switch
    {
        MachineType.Dispenser => new Color(1f, 1f, 1f),
        MachineType.Conveyor  => new Color(1f, 1f, 1f),
        MachineType.Smelter   => new Color(1f, 1f, 1f),
        MachineType.Cutter    => new Color(1f, 1f, 1f),
        MachineType.Press     => new Color(1f, 1f, 1f), 
        MachineType.Assembler => new Color(1f, 1f, 1f),
        MachineType.Seller    => new Color(1f, 1f, 1f),
        _ => Color.white
    };
}
    private void TryRemoveMachine(Vector2Int pos)
    {
        var cell = _grid.GetCell(pos);
        if (cell == null || cell.IsEmpty)
        {
            Debug.Log("[GridView] Нечего удалять");
            return;
        }
        var machine = _grid.RemoveAt(pos);
        if (machine == null) return;
        if (_machinesParent != null)
        {
            foreach (Transform child in _machinesParent)
            {
                var marker = child.GetComponent<MachineVisualMarker>();
                if (marker != null && marker.GridPosition == pos)
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
        }
        float refund = machine.Config.baseCost * 0.5f;
        _economy.AddCurrency(refund);
        _events.RaiseMachineRemoved(pos);
        Debug.Log($"[GridView] Машина удалена из {pos}. Возврат: {refund}");
    }
}