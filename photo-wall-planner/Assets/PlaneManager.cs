using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARSubsystems;

public class PlaneManager : PressInputBase
{
	[SerializeField] private Camera arCamera;
	public GameObject planePrefab;  // Prefab for the new plane to create
	private GameObject currentPlane;  // Reference to the plane being created
	private ARRaycastManager raycastManager;

	private Vector3 topLeftCorner;
	private Vector3 bottomRightCorner;
	private bool isSelectingTopLeft = true;

	private static readonly List<ARRaycastHit> _hits = new();
	private Ray ray;

	void Start()
	{
		raycastManager = FindFirstObjectByType<ARRaycastManager>();
	}

	protected override void OnPressBegan(Vector3 position)
	{
		base.OnPressBegan(position);

		// Prevent touches over UI
		if (EventSystem.current.IsPointerOverGameObject()) return;

		// Raycast to check if touching a plane
		if (!raycastManager.Raycast(position, _hits, TrackableType.PlaneWithinPolygon)) return;

		ray = arCamera.ScreenPointToRay(position);

		// Check if a frame or another plane is hit
		if (Physics.Raycast(ray, out RaycastHit hitObject))
		{
			if (hitObject.transform.CompareTag("Placable")) return; // Avoid placing on an existing object
		}

		// Handle the corner selection
		if (isSelectingTopLeft)
		{
			topLeftCorner = _hits[0].pose.position;
			Debug.Log("Top Left Corner Set: " + topLeftCorner);
			isSelectingTopLeft = false; // Move to bottom-right selection
		}
		else
		{
			bottomRightCorner = _hits[0].pose.position;
			Debug.Log("Bottom Right Corner Set: " + bottomRightCorner);
			CreateOrUpdatePlane(); // Create or update the plane based on corner selection
			ResetSelection(); // Reset selection for next use
		}
	}

	private void CreateOrUpdatePlane()
	{
		// Check if a plane already exists
		if (currentPlane == null)
		{
			// Create a new plane
			currentPlane = Instantiate(planePrefab, topLeftCorner, Quaternion.identity);
			Debug.Log("Plane Created: " + currentPlane.name);
		}
		else
		{
			// Update the existing plane's position and size
			Debug.Log("Updating Existing Plane: " + currentPlane.name);
		}

		// Calculate width and height based on corners
		float width = Mathf.Abs(bottomRightCorner.x - topLeftCorner.x);
		float height = Mathf.Abs(bottomRightCorner.z - topLeftCorner.z);
		Debug.Log(bottomRightCorner.x + " " + topLeftCorner.x);
		Debug.Log("Width: " + width + " Height: " + height);
		// Set the scale of the plane
		currentPlane.transform.localScale = new Vector3(width, currentPlane.transform.localScale.y, height);
		currentPlane.transform.position = new Vector3(
			 (topLeftCorner.x + bottomRightCorner.x) / 2,
			 currentPlane.transform.position.y,
			 (topLeftCorner.z + bottomRightCorner.z) / 2
		);
	}

	private void ResetSelection()
	{
		// Reset corner selection flags
		isSelectingTopLeft = true;
	}
}
