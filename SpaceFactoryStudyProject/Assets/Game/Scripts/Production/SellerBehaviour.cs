using UnityEngine;

public class SellerBehaviour : IMachineBehaviour
{
    private MachineInstance _owner;
    private float _timer;
    private float _interval = 0.6f;
    public void Bind(MachineInstance owner)
    {
        _owner = owner;
    }
    public void Tick(float deltaTime, ProductionContext context)
    {
        _timer += deltaTime;
        if (_timer < _interval) return;
        _timer = 0f;
        Vector2Int pos = _owner.GridPosition;
        if (!context.Grid.HasItem(pos))
            return;
        var item = context.Grid.RemoveItem(pos);
        if (item == null)
        {
            Debug.LogWarning("[Seller] HasItem = true, но RemoveItem вернул null");
            return;
        }
        float price = item.Resource.baseSellPrice;
        context.Economy.RecordSale(price);
        context.Events.RaiseResourceSold(item.Resource, price);
        context.OnItemSold?.Invoke(pos);
        Debug.Log($"[Seller] Продал {item.Resource.displayName} за {price}");
    }
}