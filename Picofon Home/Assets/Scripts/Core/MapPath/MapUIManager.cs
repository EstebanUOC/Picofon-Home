using Picofon.Components;
using Picofon.Utils;

namespace Picofon.Core.MapPath
{
    using UnityEngine;

    public class MapUIManager : MonoBehaviour
    {
        [SerializeField]
        private TransitionData _transition;

        public void Start()
        {
            SceneOrientationHelper.LockToLandscape();
        }

        public void HandleExitButtonClicked()
        {
            _transition.LoadSourceScene();
        }
    }
}
