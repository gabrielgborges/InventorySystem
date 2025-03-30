using UnityEngine;

namespace MLB.Inventory.Items
{
    public sealed class ItemController : MonoBehaviour, ISelectableItem, IStoreableItem
    {
        [SerializeField] private ItemPhysicsComponent _physicsComponent;
        
        private ItemBase _itemData;
        
        public ItemData ItemData => _itemData.Data;

        public void Setup<T>(T data) where T : ItemBase
        {
            _itemData = data;
            _physicsComponent.MoveForward();
        }

        public void Hover()
        {
        }

        public void Select()
        {
        
        }

        public void OnDrag(Vector3 position)
        {
            _physicsComponent.StopMovement();
            transform.position = position;
        }

        public GameObject Deselect()
        {
            transform.rotation = Quaternion.identity;
            _physicsComponent.MoveForward();
            return gameObject;
        }
    }
}
