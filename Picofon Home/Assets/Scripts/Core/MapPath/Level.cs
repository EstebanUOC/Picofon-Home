using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField]
    private Image IconImage;

    [SerializeField]
    private TMP_Text LabelText;

    [Header("States Overlays")]
    [SerializeField]
    private GameObject LockedOverlay;

    private LevelData data;
    private Button button;
    private bool isLocked = false;
    private int levelNumber = 1;

    private void SetData(LevelData value)
    {
        data = value;
        IconImage.sprite = data.LevelIcon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
            UnityEngine.SceneManagement.SceneManager.LoadScene(data.SceneName)
        );
    }

    private void SetIsLocked(bool value)
    {
        isLocked = value;
        LockedOverlay.SetActive(isLocked);
        button.interactable = !isLocked;
    }

    private void SetLevelNumber(int value)
    {
        levelNumber = value;
        LabelText.text = levelNumber.ToString();
    }

    public void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Init(LevelData levelData, int number, bool locked)
    {
        SetData(levelData);
        SetLevelNumber(number);
        SetIsLocked(locked);
    }
}
