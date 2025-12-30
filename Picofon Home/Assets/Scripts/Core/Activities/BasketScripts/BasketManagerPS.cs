using BasketResponses;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ActivitiesResult = BasketResponses.ApiResult<BasketResponses.ActivitiesData<BasketResponses.SelectActivity>>;

public class BasketManagerPS : MonoBehaviour
{
    private SelectActivity[] _activities;
    private SelectActivity _currentActivity;

    public void Awake()
    {
        Application.targetFrameRate = 60;

        LoadActivities().Forget();
    }

    private async UniTaskVoid LoadActivities()
    {
        BasketService basketService = new();

        ActivityRequestParams @params = new() { PlanId = "36", ChildId = "98765432M" };

        ActivitiesResult result = await basketService.GetActivities<ActivitiesData<SelectActivity>>(
            @params
        );

        // NOTE: Wait a frame to ensure all initializations are done, Do not delete, 100% necessary
        await UniTask.Yield();

        if (!result.Success)
        {
            Debug.LogError($"Error loading activities: {result.Message}");
            return;
        }

        _activities = result.Data.Activities;

        if (_activities is null || _activities.Length == 0)
        {
            Debug.LogWarning("No hay actividades cargadas.");
            return;
        }

        ChangeActivity();
    }

    private void ChangeActivity()
    {
        _currentActivity = _activities[0];

        foreach (var word in _currentActivity.Words)
        {
            Debug.Log(
                $"Word: {word.Word}, Path: {word.Path}, Syllabified: {word.SyllabifiedWord}, Sound: {word.Sound}"
            );
        }
    }
}
