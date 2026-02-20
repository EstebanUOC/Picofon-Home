using System;
using Cysharp.Threading.Tasks;
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

public sealed class LevelItemManager : MonoBehaviour
{
    [Space]
    [SerializeField]
    private LevelScene _scene;

    [SerializeField]
    private LevelConfig[] _configurations;

    [Header("References")]
    [SerializeField]
    private GameObject _levelPrefab;

    [SerializeField]
    private Marker _marker;

    [SerializeField]
    private LevelPath _path;

    [SerializeField]
    private RectTransform _contentRect;

    [Header("Grid placement")]
    [SerializeField]
    private int _startPos = -400;

    [SerializeField]
    private Vector2 _spacing = new(450f, -500f);

    private int _columns = 2;
    private RectTransform _container;

    public void Awake()
    {
        _container = GetComponent<RectTransform>();
    }

    public void RenderLevels(int count, int last, int current)
    {
        float containerMiddle = _container.rect.width / 2;
        float spacingMiddle = _spacing.x / 2;

        int childCount = _container.childCount;

        Span<Vector2> positions = stackalloc Vector2[count];

        for (int i = 0; i < count; i++)
        {
            int col = i % _columns;
            int row = i;
            float x = containerMiddle - spacingMiddle + (col * _spacing.x);
            float y = _startPos + (row * _spacing.y);
            Vector2 position = new(x, y);

            RectTransform child;

            if (i < childCount)
            {
                child = _container.GetChild(i).GetComponent<RectTransform>();
            }
            else
            {
                GameObject obj = Instantiate(_levelPrefab, _container);
                child = obj.transform.GetComponent<RectTransform>();
            }

            child.anchoredPosition = position;
            positions[i] = child.position;

            LevelItemView comp = child.GetComponent<LevelItemView>();

            bool locked = i > current;

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

            comp.Init(in data);
        }

        if (count < childCount)
        {
            for (int i = count; i < childCount; i++)
            {
                Transform child = _container.GetChild(i);
                child.gameObject.SetActive(false);
            }
        }

        _path.ChangePath(positions);

        const float offset = 300f;

        RectTransform bottomLevel = _container.GetChild(count - 1).GetComponent<RectTransform>();

        _contentRect.sizeDelta = new Vector2(
            _contentRect.sizeDelta.x,
            -bottomLevel.anchoredPosition.y + offset
        );

        int markerIndex = last >= 0 ? last : current;

        RectTransform markerLevel = _container.GetChild(markerIndex).GetComponent<RectTransform>();
        _marker.PositionMarker(markerLevel.anchoredPosition);

        if (last >= 0)
            MoveMarkerToLevel(current).Forget();
    }

    public void OnValidate()
    {
        if (_container == null)
            return;

        int childCount = _container.childCount;

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
        }
    }

    private async UniTaskVoid MoveMarkerToLevel(int levelIndex)
    {
        await UniTask.WaitForSeconds(1f);

        RectTransform targetLevel = _container.GetChild(levelIndex) as RectTransform;
        _marker.MoveMarker(targetLevel.anchoredPosition);
    }
}
