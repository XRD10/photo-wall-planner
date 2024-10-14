using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{

    public Button choosePictureButton;

    private GameObject imageObject;

    private GameObject frameObject;

    public GameObject frameManager;
    private SpriteRenderer spriteRenderer;
    private GameObject frameToSpawn;

    void Start()
    {

      
            frameToSpawn = frameManager.GetComponent<FramePlacer>().GetPlacedObject();  /

            imageObject = frameToSpawn.transform.Find("Image").gameObject;
            frameObject = frameToSpawn.transform.Find("Frame").gameObject;

            spriteRenderer = imageObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("The child object 'Image' does not contain a SpriteRenderer component.");
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

            float targetWidth = frameObject.GetComponent<SpriteRenderer>().bounds.size.x; 
            float targetHeight = frameObject.GetComponent<SpriteRenderer>().bounds.size.z; 
            Debug.Log(targetWidth);
            Debug.Log("ANDTWO");
            Debug.Log(frameObject.GetComponent<SpriteRenderer>().bounds.size.z.ToString());

            Debug.Log("Bounds: " + frameObject.GetComponent<SpriteRenderer>().bounds.ToString());

            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            spriteRenderer.sprite = newSprite;

            float currentWidth = spriteRenderer.bounds.size.x; // This is in world units

            float scaleFactor = targetWidth / currentWidth;
            Debug.Log("BoundsTwo: " + spriteRenderer.bounds.size.ToString());
            Debug.Log(targetWidth);
            Debug.Log("AND");
            Debug.Log(currentWidth);

             Debug.Log("Scale factor: " + scaleFactor);
            
            imageObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        }
    }, "Select an image", "image/*");

    Debug.Log("Permission result: " + permission);
}
}