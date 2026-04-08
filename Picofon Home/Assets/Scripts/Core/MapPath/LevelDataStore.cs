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

    private TherapyPlanService _service;

    public void Awake()
    {
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

        if (!HasPlans())
        {
            PerformanceLog.LogError("No plans loaded, cannot determine current level.");
            return;
        }

        int index = 0;

        foreach (TherapyPlan plan in _cachedPlans)
        {
            if (plan.Status == TherapyStatus.Active)
            {
                _currentLevel = index;
                break;
            }
            index++;
        }
    }

    public async UniTask CreateDefaultPlans(string id, string assignedById)
    {
        await CreatePlans(id, assignedById);
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
        _service = new();
        CancellationToken token = this.GetCancellationTokenOnDestroy();

        ApiResult<TherapyData> result = await _service.GetAllPlans<TherapyData>(childId, token);

        if (!result.Success)
        {
            PerformanceLog.LogError($"Error loading activities: {result.Message}");
            return;
        }

        TherapyPlan[] plans = result.Data.Plans;

        if (plans is null || plans.Length == 0)
        {
            PerformanceLog.LogError("No therapy plans found for the child.");
            return;
        }

        SavePlans(plans);
    }

    private async UniTask CreatePlans(string childId, string assignedById)
    {
        CancellationToken token = this.GetCancellationTokenOnDestroy();

        ApiResult<TherapyData> result = await _service.CreateDefaultPlans<TherapyData>(
            childId,
            assignedById,
            token: token
        );

        if (!result.Success)
        {
            PerformanceLog.LogError($"Error loading activities: {result.Message}");
            return;
        }

        TherapyPlan[] plans = result.Data.Plans;

        if (plans is null || plans.Length == 0)
        {
            PerformanceLog.LogError("No therapy plans found for the child.");
            return;
        }

        SavePlans(plans);
    }
}
