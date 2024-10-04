using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;  

public class MoveObject : PressInputBase
{
    [SerializeField] private ARRaycastManager raycastManager;
    private static readonly List<ARRaycastHit> _hits = new();
    private readonly string targetTag = Tag.Placable.ToString();  
    
    private bool isDragging = false;      
    private Transform objectToMove;       
    private TrackableId initialPlaneId;   

    protected override void OnPress(Vector3 position)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (!raycastManager.Raycast(position, _hits, TrackableType.PlaneWithinPolygon)) return;

        var hitPose = _hits[0].pose;
        var hitTrackableId = _hits[0].trackableId;

        Ray ray = new Ray(Camera.main.transform.position, hitPose.position - Camera.main.transform.position);

        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            if (hit.transform.CompareTag(targetTag))
            {
                objectToMove = hit.transform;
                isDragging = true; 
                initialPlaneId = hitTrackableId;  
            }
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            Vector2 screenPosition = Pointer.current.position.ReadValue();

            if (raycastManager.Raycast(screenPosition, _hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = _hits[0].pose;
                var hitTrackableId = _hits[0].trackableId;

                if (hitTrackableId == initialPlaneId)
                {
                    objectToMove.position = hitPose.position;  
                }
            }

            if (Pointer.current.press.isPressed == false)
            {
                isDragging = false;
                objectToMove = null;
            }
        }
    }
}
