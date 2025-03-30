using MLB.Inventory.Items;

public class OnSelectItemSlotEvent : GameEventBase
{
    public ItemSlot Item;

    public OnSelectItemSlotEvent(ItemSlot item)
    {
        Item = item;
    }
}
