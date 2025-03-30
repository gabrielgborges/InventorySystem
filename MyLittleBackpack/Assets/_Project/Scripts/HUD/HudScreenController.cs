using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudScreenController : ScreenControllerBase
{
    private IEventService _eventService;
    
    private void Start()
    {
        Initialize();
    }

    protected override async void Initialize()
    {
        base.Initialize();
        _eventService = await ServiceLocator.GetService<IEventService>();
        _eventService.AddListener<OnDragItemEvent>(ShowHud, GetHashCode());
        _eventService.AddListener<OnDropItemEvent>(HideHud, GetHashCode());
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _eventService.RemoveListener<OnDragItemEvent>(GetHashCode());
        _eventService.RemoveListener<OnDropItemEvent>(GetHashCode());
    }

    private void ShowHud( OnDragItemEvent obj)
    {
        gameObject.SetActive(true);
    }

    private void HideHud(OnDropItemEvent obj)
    {
        gameObject.SetActive(false);
    }
}
