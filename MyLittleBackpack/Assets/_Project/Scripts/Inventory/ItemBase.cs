using UnityEngine;

namespace MLB.Inventory
{
    public class ItemBase
    {
        protected ItemData _data;
        protected string ID;

        public ItemBase(ItemData data, string id)
        {
            _data = data;
            ID = id;
        }
        
        protected virtual void Setup(){}
    }
}

