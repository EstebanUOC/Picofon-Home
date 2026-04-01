using UnityEngine;
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

public class LevelItemView : MonoBehaviour
{
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

    public LevelConfig Config => _config;
    public int Index => _index;

    private LevelConfig _config;
    private int _index = 0;

    private readonly Color32 _syllableColor = new(255, 255, 255, 255);
    private readonly Color32 _phonemeColor = new(206, 129, 225, 255);

    public void Init(in LevelData data)
    {
        _index = data.Id;
        _config = data.Config;

        SetState(data.State);
        SetBackgroundColor(data.Type);

        _icon.sprite = data.Config.LevelIcon;
    }

    private void SetState(LevelState value)
    {
        switch (value)
        {
            case LevelState.Locked:
                Instantiate(_lockedOverlay, transform);
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
}
