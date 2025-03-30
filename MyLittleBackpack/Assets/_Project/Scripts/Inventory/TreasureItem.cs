using UnityEngine;

namespace MLB.Inventory.Items
{
    public class TreasureItem : ItemBase
    {
        private int _value;
        private readonly int _minValue = 0;
        private readonly int _maxValue = 100;

        public int Value => _value;

        public TreasureItem(ItemData data, string id) : base(data, id)
        {
           Setup();
        }

        protected override void Setup()
        {
            _value = Random.Range(_minValue, _maxValue);
        }
    }
}