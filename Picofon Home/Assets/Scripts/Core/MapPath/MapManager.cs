using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

public enum ActivityType : byte
{
    Judge = 1,
    Select = 2,
    Relate = 3,
}

public enum LanguageID : byte
{
    Catalan = 1,
    Spanish = 2,
}

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private LevelSelectEventChannel _eventChannel;

    [Space]
    [SerializeField]
    private LevelItemManager _itemManager;

    [Space]
    [SerializeField]
    private Fade _transition;

    [SerializeField]
    private Counter _counter;

    private string _conductedById;

    public void Start()
    {
        string childId = MapPathPayload.ChildId;
        _conductedById = MapPathPayload.ConductedById;

        _transition.Active();

#if DEBUG
        if (string.IsNullOrEmpty(childId))
        {
            childId = "88345678A";
            Debug.LogWarning("Using default ChildId for testing in Unity Editor.");
        }
        if (string.IsNullOrEmpty(_conductedById))
        {
            _conductedById = "noXJSkWJnCW5iSEu32n5Kvofq5a2";
            Debug.LogWarning("Using default ConductedById for testing in Unity Editor.");
        }
# endif

        LoadPlans(childId).Forget();
    }

    public void OnEnable()
    {
        _eventChannel.OnEventRaised += HandleLevelSelected;
    }

    public void OnDestroy()
    {
        _eventChannel.OnEventRaised -= HandleLevelSelected;
    }

    private async UniTaskVoid LoadPlans(string childId)
    {
        LevelDataStore instance = LevelDataStore.Instance;

        await instance.LoadPlans(childId);

        if (!instance.HasPlans() || !instance.HasActivePlans())
        {
            PerformanceLog.Log("No active plans found for the child.");
        }

        await LoadOralnitas(childId);

        await UniTask.WaitForEndOfFrame(this);

        Sequence sequence = _transition.ZoomIn();

        _itemManager.RenderLevels(
            count: instance.GetPlansCount(),
            last: instance.LastLevel,
            current: instance.CurrentLevel,
            sequence: in sequence
        );
    }

    private void HandleLevelSelected(LevelConfig config, int index)
    {
        TherapyPlan plan = LevelDataStore.Instance.GetPlanByIndex(index);

        TherapyTemplate template = plan.TherapyTemplate;

        LevelPayload.Params = new ActivityRequestParams
        {
            PlanId = plan.TherapyPlanId,
            ChildId = plan.ChildId,
            ConductedById = _conductedById,
        };

        LevelPayload.Skill = (ActivitySkill)template.SkillId;
        LevelPayload.Language = (LanguageID)plan.LanguageId;

        LevelPayload.IsFinalLevel =
            LevelDataStore.Instance.GetLastPlan().TherapyPlanId == plan.TherapyPlanId;

        LevelPayload.Vowel = plan.Vowel;

        ActivityType type = (ActivityType)template.TaskTypeId;

        LevelPayload.TaskCompleted = plan.Status == TherapyStatus.Completed;

        LevelPayload.IsAIEnabled = MapPathPayload.IsAIEnabled;

        string suffix = type switch
        {
            ActivityType.Judge => "J",
            ActivityType.Select => "S",
            ActivityType.Relate => "R",
            _ => "",
        };

        string scene = $"{config.SceneName}_{suffix}";

        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    private async UniTask LoadOralnitas(string childId)
    {
        OralnitasService service = new(0);

        ApiResult<OralnitasData> response = await service.GetOralnitas(childId);

        if (!response.Success)
        {
            PerformanceLog.Log($"Failed to load Oralnitas data: {response.Message}");
            return;
        }

        _counter.SetScore(response.Data.CorrectAnswers);
    }
}
