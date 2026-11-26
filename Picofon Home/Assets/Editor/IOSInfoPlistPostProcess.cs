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

        // Firebase Analytics desactivado en iOS (app infantil): sin eventos
        // automáticos ni consentimientos de Google Analytics por defecto.
        plist.root.SetBoolean("FIREBASE_ANALYTICS_COLLECTION_DEACTIVATED", true);
        plist.root.SetBoolean("GOOGLE_ANALYTICS_DEFAULT_ALLOW_ANALYTICS_STORAGE", false);
        plist.root.SetBoolean("GOOGLE_ANALYTICS_DEFAULT_ALLOW_AD_STORAGE", false);
        plist.root.SetBoolean("GOOGLE_ANALYTICS_DEFAULT_ALLOW_AD_USER_DATA", false);
        plist.root.SetBoolean("GOOGLE_ANALYTICS_DEFAULT_ALLOW_AD_PERSONALIZATION_SIGNALS", false);
        plist.WriteToFile(plistPath);
    }
}
#endif
