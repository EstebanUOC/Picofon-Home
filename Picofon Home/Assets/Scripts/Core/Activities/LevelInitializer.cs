using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    private TherapyPlan plan;

    private void Start()
    {
        int id = LevelPayload.PlanIndex;
        plan = LevelDataStore.Instance.GetPlanByIndex(id);

        Debug.Log($"Initializing level with");
        Debug.Log($"ChildId: {plan.ChildId}");

        LevelPayload.PlanIndex = -1;
    }
}
