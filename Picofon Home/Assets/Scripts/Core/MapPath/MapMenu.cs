using Picofon.Components;
using Picofon.Core.MapPath;
using Picofon.Utils;
using UnityEngine;

namespace Picofon.Core
{
    public class MapMenu : MonoBehaviour
    {
        [SerializeField]
        private SimpleButton _toggleButton;

        [SerializeField]
        private GameObject _panel;

        [SerializeField]
        private MapUIManager _uiManager;

        [SerializeField]
        private Counter _counter;

        [SerializeField]
        private VideoManager _videoManager;

        [Space]
        [SerializeField]
        private SimpleButton _oralnitasButton;

        [SerializeField]
        private SimpleButton _playButton;

        [SerializeField]
        private SimpleButton _exitButton;

        private bool _isPanelActive = false;

        public void Start()
        {
            _toggleButton.OnClick += HandleButtonClick;

            _exitButton.OnClick += () =>
            {
                _uiManager.HandleExitButtonClicked();
            };

            _oralnitasButton.OnClick += () =>
            {
                _counter.Show().Forget();
            };

            _playButton.OnClick += () =>
            {
                _videoManager.PlayVideo();
            };
        }

        private void HandleButtonClick()
        {
            _isPanelActive = !_isPanelActive;
            _panel.SetActive(_isPanelActive);
        }
    }
}
