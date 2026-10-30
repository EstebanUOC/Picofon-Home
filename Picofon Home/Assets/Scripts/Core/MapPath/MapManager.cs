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
    using UnityEngine.SceneManagement;

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

        [SerializeField]
        private ModalMap _modal;

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
                _conductedById = "noXJSkWJnCW5iSEu32n5Kvofq5a2";

                MapPathPayload.NextAction = NextAction.Reload;

                PerformanceLog.LogWarning("Using default parameters for testing in Editor");
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
            switch (MapPathPayload.NextAction)
            {
                case NextAction.Load:
                    await LoadOralnitas(childId);

                    await UniTask.WaitForEndOfFrame(this);

                    break;

                case NextAction.Reload:
                    await LevelDataStore.Instance.LoadPlans(childId);

                    break;

                case NextAction.CalculateLR:
                    await CalculateLearningRate(childId);

                    await LevelDataStore.Instance.LoadPlans(childId);

                    break;

                default:
                    break;
            }

            Sequence sequence = _transition.ZoomIn();

            _itemManager.RenderLevels(store: LevelDataStore.Instance, sequence: in sequence);
        }

        private async UniTask CalculateLearningRate(string childId)
        {
            LearningRateService service = new(0);

            int planId = LevelDataStore.Instance.GetLastPlan().TherapyPlanId;

            ApiResult<LearningRateData> result = await service.GetLearningRate(
                childId: childId,
                therapyPlan: planId
            );

            if (!result.Success)
            {
                // TODO: Show modal with error message, and load the AuthScene after user clicks OK

                await _modal.Show("No s'han pogut carregar els nivells següents.");

                await LoadScene("AuthScene");

                return;
            }

            TherapyPlanBulkData data = new()
            {
                ChildId = childId,
                AssignedById = MapPathPayload.ConductedById,
                Vowel = LevelDataStore.Instance.GetLastPlan().Vowel,
                Levels = result.Data.Levels,
            };

            ApiResult resultCreate = await service.CreateTherapyPlanBulk(data);

            if (!resultCreate.Success)
            {
                await _modal.Show("No s'han pogut carregar els nivells següents.");

                await LoadScene("AuthScene");

                return;
            }

            await _modal.Show("Niveles cargados correctamente, ¡a jugar!");
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

            SceneManager.LoadScene(scene);
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

        private async UniTask LoadScene(string sceneName)
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
            loadOperation.allowSceneActivation = false;

            await UniTask.WaitUntil(() => loadOperation.progress >= 0.9f);
        }
    }
}
