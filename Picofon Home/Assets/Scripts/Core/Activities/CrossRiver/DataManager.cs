using BasketResponses;
using Cysharp.Threading.Tasks;
using ActivitiesResult = ApiResult<ActivitiesData<BasketResponses.JudgeActivity>>;

public class DataManager
{
    #region Fields

    private JudgeActivity[] _activities;
    private int _currentIndex;

    #endregion

    public JudgeActivity GetCurrentActivity()
    {
        return _activities?[_currentIndex];
    }

    public JudgeActivity[] GetActivities()
    {
        return _activities;
    }

    public int GetActivityCount()
    {
        return _activities?.Length ?? 0;
    }

    public int GetCurrentIndex()
    {
        return _currentIndex;
    }

    public bool HasActivities()
    {
        return _activities is { Length: > 0 };
    }

    public bool MoveNext()
    {
        if (_activities == null || _currentIndex >= _activities.Length - 1)
            return false;

        _currentIndex++;
        return true;
    }

    public async UniTask<ActivitiesResult> LoadActivities(ActivityRequestParams @params)
    {
        BasketService service = new();
        ActivitiesResult result = await service.GetActivities<ActivitiesData<JudgeActivity>>(
            @params
        );

        if (result.Success && result.Data is { Activities: { Length: > 0 } acts })
        {
            _activities = acts;
            _currentIndex = 0;
        }

        return result;
    }

    public void Reset()
    {
        _activities = null;
        _currentIndex = 0;
    }
}
