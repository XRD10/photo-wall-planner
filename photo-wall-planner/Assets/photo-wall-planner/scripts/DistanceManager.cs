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

		// Get the mounting point
		Transform mountingPoint = frame.transform.Find("Frame").Find("ls_MountingPoint");

		if (mountingPoint == null)
		{
			mountingPoint = frame.transform.Find("Frame").Find("p_MountingPoint");
			if (mountingPoint == null)
			{
				Debug.LogError("Mounting point not found on frame!");
				return;
			}
		}

		// Get working area bounds
		Bounds workingAreaBounds = workingAreaPlane.GetComponent<Renderer>().bounds;
		Vector3 mountingPosition = mountingPoint.position;

		Vector3 workingAreaCenter = workingAreaPlane.transform.position;
		float positionLeft = workingAreaCenter.y - workingAreaBounds.size.y / 2;
		float positionRight = workingAreaCenter.y + workingAreaBounds.size.y / 2;
		float positionTop = workingAreaCenter.z + workingAreaBounds.size.z / 2;
		float positionBottom = workingAreaCenter.z - workingAreaBounds.size.z / 2;


		float distanceLeft = Mathf.Abs(mountingPosition.y - positionLeft);
		float distanceRight = Mathf.Abs(mountingPosition.y - positionRight);
		float distanceTop = Mathf.Abs(mountingPosition.z - positionTop);
		float distanceBottom = Mathf.Abs(mountingPosition.z - positionBottom);

		CreateOrUpdateDistanceText(frame, distanceLeft, "LeftEdgeDistance", new Vector3(-0.7f, 0.3f, 0));
		CreateOrUpdateDistanceText(frame, distanceRight, "RightEdgeDistance", new Vector3(0.7f, 0.3f, 0));
		CreateOrUpdateDistanceText(frame, distanceTop, "TopEdgeDistance", new Vector3(0, 0.3f, 0.7f));
		CreateOrUpdateDistanceText(frame, distanceBottom, "BottomEdgeDistance", new Vector3(0, 0.3f, -0.7f));
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
