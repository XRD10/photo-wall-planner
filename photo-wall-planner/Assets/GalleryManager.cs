using UnityEngine;
using UnityEngine.UI;
public class GalleryManager : MonoBehaviour
{
    // Reference to the Button that will open the gallery
    public Button choosePictureButton;

    // Reference to the RawImage that will display the selected image
    public RawImage displayImage;

    void Start()
    {
        // Ensure the button is assigned in the inspector or find it programmatically
        if (choosePictureButton != null)
        {
            // Add a listener to the button to call the OpenGallery function when clicked
            choosePictureButton.onClick.AddListener(OpenGallery);
        }
        else
        {
            Debug.LogError("Open Gallery Button is not assigned.");
        }
    }

    // Method to open the phone's gallery and select an image
    public void OpenGallery()
    {
        // Request permission to access the gallery
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                // Load the image from the selected path into a Texture2D
                Texture2D texture = NativeGallery.LoadImageAtPath(path, 512);
                if (texture == null)
                {
                    Debug.LogError("Couldn't load texture from " + path);
                    return;
                }

                // Display the selected image in the RawImage UI element
                displayImage.texture = texture;
                displayImage.SetNativeSize(); // Optional: Adjust the RawImage size to match the texture
            }
        }, "Select an image", "image/*");

        Debug.Log("Permission result: " + permission);
    }
}
