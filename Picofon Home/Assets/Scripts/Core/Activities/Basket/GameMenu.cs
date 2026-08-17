using Picofon.Utils;

namespace Picofon.Activities.Basket
{
    using System;
    using UnityEngine;

    public enum GameMenuEvent
    {
        Clue,
        Replay,
        Exit,
    }

    public class GameMenu : MonoBehaviour
    {
        [SerializeField]
        private SimpleButton _toggleButton;

        [SerializeField]
        private GameObject _panel;

        [Space]
        [SerializeField]
        private SimpleEventButton<GameMenuEvent> _clueButton;

        [SerializeField]
        private SimpleEventButton<GameMenuEvent> _replayButton;

        [SerializeField]
        private SimpleEventButton<GameMenuEvent> _exitButton;

        public event Action<GameMenuEvent> OnMenuOptionSelected;

        private bool _isPanelActive = false;

        public void Awake()
        {
            _toggleButton.OnClick += HandleButtonClick;

            GenericEventChannel<GameMenuEvent> eventChannel = new();

            _clueButton.EventChannel = eventChannel;
            _replayButton.EventChannel = eventChannel;
            _exitButton.EventChannel = eventChannel;

            eventChannel.OnRaised += HandleOptionSelected;
        }

        private void HandleOptionSelected(GameMenuEvent menuEvent)
        {
            OnMenuOptionSelected?.Invoke(menuEvent);
        }

        private void HandleButtonClick()
        {
            _isPanelActive = !_isPanelActive;
            _panel.SetActive(_isPanelActive);
        }
    }
}
