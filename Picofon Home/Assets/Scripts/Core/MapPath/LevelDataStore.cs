using UnityEngine;

public class LevelDataStore : MonoBehaviour
{
    public static LevelDataStore Instance { get; private set; }

    private TherapyPlan[] _cachedPlans;

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

    public void SavePlans(TherapyPlan[] plans)
    {
        _cachedPlans = plans;
    }

    public TherapyPlan GetPlanByIndex(int index)
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

    public void LevelCompleted()
    {
        _lastLevel = _currentLevel;
        _currentLevel++;
    }
}
