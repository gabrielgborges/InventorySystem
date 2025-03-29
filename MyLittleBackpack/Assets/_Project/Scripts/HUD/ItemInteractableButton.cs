using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInteractableButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Button _button;

 
    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }
}
