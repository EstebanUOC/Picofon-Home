using System;
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

    [SerializeField]
    private Image BackgroundImage;

    [Header("States Overlays")]
    [SerializeField]
    private GameObject LockedOverlay;

    private LevelData data;
    private Button button;
    private bool isLocked = false;
    private int levelNumber = 1;

    private void SetData(LevelData value, Action onClick)
    {
        data = value;
        IconImage.sprite = data.LevelIcon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke());
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

    private void SetBackgroundColor(int levelType)
    {
        BackgroundImage.color = levelType switch
        {
            0 => new Color32(70, 153, 178, 255),
            1 => new Color32(206, 129, 225, 255),
            _ => new Color32(255, 255, 255, 255),
        };
    }

    public enum LevelType
    {
        Syllable = 0,
        Phoneme = 1,
    }

    public void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Init(
        LevelData levelData,
        int number,
        bool locked,
        LevelType levelType = LevelType.Syllable,
        Action onClick = null
    )
    {
        SetData(levelData, onClick);
        SetLevelNumber(number);
        SetIsLocked(locked);
        SetBackgroundColor((int)levelType);
    }
}
