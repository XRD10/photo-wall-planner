using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class FramePlacer : PressInputBase
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject objectToPlace;
    [SerializeField] private FrameMenuUI frames;
    [SerializeField] private Canvas setFrameSizesCanvas;
    [SerializeField] private Camera arCamera;
    private GameObject instance;
    private static readonly List<ARRaycastHit> _hits = new();
    private Ray ray;



    protected override void OnPressBegan(Vector3 position)
    {
        base.OnPressBegan(position);
        if (EventSystem.current.IsPointerOverGameObject()) return;
        objectToPlace = frames.GetFrame();
        if (!raycastManager.Raycast(position, _hits, TrackableType.PlaneWithinPolygon)) return;

        ray = arCamera.ScreenPointToRay(position);

        if (Physics.Raycast(ray, out RaycastHit hitObject))
            if (hitObject.transform.CompareTag("Placable")) return;
        if(objectToPlace == null)
        {
            Debug.LogError("No object selected");
            return;
        }

        PlaceFrame();
        
    }
   public void PlaceFrame()
    {
    if (objectToPlace == null)
    {
        Debug.LogError("No object to place");
        return;
    }

    // Ensure a valid raycast hit is available
    if (_hits.Count == 0)
    {
        Debug.LogError("No AR hit detected");
        return;
    }

    var hitpose = _hits[0].pose;

    objectToPlace.transform.position = hitpose.position;
    objectToPlace.transform.localScale = new Vector3(objectToPlace.transform.localScale.x, objectToPlace.transform.localScale.y / 10, objectToPlace.transform.localScale.z);
    objectToPlace.transform.up = hitpose.up;
    float yRotation = frames.GetLandscape() ? 0f : 90f;
    objectToPlace.transform.Rotate(0, yRotation, 0, Space.Self);
    objectToPlace.SetActive(true);
    objectToPlace.tag = "Placable";

    Debug.Log("Object placed successfully at position: " + hitpose.position);
    }


    public GameObject PlaceCustomFrame(float sizeX, float sizeZ)
    {
        //from cm to unity units (m)
        sizeX /= 100;
        sizeZ /= 100;   

        var hitpose = _hits[0].pose;

        instance = Instantiate(objectToPlace, hitpose.position, hitpose.rotation);
        instance.transform.localScale = new Vector3(sizeX, objectToPlace.transform.localScale.y / 10, sizeZ);

        return instance;
    }

   public GameObject GetPlacedObject(){
    if(instance){
    }
     return instance;
   }

    
}