namespace Picofon.Core.MapPath
{
    using System;
    using Cysharp.Threading.Tasks;
    using PrimeTween;
    using UnityEngine;

    public enum LevelScene
    {
        BasketScene,
        CrossRiverScene,
        BalloonPopSeaScene,
        BalloonPopParty,
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
        # region Constants

        private const int Rows = 2;

        # endregion

        # region References

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

        # endregion


        // Variables

        private float _containerMiddle;

        private RectTransform _container;

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

        public void RenderLevels(LevelDataStore store, in Sequence sequence)
        {
            int count = store.GetPlansCount();
            int current = store.CurrentLevel;
            int last = store.LastLevel;

            float spacingMiddle = _spacing.y / 2;

            int childCount = _container.childCount;

            Span<Vector2> positions = stackalloc Vector2[count];

            for (int i = 0; i < count; i++)
            {
                int col = i;
                int row = i % Rows;
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

                LevelConfig config = _configurations[i & 1];

                ActivityType activityType = (ActivityType)
                    store.GetPlanByIndex(i).TherapyTemplate.TaskTypeId;

                if (activityType != ActivityType.Judge)
                {
                    config = _configurations[0];
                }

                LevelType type = (i & 1) == 0 ? LevelType.Syllable : LevelType.Phoneme;

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

            RectTransform bottomLevel = _container
                .GetChild(count - 1)
                .GetComponent<RectTransform>();

            _contentRect.sizeDelta = new Vector2(
                bottomLevel.anchoredPosition.x + offset,
                _contentRect.sizeDelta.y
            );

            int markerIndex = current;

            if (last >= 0)
            {
                markerIndex = last;
            }

            RectTransform markerLevel = _container
                .GetChild(markerIndex)
                .GetComponent<RectTransform>();

            _marker.PositionMarker(markerLevel.anchoredPosition);

            RectTransform targetLevel = markerLevel;

            if (last >= 0)
            {
                targetLevel = _container.GetChild(current).GetComponent<RectTransform>();
                MoveMarkerToLevel(current, in sequence);
            }

            float targetPositionX = targetLevel.anchoredPosition.x - _containerMiddle;

            if (targetPositionX > 0)
            {
                sequence.Group(
                    Tween.UIAnchoredPositionX(
                        _contentRect,
                        endValue: -targetPositionX,
                        duration: 1f,
                        ease: Ease.OutCubic
                    )
                );
            }
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
                int col = i % Rows;
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
}
