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
	[SerializeField] private GameObject CompleteButton;

	private GameObject workingAreaPlane;
	private static readonly List<ARRaycastHit> _hits = new List<ARRaycastHit>();
	private Ray ray;
	private bool isResizing = false;
	private float initialTouchDistance;
	private Vector3 initialScale;
	private float minSize = 0.5f;
	private float maxSize = 5f;
	private bool isWorkingAreaEditingComplete = false;

	protected override void OnPressBegan(Vector3 position)
	{
		if (isWorkingAreaEditingComplete) return;
		base.OnPressBegan(position);

		if (EventSystem.current.IsPointerOverGameObject()) return;

		if (!raycastManager.Raycast(position, _hits, TrackableType.PlaneWithinPolygon)) return;

		ray = arCamera.ScreenPointToRay(position);
		if (Physics.Raycast(ray, out RaycastHit hitObject))
		{
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

		// reference: https://discussions.unity.com/t/arfoundation-vertical-plane-recognition-position-rotation-on-plane-normal/786665
		Vector3 normal = -hitpose.up;
		Quaternion planeRotation = Quaternion.LookRotation(normal, Vector3.up);
		if (workingAreaPlane == null)
		{

			CompleteButton.SetActive(true);
			workingAreaPlane = Instantiate(workingAreaPlanePrefab, hitpose.position, planeRotation);
			workingAreaPlane.name = "WorkingArea";
			workingAreaPlane.tag = "Placable";
		}
		else
		{
			workingAreaPlane.transform.SetPositionAndRotation(hitpose.position, planeRotation);
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
		workingAreaPlane.tag = "Untagged";
		CompleteButton.SetActive(false);

		// TODO find out why this is not working
		// because the objects are not active
		GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("VisibleButton");

		foreach (GameObject obj in objectsWithTag)
		{
			Debug.Log(obj.name);
			obj.SetActive(true);
		}
	}

	public bool IsEditingComplete()
	{
		return isWorkingAreaEditingComplete;
	}

	public bool IsPointInsideWorkingArea(Vector3 point)
	{
		Debug.Log(point);
		if (workingAreaPlane == null || !isWorkingAreaEditingComplete)
		{
			return false;
		}

		Quaternion rotationXZ = Quaternion.Euler(
				workingAreaPlane.transform.rotation.eulerAngles.x,
				0f,
				workingAreaPlane.transform.rotation.eulerAngles.z
		  );

		// Convert the point to local space of the working area, ignoring Y rotation
		Vector3 localPoint = Quaternion.Inverse(rotationXZ) * (point - workingAreaPlane.transform.position);

		// Check if the point is within the bounds of the working area
		float halfWidth = workingAreaPlane.transform.localScale.x / 2f;
		float halfLength = workingAreaPlane.transform.localScale.z / 2f;

		bool isInside = localPoint.x >= -halfWidth && localPoint.x <= halfWidth &&
							 localPoint.z >= -halfLength && localPoint.z <= halfLength;

		return isInside;
	}

	public Vector3 GetWorkingAreaCenter()
	{
		return workingAreaPlane.transform.position;
	}

	public Vector3 GetWorkingAreaSize()
	{
		return workingAreaPlane.transform.localScale;
	}

	public Quaternion GetWorkingAreaRotation()
	{
		return workingAreaPlane.transform.rotation;
	}
}