namespace MLB.Inventory
{
    public class TreasureItem : ItemBase
    {
        private int _value;
        
        public TreasureItem(ItemData data,string id, int value) : base(data, id)
        {
            _value = value;
        }
    }
}