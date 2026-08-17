using Picofon.Activities.Basket.DTOs.Responses;
using Picofon.Components;

namespace Picofon.Activities.Basket
{
    using System;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class BasketUIManager : MonoBehaviour
    {
        [Space]
        [SerializeField]
        private ItemManager _itemManager;

        [SerializeField]
        private ItemClueManager _itemClueManager;

        [SerializeField]
        private GameMenu _gameMenu;

        [SerializeField]
        private ResponsiveTransform[] _responsiveTransforms;

        [SerializeField]
        private UIResponsiveTransform[] _responsiveUITransforms;

        private AudioClip _introAudio;

        public void Awake()
        {
            SceneOrientationHelper.LockToLandscape();

            _gameMenu.OnMenuOptionSelected += HandleMenuOptionSelected;

            if (SceneOrientationHelper.IsTablet())
                ApplyResponsiveLayout();
        }

        public void Prueba()
        {
            _itemManager.Prueba();
        }

        public void SetViewContent(in ViewContentDTO content)
        {
            _itemManager.SetItemsContent(in content);
        }

        public void SetAudioClips(AudioClip[] clips)
        {
            _itemManager.SetItemsAudio(clips);
        }

        public void Reset()
        {
            _itemClueManager.SetClueVisibility(false);
        }

        public void SetIntroAudio(AudioClip clip)
        {
            _introAudio = clip;
        }

        private void HandleMenuOptionSelected(GameMenuEvent menuEvent)
        {
            switch (menuEvent)
            {
                case GameMenuEvent.Clue:
                    _itemClueManager.ToggleClueVisibility();
                    break;
                case GameMenuEvent.Exit:
                    BackToMap();
                    break;
                case GameMenuEvent.Replay:
                    AudioManager.Instance.PlayVoice(_introAudio);
                    break;
            }
        }

        private void ApplyResponsiveLayout()
        {
            foreach (ResponsiveTransform responsiveRect in _responsiveTransforms)
            {
                Transform target = responsiveRect.Target;

                target.localPosition = responsiveRect.Position;
                target.localRotation = responsiveRect.Rotation;
                target.localScale = responsiveRect.Scale;

                if (!responsiveRect.Mirror)
                    continue;

                Transform mirrorTarget = responsiveRect.TargetMirror;

                mirrorTarget.localPosition = responsiveRect.Position * new Vector2(-1f, 1f);
            }

            foreach (UIResponsiveTransform responsiveRect in _responsiveUITransforms)
            {
                RectTransform target = responsiveRect.Target;

                target.anchorMin = responsiveRect.AnchorMin;
                target.anchorMax = responsiveRect.AnchorMax;
                target.pivot = responsiveRect.Pivot;
                target.anchoredPosition = responsiveRect.Position;
                target.localRotation = responsiveRect.Rotation;
                target.localScale = responsiveRect.Scale;

                if (responsiveRect.Size != Vector2.zero)
                    target.sizeDelta = responsiveRect.Size;

                if (!responsiveRect.Mirror)
                    continue;

                RectTransform mirrorTarget = responsiveRect.TargetMirror;

                mirrorTarget.anchoredPosition = responsiveRect.Position * new Vector2(-1f, 1f);
            }
        }

        private void BackToMap()
        {
            SceneManager.LoadScene("MapPathScene");
            AudioManager.Instance.StopVoice();
        }
    }

    [Serializable]
    public struct ResponsiveTransform
    {
        public bool Mirror;
        public Transform Target;
        public Transform TargetMirror;
        public Vector2 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
    }
}
