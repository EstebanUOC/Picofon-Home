namespace Picofon.Core.MapPath
{
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

        [Header("Overlays")]
        [SerializeField]
        private GameObject _locked;

        [SerializeField]
        private GameObject _completed;

        public LevelConfig Config => _config;
        public int Index => _index;

        private LevelConfig _config;
        private int _index = 0;

        private readonly Color32 _syllableColor = new(255, 255, 255, 255);
        private readonly Color32 _phonemeColor = new(206, 129, 225, 255);

        private const string ScreeningContentPath = "Images/MapPath/Level/ContentGold";
        private const string ScreeningShadowPath = "Images/MapPath/Level/ShadowGold";

        private static Sprite _screeningContent;
        private static Sprite _screeningShadow;

        private Sprite _defaultContent;
        private Sprite _defaultShadow;

        public void Awake()
        {
            _defaultContent = _content.sprite;
            _defaultShadow = _shadow.sprite;
        }

        public void Init(in LevelData data)
        {
            _index = data.Id;
            _config = data.Config;

            SetState(data.State);

            if (data.IsScreening && LoadScreeningSprites())
            {
                _content.sprite = _screeningContent;
                _shadow.sprite = _screeningShadow;
                _content.color = _syllableColor;
                _shadow.color = _syllableColor;
            }
            else
            {
                _content.sprite = _defaultContent;
                _shadow.sprite = _defaultShadow;
                SetBackgroundColor(data.Type);
            }

            _icon.sprite = data.Config.LevelIcon;
        }

        private static bool LoadScreeningSprites()
        {
            if (_screeningContent == null)
                _screeningContent = Resources.Load<Sprite>(ScreeningContentPath);

            if (_screeningShadow == null)
                _screeningShadow = Resources.Load<Sprite>(ScreeningShadowPath);

            return _screeningContent != null && _screeningShadow != null;
        }

        private void SetState(LevelState value)
        {
            switch (value)
            {
                case LevelState.Locked:
                    Instantiate(_locked, transform);
                    break;
                case LevelState.Unlocked:
                    break;
                case LevelState.Completed:
                    Instantiate(_completed, transform);
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
}
