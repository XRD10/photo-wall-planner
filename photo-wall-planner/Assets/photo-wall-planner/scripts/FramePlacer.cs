using System;
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
        instance.tag = "Placable";
        applyPicture();
        return instance;
    }

    public void applyPicture()
    {
        GameObject imageObject = instance.transform.Find("Image").gameObject;
        GameObject frameObject = instance.transform.Find("Frame").gameObject;
        SpriteRenderer spriteRenderer = imageObject.GetComponent<SpriteRenderer>();
        Boolean isLandscape = frames.GetLandscape();
        float scaleFactor;

        Texture2D texture = galleryManager.getPictureFromGallery();
        Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        float targetSizeLandscape;
        float currentSizeLandscape;
        float targetSizePortrait;
        float currentSizePortrait;
        spriteRenderer.sprite = newSprite;
        if (isLandscape)
        {
            targetSizeLandscape = frameObject.GetComponent<SpriteRenderer>().bounds.size.y;
            Debug.Log(newSprite.bounds.size.x + " AND " + newSprite.bounds.size.y + " AND " + newSprite.bounds.size.x);

            if (newSprite.bounds.size.x > newSprite.bounds.size.y)
            {
                currentSizeLandscape = spriteRenderer.bounds.size.y;
                scaleFactor = targetSizeLandscape / currentSizeLandscape;
                imageObject.transform.localScale = new Vector3(scaleFactor * 0.6f, scaleFactor * 0.6f, 1);
            }
            else
            {
                currentSizeLandscape = spriteRenderer.bounds.size.y;
                scaleFactor = targetSizeLandscape / currentSizeLandscape;
                imageObject.transform.localScale = new Vector3(scaleFactor * 0.8f, scaleFactor * 0.8f, 1);
            }

        }
        else
        {
            targetSizePortrait = frameObject.GetComponent<SpriteRenderer>().bounds.size.z;
            if (newSprite.bounds.size.x > newSprite.bounds.size.y)
            {
                currentSizePortrait = spriteRenderer.bounds.size.z;
                scaleFactor = targetSizePortrait / currentSizePortrait;
                imageObject.transform.localScale = new Vector3(scaleFactor * 0.6f, scaleFactor * 0.6f, 1);
            }
            else
            {
                currentSizePortrait = spriteRenderer.bounds.size.z;
                targetSizePortrait = frameObject.GetComponent<SpriteRenderer>().bounds.size.z;
                scaleFactor = targetSizePortrait / currentSizePortrait;
                imageObject.transform.localScale = new Vector3(scaleFactor * 0.8f, scaleFactor * 0.8f, 1);
            }

        }

        // Apply rotation to the image based on landscape or portrait mode
        // float yRotation = isLandscape ? -90f : -180f;
        float yRotation = isLandscape ? 0f : -90f;
        imageObject.transform.Rotate(0, 0, yRotation);
    }
}