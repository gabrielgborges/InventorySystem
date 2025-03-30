public class OnRemoveInventoryItemEvent : GameEventBase
{
    public ItemSlot Item;

    public OnRemoveInventoryItemEvent(ItemSlot item)
    {
        Item = item;
    }
}
