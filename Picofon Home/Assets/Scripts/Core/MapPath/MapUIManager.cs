using UnityEngine;

public class MapUIManager : MonoBehaviour
{
    [SerializeField]
    private TransitionData _transition;

    [SerializeField]
    private SimpleButton _exitButton;

    [SerializeField]
    private UIResponsiveTransform _responsiveTransform;

    public void Start()
    {
        SceneOrientationHelper.LockToLandscape();

        _exitButton.OnClick += HandleExitButtonClicked;

        if (SceneOrientationHelper.IsTablet())
        {
            RectTransform target = _responsiveTransform.Target;
            target.localScale = _responsiveTransform.Scale;
        }
    }

    private void HandleExitButtonClicked()
    {
        _transition.LoadSourceScene();
    }
}
