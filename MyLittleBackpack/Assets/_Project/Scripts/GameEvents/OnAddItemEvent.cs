using MLB.Inventory.Items;

public class OnAddItemEvent : GameEventBase
{
    public ItemData Item;

    public OnAddItemEvent(ItemData item)
    {
        Item = item;
    }
}
