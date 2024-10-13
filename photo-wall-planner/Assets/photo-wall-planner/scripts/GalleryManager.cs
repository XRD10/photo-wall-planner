using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
    public GameObject parentObject;

    public Button choosePictureButton;

    private GameObject imageObject;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        if (parentObject != null)
        {
            imageObject = parentObject.transform.Find("Image").gameObject;

            spriteRenderer = imageObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("The child object 'Image' does not contain a SpriteRenderer component.");
            }
        }
        else
        {
            Debug.LogError("Parent object is not assigned.");
        }

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
        if (imageObject == null || spriteRenderer == null)
        {
            Debug.LogError("Image object or SpriteRenderer component is not assigned.");
            return;
        }
        choosePictureButton.gameObject.SetActive(false);
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path, 512);
                if (texture == null)
                {
                    Debug.LogError("Couldn't load texture from " + path);
                    return;
                }

                Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                
                spriteRenderer.sprite = newSprite;

            }
        }, "Select an image", "image/*");

        Debug.Log("Permission result: " + permission);
    }
}
