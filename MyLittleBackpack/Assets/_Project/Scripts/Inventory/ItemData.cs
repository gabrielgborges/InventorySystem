using System;
using UnityEngine;

namespace MLB.Inventory
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "MLB/InventorySystem/ItemData")]
    public class ItemData : ScriptableObject
    {
        public GameObject Prefab;
        public string Name;
        public ItemType ItemType;
        public int Weight;
    }
}