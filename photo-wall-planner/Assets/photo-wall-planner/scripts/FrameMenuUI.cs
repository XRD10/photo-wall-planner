using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FrameMenuUI : MonoBehaviour
{
    
    [SerializeField] 
    private GameObject buttonPrefab;
    [SerializeField] 
    private Transform buttonContainer;
    [SerializeField]
    List<GameObject> p_FrameObjects = new List<GameObject>();
    [SerializeField]
    List<GameObject> ls_FrameObjects = new List<GameObject>(); //HERE we set images
    [SerializeField]
    public GameObject FrameToSpawn;
    [SerializeField]
    public GameObject OrientationWindow;
    private int NumberOfFrames;
    [SerializeField]
    private GameObject FramesButton;
    [SerializeField]
    private GameObject FramesList;
    [SerializeField]
    private Canvas setCustomFrameWindow;
    [SerializeField]
    private Canvas galleryMenu;
    [SerializeField]
    private List<GameObject> FrameSelection = new List<GameObject>();
    private  GameObject instance;

    private bool landScape = false;

    //UI interactions
    public void OpenOrientationWindow()
    {
        FramesButton.SetActive(false);
        OrientationWindow.SetActive(true);
    }

    

    // Method to set the number of frames in the list
    private void SetNumberOfFrames()
    {
        NumberOfFrames = landScape ? ls_FrameObjects.Count : p_FrameObjects.Count;
    }


    //Passed to FramePlacer
    public GameObject GetFrame()
    {
        return FrameToSpawn;
    }

    public void SetFrameSelection(List<GameObject> frameList)
    {
        NumberOfFrames = frameList.Count;
        FrameSelection.Clear();
        FrameSelection.AddRange(frameList);
    }

    public void SetLandscape()
    {
        landScape = true;
        SetFrameSelection(ls_FrameObjects);
       // SetNumberOfFrames();
        OrientationWindow.SetActive(false);
        FramesList.SetActive(true);
        PopulateObjectList();
    }
    public void SetPortrait()
    {
        landScape = false;
        OrientationWindow.SetActive(false);
       // SetNumberOfFrames();
        SetFrameSelection(p_FrameObjects);
        FramesList.SetActive(true);
        PopulateObjectList();
    }

    public bool GetLandscape()
    {
        return landScape;
    }

    public void SetCustomFrame()
    {
        OrientationWindow.SetActive(false);
        setCustomFrameWindow.enabled = true;
    }




    //Making buttons in the list
    void PopulateObjectList()
    {
        if ((NumberOfFrames <= 0 || (buttonPrefab == null)))
        {
            Debug.LogError("Objects error - Check object list" + NumberOfFrames);
            return;
        }

        for (int i = 0; i < NumberOfFrames; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = FrameSelection[(int)i].name;

            int index = i; // Create a local copy of the index for the lambda function

            newButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                DestroyAllChildren();
                FramesList.SetActive(false);
                SetSpawnObject(index); //Here we SET
                FramesButton.SetActive(true); 
                galleryMenu.gameObject.SetActive(true);

            });
        }
    }

    private void DestroyAllChildren()
    {
        foreach(Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
    }

    //Selects the frame to be spawned
    public void SetSpawnObject(int index)
    {
        if (index >= 0 && index < NumberOfFrames)
        {
                FrameToSpawn = FrameSelection[index];
                return;
        }
        Debug.LogError("Index error - Check object list");
        return;
    }
}
