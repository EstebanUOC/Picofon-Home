using System;
using BasketResponses;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ActivitiesResult = ApiResult<BasketResponses.ActivitiesData<BasketResponses.RelateActivity>>;

public class BasketManagerRE : MonoBehaviour
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

    [SerializeField]
    private AudioClip _instructionClip;

    [SerializeField]
    private AudioClip _positiveClip;

    [SerializeField]
    private AudioClip _negativeClip;

    private int _currentActivityIndex = 0;

    private RelateActivity[] _activities;
    private RelateActivity _currentActivity;

    private readonly Sprite[] _icons = new Sprite[5];
    private readonly string[] _texts = new string[5];
    private readonly string[] _syllabifiedWords = new string[5];

    public void Awake()
    {
        Application.targetFrameRate = 60;

        _answerManager.OnHoopSelected += HandleHoopSelected;

        _feedbackController.Init();

        LoadActivities().Forget();
    }

    private async UniTaskVoid LoadActivities()
    {
        BasketService basketService = new();

        ActivityRequestParams @params = new() { PlanId = "43", ChildId = "19013454K" };

        ActivitiesResult result = await basketService.GetActivities<ActivitiesData<RelateActivity>>(
            @params
        );

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

        AudioManager.Instance.PlayVoice(_instructionClip);
        await AudioManager.Instance.WaitVoiceToEnd();
    }

    private void HandleHoopSelected(int hoopIndex)
    {
        Transform hoopTransform = _hoopManager.GetHoopTransform(hoopIndex);

        _ballController.LaunchBall(hoopTransform);

        bool isCorrect = _currentActivity.Words[hoopIndex].Answer;

        AudioClip clip = isCorrect ? _positiveClip : _negativeClip;

        AudioManager.Instance.PlayVoice(clip);

        InitCount(isCorrect).Forget();
    }

    private async UniTaskVoid InitCount(bool isCorrect)
    {
        FeedbackType feedbackType = isCorrect ? FeedbackType.Positive : FeedbackType.Neutral;

        await UniTask.WaitForSeconds(2f);

        await _feedbackController.ShowFeedback(feedbackType);

        ChangeActivity();
    }

    private void ChangeActivity()
    {
        _currentActivity = _activities[_currentActivityIndex];
        _currentActivityIndex = (_currentActivityIndex + 1) % _activities.Length;

        _texts[0] = _currentActivity.MainWord.Word;
        _icons[0] = LoadSprite(_currentActivity.MainWord.Path);
        _syllabifiedWords[0] = _currentActivity.MainWord.SyllabifiedWord;

        for (int i = 0; i < _texts.Length - 1; i++)
        {
            int index = i + 1;
            _texts[index] = _currentActivity.Words[i].Word;
            _icons[index] = LoadSprite(_currentActivity.Words[i].Path);
            _syllabifiedWords[index] = _currentActivity.Words[i].SyllabifiedWord;
        }

        Span<bool> answers = stackalloc bool[4];
        for (int i = 0; i < answers.Length; i++)
        {
            answers[i] = _currentActivity.Words[i].Answer;
        }

        AnswerDTO answer = new(answers);
        _hoopManager.SetHoopStates(in answer);

        ViewContentDTO content = new(_icons, _texts);

        _uiManager.SetViewContent(in content);

        ViewContentDTO feedbackContent = new(_icons, _syllabifiedWords);

        _feedbackController.SetItemsContent(in feedbackContent);

        ResetActivity();
    }

    private void ResetActivity()
    {
        _uiManager.Reset();
        _ballController.Reset();
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
