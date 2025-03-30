using System;
using System.Collections.Generic;
using MLB.Inventory;
using MLB.Inventory.Items;
using UnityEngine;

public class InventoryDataComponent : MonoBehaviour
{
    [SerializeField] private InventoryData _startData;

    private ItemData _arrivedItem;
    private List<ItemSlot> _currentItemSlots = new List<ItemSlot>();

    public bool HoldingItem => _arrivedItem != null;

    public List<ItemSlot> CurrentItemSlots => _currentItemSlots;
    
    public void Initialize()
    {
        foreach (ItemType itemType in _startData.InventorySlots)
        {
            _currentItemSlots.Add(new ItemSlot(null, _startData.GetSprite(itemType),
                _startData.GetSprite(itemType), itemType));
        }
    }

    public void SetPreparedItem(IStoreableItem item)
    {
        _arrivedItem = item != null ? item.ItemData : null;
    }

    public void RemovePreparedItem()
    {
        _arrivedItem = null;
    }

    public bool AddPreparedItem()
    {
        if (FittedInAvailableSlot())
        {
            ConfirmAddingItem();
            SetPreparedItem(null);
            return true;
        }
        
        SetPreparedItem(null);
        return false;
    }

    public bool RemoveItem(ItemData item)
    {
        for (var index = 0; index < CurrentItemSlots.Count; index++)
        {
            var itemSlot = CurrentItemSlots[index];
            if (itemSlot.ItemData == item)
            {
                _currentItemSlots[index] = new ItemSlot(null, itemSlot.EmptySprite,
                    itemSlot.EmptySprite, itemSlot.ItemType);
                return true;
            }
        }

        return false;
    }

    private bool FittedInAvailableSlot()
    {
        int index = _currentItemSlots.FindIndex(slot => slot.ItemData == null && slot.ItemType == _arrivedItem.ItemType);
        if (index != -1)
        {
            ItemSlot updatedSlot = new ItemSlot(_arrivedItem, _arrivedItem.Sprite,
                _startData.GetSprite(_arrivedItem.ItemType), _currentItemSlots[index].ItemType);
            
            _currentItemSlots[index] = updatedSlot;
            return true;
        }
        return false;
    }

    private async void ConfirmAddingItem()
    {
        IEventService eventService = await ServiceLocator.GetService<IEventService>();
        eventService.TryInvokeEvent(new OnAddItemEvent(_arrivedItem));  
    }
}

[Serializable]
public struct ItemSlot
{
    public ItemData ItemData;
    public Sprite CurrentSprite;
    public Sprite EmptySprite;
    public ItemType ItemType;

    public ItemSlot(ItemData itemData, Sprite currentSprite, Sprite emptySprite, ItemType itemType)
    {
        ItemData = itemData;
        CurrentSprite = currentSprite;
        EmptySprite = emptySprite;
        ItemType = itemType;
    }
}