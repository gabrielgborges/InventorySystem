using System;
using System.Collections;
using System.Collections.Generic;
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
        gameObject.SetActive(false);
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
        if (_isDragging)
        {
            Vector3 draggingPosition = GetWorldPositionAtHeight(cursorRay, _dragHeight);
            _selectedItem.OnDrag(draggingPosition);
            return;
        }
        
        
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            //cursorRay.direction = _mainCamera.transform.forward;//cameraController.transform.forward;
            cursorRay.origin = _mainCamera.transform.position + _mainCamera.nearClipPlane * _mainCamera.transform.forward;//cameraController.transform.position + _mainCamera.nearClipPlane * cameraController.transform.forward;
        }
        Debug.DrawRay(cursorRay.origin, cursorRay.direction * _clickDistance, Color.red);

        // Check if there is any surface to grapple on to around the cursor.
        RaycastHit hitInfo;
        bool gotHit = false;
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
        
        
        
        // If raycast was hit, raycast returned a valid hitInfo, target is within player's grapple
        // distance*, and target is in front of player, record that we got a valid hit!
        // *Check the spherecast erronous hitInfo comment a bit below in the for loop.
        // if (tryRaycast && hitInfo.distance != 0f && (hitInfo.point - cameraController.transform.parent.position).magnitude <= _clickDistance && Vector3.Dot(hitInfo.point - transform.position, Quaternion.AngleAxis(-cameraController.cameraLook.x, Physics.gravity) * Vector3.forward) > 0f) {
        //     gotHit = true;
        // } else {
        //     // Perform multiple iteration binary search with different radii of spherecasts checking
        //     // surfaces to hit around the cursor.
        //     ulong total = 1ul << assistIterations; // Left shift is same as 2^assistIterations.
        //     ulong max = total;
        //     ulong min = 0ul;
        //     for (int i = 0; i < assistIterations; i++) {
        //         double tryRadius = (0.5 * assistRadius * (max + min)) / total;
        //         RaycastHit tryHitInfo;
        //         if (Physics.SphereCast(cursorRay, (float)tryRadius, out tryHitInfo, _clickDistance + cameraController.cameraZoom, grappleLayers, QueryTriggerInteraction.Ignore)) {
        //             // If a spherecast scrapes past a colliding surface, it can return an erronous hitInfo that returns no useful information
        //             // other than the fact it has hit something. We can detect this by checking if the distance is 0f and treat it as if nothing
        //             // was hit as if we did hit something, we must have a valid hit point.
        //             // Because we are casting a sphere and the distance is the maximum possible distance we'd need, we have to recheck
        //             // to make sure the player can grapple that far.
        //             // Only grapple surfaces in front of the player.
        //             if (tryHitInfo.distance == 0f) {
        //                 // Scraping past valid surface, increase radius.
        //                 min = max - ((max - min) >> 1);
        //             } else if ((tryHitInfo.point - cameraController.transform.parent.position).magnitude > grappleDistance) {
        //                 // Target beyond player grapple distance reach, increase radius
        //                 // as we may be threading the needle through a hole.
        //                 min = max - ((max - min) >> 1);
        //             } else if (Vector3.Dot(tryHitInfo.point - transform.position, Quaternion.AngleAxis(-cameraController.cameraLook.x, Physics.gravity) * Vector3.forward) <= 0f) {
        //                 // Target is too close such that it's behind the player.
        //                 // Decrease radius as we may be hitting something too close due
        //                 // to the side of the sphere.
        //                 max = min + ((max - min) >> 1);
        //             } else {
        //                 // Hit is valid! Record it and see if we can get something
        //                 // closer to the cursor by decreasing radius.
        //                 hitInfo = tryHitInfo;
        //                 gotHit = true;
        //                 max = min + ((max - min) >> 1);
        //             }
        //         } else {
        //             // Hit nothing, increase radius to see if something further away
        //             // from cursor.
        //             min = max - ((max - min) >> 1);
        //         }
        //     }
        // }
        //
        // // Now place crosshair and set location and transform hit if valid.
        // // Check if point is visible in the screen.
        // Vector3 screenHitPosition = m_camera.WorldToScreenPoint(hitInfo.point);
        // Vector2 viewportPos = m_camera.ScreenToViewportPoint(screenHitPosition);
        // Bounds bounds = new Bounds(0.5f * Vector2.one, Vector2.one);
        // if (gotHit && bounds.Contains(viewportPos)) {
        //     crosshair.enabled = true;
        //     validTransform = hitInfo.transform;
        //     validLocation = validTransform.InverseTransformPoint(hitInfo.point);
        //     crosshair.rectTransform.position = m_camera.WorldToScreenPoint(hitInfo.point);
        // } else if (validTransform && ((validTransform.TransformPoint(validLocation) - cursorRay.origin) - Vector3.Project(validTransform.TransformPoint(validLocation) - cursorRay.origin, cursorRay.direction)).sqrMagnitude <= assistRadius * assistRadius && Physics.CheckSphere(validTransform.TransformPoint(validLocation), 2f * Physics.defaultContactOffset, grappleLayers, QueryTriggerInteraction.Ignore)) {
        //     crosshair.enabled = true;
        //     crosshair.rectTransform.position = m_camera.WorldToScreenPoint(validTransform.TransformPoint(validLocation));
        // } else {
        //     validTransform = null;
        //     crosshair.rectTransform.position = cursorPosition;
        //     crosshair.enabled = false;
        // }
        //
        // // Determine visual related animation values.
        // Vector3 grappleToPoint = grappleTransform.TransformPoint(grappleLocation) - grappleLine.position;
        // float grappleTotalLength = grappleToPoint.magnitude;
        // m_grappleLength = Mathf.Clamp(m_grappleLength + grappleSpeed * Time.deltaTime * (m_grapple ? 1f : -1f), 0f, grappleTotalLength);
        // m_grappleInterpolant = m_grappleLength / grappleTotalLength;
        // m_grappleAnimation = Mathf.Clamp01(m_grappleAnimation + grappleAnimationSpeed * Time.deltaTime);
        //
        // // Scale grapple line mesh to target and move the tip accordingly.
        // grappleLine.forward = grappleToPoint;
        // grappleLine.localScale = Vector3.Scale(Vector3.one, new Vector3(1f, 1f, m_grappleLength));
        // grappleTip.position = grappleLine.position + grappleLine.TransformVector(new Vector3(0f, 0f, 1f));
        //
        // // Set custom shader material attributes.
        // m_grappleLineMaterialInstance.SetFloat("_Frequency", grappleFrequency.Evaluate(m_grappleAnimation));
        // m_grappleLineMaterialInstance.SetFloat("_Compensate", grappleThicknessCompensation.Evaluate(m_grappleAnimation));
        // m_grappleLineMaterialInstance.SetFloat("_Offset", m_grappleLineMaterialInstance.GetFloat("_Offset") + Time.deltaTime * grappleRippleSpeed.Evaluate(m_grappleAnimation));
        // m_grappleLineMaterialInstance.SetFloat("_Scale", grappleScale.Evaluate(m_grappleAnimation));
        //
        // // Rotate the visuals to face the camera.
        // Vector3 camGrappleDiff = cameraController.transform.position - grappleLine.transform.position;
        // Vector3 rejectedCameraForward = camGrappleDiff - Vector3.Project(camGrappleDiff, grappleLine.transform.forward);
        // grappleLine.transform.rotation = Quaternion.LookRotation(grappleLine.transform.forward, rejectedCameraForward);
        // grappleBase.rotation = Quaternion.LookRotation(cameraController.transform.position - grappleBase.position);
        // grappleTip.rotation = Quaternion.LookRotation(cameraController.transform.position - grappleTip.position);
        //
        // // Fade line when camera clips through it.
        // Color lineColor = m_grappleLineMaterialInstance.GetColor("_Color");
        // if ((camGrappleDiff.magnitude - m_camera.nearClipPlane) <= grappleTotalLength && Vector3.Dot(camGrappleDiff, grappleToPoint) >= 0f) {
        //     lineColor.a = Mathf.InverseLerp(0f, m_grappleLineMaterialInstance.GetFloat("_Thickness") + CameraController.NearCameraBox(m_camera).magnitude, rejectedCameraForward.magnitude);
        // } else {
        //     lineColor.a = 1f;
        // }
        // m_grappleLineMaterialInstance.SetColor("_Color", lineColor);
        //
        // // Input buffering for grapple.
        // if (m_grappleBufferTimer > 0f) {
        //     ToggleGrapple();
        // }
        // m_grappleBufferTimer -= Time.deltaTime;

        
    }
}
