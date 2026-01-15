using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum LevelType : byte
{
    Syllable = 0,
    Phoneme = 1,
}

public enum LevelState : byte
{
    Locked = 0,
    Unlocked = 1,
    Completed = 2,
}

public class LevelItemView : MonoBehaviour, IPointerClickHandler
{
    [Space(15)]
    [SerializeField]
    private Image _icon;

    [SerializeField]
    private Image _background;

    [Space(15)]
    [SerializeField]
    private GameObject _lockedOverlay;

    private readonly Color32 _syllableColor = new(70, 153, 178, 255);
    private readonly Color32 _phonemeColor = new(206, 129, 225, 255);

    public void Init(LevelData data, LevelState state, LevelType type = LevelType.Syllable)
    {
        SetState(state);
        SetBackgroundColor(type);

        _icon.sprite = data.LevelIcon;
    }

    private void SetState(LevelState value)
    {
        switch (value)
        {
            case LevelState.Locked:
                _lockedOverlay.SetActive(true);
                break;
            case LevelState.Unlocked:
                break;
            case LevelState.Completed:
                break;
        }
    }

    private void SetBackgroundColor(LevelType levelType)
    {
        _background.color = levelType switch
        {
            LevelType.Syllable => _syllableColor,
            LevelType.Phoneme => _phonemeColor,
            _ => _syllableColor,
        };
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Level clicked!");
    }
}
