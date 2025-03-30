using UnityEngine;

namespace MLB.Inventory.Items
{
public class ConsumableItem : ItemBase
{
    private int _amount;
    private readonly int _minAmount = 0;
    private readonly int _maxAmount = 100;

    public int Amount => _amount;

    public ConsumableItem(ItemData data, string id) : base(data, id)
    {
        Setup();
    }

    protected override void Setup()
    {
        _amount = Random.Range(_minAmount, _maxAmount);
    }
}
}