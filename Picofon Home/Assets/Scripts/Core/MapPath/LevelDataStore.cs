using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LevelDataStore : MonoBehaviour
{
    public static LevelDataStore Instance { get; private set; }

    private TherapyPlan[] _cachedPlans;

    private string _lastId;

    private int _currentLevel = 0;
    private int _lastLevel = -1;

    public int CurrentLevel => _currentLevel;

    public int LastLevel => _lastLevel;

    public void Awake()
    {
        _currentLevel = GamePrefs.LastCompletedLevel;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    public async UniTask LoadPlans(string id)
    {
        if (HasPlans() && _lastId == id)
            return;

        _lastId = id;

        await GetPlans(id);
    }

    public void SavePlans(TherapyPlan[] plans)
    {
        _cachedPlans = plans;
    }

    public bool HasPlans()
    {
        return _cachedPlans != null && _cachedPlans.Length > 0;
    }

    public int GetPlansCount()
    {
        return _cachedPlans.Length;
    }

    public void LevelCompleted()
    {
        _lastLevel = _currentLevel;
        _currentLevel++;
    }

    public TherapyPlan GetPlanByIndex(int index)
    {
        if (index >= 0 && index < _cachedPlans.Length)
        {
            return _cachedPlans[index];
        }

        return null;
    }

    private async UniTask GetPlans(string childId)
    {
        TherapyPlanService service = new();
        CancellationToken token = this.GetCancellationTokenOnDestroy();

        ApiResult<TherapyData> result = await service.GetAllPlans<TherapyData>(childId, token);

        if (!result.Success)
        {
            Debug.LogError($"Error loading activities: {result.Message}");
            return;
        }

        TherapyPlan[] plans = result.Data.Plans;

        if (plans is null || plans.Length == 0)
        {
            Debug.LogError("No therapy plans found for the child.");
            return;
        }

        SavePlans(plans);
    }
}
