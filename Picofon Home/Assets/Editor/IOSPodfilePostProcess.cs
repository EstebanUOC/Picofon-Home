#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

/// <summary>
/// Xcode 27 solo admite deployment target >= 15.0, pero varios pods de Firebase
/// (gRPC, abseil, leveldb, nanopb...) declaran 9.0-12.0 y el build falla.
/// External Dependency Manager genera el Podfile en el orden 40 y ejecuta
/// "pod install" en el 50; aqui (45) se anade un post_install que fuerza el
/// mismo minimo que Player Settings a todos los pods.
/// </summary>
public static class IOSPodfilePostProcess
{
    private const string Marker = "# Picofon: deployment target minimo para todos los pods";

    [PostProcessBuild(45)]
    public static void OnPostProcessBuild(BuildTarget target, string buildPath)
    {
        if (target != BuildTarget.iOS)
        {
            return;
        }

        string podfilePath = Path.Combine(buildPath, "Podfile");
        if (!File.Exists(podfilePath))
        {
            return;
        }

        string podfile = File.ReadAllText(podfilePath);
        if (podfile.Contains(Marker))
        {
            return;
        }

        string minVersion = PlayerSettings.iOS.targetOSVersionString;

        podfile += "\n" + Marker + "\n"
            + "post_install do |installer|\n"
            + "  installer.pods_project.targets.each do |t|\n"
            + "    t.build_configurations.each do |config|\n"
            + "      config.build_settings['IPHONEOS_DEPLOYMENT_TARGET'] = '" + minVersion + "'\n"
            + "    end\n"
            + "  end\n"
            + "end\n";

        File.WriteAllText(podfilePath, podfile);
    }
}
#endif
