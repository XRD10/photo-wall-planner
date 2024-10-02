using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class FramePlacer : PressInputBase
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject objectToPlace;
    [SerializeField] private Canvas setFrameSizesCanvas;
    private static readonly List<ARRaycastHit> _hits = new();
    private bool _framePlaced;

    protected override void OnPressBegan(Vector3 position)
    {
        base.OnPressBegan(position);
        if (_framePlaced) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (!raycastManager.Raycast(position, _hits, TrackableType.PlaneWithinPolygon)) return;

        setFrameSizesCanvas.enabled = true;
    }

    //TODO: Add size Z
    public void PlaceFrame(float sizeX, float sizeZ)
    {
        //from cm to unity units (m)
        sizeX /= 100;
        sizeZ /= 100;

        var hitpose = _hits[0].pose;
        objectToPlace.transform.localScale = new Vector3(sizeX, objectToPlace.transform.localScale.y/100, sizeZ);
        Instantiate(objectToPlace, hitpose.position, hitpose.rotation);
        _framePlaced = true;
    }
}
