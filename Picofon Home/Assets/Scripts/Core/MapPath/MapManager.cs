using Picofon.Components;
using Picofon.Core.Auth;
using Picofon.Core.MapPath.Events;
using Picofon.Core.MapPath.Models;
using Picofon.Core.MapPath.Services;
using Picofon.Core.Network;
using Picofon.Utils;

namespace Picofon.Core.MapPath
{
    using Cysharp.Threading.Tasks;
    using PrimeTween;
    using UnityEngine;

    public enum ActivityType : byte
    {
        Judge = 1,
        Select = 2,
        Relate = 3,
    }

    public class MapManager : MonoBehaviour
    {
        #region References

        [SerializeField]
        private LevelSelectEventChannel _eventChannel;

        [Space]
        [SerializeField]
        private LevelItemManager _itemManager;

        [Space]
        [SerializeField]
        private Fade _transition;

        [SerializeField]
        private Counter _counter;

        #endregion

        // Variables

        private string _conductedById;

        public void Start()
        {
            GamePrefs.PreferredLanguageID = MapPathPayload.LanguageId;

            string childId = MapPathPayload.ChildId;
            _conductedById = MapPathPayload.ConductedById;

            _transition.Active();

#if DEBUG

            if (string.IsNullOrEmpty(childId))
            {
                childId = "77345678B";
                PerformanceLog.LogWarning("Using default ChildId for testing in Unity Editor.");
            }

            if (string.IsNullOrEmpty(_conductedById))
            {
                _conductedById = "noXJSkWJnCW5iSEu32n5Kvofq5a2";
                PerformanceLog.LogWarning(
                    "Using default ConductedById for testing in Unity Editor."
                );
            }

# endif

            LoadPlans(childId).Forget();
        }

        public void OnEnable()
        {
            _eventChannel.OnEventRaised += HandleLevelSelected;
        }

        public void OnDestroy()
        {
            _eventChannel.OnEventRaised -= HandleLevelSelected;
        }

        private async UniTaskVoid LoadPlans(string childId)
        {
            LevelDataStore instance = LevelDataStore.Instance;

            await instance.LoadPlans(childId);

            if (!instance.HasPlans() || !instance.HasActivePlans())
            {
                PerformanceLog.Log("No active plans found for the child.");
            }

            await LoadOralnitas(childId);

            await UniTask.WaitForEndOfFrame(this);

            Sequence sequence = _transition.ZoomIn();

            _itemManager.RenderLevels(store: instance, sequence: in sequence);
        }

        private void HandleLevelSelected(LevelConfig config, int index)
        {
            TherapyPlan plan = LevelDataStore.Instance.GetPlanByIndex(index);

            TherapyTemplate template = plan.TherapyTemplate;

            LevelPayload.Params = new ActivityRequestParams
            {
                PlanId = plan.TherapyPlanId,
                ChildId = plan.ChildId,
                ConductedById = _conductedById,
            };

            LevelPayload.Skill = (ActivitySkill)template.SkillId;
            LevelPayload.Language = (LanguageID)plan.LanguageId;

            LevelPayload.IsFinalLevel =
                LevelDataStore.Instance.GetLastPlan().TherapyPlanId == plan.TherapyPlanId;

            LevelPayload.Vowel = plan.Vowel;

            ActivityType type = (ActivityType)template.TaskTypeId;

            LevelPayload.TaskCompleted = plan.Status == TherapyStatus.Completed;

            LevelPayload.IsAIEnabled = MapPathPayload.IsAIEnabled;

            string suffix = type switch
            {
                ActivityType.Judge => "J",
                ActivityType.Select => "S",
                ActivityType.Relate => "R",
                _ => "",
            };

            string scene = $"{config.SceneName}_{suffix}";

            UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
        }

        private async UniTask LoadOralnitas(string childId)
        {
            OralnitasService service = new(0);

            ApiResult<OralnitasData> response = await service.GetOralnitas(childId);

            if (!response.Success)
            {
                PerformanceLog.Log($"Failed to load Oralnitas data: {response.Message}");
                return;
            }

            _counter.SetScore(response.Data.CorrectAnswers);
        }
    }
}
