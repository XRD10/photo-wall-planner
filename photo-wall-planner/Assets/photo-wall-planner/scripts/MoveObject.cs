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
    private readonly string targetTag = "Placable";

    private bool isDragging = false;
    private Transform objectToMove;
    private TrackableId initialPlaneId;

    //Selecting and deleting a frame
    [SerializeField] 
    public bool selected = false;
    [SerializeField] 
    private GameObject selectedFrame = null;
    [SerializeField] 
    private GameObject deleteButton;
    [SerializeField] 
    private GameObject deselectButton;
    [SerializeField] 
    private GameObject saveButton;
    [SerializeField] 
    private GameObject framesButton;

    protected override void OnPress(Vector3 position)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (raycastManager == null) return;

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
                selectFrame(objectToMove.gameObject);  
            }
        }
    }

    private void selectFrame(GameObject frame)
    {
        selected = true;
        selectedFrame = frame;
        deleteButton.SetActive(true);
        deselectButton.SetActive(true);
        saveButton.SetActive(false);
        framesButton.SetActive(false);
    }

    public void DeleteFrame()
    {
        Destroy(selectedFrame);
        selected = false;
        ToggleUIButtons();
    }

    public void DeselectFrame()
    {
        selectedFrame = null;
        selected = false;
        ToggleUIButtons();
    }

    private void ToggleUIButtons()
    {
                deleteButton.SetActive(!deleteButton.activeSelf);
        deselectButton.SetActive(!deselectButton.activeSelf);
                saveButton.SetActive(!saveButton.activeSelf);
        framesButton.SetActive(!framesButton.activeSelf);
    }

    public bool isSelected()
    {
        return selected;
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
