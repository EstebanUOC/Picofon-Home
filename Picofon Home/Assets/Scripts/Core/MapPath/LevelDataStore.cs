using System.Collections.Generic;
using UnityEngine;

public class LevelDataStore : MonoBehaviour
{
    public static LevelDataStore Instance { get; private set; }

    private readonly Dictionary<int, TherapyPlan> _cachedPlans = new();

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

    public void SavePlans(TherapyPlan[] plan)
    {
        foreach (var p in plan)
        {
            _cachedPlans.Add(p.TherapyPlanId, p);
        }
    }

    public void RegisterPlan(TherapyPlan plan)
    {
        _cachedPlans.Add(plan.TherapyPlanId, plan);
    }

    public TherapyPlan GetLevelPlan(int planId)
    {
        if (_cachedPlans.TryGetValue(planId, out var plan))
            return plan;

        return null;
    }

    public static bool HasPlans()
    {
        return Instance != null && Instance._cachedPlans.Count > 0;
    }

    public TherapyPlan[] GetAllPlans()
    {
        TherapyPlan[] plans = new TherapyPlan[_cachedPlans.Count];
        _cachedPlans.Values.CopyTo(plans, 0);
        return plans;
    }
}
