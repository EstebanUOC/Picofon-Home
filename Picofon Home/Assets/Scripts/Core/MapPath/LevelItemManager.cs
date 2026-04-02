using System;
using Cysharp.Threading.Tasks;
using PrimeTween;
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
    public readonly int Id { get; init; }
    public readonly LevelConfig Config { get; init; }
    public readonly LevelType Type { get; init; }
    public readonly LevelState State { get; init; }
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
    private RectTransform _canvas;

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
    private int _startPos = 250;

    [SerializeField]
    private Vector2 _spacing = new(1000, 320);

    private int _rows = 2;
    private RectTransform _container;

    private float _containerMiddle;

    public void Awake()
    {
        _container = GetComponent<RectTransform>();

        CalculateMiddle().Forget();
    }

    private async UniTaskVoid CalculateMiddle()
    {
        await UniTask.WaitForEndOfFrame(this);

        _containerMiddle = _canvas.rect.width / 2;
    }

    public void RenderLevels(int count, int last, int current, in Sequence sequence)
    {
        float spacingMiddle = _spacing.y / 2;

        int childCount = _container.childCount;

        Span<Vector2> positions = stackalloc Vector2[count];

        for (int i = 0; i < count; i++)
        {
            int col = i;
            int row = i % _rows;
            float x = _startPos + (col * _spacing.x);
            float y = spacingMiddle * (row == 0 ? -1 : 1);
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
            LevelButton button = child.GetComponent<LevelButton>();

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

            LevelState state = LevelState.Unlocked;

            if (locked)
                state = LevelState.Locked;

            if (i < current)
                state = LevelState.Completed;

            bool enabled = !locked && i == current;

            LevelData data = new()
            {
                Id = i,
                Config = config,
                Type = type,
                State = state,
            };

            button.Init(enabled: enabled, itemView: comp);
            comp.Init(data: in data);
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

        const float offset = 600;

        RectTransform bottomLevel = _container.GetChild(count - 1).GetComponent<RectTransform>();

        _contentRect.sizeDelta = new Vector2(
            bottomLevel.anchoredPosition.x + offset,
            _contentRect.sizeDelta.y
        );

        int markerIndex = last >= 0 ? last : current;
        markerIndex = 2;

        RectTransform markerLevel = _container.GetChild(markerIndex).GetComponent<RectTransform>();
        _marker.PositionMarker(markerLevel.anchoredPosition);

        float targetX = markerLevel.anchoredPosition.x - _containerMiddle;

        if (targetX > 0)
        {
            sequence.Chain(
                Tween.UIAnchoredPositionX(_contentRect, -targetX, 1f, ease: Ease.OutCubic)
            );
        }

        if (last >= 0)
            MoveMarkerToLevel(current, in sequence);
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
            int col = i % _rows;
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

    private void MoveMarkerToLevel(int levelIndex, in Sequence sequence)
    {
        RectTransform targetLevel = _container.GetChild(levelIndex) as RectTransform;

        _marker.MoveMarker(targetLevel.anchoredPosition, in sequence);
    }
}
