using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

// reference: https://stackoverflow.com/questions/60475027/unity-android-save-screenshot-in-gallery
public class SaveManager : MonoBehaviour
{
	[SerializeField] private GameObject button;
	[SerializeField] private GameObject iconButton;
	[SerializeField] private DistanceManager distanceManager;

	public void SaveProject()
	{
		Debug.Log("Saving project...");
		iconButton.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

		StartCoroutine(ResetScaleAfterDelay());
		TakeScreenshot();
	}

	IEnumerator ResetScaleAfterDelay()
	{
		yield return new WaitForSeconds(0.15f);

		iconButton.transform.localScale = Vector3.one;
	}

	public void TakeScreenshot()
	{
		// distanceManager.ToggleDistanceDisplay();
		StartCoroutine(TakeScreenshotAndSave());
		// distanceManager.ToggleDistanceDisplay();
	}

	private IEnumerator TakeScreenshotAndSave()
	{
		distanceManager.ToggleDistanceDisplay();

		string timeStamp = System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
		yield return new WaitForEndOfFrame();

		Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		ss.Apply();

		string filePath = GetScreenshotPath(timeStamp);
		File.WriteAllBytes(filePath, ss.EncodeToPNG());

		// Notify gallery only on Android
#if UNITY_ANDROID && !UNITY_EDITOR
            NotifyAndroidGallery(filePath);
#endif

		Debug.Log($"Screenshot saved to: {filePath}");
		distanceManager.ToggleDistanceDisplay();

		Destroy(ss);
	}

	private string GetScreenshotPath(string timeStamp)
	{
		string folderPath;
		string fileName = "Export-" + timeStamp + ".png";

#if UNITY_ANDROID && !UNITY_EDITOR
            folderPath = GetAndroidExternalStoragePath();
#elif UNITY_IOS && !UNITY_EDITOR
            folderPath = Application.persistentDataPath;
#else
		// For editor and other platforms, save to application data folder
		folderPath = Path.Combine(
			 System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
			 "PhotoWallPlanner"
		);
#endif

		// Ensure directory exists
		if (!Directory.Exists(folderPath))
		{
			Directory.CreateDirectory(folderPath);
		}

		return Path.Combine(folderPath, fileName);
	}

	private string GetAndroidExternalStoragePath()
	{
		var jc = new AndroidJavaClass("android.os.Environment");
		var path = jc.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory",
			 jc.GetStatic<string>("DIRECTORY_DCIM"))
			 .Call<string>("getAbsolutePath");
		return Path.Combine(path, "PhotoWallPlanner");
	}

	private void NotifyAndroidGallery(string filePath)
	{
		AndroidJavaClass mediaScanner = new AndroidJavaClass("android.media.MediaScannerConnection");
		AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
			 .GetStatic<AndroidJavaObject>("currentActivity");
		mediaScanner.CallStatic("scanFile", activity, new string[] { filePath }, null, null);
	}


	// todo use maybe to hide button
	void SetImageTransparency(GameObject parentButton, float alpha)
	{
		Image[] images = parentButton.GetComponentsInChildren<Image>();

		foreach (Image img in images)
		{
			img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
		}
	}
}
