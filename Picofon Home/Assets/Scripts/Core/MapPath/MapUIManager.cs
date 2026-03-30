using UnityEngine;

public class MapUIManager : MonoBehaviour
{
    [SerializeField]
    private TransitionData _transition;

    [SerializeField]
    private SimpleButton _exitButton;

    [SerializeField]
    private Fade _fade;

    public void Start()
    {
        SceneOrientationHelper.LockToLandscape();

        _exitButton.OnClick += HandleExitButtonClicked;

        _fade.ZoomIn();
    }

    private void HandleExitButtonClicked()
    {
        _transition.LoadSourceScene();
    }
}
