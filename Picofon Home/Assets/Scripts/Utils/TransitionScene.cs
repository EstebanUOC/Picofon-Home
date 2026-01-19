using UnityEngine;

[CreateAssetMenu(fileName = "TransitionScene", menuName = "Utils/TransitionScene")]
public class TransitionScene : ScriptableObject
{
    public string Scene1;
    public string Scene2;

    public void GoTo()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(Scene2);
    }

    public void Back()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(Scene1);
    }
}
