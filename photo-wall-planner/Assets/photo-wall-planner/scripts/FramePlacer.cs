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
	[SerializeField] private FrameMenuUI frames;
	[SerializeField] private Canvas setFrameSizesCanvas;
	[SerializeField] private Camera arCamera;
	[SerializeField] private WorkingAreaManager workingAreaManager;
	private static readonly List<ARRaycastHit> _hits = new();
	private Ray ray;

	[SerializeField] private Vector3 xTextSpawn;
	[SerializeField] private Vector3 yTextSpawn;
	[SerializeField] private TMP_FontAsset fontXY;


	protected override void OnPressBegan(Vector3 position)
	{
		base.OnPressBegan(position);
		if (EventSystem.current.IsPointerOverGameObject()) return;
		objectToPlace = frames.GetFrame();
		if (!raycastManager.Raycast(position, _hits, TrackableType.PlaneWithinPolygon)) return;

		ray = arCamera.ScreenPointToRay(position);

		if (Physics.Raycast(ray, out RaycastHit hitObject))
			if (hitObject.transform.CompareTag("Placable")) return;

		// check if working area was created
		if (!GameObject.Find("WorkingArea"))
		{
			Debug.LogError("No working area created");
			return;
		}

		if (!workingAreaManager.IsEditingComplete())
		{
			return;
		}

		if (objectToPlace == null)
		{
			Debug.LogError("No object selected");
			return;
		}

		if (!workingAreaManager.IsPointInsideWorkingArea(_hits[0].pose.position))
		{
			Debug.LogError("Cannot place frame outside the working area");
			return;
		}

		PlaceFrame();


	}
	public void PlaceFrame()
	{
		var hitpose = _hits[0].pose;

		GameObject instance = Instantiate(objectToPlace, hitpose.position + hitpose.up * 0.02f, Quaternion.identity);
		instance.transform.localScale = new Vector3(objectToPlace.transform.localScale.x, objectToPlace.transform.localScale.y / 10, objectToPlace.transform.localScale.z);

		instance.transform.up = hitpose.up;
		Debug.Log(hitpose.up);
		Debug.Log(instance.transform.up);

		float yRotation = frames.GetLandscape() ? 0f : 90f;
		instance.transform.Rotate(0, yRotation, 0, Space.Self);
		instance.tag = "Placable";

		DisplayFrameDistances(instance);
	}

	public void PlaceCustomFrame(float sizeX, float sizeZ)
	{
		//from cm to unity units (m)
		sizeX /= 100;
		sizeZ /= 100;

		var hitpose = _hits[0].pose;

		if (!workingAreaManager.IsPointInsideWorkingArea(hitpose.position))
		{
			Debug.LogWarning("Cannot place custom frame outside the working area");
			return;
		}

		GameObject instance = Instantiate(objectToPlace, hitpose.position, hitpose.rotation);
		instance.transform.localScale = new Vector3(sizeX, objectToPlace.transform.localScale.y / 10, sizeZ);
		DisplayFrameDistances(instance);

	}

	private void DisplayFrameDistances(GameObject frame)
	{
		Vector3 framePosition = frame.transform.position;

		// Get working area bounds
		Vector3 minBounds = workingAreaManager.GetWorkingAreaMinBounds();
		Vector3 maxBounds = workingAreaManager.GetWorkingAreaMaxBounds();

		// Calculate distances to each edge of the working area
		float distanceToLeftEdge = Mathf.Abs(framePosition.x - minBounds.x);
		float distanceToRightEdge = Mathf.Abs(maxBounds.x - framePosition.x);
		// TODO not working
		float distanceToTopEdge = Mathf.Abs(maxBounds.z - framePosition.z);
		float distanceToBottomEdge = Mathf.Abs(framePosition.z - minBounds.z);

		Debug.Log("Frame Position: " + frame.transform.rotation.eulerAngles);


		// Display the distances using TextMeshPro or UI elements
		DisplayDistanceText(frame, distanceToLeftEdge, "LeftEdgeDistance", new Vector3(-0.7f, 0.3f, 0));
		DisplayDistanceText(frame, distanceToRightEdge, "RightEdgeDistance", new Vector3(0.7f, 0.3f, 0));
		DisplayDistanceText(frame, distanceToTopEdge, "TopEdgeDistance", new Vector3(0, 0.3f, 0.7f));
		DisplayDistanceText(frame, distanceToBottomEdge, "BottomEdgeDistance", new Vector3(0, 0.3f, -0.7f));
	}

	private void DisplayDistanceText(GameObject frame, float distance, string textObjectName, Vector3 localPosition)
	{
		GameObject distanceText = new GameObject(textObjectName);
		distanceText.transform.SetParent(frame.transform, false);

		TextMeshPro distanceTextMesh = distanceText.AddComponent<TextMeshPro>();
		distanceTextMesh.text = (int)(distance * 100) + "cm";
		distanceTextMesh.color = Color.red;
		distanceTextMesh.fontSize = 1.5f;
		distanceTextMesh.alignment = TextAlignmentOptions.Center;

		distanceText.transform.localScale = Vector3.one;
		distanceText.transform.localPosition = localPosition;
		distanceText.transform.localRotation = Quaternion.Euler(90, -90, 0);
	}
}
