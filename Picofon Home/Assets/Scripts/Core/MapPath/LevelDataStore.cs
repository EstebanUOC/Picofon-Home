using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LevelDataStore : MonoBehaviour
{
    public static LevelDataStore Instance { get; private set; }

    private TherapyPlan[] _cachedPlans;

    private int _currentLevel = 0;
    private int _lastLevel = -1;

    private bool _someActivePlanFound = false;

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
        _someActivePlanFound = false;

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
                _someActivePlanFound = true;
                break;
            }
            index++;
        }
    }

    public void SavePlans(TherapyPlan[] plans)
    {
        _cachedPlans = plans;
    }

    public bool HasPlans()
    {
        return _cachedPlans != null && _cachedPlans.Length > 0;
    }

    public bool HasNoPlans()
    {
        return _cachedPlans == null || _cachedPlans.Length == 0;
    }

    public bool HasActivePlans()
    {
        return HasPlans() && _someActivePlanFound;
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

    public TherapyPlan GetLastPlan()
    {
        if (HasPlans())
        {
            return _cachedPlans[^1];
        }

        return null;
    }

    private async UniTask GetPlans(string childId)
    {
        _service = new TherapyPlanService(0);

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

            _cachedPlans = Array.Empty<TherapyPlan>();

            return;
        }

        SavePlans(plans);
    }
}
