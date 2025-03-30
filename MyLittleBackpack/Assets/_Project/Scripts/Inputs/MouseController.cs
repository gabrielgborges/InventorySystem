using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class MouseController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _clickDistance = 300;
    [FormerlySerializedAs("grappleLayers")]
    [Tooltip("GameObject layers that the player can grapple onto.")]
    [SerializeField]
    LayerMask _grabLayers;
    [SerializeField] private int _dragHeight = 2;
    
    PlayerController _inputMap;
    private ISelectableItem _selectedItem;
    private bool _isDragging;
    private IEventService _eventService;
    
    private async void Awake()
    {
        _inputMap = new PlayerController();
        _inputMap.Player.Enable();
        _inputMap.Player.SelectItem.performed += SelectItem;
        _inputMap.Player.SelectItem.canceled += StopSelection;

        _eventService = await ServiceLocator.GetService<IEventService>();
    }

    private void SelectItem(InputAction.CallbackContext obj)
    {
        if (_selectedItem != null)
        {
            _selectedItem.Select();
            _isDragging = true;
            _eventService.TryInvokeEvent(new OnDragItemEvent());
        }
    }
    
    private void StopSelection(InputAction.CallbackContext obj)
    {
        _isDragging = false;
        if (_selectedItem != null)
        {
            GameObject droppedObject = _selectedItem.Deselect();
            _eventService.TryInvokeEvent(new OnDropItemEvent(droppedObject));
            _selectedItem = null;
        }
    }
    
    private Vector3 GetWorldPositionAtHeight(Ray ray, float heightY)
    {
        float t = (heightY - ray.origin.y) / ray.direction.y;
        return ray.origin + ray.direction * t;
    }

    void FixedUpdate() {
        // Read mouse position on screen.
        Vector3 cursorPosition = Mouse.current.position.ReadValue();//_cameraAimAction.ReadValue<Vector2>();

        // Evaluate ray for casting in world space.
        Ray cursorRay = _mainCamera.ScreenPointToRay(cursorPosition);
        Debug.DrawRay(cursorRay.origin, cursorRay.direction * _clickDistance, Color.red);

        if (_isDragging)
        {
            Vector3 draggingPosition = GetWorldPositionAtHeight(cursorRay, _dragHeight);
            _selectedItem.OnDrag(draggingPosition);
            return;
        }
        
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            return;
        }
        
        RaycastHit hitInfo;

        bool tryRaycast = Physics.Raycast(cursorRay, out hitInfo, _clickDistance, _grabLayers, QueryTriggerInteraction.Collide);//cameraController.cameraZoom, grappleLayers, QueryTriggerInteraction.Ignore);
        if (tryRaycast)
        {
           if(hitInfo.collider.gameObject.TryGetComponent(out ISelectableItem selectableItem))
           {
               _selectedItem = selectableItem;
               _selectedItem.Hover();
           }
        }
        else
        {
            _selectedItem = null;
        }
    }
}
