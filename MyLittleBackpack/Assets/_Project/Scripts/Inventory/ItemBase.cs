using UnityEngine;

namespace MLB.Inventory.Items
{
    public class ItemBase
    {
        protected ItemData _data;

        public ItemData Data => _data;

        public ItemBase(ItemData data, string id)
        {
            _data = data;
            _data.ID = id;
        }
        
        protected virtual void Setup(){}
    }
}

