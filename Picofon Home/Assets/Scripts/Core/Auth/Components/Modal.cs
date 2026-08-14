using Picofon.Core.Auth;
using Picofon.Core.Auth.Modals;
using Picofon.Utils;

namespace Picofon.Core.Auth.Components
{
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

        public RectTransform Panel;
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

        [SerializeField]
        private GameObject _creditsMenuObject;

        [Space]
        [SerializeField]
        private Image _background;

        private const float Duration = 0.5f;

        private GenericEventChannel _eventChannel;
        private ReusableCompletionSource<bool> _taskCompletion;

        private DebugMenu _debugMenu;
        private OptionsMenu _optionsMenu;
        private ContentMenu _contentMenu;

        private AnimationCurve _animationCurve;
        private RectTransform _currentContent;
        private RectTransform _currentPanel;
        private GameObject _currentMenuObject;

        private Action _onAnimationComplete;

        private bool _queu;
        private bool _isClosed = true;

        private ModalData _queuedData;

        public void Awake()
        {
            _contentObject.SetActive(false);
            _debugMenuObject.SetActive(false);
            _optionsMenuObject.SetActive(false);
            _creditsMenuObject.SetActive(false);

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
                _isClosed = true;

                gameObject.SetActive(false);

                if (_queu)
                {
                    _queu = false;
                    _ = Show(_queuedData);
                }
            };
        }

        public async UniTask<bool> Show(ModalData data)
        {
            if (_isClosed == false)
            {
                _queuedData = data;
                _queu = true;

                return false;
            }

            gameObject.SetActive(true);

            _contentObject.SetActive(true);

            _currentMenuObject = _contentObject;
            _currentContent = _contentObject.GetComponent<RectTransform>();
            _currentPanel = data.Panel;

            AnimateOpen();

            if (_contentMenu == null)
            {
                _contentMenu = _contentObject.GetComponent<ContentMenu>();

                _contentMenu.EventChannel = _eventChannel;
                _contentMenu.TaskCompletion = _taskCompletion;
            }

            return await _contentMenu.Show(data);
        }

        public void ShowOptions(RectTransform panel, float version)
        {
            gameObject.SetActive(true);

            _optionsMenuObject.SetActive(true);

            _currentMenuObject = _optionsMenuObject;
            _currentContent = _optionsMenuObject.GetComponent<RectTransform>();
            _currentPanel = panel;

            AnimateOpen();

            if (_optionsMenu == null)
            {
                _optionsMenu = _optionsMenuObject.GetComponent<OptionsMenu>();

                _optionsMenu.EventChannel = _eventChannel;
                _optionsMenu.SetVersionText(version);
            }

            _optionsMenu.Show();
        }

        public void ShowDebugMenu(RectTransform panel)
        {
            gameObject.SetActive(true);

            _debugMenuObject.SetActive(true);

            _currentMenuObject = _debugMenuObject;
            _currentContent = _debugMenuObject.GetComponent<RectTransform>();
            _currentPanel = panel;

            AnimateOpen();

            if (_debugMenu == null)
            {
                _debugMenu = _debugMenuObject.GetComponent<DebugMenu>();

                _debugMenu.EventChannel = _eventChannel;
            }

            _debugMenu.Show();
        }

        public void ShowCredits()
        {
            Tween.Delay(
                Duration,
                () =>
                {
                    gameObject.SetActive(true);

                    _creditsMenuObject.SetActive(true);

                    _currentMenuObject = _creditsMenuObject;
                    _currentContent = _creditsMenuObject.GetComponent<RectTransform>();

                    AnimateOpen();
                }
            );
        }

        private void AnimateOpen()
        {
            _isClosed = false;

            _ = Sequence
                .Create()
                .Group(
                    Tween.Alpha(
                        _background,
                        startValue: 0,
                        endValue: 0.7f,
                        duration: Duration,
                        ease: _animationCurve
                    )
                )
                .Group(
                    Tween.Scale(
                        _currentContent,
                        startValue: Vector3.one * 0.8f,
                        endValue: Vector3.one,
                        duration: Duration,
                        ease: _animationCurve
                    )
                )
                .Group(
                    Tween.UIAnchoredPositionY(
                        _currentContent,
                        startValue: -1500f,
                        endValue: 0f,
                        duration: Duration,
                        ease: _animationCurve
                    )
                )
                .Group(
                    Tween.Scale(
                        _currentPanel,
                        startValue: Vector3.one,
                        endValue: Vector3.one * 0.95f,
                        duration: Duration,
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
                        duration: Duration,
                        ease: _animationCurve
                    )
                )
                .Group(
                    Tween.Scale(
                        _currentContent,
                        startValue: Vector3.one,
                        endValue: Vector3.one * 0.8f,
                        duration: Duration,
                        ease: _animationCurve
                    )
                )
                .Group(
                    Tween.UIAnchoredPositionY(
                        _currentContent,
                        startValue: 0f,
                        endValue: -1700f,
                        duration: Duration - 0.3f,
                        ease: _animationCurve
                    )
                )
                .Group(
                    Tween.Scale(
                        _currentPanel,
                        startValue: Vector3.one * 0.95f,
                        endValue: Vector3.one,
                        duration: Duration,
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
}
