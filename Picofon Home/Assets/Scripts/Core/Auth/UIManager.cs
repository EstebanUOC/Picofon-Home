using Picofon.Components;
using Picofon.Core.Auth.Components;
using Picofon.Core.Auth.Modals;
using Picofon.Core.Auth.Panels;

namespace Picofon.Core.Auth
{
    using System;
    using Cysharp.Threading.Tasks;
    using PrimeTween;
    using UnityEngine;
    using UnityEngine.UI;

    public enum PanelEnum : byte
    {
        Login,
        Disclaimer,
        Role,
        Children,
        RegisterChild,
        RegisterParent,
    }

    public enum ModalEnum : byte
    {
        Options,
        DebugMenu,
    }

    public enum LoadingEnum : byte
    {
        Boot,
        Map,
        Normal,
    }

    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private Image _fadeOverlay;

        [SerializeField]
        private RectTransform[] _panels;

        [SerializeField]
        private UIResponsiveTransform[] _responsiveTransforms;

        [Space]
        [SerializeField]
        private LoadingPanel _loadingPanel;

        [SerializeField]
        private Modal _modalPanel;

        [Space]
        public float VersionNumber = 0.2f;

        private RectTransform _currentPanel;
        private RectTransform _nextPanel;

        private Action _onAnimationComplete;

        public void Awake()
        {
            if (SceneOrientationHelper.IsTablet())
            {
                foreach (UIResponsiveTransform responsiveRect in _responsiveTransforms)
                {
                    responsiveRect.Target.localScale = responsiveRect.Scale;
                }
            }

            SceneOrientationHelper.LockToPortrait();

            foreach (RectTransform panel in _panels)
            {
                panel.gameObject.SetActive(false);
            }

            _onAnimationComplete = () =>
            {
                const float duration = 0.2f;

                Tween.Alpha(
                    _fadeOverlay,
                    endValue: 0f,
                    duration,
                    startDelay: 0.13f,
                    ease: Ease.InOutCubic
                );

                _currentPanel.localScale = Vector3.one;

                _currentPanel.gameObject.SetActive(false);

                _currentPanel = _nextPanel;
                _currentPanel.gameObject.SetActive(true);
            };
        }

        public void ShowPanel(PanelEnum panel, bool animate = true)
        {
            int index = (int)panel;

            if (!animate)
            {
                _currentPanel = _panels[index];
                _currentPanel.gameObject.SetActive(true);
                return;
            }

            _nextPanel = _panels[index];

            ChangePanel();
        }

        public void ShowLoading(LoadingEnum loading)
        {
            _loadingPanel.Show(loading);
        }

        public void HideLoading(LoadingEnum loading)
        {
            _loadingPanel.Hide(loading);
        }

        public void PlayMapTransition()
        {
            _loadingPanel.ShowMapTransition();
        }

        public void ContinueMapTransition(bool success)
        {
            _loadingPanel.ContinueMapTransition(success);
        }

        public async UniTask ShowModal(ModalData data)
        {
            await _modalPanel.Show(data);
        }

        public void ShowModal(RectTransform panel, ModalEnum modal)
        {
            switch (modal)
            {
                case ModalEnum.Options:
                    _modalPanel.ShowOptions(panel, VersionNumber);
                    break;
                case ModalEnum.DebugMenu:
                    _modalPanel.ShowDebugMenu(panel);
                    break;
            }
        }

        private void ChangePanel()
        {
            _fadeOverlay.gameObject.SetActive(true);

            const float duration = 0.2f;

            Sequence
                .Create()
                .Group(
                    Tween.Alpha(
                        _fadeOverlay,
                        endValue: 1f,
                        duration: duration,
                        ease: Ease.InOutCubic
                    )
                )
                .Group(
                    Tween.Scale(
                        _currentPanel,
                        startValue: Vector3.one,
                        endValue: Vector3.one * 0.95f,
                        duration: duration,
                        ease: Ease.InOutCubic
                    )
                )
                .OnComplete(_onAnimationComplete);
        }
    }
}
