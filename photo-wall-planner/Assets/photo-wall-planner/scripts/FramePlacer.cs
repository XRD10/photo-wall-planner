using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class FramePlacer : PressInputBase
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject objectToPlace;
    [SerializeField] private FrameMenuUI frames;
    [SerializeField] private Canvas setFrameSizesCanvas;
    [SerializeField] private Camera arCamera;
    [SerializeField] private GalleryManager galleryManager;

    private GameObject instance;
    private static readonly List<ARRaycastHit> _hits = new();
    private Ray ray;



    protected override void OnPressBegan(Vector3 position)
    {
        base.OnPressBegan(position);
        if (EventSystem.current.IsPointerOverGameObject()) return;
        objectToPlace = frames.GetFrame();
        if (!raycastManager.Raycast(position, _hits, TrackableType.PlaneWithinPolygon)) return;

        ray = arCamera.ScreenPointToRay(position);

        if (Physics.Raycast(ray, out RaycastHit hitObject))
            if (hitObject.transform.CompareTag("Placable")) return;
        if (objectToPlace == null)
        {
            Debug.LogError("No object selected");
            return;
        }

        PlaceFrame();

    }
    public void PlaceFrame()
    {
        //from cm to unity units (m)

        var hitpose = _hits[0].pose;

        instance = Instantiate(objectToPlace, hitpose.position, Quaternion.identity);
        instance.transform.localScale = new Vector3(objectToPlace.transform.localScale.x, objectToPlace.transform.localScale.y / 10, objectToPlace.transform.localScale.z);

        instance.transform.up = hitpose.up;

        float yRotation = frames.GetLandscape() ? 0f : 90f;
        instance.transform.Rotate(0, yRotation, 0, Space.Self);
        instance.tag = "Placable";
        applyPicture();

    }

    public GameObject PlaceCustomFrame(float sizeX, float sizeZ)
    {
        //from cm to unity units (m)
        sizeX /= 100;
        sizeZ /= 100;

        var hitpose = _hits[0].pose;

        instance = Instantiate(objectToPlace, hitpose.position, hitpose.rotation);
        instance.transform.localScale = new Vector3(sizeX, objectToPlace.transform.localScale.y / 10, sizeZ);
        return instance;
    }

  public void applyPicture()
{
    StartCoroutine(WaitAndApplyPicture());
}

private IEnumerator WaitAndApplyPicture()
{
    // Wait until the frame object is fully placed (1 frame delay)
    yield return new WaitForEndOfFrame();

    Debug.Log("Apply after frame placement", instance);

    GameObject imageObject = instance.transform.Find("Image").gameObject;
    GameObject frameObject = instance.transform.Find("Frame").gameObject;
    SpriteRenderer spriteRenderer = imageObject.GetComponent<SpriteRenderer>();

    float targetWidth = frameObject.GetComponent<SpriteRenderer>().bounds.size.x;
    float targetHeight = frameObject.GetComponent<SpriteRenderer>().bounds.size.y;

    Debug.Log("Target Width after placement: " + targetWidth);
    Debug.Log("Target Height after placement: " + targetHeight);

    Texture2D texture = galleryManager.getPictureFromGallery();
    Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    spriteRenderer.sprite = newSprite;

    float currentWidth = spriteRenderer.bounds.size.x; 
    float currentHeight = spriteRenderer.bounds.size.y; 

    float scaleFactor = targetHeight / currentHeight;

    Debug.Log("Scale Factor after placement: " + scaleFactor);
    imageObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);

    // Apply rotation to the image based on landscape or portrait mode
    Debug.Log("Landscape"+frames.GetLandscape());

    float yRotation = frames.GetLandscape() ? -90f : -180f;
    imageObject.transform.Rotate(0, 0, yRotation);
}




}