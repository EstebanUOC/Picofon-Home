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

        // Niveles del screening inicial (HOME_DISCOVERY): sprites dorados. Image.color
        // multiplica el sprite azul, así que con un tinte no se puede conseguir dorado.
        private const string ScreeningContentPath = "Images/MapPath/Level/ContentGold";
        private const string ScreeningShadowPath = "Images/MapPath/Level/ShadowGold";

        // Icono SPS: mano en "V" de la hoja de manos de segmentación
        private const string SpsIconSheetPath = "Images/Segmentation/Hands";
        private const string SpsIconName = "Hands3_2";

        private static Sprite _screeningContent;
        private static Sprite _screeningShadow;
        private static Sprite _spsIcon;

        // Sprites del prefab (malla), para restaurarlos al reutilizar el item
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

            bool spsIcon = data.IsSps && LoadSpsIcon();

            _icon.sprite = spsIcon ? _spsIcon : data.Config.LevelIcon;

            // La mano es vertical (584x704): sin preserveAspect se deformaría
            _icon.preserveAspect = spsIcon;
        }

        private static bool LoadScreeningSprites()
        {
            if (_screeningContent == null)
                _screeningContent = Resources.Load<Sprite>(ScreeningContentPath);

            if (_screeningShadow == null)
                _screeningShadow = Resources.Load<Sprite>(ScreeningShadowPath);

            return _screeningContent != null && _screeningShadow != null;
        }

        private static bool LoadSpsIcon()
        {
            if (_spsIcon != null)
                return true;

            foreach (Sprite sprite in Resources.LoadAll<Sprite>(SpsIconSheetPath))
            {
                if (sprite.name == SpsIconName)
                {
                    _spsIcon = sprite;
                    break;
                }
            }

            return _spsIcon != null;
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
