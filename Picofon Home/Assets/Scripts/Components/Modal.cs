using System;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct ModalData
{
    public string Title;
    public string Message;
}

public class Modal : MonoBehaviour, IPointerClickHandler
{
    [Space]
    [SerializeField]
    private GameObject _contentObject;

    [SerializeField]
    private GameObject _debugMenuObject;

    [SerializeField]
    private GameObject _optionsMenuObject;

    [Space]
    [SerializeField]
    private Image _background;

    [SerializeField]
    private RectTransform _panel;

    private const float _duration = 0.5f;

    private GenericEventChannel _eventChannel;
    private ReusableCompletionSource<bool> _taskCompletion;

    private DebugMenu _debugMenu;

    private OptionsMenu _optionsMenu;

    private ContentMenu _contentMenu;

    private AnimationCurve _animationCurve;

    private GameObject _currentMenuObject;
    private RectTransform _currentContent;

    private Action _onAnimationComplete;

    public void Awake()
    {
        _eventChannel = new GenericEventChannel();

        _eventChannel.OnRaised += HandleClose;

        _taskCompletion = new ReusableCompletionSource<bool>();

        Keyframe k0 = new(0f, 0f)
        {
            outTangent = 0.84f / 0.165f,
            outWeight = 0.165f,
            weightedMode = WeightedMode.Out,
        };

        Keyframe k1 = new(1f, 1f)
        {
            inTangent = 0f,
            inWeight = 1f - 0.440f,
            weightedMode = WeightedMode.In,
        };

        _animationCurve = new(k0, k1);

        _onAnimationComplete = () =>
        {
            _currentMenuObject.SetActive(false);
            gameObject.SetActive(false);
        };
    }

    public async UniTask<bool> Show(ModalData data)
    {
        gameObject.SetActive(true);

        _contentObject.SetActive(true);

        _currentMenuObject = _contentObject;
        _currentContent = _contentObject.GetComponent<RectTransform>();

        AnimateOpen();

        if (_contentMenu == null)
        {
            _contentMenu = _contentObject.GetComponent<ContentMenu>();

            _contentMenu.EventChannel = _eventChannel;
            _contentMenu.TaskCompletion = _taskCompletion;
        }

        return await _contentMenu.Show(data);
    }

    public async UniTask<bool> ShowOptions()
    {
        gameObject.SetActive(true);

        _optionsMenuObject.SetActive(true);

        _currentMenuObject = _optionsMenuObject;
        _currentContent = _optionsMenuObject.GetComponent<RectTransform>();

        AnimateOpen();

        if (_optionsMenu == null)
        {
            _optionsMenu = _optionsMenuObject.GetComponent<OptionsMenu>();

            _optionsMenu.EventChannel = _eventChannel;
            _optionsMenu.TaskCompletion = _taskCompletion;
        }

        return await _optionsMenu.Show();
    }

    public async UniTask<DebugMenuResult> ShowDebugMenu()
    {
        gameObject.SetActive(true);

        _debugMenuObject.SetActive(true);

        _currentMenuObject = _debugMenuObject;
        _currentContent = _debugMenuObject.GetComponent<RectTransform>();

        AnimateOpen();

        if (_debugMenu == null)
        {
            _debugMenu = _debugMenuObject.GetComponent<DebugMenu>();

            _debugMenu.EventChannel = _eventChannel;
        }

        return await _debugMenu.Show();
    }

    private void AnimateOpen()
    {
        _ = Sequence
            .Create()
            .Group(
                Tween.Alpha(
                    _background,
                    startValue: 0,
                    endValue: 0.7f,
                    duration: _duration,
                    ease: _animationCurve
                )
            )
            .Group(
                Tween.Scale(
                    _currentContent,
                    startValue: Vector3.one * 0.8f,
                    endValue: Vector3.one,
                    duration: _duration,
                    ease: _animationCurve
                )
            )
            .Group(
                Tween.UIAnchoredPositionY(
                    _currentContent,
                    startValue: -1500f,
                    endValue: 0f,
                    duration: _duration,
                    ease: _animationCurve
                )
            )
            .Group(
                Tween.Scale(
                    _panel,
                    startValue: Vector3.one,
                    endValue: Vector3.one * 0.85f,
                    duration: _duration,
                    ease: _animationCurve
                )
            );
    }

    private void HandleClose()
    {
        _ = Sequence
            .Create()
            .Group(
                Tween.Alpha(
                    _background,
                    startValue: 0.7f,
                    endValue: 0,
                    duration: _duration,
                    ease: _animationCurve
                )
            )
            .Group(
                Tween.Scale(
                    _currentContent,
                    startValue: Vector3.one,
                    endValue: Vector3.one * 0.8f,
                    duration: _duration,
                    ease: _animationCurve
                )
            )
            .Group(
                Tween.UIAnchoredPositionY(
                    _currentContent,
                    startValue: 0f,
                    endValue: -1500f,
                    duration: _duration,
                    ease: _animationCurve
                )
            )
            .Group(
                Tween.Scale(
                    _panel,
                    startValue: Vector3.one * 0.85f,
                    endValue: Vector3.one,
                    duration: _duration,
                    ease: _animationCurve
                )
            )
            .OnComplete(_onAnimationComplete);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var clicked = eventData.pointerPressRaycast.gameObject;

        if (clicked == _background.gameObject)
        {
            HandleClose();
        }
    }
}
