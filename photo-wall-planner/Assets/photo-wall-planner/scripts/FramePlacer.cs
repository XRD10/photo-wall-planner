using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class FramePlacer : PressInputBase
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject objectToPlace;
    [SerializeField] private Camera arCamera;
    private static readonly List<ARRaycastHit> _hits = new();
    private Ray ray;

    protected override void OnPressBegan(Vector3 position)
    {
        base.OnPressBegan(position);
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (!raycastManager.Raycast(position, _hits, TrackableType.PlaneWithinPolygon)) return;
        var hitpose = _hits[0].pose;
        ray = arCamera.ScreenPointToRay(position);

        if (Physics.Raycast(ray, out RaycastHit hitObject))
            if (hitObject.transform.CompareTag(Tag.Placable.ToString())) return;

        Instantiate(objectToPlace, hitpose.position, hitpose.rotation);
    }
}
