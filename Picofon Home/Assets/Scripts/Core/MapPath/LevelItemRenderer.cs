using DG.Tweening;
using UnityEngine;

public enum LevelScene
{
    BasketScene,
    BalloonPopSeaScene,
    BalloonPopParty,
    CrossRiverScene,
}

public readonly ref struct LevelData
{
    public readonly int id;
    public readonly LevelConfig config;
    public readonly LevelType type;
    public readonly LevelState state;

    public LevelData(int id, LevelConfig config, LevelType type, LevelState state)
    {
        this.id = id;
        this.config = config;
        this.type = type;
        this.state = state;
    }
}

public class LevelItemRenderer : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private LevelScene _scene;

    [Header("Components")]
    public RectTransform _container;
    public GameObject _prefab;

    [Header("Grid placement")]
    [SerializeField]
    private int _startPos = -400;

    [SerializeField]
    private Vector2 _spacing = new(450f, -500f);

    [Space(15)]
    [SerializeField]
    private LevelConfig[] _configurations;

    [Space(15)]
    [SerializeField]
    private int _num;

    [SerializeField]
    private RectTransform _continue;

    private int _columns = 2;

    public void Start()
    {
        const float moveAmount = 20f;

        _continue
            .DOAnchorPosY(_continue.anchoredPosition.y + moveAmount, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void RenderLevels(int count)
    {
        int lastCompleted = GamePrefs.LastCompletedLevel;
        // TODO: TEMP code (delete this later)
        lastCompleted = 2;

        float containerMiddle = _container.rect.width / 2;
        float spacingMiddle = _spacing.x / 2;

        for (int i = 0; i < count; i++)
        {
            int col = i % _columns;
            int row = i;
            float x = containerMiddle - spacingMiddle + (col * _spacing.x);
            float y = _startPos + (row * _spacing.y);
            Vector2 position = new(x, y);

            Transform child = _container.GetChild(i);
            if (child is null)
            {
                GameObject obj = Instantiate(_prefab, _container);
                child = obj.transform;
            }

            child.GetComponent<RectTransform>().anchoredPosition = position;

            LevelItemView comp = child.GetComponent<LevelItemView>();

            bool locked = i > lastCompleted;

            LevelConfig config = _configurations[i % _configurations.Length];

            // TODO: TEMP code (delete this later) (add #if UNITY_EDITOR)
            if (true)
            // if (i == 0)
            {
                foreach (var cnf in _configurations)
                {
                    if (cnf.SceneName == _scene.ToString())
                    {
                        config = cnf;
                        break;
                    }
                }
            }

            LevelType type = i % 2 == 0 ? LevelType.Syllable : LevelType.Phoneme;

            LevelState state = locked ? LevelState.Locked : LevelState.Unlocked;

            LevelData data = new(i, config, type, state);

            comp.Init(data);
        }
    }

    public void OnValidate()
    {
        if (_container == null)
            return;

        int childCount = _container.childCount;
        Vector2 offset = new(0f, -240f);

        float containerMiddle = _container.rect.width / 2;
        float spacingMiddle = _spacing.x / 2;

        for (int i = 0; i < childCount; i++)
        {
            int col = i % _columns;
            int row = i;
            float x = containerMiddle - spacingMiddle + (col * _spacing.x);
            float y = _startPos + (row * _spacing.y);

            Vector2 position = new(x, y);

            RectTransform child = _container.GetChild(i) as RectTransform;
            if (child != null)
            {
                child.anchoredPosition = position;
            }

            if (i != _num)
            {
                continue;
            }

            if (child != null)
            {
                _continue.anchoredPosition = child.anchoredPosition - offset;
            }
        }
    }
}
