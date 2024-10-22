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
    List<GameObject> ls_FrameObjects = new List<GameObject>();
    [SerializeField]
    public GameObject FrameToSpawn = null;
    [SerializeField]
    private GameObject customFrame;
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
    private List<GameObject> FrameSelection = new List<GameObject>();

    private bool landScape = false;
    private float sizeX;
    private float sizeZ;
    private bool isCustomFrame = false;

    //UI interactions
    public void OpenOrientationWindow()
    {
        FramesButton.SetActive(false);
        OrientationWindow.SetActive(true);
    }

    //Passed to FramePlacer
    public GameObject GetFrame()
    {
        return FrameToSpawn;
    }

    public (float, float) GetCustomFrameSize()
    {
        return (sizeX, sizeZ);
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
        if (NumberOfFrames <= 0 || (buttonPrefab == null))
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
                SetSpawnObject(index);
                FramesButton.SetActive(true);
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
    private void SetSpawnObject(int index)
    {
        if (index >= 0 && index < NumberOfFrames)
        {
                FrameToSpawn = FrameSelection[index];
                isCustomFrame = false;
                return;
        }
        Debug.LogError("Index error - Check object list");
        return;
    }

    public void SetCustomSpawnObject(float sizeX, float sizeZ)
    {
        FrameToSpawn = customFrame;
        this.sizeX = sizeX;
        this.sizeZ = sizeZ;
        isCustomFrame = true;
    }

    public bool IsCustomFrame(){
        return isCustomFrame;
    }
}
