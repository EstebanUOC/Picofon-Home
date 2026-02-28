using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

public class BuildScript
{
    public static void BuildAndroid()
    {
        PlayerSettings.Android.useCustomKeystore = false;

        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Android,
            BuildTarget.Android
        );

        AddressableAssetSettings.BuildPlayerContent();

        BuildPlayerOptions options = new()
        {
            scenes = new[]
            {
                "Assets/Scenes/AuthScene.unity",
                "Assets/Scenes/MapPathScene.unity",
                "Assets/Scenes/Activities/BasketScene_J.unity",
                "Assets/Scenes/Activities/BasketScene_R.unity",
                "Assets/Scenes/Activities/BasketScene_S.unity",
            },

            locationPathName = "BUILD/Picofon.apk",
            target = BuildTarget.Android,
            options = BuildOptions.Development,
        };

        BuildPipeline.BuildPlayer(options);
    }
}
