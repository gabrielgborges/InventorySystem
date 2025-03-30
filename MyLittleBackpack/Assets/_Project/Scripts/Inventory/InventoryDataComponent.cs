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

    public bool AddPreparedItem()
    {
        if (FittedInAvailableSlot())
        {
            SetPreparedItem(null);
            return true;
        }
        
        SetPreparedItem(null);
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
            Debug.Log("Fitted in index : " + index);
            return true;
        }
        return false;
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