using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class MapUIManager : MonoBehaviour
{
    [SerializeField]
    private TransitionData _transition;

    [SerializeField]
    private SimpleButton _exitButton;

    [SerializeField]
    private Image _fade;

    public void Start()
    {
        SceneOrientationHelper.LockToLandscape();

        _exitButton.OnClick += HandleExitButtonClicked;

        _fade.color = new Color(0, 0, 0, 1);

        Tween.Alpha(_fade, endValue: 0, duration: 1f, ease: Ease.InOutQuad);
    }

    private void HandleExitButtonClicked()
    {
        _transition.LoadSourceScene();
    }
}
