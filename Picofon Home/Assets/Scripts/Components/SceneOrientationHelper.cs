namespace Picofon.Components
{
    using UnityEngine;

    public static class SceneOrientationHelper
    {
        private static ScreenOrientation _lastOrientation;
        private static bool _checkedFPS = false;

        private static DeviceType _deviceType = DeviceType.Any;

        public static bool IsTablet()
        {
            if (_deviceType != DeviceType.Any)
                return _deviceType == DeviceType.Tablet;

            float dpi = Screen.dpi;

            bool isTablet;

            if (dpi == 0)
            {
                int minSide = Mathf.Min(Screen.width, Screen.height);
                float aspect = (float)Mathf.Max(Screen.width, Screen.height) / minSide;

                isTablet = minSide >= 700 && aspect < 2.1f;

                _deviceType = isTablet ? DeviceType.Tablet : DeviceType.Mobile;
                return isTablet;
            }

            float widthInches = Screen.width / dpi;
            float heightInches = Screen.height / dpi;
            float diagonal = Mathf.Sqrt(widthInches * widthInches + heightInches * heightInches);

            isTablet = diagonal >= 6.5f;

            _deviceType = isTablet ? DeviceType.Tablet : DeviceType.Mobile;

            return isTablet;
        }

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
}
