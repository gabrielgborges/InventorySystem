using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private InventoryPhysicsComponent _physicsComponent;
    [SerializeField] private InventoryDataComponent _dataComponent;

    private IEventService _eventService;
    
    private void Start()
    {
        Initialize();
    }

    private async void Initialize()
    {
        _physicsComponent.OnItemArrived = _dataComponent.SetPreparedItem;
        
        _eventService = await ServiceLocator.GetService<IEventService>();
        _eventService.AddListener<OnDropItemEvent>(OnDropItemHandler, GetHashCode());
        
        _dataComponent.Initialize();
    }

    private void OnDestroy()
    {
        _eventService.RemoveListener<OnDropItemEvent>(GetHashCode());
    }

    private void OnDropItemHandler(OnDropItemEvent obj)
    {
        if (_dataComponent.HoldingItem)
        {
            _dataComponent.AddPreparedItem();
            Destroy(obj.Item);
        }
    }
}
