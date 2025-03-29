using MLB.Inventory;
using UnityEngine;

public class ItemController : MonoBehaviour, ISelectableItem
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
        Debug.Log("Dragging");
    }

    public void Deselect()
    {
        transform.rotation = Quaternion.identity;
        _physicsComponent.MoveForward();
    }
}
