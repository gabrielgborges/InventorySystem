using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInteractableButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button _button;
    
    private bool _isSelected = false;
    
    private void Start()
    {
        Setup();
    }

    private async void Setup()
    {
        IEventService eventService = await ServiceLocator.GetService<IEventService>();
        eventService.AddListener<OnDropItemEvent>(DiscardItem, GetHashCode());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isSelected = true;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        _isSelected = false;
    }

    private void DiscardItem(OnDropItemEvent onDropItemEvent)
    {
        if (_isSelected)
        {
            Destroy(onDropItemEvent.Item);
        }
        _isSelected = false;
    }
}
