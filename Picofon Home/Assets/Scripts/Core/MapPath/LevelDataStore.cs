using UnityEngine;

public class LevelDataStore : MonoBehaviour
{
    public static LevelDataStore Instance { get; private set; }

    private TherapyPlan[] _cachedPlans;

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

    public void SavePlans(TherapyPlan[] plans)
    {
        _cachedPlans = plans;
    }

    public TherapyPlan GetLevelPlan(int index)
    {
        if (index >= 0 && index < _cachedPlans.Length)
        {
            return _cachedPlans[index];
        }

        return null;
    }

    public bool HasPlans()
    {
        return _cachedPlans != null && _cachedPlans.Length > 0;
    }

    public TherapyPlan[] GetAllPlans()
    {
        return _cachedPlans;
    }

    public int GetPlansCount()
    {
        return _cachedPlans.Length;
    }
}
