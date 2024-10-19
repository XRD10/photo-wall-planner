using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{

    [SerializeField]
    private Button choosePictureButton;
    [SerializeField]
    private Canvas galleryMenu;
    private Texture2D texture;

    void Start()
    {
        if (choosePictureButton != null)
        {
            choosePictureButton.onClick.AddListener(OpenGallery);
        }
        else
        {
            Debug.LogError("Choose Picture Button is not assigned.");
        }
    }

    public void OpenGallery()
    {
        galleryMenu.gameObject.SetActive(false);
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                texture = NativeGallery.LoadImageAtPath(path, 512);
                if (texture == null)
                {
                    Debug.LogError("Couldn't load texture from " + path);
                    return;
                }


            }
        }, "Select an image", "image/*");
    }

    public Texture2D getPictureFromGallery()
    {
        return texture;
    }
}