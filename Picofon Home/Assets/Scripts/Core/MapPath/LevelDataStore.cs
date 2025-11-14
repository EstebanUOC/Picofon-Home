using System.Collections.Generic;
using UnityEngine;

public class LevelDataStore : MonoBehaviour
{
    public static LevelDataStore Instance { get; private set; }

    private readonly Dictionary<int, TherapyPlan> cachedPlans = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    public void RegisterPlan(TherapyPlan plan)
    {
        cachedPlans.Add(plan.Id, plan);
    }

    public TherapyPlan GetLevelPlan(int planId)
    {
        if (cachedPlans.TryGetValue(planId, out var plan))
            return plan;

        return null;
    }

    public static bool ExistsPlans()
    {
        return Instance != null && Instance.cachedPlans.Count > 0;
    }

    public List<TherapyPlan> GetAllPlans()
    {
        return new List<TherapyPlan>(cachedPlans.Values);
    }
}
