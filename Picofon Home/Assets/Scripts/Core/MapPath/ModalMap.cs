using Cysharp.Threading.Tasks;
using Picofon.Utils;
using TMPro;
using UnityEngine;

namespace Picofon.Core
{
    public class ModalMap : MonoBehaviour
    {
        [SerializeField]
        private SimpleButton _button;

        [SerializeField]
        private TMP_Text _text;

        private ReusableCompletionSource<bool> _taskCompletion;

        public void Awake()
        {
            _taskCompletion = new ReusableCompletionSource<bool>();

            _button.OnClick += OnButtonClicked;
        }

        public async UniTask<bool> Show(string text)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            _text.text = text;

            gameObject.SetActive(true);

            bool result = await _taskCompletion.Task;

            gameObject.SetActive(false);

            _taskCompletion.Reset();

            return result;
        }

        private void OnButtonClicked()
        {
            _taskCompletion.TrySetResult(true);
        }
    }
}
