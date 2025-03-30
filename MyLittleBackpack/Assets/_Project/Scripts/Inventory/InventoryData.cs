using System;
using System.Collections.Generic;
using UnityEngine;

namespace MLB.Inventory
{
    [CreateAssetMenu(fileName = "InventoryData", menuName = "MLB/InventorySystem/InventoryData")]
    public class InventoryData : ScriptableObject
    {
        [Header("The maximum number of items that can be included in the inventory is 25")]
        [SerializeField] private List<ItemType> _inventorySlots;
        [SerializeField] private List<ItemTypeAndImagePair> _imageByItemType;

        public List<ItemType> InventorySlots => _inventorySlots;

        public Sprite GetSprite(ItemType itemType)
        {
            foreach (ItemTypeAndImagePair imageItemPair in _imageByItemType)
            {
                if (imageItemPair.ItemType == itemType)
                {
                    return imageItemPair.ItemImage;
                }
            }
            return null;
        }
    }

    [Serializable]
    public struct ItemTypeAndImagePair
    {
        public ItemType ItemType;
        public Sprite ItemImage;
    }
}
