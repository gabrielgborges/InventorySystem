using UnityEngine;

public class InventoryController : MonoBehaviour, ISelectableItem
{
    [SerializeField] private InventoryPhysicsComponent _physicsComponent;
    [SerializeField] private InventoryDataComponent _dataComponent;
    [SerializeField] private SingleItemSpawner _spawner;
    
    private ScreenControllerBase _inventoryScreen;
    private IEventService _eventService;
    private IScreenService _screenService;
    
    private bool _inventoryIsOpened => _inventoryScreen != null;

    public void Hover()
    {
    }

    public void Select()
    {
        _screenService.LoadScreen<InventoryScreenController>(GameScreen.INVENTORY, HandleInventoryScreenOpened);
    }

    public void OnDrag(Vector3 position)
    {
    }

    public GameObject Deselect()
    {
        return null;
    }
    
    private void Start()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        _eventService.RemoveListener<OnDropItemEvent>(GetHashCode());
    }
    
    private async void Initialize()
    {
        _physicsComponent.OnItemArrived = _dataComponent.SetPreparedItem;
        
        _screenService = await ServiceLocator.GetService<IScreenService>();
        _eventService = await ServiceLocator.GetService<IEventService>();
        _eventService.AddListener<OnDropItemEvent>(OnDropItemHandler, GetHashCode());
        _eventService.AddListener<OnRemoveInventoryItemEvent>(RemoveItemHandler, GetHashCode());
        _dataComponent.Initialize();
    }

    private void RemoveItemHandler(OnRemoveInventoryItemEvent obj)
    {
        _dataComponent.RemoveItem(obj.Item.ItemData);
        _spawner.Spawn(obj.Item.ItemData);
    }

    private void OnDropItemHandler(OnDropItemEvent obj)
    {
        if (_dataComponent.HoldingItem)
        {
            TryAddItemToInventory(obj.Item);
        }
        else if (_inventoryIsOpened)
        {
            _inventoryScreen.Close();
        }
    }

    private void TryAddItemToInventory(GameObject item )
    {
        bool inventoryIsNotFull = _dataComponent.AddPreparedItem();
        if (inventoryIsNotFull)
        {
            Destroy(item);
        }
        else
        {
            item.GetComponent<Transform>().position += (Vector3.right + Vector3.up) * 2;
        }
    }
    
    private void HandleInventoryScreenOpened(InventoryScreenController screen)
    {
        screen.SetupInventory(_dataComponent.CurrentItemSlots);
        _inventoryScreen = screen;
    } 
}
