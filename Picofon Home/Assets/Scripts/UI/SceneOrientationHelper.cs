using UnityEngine;

public static class SceneOrientationHelper
{
    private static ScreenOrientation _lastOrientation;
    private static bool _checkedFPS = false;

    public static void LockToPortrait()
    {
        Screen.autorotateToPortrait = true;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.orientation = ScreenOrientation.Portrait;

        CheckFPS();
    }

    public static void LockToLandscape()
    {
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.orientation = ScreenOrientation.AutoRotation;

        CheckFPS();
    }

    public static void UnlockPortrait()
    {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.AutoRotation;

        CheckFPS();
    }

    public static void UnlockLandscape()
    {
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = true;
        Screen.orientation = ScreenOrientation.AutoRotation;

        CheckFPS();
    }

    public static bool ChangeOrientation()
    {
        return _lastOrientation != Screen.orientation;
    }

    private static void CheckFPS()
    {
        if (!_checkedFPS)
        {
            _checkedFPS = true;
            Application.targetFrameRate = 60;

            Screen.autorotateToPortraitUpsideDown = false;
        }

        _lastOrientation = Screen.orientation;
    }
}
