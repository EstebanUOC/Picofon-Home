using UnityEngine;

public class WordSegmentationManager : MonoBehaviour
{
    [SerializeField]
    private GameObject background;

    [SerializeField]
    private GameObject background1;

    [SerializeField]
    private SimpleButton button;

    public void Start()
    {
        button.OnClick += OnButtonClick;
    }

    private void OnButtonClick()
    {
        background.SetActive(!background.activeSelf);
        background1.SetActive(!background1.activeSelf);
    }
}
