using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    private TherapyPlan plan;

    private void Start()
    {
        int id = LevelPayload.PlanId;
        plan = LevelDataStore.Instance.GetLevelPlan(id);

        Debug.Log($"Initializing level with");
        Debug.Log($"TherapyTemplateId: {plan.TherapyTemplateId}");
        Debug.Log($"ChildId: {plan.ChildId}");
        Debug.Log($"TaskType: {plan.TherapyTemplate.TaskTypeId}");

        LevelPayload.PlanId = -1;
    }
}
