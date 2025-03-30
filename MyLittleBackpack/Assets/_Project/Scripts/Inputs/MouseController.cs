using UnityEngine;
using UnityEngine.InputSystem;

public class MouseController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _clickDistance = 300;
    [Tooltip("Precision of search for nearest surface to grapple on to. At least 8. Higher is more precise at the expense of computational cost.")]
    [SerializeField]
    [Range(0, 32)]
    int assistIterations = 12;
    [Tooltip("GameObject layers that the player can grapple onto.")]
    [SerializeField]
    LayerMask grappleLayers;
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
        //cursorPosition.z = _mainCamera.nearClipPlane;

        // Evaluate ray for casting in world space.
        Ray cursorRay = _mainCamera.ScreenPointToRay(cursorPosition);
        Debug.DrawRay(cursorRay.origin, cursorRay.direction * _clickDistance, Color.red);

        if (_isDragging)
        {
            Vector3 draggingPosition = GetWorldPositionAtHeight(cursorRay, _dragHeight);
            _selectedItem.OnDrag(draggingPosition);
            return;
        }
        Debug.DrawRay(cursorRay.origin, cursorRay.direction * _clickDistance, Color.red);

        
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            //cursorRay.direction = _mainCamera.transform.forward;//cameraController.transform.forward;
            cursorRay.origin = _mainCamera.transform.position + _mainCamera.nearClipPlane * _mainCamera.transform.forward;//cameraController.transform.position + _mainCamera.nearClipPlane * cameraController.transform.forward;
        }
        Debug.DrawRay(cursorRay.origin, cursorRay.direction * _clickDistance, Color.red);

        // Check if there is any surface to grapple on to around the cursor.
        RaycastHit hitInfo;
        // Cast a ray from the camera based on cursor position.
        // Only detect game objects on layers that are grapple-able. Ignore non-colliding triggers.
        // The ray distance adds the player's grapple distance and the zoom distance which will
        // guarantee the ray will be longer than or equal to the player's grapple distance. We can
        // filter anything beyond after.
        bool tryRaycast = Physics.Raycast(cursorRay, out hitInfo, _clickDistance, grappleLayers, QueryTriggerInteraction.Ignore);//cameraController.cameraZoom, grappleLayers, QueryTriggerInteraction.Ignore);
        if (tryRaycast)
        {
           if(hitInfo.collider.gameObject.TryGetComponent(out ISelectableItem selectableItem))
           {
               _selectedItem = selectableItem;
               _selectedItem.Hover();
               Debug.LogWarning("Selected");
           }
        }
    }
}
