using System;
using MLB.Inventory;
using MLB.Inventory.Items;
using UnityEngine;

public class SingleItemSpawner : MonoBehaviour
{
    public void Spawn(ItemData itemData)
    {
        ItemController itemController = Instantiate(itemData.Prefab, transform).GetComponent<ItemController>();
        switch (itemData.ItemType)
        {
            case ItemType.TREASURE:
                TreasureItem treasureItem = new TreasureItem(itemData, Guid.NewGuid().ToString());
                itemController.Setup(treasureItem);
                break;
            default:
                Debug.LogWarning("Item type not supported");
                break;
        }
    }
}
