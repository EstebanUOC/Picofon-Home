using Picofon.Components;
using Picofon.Utils;

namespace Picofon.Core.MapPath
{
    using UnityEngine;

    public class MapUIManager : MonoBehaviour
    {
        [SerializeField]
        private TransitionData _transition;

        [SerializeField]
        private SimpleButton _exitButton;

        public void Start()
        {
            SceneOrientationHelper.LockToLandscape();

            _exitButton.OnClick += HandleExitButtonClicked;
        }

        private void HandleExitButtonClicked()
        {
            _transition.LoadSourceScene();
        }
    }
}
