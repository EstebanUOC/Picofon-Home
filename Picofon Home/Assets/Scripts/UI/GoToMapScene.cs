using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToMapScene : MonoBehaviour
{
    public void GoToMap()
    {
        SceneManager.LoadScene("MapPath");
    }
}
