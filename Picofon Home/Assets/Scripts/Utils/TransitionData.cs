namespace Picofon.Utils
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "TransitionData", menuName = "Utils/TransitionData")]
    public sealed class TransitionData : ScriptableObject
    {
        [SerializeField]
        private string _sourceScene;

        [SerializeField]
        private string _targetScene;

        public string SourceScene => _sourceScene;
        public string TargetScene => _targetScene;

        public void LoadTargetScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(_targetScene);
        }

        public void LoadSourceScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(_sourceScene);
        }
    }
}
