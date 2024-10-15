using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class WorkingAreaManager : PressInputBase
{
	[SerializeField] private ARRaycastManager raycastManager;
	[SerializeField] private GameObject workingAreaPlanePrefab;
	[SerializeField] private Camera arCamera;

	public float minSize = 0.5f;
	public float maxSize = 5f;

	private GameObject workingAreaPlane;
	private static readonly List<ARRaycastHit> _hits = new List<ARRaycastHit>();
	private Ray ray;
	private bool isResizing = false;
	private float initialTouchDistance;
	private Vector3 initialScale;
	public bool isWorkingAreaEditingComplete = false;

	protected override void OnPressBegan(Vector3 position)
	{
		if (isWorkingAreaEditingComplete) return;
		base.OnPressBegan(position);

		if (EventSystem.current.IsPointerOverGameObject()) return;

		if (!raycastManager.Raycast(position, _hits, TrackableType.PlaneWithinPolygon)) return;

		ray = arCamera.ScreenPointToRay(position);
		if (Physics.Raycast(ray, out RaycastHit hitObject))
		{
			// Use string comparison instead of CompareTag
			if (hitObject.transform.gameObject.name == "WorkingArea") return;
		}

		PlaceWorkingArea();
	}


	void Update()
	{
		if (Input.touchCount == 2)
		{
			HandlePinchToResize();
		}
	}

	void PlaceWorkingArea()
	{
		var hitpose = _hits[0].pose;

		Quaternion planeRotation = Quaternion.FromToRotation(Vector3.up, hitpose.up);
		Vector3 adjustedPosition = hitpose.position + hitpose.up * 0.01f;

		if (workingAreaPlane == null)
		{
			workingAreaPlane = Instantiate(workingAreaPlanePrefab, adjustedPosition, planeRotation);
			workingAreaPlane.name = "WorkingArea";
			workingAreaPlane.tag = "Placable";
		}
		else
		{
			workingAreaPlane.transform.SetPositionAndRotation(adjustedPosition, planeRotation);
		}
	}

	void HandlePinchToResize()
	{
		Touch touch0 = Input.GetTouch(0);
		Touch touch1 = Input.GetTouch(1);

		if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
		{
			isResizing = true;
			initialTouchDistance = Vector2.Distance(touch0.position, touch1.position);
			initialScale = workingAreaPlane.transform.localScale;
		}
		else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
		{
			float currentTouchDistance = Vector2.Distance(touch0.position, touch1.position);
			float scaleFactor = currentTouchDistance / initialTouchDistance;

			Vector3 newScale = initialScale * scaleFactor;
			newScale.x = Mathf.Clamp(newScale.x, minSize, maxSize);
			newScale.z = Mathf.Clamp(newScale.z, minSize, maxSize);

			workingAreaPlane.transform.localScale = new Vector3(newScale.x, workingAreaPlane.transform.localScale.y, newScale.z);
		}
		else if (touch0.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Ended)
		{
			isResizing = false;
		}
	}

	public void SetCustomSize(float sizeX, float sizeZ)
	{
		if (workingAreaPlane != null)
		{
			sizeX = Mathf.Clamp(sizeX, minSize, maxSize);
			sizeZ = Mathf.Clamp(sizeZ, minSize, maxSize);
			workingAreaPlane.transform.localScale = new Vector3(sizeX, workingAreaPlane.transform.localScale.y, sizeZ);
		}
	}

	public void CompleteEditing()
	{
		isWorkingAreaEditingComplete = true;
		Debug.Log("Working area editing completed.");
		workingAreaPlane.tag = null;
	}

	public bool IsEditingComplete()
	{
		return isWorkingAreaEditingComplete;
	}
}