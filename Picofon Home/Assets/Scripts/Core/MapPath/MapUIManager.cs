using UnityEngine;

public class MapUIManager : MonoBehaviour
{
    [SerializeField]
    private TransitionScene _transition;

    [SerializeField]
    private LevelButton _exitButton;

    public void Start()
    {
        _exitButton.OnClick += HandleExitButtonClicked;
    }

    private void HandleExitButtonClicked()
    {
        _transition.Back();
    }
}
