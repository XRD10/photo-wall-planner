using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class FramePlacer : PressInputBase
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject objectToPlace;
    [SerializeField] private FrameMenuUI frameMenuUI;
    [SerializeField] private Canvas setFrameSizesCanvas;
    [SerializeField] private Camera arCamera;
    private static readonly List<ARRaycastHit> _hits = new();
    private Ray ray;

    protected override void OnPressBegan(Vector3 position)
    {
        base.OnPressBegan(position);
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (!raycastManager.Raycast(position, _hits, TrackableType.PlaneWithinPolygon)) return;

        ray = arCamera.ScreenPointToRay(position);

        if (Physics.Raycast(ray, out RaycastHit hitObject))
            if (hitObject.transform.CompareTag("Placable")) return;

        objectToPlace = frameMenuUI.GetFrame();

        if (objectToPlace == null)
        {
            Debug.Log("No object selected");
            return;
        }

        if (frameMenuUI.IsCustomFrame())
        {
            (float sizeX, float sizeZ) = frameMenuUI.GetCustomFrameSize();
            PlaceCustomFrame(sizeX, sizeZ);
        }
        else
        {

            PlaceFrame();
        }

    }
    public void PlaceFrame()
    {
        var hitpose = _hits[0].pose;

        GameObject instance = Instantiate(objectToPlace, hitpose.position, Quaternion.identity);
        instance.transform.localScale = new Vector3(objectToPlace.transform.localScale.x, objectToPlace.transform.localScale.y / 10, objectToPlace.transform.localScale.z);

        instance.transform.up = hitpose.up;

        float yRotation = frameMenuUI.GetLandscape() ? 0f : 90f;
        instance.transform.Rotate(0, yRotation, 0, Space.Self);
        SetText(instance, instance.transform.localScale.z, instance.transform.localScale.x);
        instance.tag = "Placable";

    }

    public void PlaceCustomFrame(float sizeX, float sizeZ)
    {
        //from cm to unity units (m)
        sizeX /= 100;
        sizeZ /= 100;

        var hitpose = _hits[0].pose;

        GameObject instance = Instantiate(objectToPlace, hitpose.position, hitpose.rotation);
        instance.transform.localScale = new Vector3(sizeX, objectToPlace.transform.localScale.y / 10, sizeZ);
        instance.tag = "Placable";
        SetText(instance, sizeZ, sizeX);
        
    }

    private void SetText(GameObject frame, float x, float y)
    {
        if (frame == null) return;

        Vector3 parentScale = frame.transform.lossyScale;
        Vector3 inverseScale = new Vector3(x / parentScale.x * 4, 4f, y / parentScale.z);

        GameObject xText = new GameObject("XText");
        xText.layer = LayerMask.NameToLayer("UI");
        xText.transform.SetParent(frame.transform, false);

        TextMeshPro xTextMesh = xText.AddComponent<TextMeshPro>();
        xTextMesh.text = (x * 100).ToString() + "cm";
        xTextMesh.color = Color.red;
        xTextMesh.fontSize = 0.1f;
        xTextMesh.alignment = TextAlignmentOptions.Center;

        xText.transform.localScale = inverseScale;
        xText.transform.localPosition = new Vector3(0.49f, 3, 0);
        xText.transform.localRotation = Quaternion.Euler(90, -90, 0);

        GameObject yText = new GameObject("YText");
        yText.layer = LayerMask.NameToLayer("UI");
        yText.transform.SetParent(frame.transform, false);

        TextMeshPro yTextMesh = yText.AddComponent<TextMeshPro>();
        yTextMesh.text = (y * 100).ToString() + "cm";
        yTextMesh.color = Color.red;
        yTextMesh.fontSize = 0.1f;
        yTextMesh.alignment = TextAlignmentOptions.Center;

        yText.transform.localScale = inverseScale;
        yText.transform.localPosition = new Vector3(0, 3, -0.49f);
        yText.transform.localRotation = Quaternion.Euler(90, -90, 90);

    }
}
