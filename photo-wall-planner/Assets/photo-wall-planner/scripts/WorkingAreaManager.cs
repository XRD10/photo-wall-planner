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
	[SerializeField] private float tolerance = 0.25f;

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

		Vector3 adjustedPosition = hitpose.position + hitpose.up * 0.01f;

		if (workingAreaPlane == null)
		{

			CompleteButton.SetActive(true);
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
		workingAreaPlane.tag = "Untagged";
		CompleteButton.SetActive(false);
	}

	public bool IsEditingComplete()
	{
		return isWorkingAreaEditingComplete;
	}

	public bool IsPointInsideWorkingArea(Vector3 point)
	{
		if (workingAreaPlane == null || !isWorkingAreaEditingComplete)
		{
			return false;
		}

		Transform planeTransform = workingAreaPlane.transform;
		Vector3 planeNormal = planeTransform.up;
		Vector3 planePoint = planeTransform.position;

		float distanceToPlane = Mathf.Abs(Vector3.Dot(planeNormal, point - planePoint));

		// If the point is too far from the plane surface, it's not inside
		if (distanceToPlane > tolerance)
		{
			Debug.Log("Point is too far from the plane");
			return false;
		}

		// Project the point onto the plane
		Vector3 projectedPoint = point - distanceToPlane * planeNormal;

		// Calculate the local axes of the plane
		Vector3 forward = planeTransform.forward;
		Vector3 right = planeTransform.right;

		// Calculate vectors from plane center to the projected point
		Vector3 toPoint = projectedPoint - planePoint;

		// Calculate the dot products to find local coordinates
		float forwardDot = Vector3.Dot(toPoint, forward);
		float rightDot = Vector3.Dot(toPoint, right);

		float halfLength = planeTransform.localScale.z / 2f;
		float halfWidth = planeTransform.localScale.x / 2f;


		return Mathf.Abs(forwardDot) <= halfLength && Mathf.Abs(rightDot) <= halfWidth;
	}
	public GameObject GetPlane()
	{
		return workingAreaPlane;
	}

	public Vector3 GetWorkingAreaMinBounds()
	{
		return workingAreaPlane.GetComponent<Renderer>().bounds.min;
	}

	public Vector3 GetWorkingAreaMaxBounds()
	{
		return workingAreaPlane.GetComponent<Renderer>().bounds.max;
	}
}