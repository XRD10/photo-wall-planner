using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;  

public class MoveObject : PressInputBase
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private string targetTag = "Pyramid";  

    private static readonly List<ARRaycastHit> _hits = new();

    private bool isDragging = false; 
    private Transform objectToMove;  

    protected override void OnPress(Vector3 position)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (!raycastManager.Raycast(position, _hits, TrackableType.PlaneWithinPolygon)) return;

        var hitpose = _hits[0].pose;

        Ray ray = new Ray(Camera.main.transform.position, hitpose.position - Camera.main.transform.position);

        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            if (hit.transform.CompareTag(targetTag))
            {
                Debug.Log("Hit the target object with tag: " + targetTag);
                objectToMove = hit.transform;
                isDragging = true;  
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
                Pose hitPose = _hits[0].pose;
                objectToMove.position = hitPose.position;  
            }

            if (Pointer.current.press.isPressed == false)  
            {
                isDragging = false;
                objectToMove = null;
            }
        }
    }
}
