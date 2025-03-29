using System.Collections;
using System.Collections.Generic;
using MLB.Inventory;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    private ItemBase _itemData;
    [SerializeField] private ItemPhysicsComponent _physicsComponent;
    
    public void Setup<T>(T data) where T : ItemBase
    {
        _itemData = data;
        _physicsComponent.MoveForward();
    }
    
}
