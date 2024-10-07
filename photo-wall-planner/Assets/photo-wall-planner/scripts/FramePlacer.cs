using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
    private static readonly List<ARRaycastHit> _hits = new();
    private Ray ray;

    private bool customFrame = false;

    protected override void OnPressBegan(Vector3 position)
    {
        base.OnPressBegan(position);
        if (EventSystem.current.IsPointerOverGameObject()) return;
        objectToPlace = frames.GetFrame();
        if (!raycastManager.Raycast(position, _hits, TrackableType.PlaneWithinPolygon)) return;

        ray = arCamera.ScreenPointToRay(position);

        if (Physics.Raycast(ray, out RaycastHit hitObject))
            if (hitObject.transform.CompareTag(Tag.Placable.ToString())) return;
        if (getCustomFrame()) { return; }
        else { PlaceFrame(); }
    }

    public void selectCustomFrame()
    {
        customFrame = true;
        frames.CloseOrientationWindow();
        setFrameSizesCanvas.enabled = true;
    }

    private bool getCustomFrame()
       { return customFrame; }

    public void setLandscape(bool landscape)
    { landscape = true; }

    public void PlaceFrame()
    {
        var orientation = _hits[0].trackable.transform.rotation;
        var hitpose = _hits[0].pose;
        if (!frames.isLandscape())
        //{ GameObject instance = Instantiate(objectToPlace, hitpose.position, Quaternion.Euler(orientation.x - 90, orientation.y, orientation.z)); }
        { GameObject instance = Instantiate(objectToPlace, hitpose.position, hitpose.rotation);
            instance.transform.rotation = Quaternion.Euler()
        }
                else 
        { GameObject instance = Instantiate(objectToPlace, hitpose.position, Quaternion.Euler(orientation.x, orientation.y, orientation.z)); }
        
    }

    public void PlaceCustomFrame(float sizeX, float sizeZ)
    {
        //from cm to unity units (m)
        sizeX /= 100;
        sizeZ /= 100;

        var hitpose = _hits[0].pose;
        GameObject instance = Instantiate(objectToPlace, hitpose.position, hitpose.rotation);
        instance.transform.localScale = new Vector3(sizeX, objectToPlace.transform.localScale.y / 10, sizeZ);
        customFrame = false;
    }
}
