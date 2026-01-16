using System.Text.Json.Serialization;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public sealed class TherapyData
{
    [JsonInclude]
    public TherapyPlan[] Plans;
}

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private LevelSelectEventChannel _eventChannel;

    [Space(15)]
    [SerializeField]
    private LevelItemRenderer _renderer;

    [SerializeField]
    private Button _exitButton;

    private string _childId = string.Empty;

    public void Start()
    {
        bool existsData = LevelDataStore.Instance.HasPlans();
        if (existsData)
        {
            int count = LevelDataStore.Instance.GetPlansCount();
            _renderer.RenderLevels(count);
            return;
        }

        _childId = MapPathPayload.ChildId;

        if (string.IsNullOrEmpty(_childId))
        {
#if !UNITY_EDITOR
            Debug.LogError("ChildId is null or empty in MapPathPayload.");
            return;
# else
            _childId = "19013454K";
            Debug.LogWarning("Using default ChildId for testing in Unity Editor.");
# endif
        }

        LoadPlans().Forget();

        _exitButton.onClick.AddListener(HandleExitButtonClicked);
    }

    public void OnEnable()
    {
        _eventChannel.OnEventRaised += HandleLevelSelected;
    }

    public void OnDestroy()
    {
        _eventChannel.OnEventRaised -= HandleLevelSelected;
    }

    private async UniTaskVoid LoadPlans()
    {
        TherapyPlanService service = new();
        LevelDataStore store = LevelDataStore.Instance;

        ApiResult<TherapyData> result = await service.GetAllPlans<TherapyData>(_childId);

        if (!result.Success)
        {
            Debug.LogError($"Error loading activities: {result.Message}");
            return;
        }

        TherapyPlan[] plans = result.Data.Plans;

        if (plans is null || plans.Length == 0)
        {
            Debug.LogError("No hay actividades cargadas.");
            return;
        }

        store.SavePlans(plans);

        _renderer.RenderLevels(plans.Length);
    }

    private void HandleLevelSelected(LevelConfig config, int index)
    {
        LevelPayload.PlanIndex = index;

        UnityEngine.SceneManagement.SceneManager.LoadScene(config.SceneName);
    }

    private void HandleExitButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("AuthScene");
    }
}
