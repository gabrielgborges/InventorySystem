using UnityEngine;

namespace MLB.Inventory.Items
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "MLB/InventorySystem/ItemData")]
    public class ItemData : ScriptableObject
    {
        public GameObject Prefab;
        public Sprite Sprite;
        public string Name;
        public ItemType ItemType;
        public int Weight;
        public string ID;
    }
}