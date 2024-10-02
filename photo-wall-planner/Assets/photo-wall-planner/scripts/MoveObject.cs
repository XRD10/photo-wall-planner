using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MoveObject : PressInputBase
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private string targetTag = "Pyramid";  // The tag to check for

    private static readonly List<ARRaycastHit> _hits = new();

   protected override void OnPress(Vector3 position)
{
    Debug.Log("Press");

    if (EventSystem.current.IsPointerOverGameObject()) return;
    
    if (!raycastManager.Raycast(position, _hits, TrackableType.PlaneWithinPolygon)) return;

    var hitpose = _hits[0].pose;
    Debug.Log("HERE");

    Ray ray = new Ray(Camera.main.transform.position, hitpose.position - Camera.main.transform.position);

    if (Physics.Raycast(ray, out RaycastHit hit, 100))
    {
        Debug.Log("hit");
        Debug.Log(hit.transform.name + " : " + hit.transform.tag);

        if (hit.transform.CompareTag(targetTag))
        {
            Debug.Log("Hit the target object with tag: " + targetTag);
        }
    }
}


}
