using UnityEngine;

public class MapUIManager : MonoBehaviour
{
    [SerializeField]
    private TransitionData _transition;

    [SerializeField]
    private SimpleButton _exitButton;

    public void Start()
    {
        _exitButton.OnClick += HandleExitButtonClicked;
    }

    private void HandleExitButtonClicked()
    {
        _transition.LoadSourceScene();
    }
}
