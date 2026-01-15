using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class TherapyData
{
    public TherapyPlan[] Plans;
}

public class MapManager : MonoBehaviour
{
    private LevelItemRenderer _renderer;
    private string _childId = string.Empty;

    public void Start()
    {
        _renderer = GetComponent<LevelItemRenderer>();

        bool existsData = LevelDataStore.HasPlans();
        if (existsData)
        {
            TherapyPlan[] plans = LevelDataStore.Instance.GetAllPlans();
            _renderer.RenderLevels(plans);
            return;
        }

        _childId = MapPathPayload.ChildId;

        if (string.IsNullOrEmpty(_childId))
        {
#if !UNITY_EDITOR
            Debug.LogError("ChildId is null or empty in MapPathPayload.");
            return;
# else
            _childId = "19013454K";
            Debug.LogWarning("Using default ChildId for testing in Unity Editor.");
# endif
        }

        LoadPlans().Forget();
    }

    private async UniTaskVoid LoadPlans()
    {
        TherapyPlanService service = new();
        LevelDataStore store = LevelDataStore.Instance;

        ApiResult<TherapyData> result = await service.GetAllPlans<TherapyData>(_childId);

        if (!result.Success)
        {
            Debug.LogError($"Error loading activities: {result.Message}");
            return;
        }

        TherapyPlan[] plans = result.Data.Plans;

        if (plans is null || plans.Length == 0)
        {
            Debug.LogWarning("No hay actividades cargadas.");
            return;
        }

        store.SavePlans(plans);

        _renderer.RenderLevels(plans);
    }
}
