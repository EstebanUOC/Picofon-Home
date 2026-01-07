using System;
using BasketResponses;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ActivitiesResult = BasketResponses.ApiResult<BasketResponses.ActivitiesData<BasketResponses.SelectActivity>>;

public class BasketManagerPS : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private BallController _ballController;

    [SerializeField]
    private FeedbackController _feedbackController;

    [Space(15)]
    [SerializeField]
    private AnswerManagerPS _answerManager;

    [SerializeField]
    private HoopManager _hoopManager;

    [SerializeField]
    private BasketUIManager _uiManager;

    private int _currentActivityIndex = 0;

    private SelectActivity[] _activities;
    private SelectActivity _currentActivity;

    private readonly Sprite[] _icons = new Sprite[4];
    private readonly string[] _texts = new string[4];
    private readonly string[] _syllabifiedWords = new string[4];

    public void Awake()
    {
        Application.targetFrameRate = 60;

        LoadActivities().Forget();

        _answerManager.OnHoopSelected += HandleHoopSelected;
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

    private void HandleHoopSelected(int hoopIndex)
    {
        Transform hoopTransform = _hoopManager.GetHoopTransform(hoopIndex);

        _ballController.LaunchBall(hoopTransform);
    }

    private void ChangeActivity()
    {
        _currentActivity = _activities[_currentActivityIndex];
        _currentActivityIndex = (_currentActivityIndex + 1) % _activities.Length;

        for (int i = 0; i < _texts.Length; i++)
        {
            _texts[i] = _currentActivity.Words[i].Word;
            _icons[i] = LoadSprite(_currentActivity.Words[i].Path);
            _syllabifiedWords[i] = _currentActivity.Words[i].SyllabifiedWord;
        }

        Span<bool> answers = stackalloc bool[4];
        for (int i = 0; i < answers.Length; i++)
        {
            if (i == 0)
            {
                answers[i] = false;
                continue;
            }

            answers[i] = true;
        }

        AnswerDTO answer = new(answers);
        _hoopManager.SetHoopStates(in answer);

        ViewContentDTO content = new(_icons, _texts);

        _uiManager.SetViewContent(in content);

        ViewContentDTO feedbackContent = new(_icons, _syllabifiedWords);

        _feedbackController.SetItemsContent(in feedbackContent);
    }

    private Sprite LoadSprite(string p)
    {
        string file = System.IO.Path.GetFileNameWithoutExtension(p);
        Sprite s = Resources.Load<Sprite>($"Images/ImgButtons/{file}");

        if (!s)
            Debug.LogWarning($"No se encontró sprite: {file}");

        return s;
    }
}
