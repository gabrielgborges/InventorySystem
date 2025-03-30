using System.Collections.Generic;
using MLB.Inventory.Items;
using UnityEngine;

public class InventoryScreenController : ScreenControllerBase
{
    [SerializeField] List<InventorySlotUIComponent> slots = new List<InventorySlotUIComponent>();
    
    public void SetupInventory(List<ItemSlot> items)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count)
            {
                slots[i].Setup(items[i]);
                continue;
            }
            slots[i].gameObject.SetActive(false);
        }
    }
}
