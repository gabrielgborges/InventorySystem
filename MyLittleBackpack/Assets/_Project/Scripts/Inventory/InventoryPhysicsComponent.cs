using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryPhysicsComponent : MonoBehaviour
{
    public Action<IStoreableItem> OnItemArrived;
    public Action OnItemRemoved;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IStoreableItem item))
        {
            OnItemArrived?.Invoke(item);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<IStoreableItem>() != null)
        {
            OnItemRemoved?.Invoke();
        }
    }
}
