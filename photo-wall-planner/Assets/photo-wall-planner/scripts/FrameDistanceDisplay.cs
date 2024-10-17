using UnityEngine;
using TMPro;

public class FrameDistanceDisplay : MonoBehaviour
{
	[SerializeField] private WorkingAreaManager workingAreaManager;
	[SerializeField] private GameObject distanceTextPrefab;
	[SerializeField] private float updateInterval = 0.5f; // Update every half second

	private float timeSinceLastUpdate = 0f;

	private void Update()
	{
		timeSinceLastUpdate += Time.deltaTime;
		if (timeSinceLastUpdate >= updateInterval)
		{
			UpdateDistances();
			timeSinceLastUpdate = 0f;
		}
	}

	private void UpdateDistances()
	{
		GameObject[] frames = GameObject.FindGameObjectsWithTag("Placable");
		Vector3 workingAreaCenter = workingAreaManager.GetWorkingAreaCenter();
		Vector3 workingAreaSize = workingAreaManager.GetWorkingAreaSize();
		Quaternion workingAreaRotation = workingAreaManager.GetWorkingAreaRotation();

		foreach (GameObject frame in frames)
		{
			Vector3 framePos = frame.transform.position;
			Vector3 localPos = Quaternion.Inverse(workingAreaRotation) * (framePos - workingAreaCenter);

			float distLeft = (workingAreaSize.x / 2) + localPos.x;
			float distRight = (workingAreaSize.x / 2) - localPos.x;
			float distFront = (workingAreaSize.z / 2) + localPos.z;
			float distBack = (workingAreaSize.z / 2) - localPos.z;

			UpdateOrCreateDistanceText(frame, "DistLeft", distLeft, new Vector3(-1, 0, 0));
			UpdateOrCreateDistanceText(frame, "DistRight", distRight, new Vector3(1, 0, 0));
			UpdateOrCreateDistanceText(frame, "DistFront", distFront, new Vector3(0, 0, 1));
			UpdateOrCreateDistanceText(frame, "DistBack", distBack, new Vector3(0, 0, -1));
		}
	}

	private void UpdateOrCreateDistanceText(GameObject frame, string name, float distance, Vector3 offset)
	{
		Transform textTransform = frame.transform.Find(name);
		if (textTransform == null)
		{
			GameObject textObj = Instantiate(distanceTextPrefab, frame.transform);
			textObj.name = name;
			textTransform = textObj.transform;
			textTransform.localPosition = offset * 0.1f; // Offset slightly from the frame
			textTransform.rotation = Quaternion.LookRotation(offset);
		}

		TextMeshPro tmp = textTransform.GetComponent<TextMeshPro>();
		tmp.text = $"{distance:F2}m";
	}
}