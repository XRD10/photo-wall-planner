using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DistanceManager : MonoBehaviour
{
	private bool displayDistances = false;
	private readonly List<GameObject> frames = new List<GameObject>();
	private WorkingAreaManager workingAreaManager;
	private void Awake()
	{
		workingAreaManager = FindFirstObjectByType<WorkingAreaManager>();
	}

	public void AddFrame(GameObject frame)
	{
		frames.Add(frame);
		CalculateAndDisplayDistances(frame);
	}

	public void ToggleDistanceDisplay()
	{
		displayDistances = !displayDistances;

		foreach (GameObject frame in frames)
		{
			if (displayDistances)
			{
				// Calculate and display distances only when turning on the display
				CalculateAndDisplayDistances(frame);
			}
			ToggleFrameDistances(frame, displayDistances);
		}
	}

	private void ToggleFrameDistances(GameObject frame, bool display)
	{
		foreach (Transform child in frame.transform)
		{
			if (child.GetComponent<TextMeshPro>())
			{
				child.gameObject.SetActive(display);
			}
		}
	}
	private void CalculateAndDisplayDistances(GameObject frame)
	{
		GameObject workingAreaPlane = workingAreaManager.GetPlane();
		Transform workingArea = workingAreaPlane.transform;

		// Get the frame bounds (assuming it has a renderer)
		Bounds frameBounds = frame.GetComponent<Renderer>().bounds;
		Vector3 frameCenter = frameBounds.center;

		// Get working area bounds
		Bounds workingAreaBounds = workingAreaPlane.GetComponent<Renderer>().bounds;

		// Calculate the distances to each edge
		float distanceToLeftEdge = Mathf.Abs(frameCenter.y - workingAreaBounds.min.y);
		float distanceToRightEdge = Mathf.Abs(workingAreaBounds.max.y - frameCenter.y);
		float distanceToTopEdge = Mathf.Abs(workingAreaBounds.max.z - frameCenter.z);
		float distanceToBottomEdge = Mathf.Abs(frameCenter.z - workingAreaBounds.min.z);

		// // Account for frame's own size
		// float frameHalfWidth = frameBounds.size.y / 2f;
		// float frameHalfLength = frameBounds.size.z / 2f;

		// // Adjust distances by subtracting frame's half size
		// distanceToLeftEdge -= frameHalfWidth;
		// distanceToRightEdge -= frameHalfWidth;
		// distanceToTopEdge -= frameHalfLength;
		// distanceToBottomEdge -= frameHalfLength;

		// Ensure distances don't go below 0
		distanceToLeftEdge = Mathf.Max(0, distanceToLeftEdge);
		distanceToRightEdge = Mathf.Max(0, distanceToRightEdge);
		distanceToTopEdge = Mathf.Max(0, distanceToTopEdge);
		distanceToBottomEdge = Mathf.Max(0, distanceToBottomEdge);

		// For debugging
		Debug.DrawLine(frameCenter, new Vector3(workingAreaBounds.min.y, frameCenter.y, frameCenter.z), Color.red, 1f);
		Debug.DrawLine(frameCenter, new Vector3(workingAreaBounds.max.y, frameCenter.y, frameCenter.z), Color.green, 1f);
		Debug.DrawLine(frameCenter, new Vector3(frameCenter.y, frameCenter.y, workingAreaBounds.max.z), Color.blue, 1f);
		Debug.DrawLine(frameCenter, new Vector3(frameCenter.y, frameCenter.y, workingAreaBounds.min.z), Color.yellow, 1f);

		// Create or update the text for displaying distances
		CreateOrUpdateDistanceText(frame, distanceToLeftEdge, "LeftEdgeDistance", new Vector3(-0.7f, 0.3f, 0));
		CreateOrUpdateDistanceText(frame, distanceToRightEdge, "RightEdgeDistance", new Vector3(0.7f, 0.3f, 0));
		CreateOrUpdateDistanceText(frame, distanceToTopEdge, "TopEdgeDistance", new Vector3(0, 0.3f, 0.7f));
		CreateOrUpdateDistanceText(frame, distanceToBottomEdge, "BottomEdgeDistance", new Vector3(0, 0.3f, -0.7f));
	}


	private void CreateOrUpdateDistanceText(GameObject frame, float distance, string textObjectName, Vector3 localPosition)
	{
		// Check if the distance text object already exists
		Transform textTransform = frame.transform.Find(textObjectName);

		if (textTransform == null)
		{
			// Create a new text object if it doesn't exist
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
		else
		{
			// Update the text if the object already exists
			textTransform.GetComponent<TextMeshPro>().text = (int)(distance * 100) + "cm";
		}
	}

}
