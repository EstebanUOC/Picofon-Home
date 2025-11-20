using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Name of your map scene (make sure it's added in Build Settings)
    public string mapSceneName = "Scenes/MapPathScene";

    
    public void LoadMapScene()
    {
        // Load the map scene
        SceneManager.LoadScene(mapSceneName);
    }
}