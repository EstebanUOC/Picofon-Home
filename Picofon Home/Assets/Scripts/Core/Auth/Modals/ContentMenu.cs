using Picofon.Core.Auth.Components;
using Picofon.Utils;

namespace Picofon.Core.Auth.Modals
{
    using Cysharp.Threading.Tasks;
    using TMPro;
    using UnityEngine;

    public class ContentMenu : MonoBehaviour
    {
        [Space]
        [SerializeField]
        private TMP_Text _title;

        [SerializeField]
        private TMP_Text _message;

        [SerializeField]
        private CustomButton _button;

        public GenericEventChannel EventChannel;

        public ReusableCompletionSource<bool> TaskCompletion;

        private TMP_Text _buttonLabel;
        private string _defaultButtonText;

        public void Awake()
        {
            _button.OnClick += HandleClick;

            _buttonLabel = _button.GetComponentInChildren<TMP_Text>(true);
            _defaultButtonText = _buttonLabel != null ? _buttonLabel.text : null;
        }

        public async UniTask<bool> Show(ModalData data)
        {
            gameObject.SetActive(true);

            TaskCompletion.Reset();

            _title.text = data.Title;
            _message.text = data.Message;

            if (_buttonLabel != null)
            {
                _buttonLabel.text = string.IsNullOrEmpty(data.ConfirmText)
                    ? _defaultButtonText
                    : data.ConfirmText;
            }

            return await TaskCompletion.Task;
        }

        private void HandleClick()
        {
            TaskCompletion.TrySetResult(true);
            EventChannel.Raise();
        }
    }
}
