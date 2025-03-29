using MLB.Inventory;
using UnityEngine;

public sealed class ItemController : MonoBehaviour, ISelectableItem
{
    private ItemBase _itemData;
    
    [SerializeField] private ItemPhysicsComponent _physicsComponent;
    
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
