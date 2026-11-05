#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

/// <summary>
/// Ajustes del Info.plist de iOS que Unity no expone en Player Settings.
/// ITSAppUsesNonExemptEncryption = false: la app solo usa cifrado estándar
/// exento (HTTPS/TLS para comunicarse con el backend y Firebase), así
/// App Store Connect no pide la "Export Compliance" en cada build.
/// </summary>
public static class IOSInfoPlistPostProcess
{
    [PostProcessBuild(100)]
    public static void OnPostProcessBuild(BuildTarget target, string buildPath)
    {
        if (target != BuildTarget.iOS)
        {
            return;
        }

        string plistPath = Path.Combine(buildPath, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
        plist.root.SetBoolean("ITSAppUsesNonExemptEncryption", false);
        plist.WriteToFile(plistPath);
    }
}
#endif
