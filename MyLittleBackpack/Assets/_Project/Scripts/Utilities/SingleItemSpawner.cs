using System;
using MLB.Inventory;
using MLB.Inventory.Items;
using UnityEngine;

public class SingleItemSpawner : MonoBehaviour
{
    public void Spawn(ItemData itemData)
    {
        ItemController itemController = Instantiate(itemData.Prefab, transform).GetComponent<ItemController>();
        itemController.transform.position += Vector3.back * 2;
        switch (itemData.ItemType)
        {
            case ItemType.TREASURE:
                TreasureItem treasureItem = new TreasureItem(itemData, Guid.NewGuid().ToString());
                itemController.Setup(treasureItem);
                break;
            case ItemType.COMBAT:
                CombatItem combatItem = new CombatItem(itemData, Guid.NewGuid().ToString());
                itemController.Setup(combatItem);
                break;
            case ItemType.CONSUMABLE:
                ConsumableItem consumableItem = new ConsumableItem(itemData, Guid.NewGuid().ToString());
                itemController.Setup(consumableItem);
                break;
            default:
                Debug.LogWarning("Item type not supported");
                break;
        }
    }
}