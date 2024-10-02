using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

// reference: https://stackoverflow.com/questions/60475027/unity-android-save-screenshot-in-gallery
public class SaveManager : MonoBehaviour
{
	public GameObject button;
	public GameObject iconButton;
	public void SaveProject()
	{
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
		StartCoroutine(TakeScreenshotAndSave());
	}

	private IEnumerator TakeScreenshotAndSave()
	{
		string timeStamp = System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
		yield return new WaitForEndOfFrame();
		Debug.Log($"executed");


		Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		ss.Apply();

		string filePath = Path.Combine(GetAndroidExternalStoragePath(), "Export-" + timeStamp + ".png");

		File.WriteAllBytes(filePath, ss.EncodeToPNG());

		// Notify the gallery about the new file
#if UNITY_ANDROID
		AndroidJavaClass mediaScanner = new AndroidJavaClass("android.media.MediaScannerConnection");
		AndroidJavaClass playerActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = playerActivity.GetStatic<AndroidJavaObject>("currentActivity");
		mediaScanner.CallStatic("scanFile", activity, new string[] { filePath }, null, null);
#endif

		Debug.Log($"Screenshot saved to gallery: {filePath}");
		Destroy(ss);
	}

	private string GetAndroidExternalStoragePath()
	{
		if (Application.platform != RuntimePlatform.Android)
			return Application.persistentDataPath;

		var jc = new AndroidJavaClass("android.os.Environment");
		var path = jc.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory",
			 jc.GetStatic<string>("DIRECTORY_DCIM"))
			 .Call<string>("getAbsolutePath");

		path = Path.Combine(path, "PhotoWallPlanner");
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		return path;
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
