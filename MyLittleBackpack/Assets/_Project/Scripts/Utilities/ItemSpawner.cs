using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MLB.Inventory;
using MLB.Inventory.Items;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private List<ItemData> _items;
    [SerializeField] private int _spawnDelay;

    private void Start()
    {
        SpawnItem();
    }

    private async void SpawnItem()
    {
        ItemData randomizedItem = _items[UnityEngine.Random.Range(0, _items.Count)];
        ItemController itemController = Instantiate(randomizedItem.Prefab, transform).GetComponent<ItemController>();
        
        switch (randomizedItem.ItemType)
        {
            case ItemType.TREASURE:
                TreasureItem treasureItem = new TreasureItem(randomizedItem, Guid.NewGuid().ToString());
                itemController.Setup(treasureItem);
                break;
            case ItemType.COMBAT:
                CombatItem combatItem = new CombatItem(randomizedItem, Guid.NewGuid().ToString());
                itemController.Setup(combatItem);
                break;
            case ItemType.CONSUMABLE:
                ConsumableItem consumableItem = new ConsumableItem(randomizedItem, Guid.NewGuid().ToString());
                itemController.Setup(consumableItem);
                break;
            default:
                Debug.LogWarning("Item type not supported");
                break;
        }
        
        await UniTask.Delay(_spawnDelay);
        SpawnItem();
    }
}
