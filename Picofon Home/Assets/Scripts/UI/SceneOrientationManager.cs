using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOrientationManager : MonoBehaviour
{
    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "LoginScene" || sceneName == "MapPath")
        {
            // Enable autorotation
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;
            Screen.orientation = ScreenOrientation.AutoRotation;
        }
        else
        {
            // Lock to landscape left
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }
}
