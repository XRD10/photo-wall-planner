using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetFrameSizesManager : MonoBehaviour
{
    private Canvas UI;
    [SerializeField] private Button setButton;
    [SerializeField] private TMP_InputField sizeXInput;
    [SerializeField] private TMP_InputField sizeZInput;

    [SerializeField] private FramePlacer framePlacer;

    void Awake()
    {
        UI = GetComponent<Canvas>();
        HideUI();
        setButton.onClick.AddListener(() =>
        {
            //TODO: Implement error handling
            if (float.TryParse(sizeXInput.text, out var sizeX) && float.TryParse(sizeZInput.text, out var sizeZ))
            {
                framePlacer.PlaceFrame(sizeX, sizeZ);
                HideUI();
            }
        });
    }

    void HideUI()
    {
        UI.enabled = false;
    }
}
