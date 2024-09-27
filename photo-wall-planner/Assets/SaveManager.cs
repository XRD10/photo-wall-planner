using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
	public GameObject button;
	public GameObject iconButton;
	public void SaveGame()
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

	void TakeScreenshot()
	{
		string screenshotFileName = $"test-screenshots/screenshot_{System.DateTime.Now.ToString("yyyyMMdd_HHmmss")}.png";
		ScreenCapture.CaptureScreenshot(screenshotFileName);
		Debug.Log($"Screenshot saved as {screenshotFileName}");
	}

	// todo use maybe
	void SetImageTransparency(GameObject parentButton, float alpha)
	{
		Image[] images = parentButton.GetComponentsInChildren<Image>();

		foreach (Image img in images)
		{
			img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
		}
	}
}
