using System;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

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
    /// <summary>
    /// Genera el proyecto Xcode en Builds/iOS (fuera de Assets, ignorado por git).
    /// Usa las escenas activas de Build Settings. Con el argumento -iosSimulator
    /// se genera para el SDK de simulador; sin el, para dispositivo (App Store).
    /// Uso: Unity -batchmode -quit -projectPath "Picofon Home" -executeMethod BuildScript.BuildIOS [-iosSimulator]
    /// </summary>
    public static void BuildIOS()
    {
        bool simulator = Environment.GetCommandLineArgs().Contains("-iosSimulator");

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);

        // El SDK y la arquitectura de simulador se fijan solo durante esta build y se
        // restauran al final, para que ProjectSettings quede siempre en DeviceSDK.
        iOSSdkVersion previousSdk = PlayerSettings.iOS.sdkVersion;
        AppleMobileArchitectureSimulator previousSimArch = PlayerSettings.iOS.simulatorSdkArchitecture;
        PlayerSettings.iOS.sdkVersion = simulator ? iOSSdkVersion.SimulatorSDK : iOSSdkVersion.DeviceSDK;
        if (simulator)
        {
            // Macs con Apple Silicon: el simulador necesita arm64.
            PlayerSettings.iOS.simulatorSdkArchitecture = AppleMobileArchitectureSimulator.ARM64;
        }

        AddressableAssetSettings.BuildPlayerContent();

        BuildPlayerOptions options = new()
        {
            scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
            locationPathName = simulator ? "Builds/iOS-Simulator" : "Builds/iOS",
            target = BuildTarget.iOS,
            options = BuildOptions.None,
        };

        var report = BuildPipeline.BuildPlayer(options);

        PlayerSettings.iOS.sdkVersion = previousSdk;
        PlayerSettings.iOS.simulatorSdkArchitecture = previousSimArch;
        AssetDatabase.SaveAssets();

        if (Application.isBatchMode && report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            EditorApplication.Exit(1);
        }
    }
}
