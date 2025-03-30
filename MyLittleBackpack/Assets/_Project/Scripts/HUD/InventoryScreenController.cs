using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryScreenController : ScreenControllerBase
{
    [FormerlySerializedAs("slots")] [SerializeField] List<InventorySlotUIComponent> _initialSlots = new List<InventorySlotUIComponent>();
    
    public void SetupInventory(List<ItemSlot> items)
    {
        for (int i = 0; i < _initialSlots.Count; i++)
        {
            bool itemsRemaining = i < items.Count;
            if (itemsRemaining)
            {
                _initialSlots[i].Setup(items[i]);
                continue;
            }
            _initialSlots[i].gameObject.SetActive(false);
        }
    }
    
    
}
