using UnityEngine;

public class InventoryController : MonoBehaviour, ISelectableItem
{
    [SerializeField] private InventoryPhysicsComponent _physicsComponent;
    [SerializeField] private InventoryDataComponent _dataComponent;

    private IEventService _eventService;
    private IScreenService _screenService;

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
            bool inventoryIsNotFull = _dataComponent.AddPreparedItem();
            if (inventoryIsNotFull)
            {
                Destroy(obj.Item);
            }
            else
            {
                obj.Item.GetComponent<Transform>().position += (Vector3.right + Vector3.up) * 2;
            }
        }
    }

    public void Hover()
    {
        Debug.Log("1");

    }

    public void Select()
    {
        Debug.Log("2");
        _screenService.LoadScreen<InventoryScreenController>(GameScreen.INVENTORY, HandleInventoryScreenOpened);
    }

    public void OnDrag(Vector3 position)
    {
        Debug.Log("3");
    }

    public GameObject Deselect()
    {
        Debug.Log("4");

        return null;
    }
    
    private void HandleInventoryScreenOpened(InventoryScreenController screen)
    {
        screen.SetupInventory(_dataComponent.CurrentItemSlots);
    } 
}
