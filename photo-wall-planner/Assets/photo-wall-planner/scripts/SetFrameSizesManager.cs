using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetFrameSizesManager : MonoBehaviour
{
    private Canvas UI;
    [SerializeField] private Button setButton;
    [SerializeField] private TMP_InputField sizeXInput;
    [SerializeField] private TMP_InputField sizeZInput;
    [SerializeField] private TMP_Text invalidInputText;

    [SerializeField] private FramePlacer framePlacer;
    [SerializeField] private GameObject framesButton;

    void Awake()
    {
        HideInvalidInputText();
        UI = GetComponent<Canvas>();
        HideUI();
        setButton.onClick.AddListener(() =>
        {
            if (float.TryParse(sizeXInput.text, out var sizeX) && float.TryParse(sizeZInput.text, out var sizeZ))
            {
                HideInvalidInputText();
                framePlacer.PlaceCustomFrame(sizeX, sizeZ);
                HideUI();
                framesButton.SetActive(true);
            }
            else
            {
                ShowInvalidInputText();
            }
        });
    }

    void HideUI()
    {
        UI.enabled = false;
    }

    private void HideInvalidInputText()
    {
        invalidInputText.enabled = false;
    }

    private void ShowInvalidInputText()
    {
        invalidInputText.enabled = true;
    }
}
