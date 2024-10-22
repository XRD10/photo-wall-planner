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
	[SerializeField] private WorkingAreaManager workingAreaManager;
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
		//from cm to unity units (m)

		var hitpose = _hits[0].pose;

		GameObject instance = Instantiate(objectToPlace, hitpose.position + hitpose.up * 0.02f, Quaternion.identity);
		instance.transform.localScale = new Vector3(objectToPlace.transform.localScale.x, objectToPlace.transform.localScale.y / 10, objectToPlace.transform.localScale.z);

		instance.transform.up = hitpose.up;
		Debug.Log(hitpose.up);
		Debug.Log(instance.transform.up);

		float yRotation = frames.GetLandscape() ? 0f : 90f;
		instance.transform.Rotate(0, yRotation, 0, Space.Self);
		instance.tag = "Placable";

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

	}
}
