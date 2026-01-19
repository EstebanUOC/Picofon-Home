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
    private Vector2 _startPos = new(300f, -300f);

    [SerializeField]
    private Vector2 _spacing = new(450f, -500f);

    [Space(15)]
    [SerializeField]
    private LevelConfig[] _configurations;

    private int _columns = 2;

    public void RenderLevels(int count)
    {
        int lastCompleted = GamePrefs.LastCompletedLevel;
        // TODO: TEMP code (delete this later)
        lastCompleted = 2;

        for (int i = 0; i < count; i++)
        {
            int col = i % _columns;
            int row = i;
            float x = _startPos.x + (col * _spacing.x);
            float y = _startPos.y + (row * _spacing.y);
            Vector2 position = new(x, y);

            GameObject obj = Instantiate(_prefab, _container);
            obj.GetComponent<RectTransform>().anchoredPosition = position;

            LevelItemView comp = obj.GetComponent<LevelItemView>();

            bool locked = i > lastCompleted;

            LevelConfig config = _configurations[i % _configurations.Length];

#if UNITY_EDITOR
            // TODO: TEMP code (delete this later)
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
#endif

            LevelType type = i % 2 == 0 ? LevelType.Syllable : LevelType.Phoneme;

            LevelState state = locked ? LevelState.Locked : LevelState.Unlocked;

            LevelData data = new(i, config, type, state);

            comp.Init(data);
        }
    }
}
