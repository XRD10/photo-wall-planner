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
		Vector3 framePosition = frame.transform.position;

		// Get working area bounds
		Vector3 minBounds = workingAreaManager.GetWorkingAreaMinBounds();
		Vector3 maxBounds = workingAreaManager.GetWorkingAreaMaxBounds();

		// Calculate distances to each edge of the working area
		float distanceToLeftEdge = Mathf.Abs(framePosition.x - minBounds.x);
		float distanceToRightEdge = Mathf.Abs(maxBounds.x - framePosition.x);
		float distanceToTopEdge = Mathf.Abs(maxBounds.z - framePosition.z);
		float distanceToBottomEdge = Mathf.Abs(framePosition.z - minBounds.z);

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
