using UnityEngine;

namespace MLB.Inventory.Items
{
    public class ItemBase
    {
        protected ItemData _data;
        protected string ID;

        public ItemData Data => _data;

        public ItemBase(ItemData data, string id)
        {
            _data = data;
            ID = id;
        }
        
        protected virtual void Setup(){}
    }
}

