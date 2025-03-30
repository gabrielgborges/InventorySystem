using UnityEngine;

public class InventoryController : MonoBehaviour, ISelectableItem
{
    [SerializeField] private InventoryPhysicsComponent _physicsComponent;
    [SerializeField] private InventoryDataComponent _dataComponent;

    private ScreenControllerBase _inventoryScreen;
    private IEventService _eventService;
    private IScreenService _screenService;
    
    private bool _inventoryIsOpened => _inventoryScreen != null;

    private void Start()
    {
        Initialize();
    }

    private async void Initialize()
    {
        _physicsComponent.OnItemArrived = _dataComponent.SetPreparedItem;
        
        _screenService = await ServiceLocator.GetService<IScreenService>();
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
    
    private void HandleInventoryScreenOpened(InventoryScreenController screen)
    {
        screen.SetupInventory(_dataComponent.CurrentItemSlots);
        _inventoryScreen = screen;
    } 
}
