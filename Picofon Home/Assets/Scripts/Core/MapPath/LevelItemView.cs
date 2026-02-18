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
    [SerializeField]
    private LevelSelectEventChannel _eventChannel;

    [Space]
    [SerializeField]
    private Image _icon;

    [SerializeField]
    private Image _shadow;

    [SerializeField]
    private Image _content;

    [Space]
    [SerializeField]
    private GameObject _lockedOverlay;

    private LevelConfig _config;
    private bool _isLocked = false;
    private int _index = 0;

    private readonly Color32 _syllableColor = new(255, 255, 255, 255);
    private readonly Color32 _phonemeColor = new(206, 129, 225, 255);

    public void Init(in LevelData data)
    {
        _index = data.id;

        SetState(data.state);
        SetBackgroundColor(data.type);

        _icon.sprite = data.config.LevelIcon;

        _config = data.config;
    }

    private void SetState(LevelState value)
    {
        switch (value)
        {
            case LevelState.Locked:
                _lockedOverlay.SetActive(true);
                _isLocked = true;
                break;
            case LevelState.Unlocked:
                break;
            case LevelState.Completed:
                break;
        }
    }

    private void SetBackgroundColor(LevelType levelType)
    {
        Color32 newColor = levelType switch
        {
            LevelType.Syllable => _syllableColor,
            LevelType.Phoneme => _phonemeColor,
            _ => _syllableColor,
        };

        _shadow.color = newColor;
        _content.color = newColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isLocked)
            return;

        _eventChannel.Raise(_config, _index);
    }
}
