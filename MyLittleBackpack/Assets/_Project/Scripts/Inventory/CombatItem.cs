using UnityEngine;

namespace MLB.Inventory.Items
{
public class CombatItem : ItemBase
{
    private int _damage;
    private readonly int _minDamage = 0;
    private readonly int _maxDamage = 100;

    public int Damage => _damage;

    public CombatItem(ItemData data, string id) : base(data, id)
    {
        Setup();
    }

    protected override void Setup()
    {
        _damage = Random.Range(_minDamage, _maxDamage);
    }
}
}