using UnityEngine;

public static class SceneOrientationHelper
{
    public static void LockToPortrait()
    {
        CheckFPS();

        Screen.autorotateToPortrait = true;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.orientation = ScreenOrientation.Portrait;
    }

    public static void UnlockPortrait()
    {
        CheckFPS();

        Screen.autorotateToPortrait = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }

    public static void LockToLandscape()
    {
        CheckFPS();

        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    private static void CheckFPS()
    {
        if (Application.targetFrameRate != 60)
        {
            Application.targetFrameRate = 60;
        }

        Screen.autorotateToPortraitUpsideDown = false;
    }
}
