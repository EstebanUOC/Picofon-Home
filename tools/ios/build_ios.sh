#!/usr/bin/env bash
# Genera, archiva y (opcionalmente) sube el build de iOS de Picofon.
#
# IMPORTANTE (17/11/2026): compilar con Xcode 26 (SDK iOS 26). Con Xcode 27
# (SDK iOS 27) la app se cierra al abrir en iOS 27, porque Unity 2022.3.62 no
# adopta el ciclo de vida UIScene que iOS 27 exige a las apps compiladas con
# el SDK 27 (UIApplicationEvaluateRuntimeIssueForNoSceneLifecycleAdoption).
# Solución definitiva: migrar a Unity 2022.3.72f1+ o Unity 6 (6000.0.68f1+).
#
# Uso:
#   tools/ios/build_ios.sh            # Unity + archive
#   tools/ios/build_ios.sh --upload   # además sube a App Store Connect
#
# Variables:
#   XCODE_APP   Xcode a usar (por defecto /Applications/Xcode-26.6.app)
#   UNITY       Editor de Unity (por defecto 2022.3.62f2)
#   OUT         Carpeta del archive y logs (por defecto ~/Desktop/picofon-ios)
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
PROJECT="$ROOT/Picofon Home"
XCODE_APP="${XCODE_APP:-/Applications/Xcode-26.6.app}"
UNITY="${UNITY:-/Applications/Unity/Hub/Editor/2022.3.62f2/Unity.app/Contents/MacOS/Unity}"
OUT="${OUT:-$HOME/Desktop/picofon-ios}"
TEAM="P426M5SKYQ"

export DEVELOPER_DIR="$XCODE_APP/Contents/Developer"
export LANG=en_US.UTF-8
mkdir -p "$OUT"

echo "Xcode: $(xcodebuild -version | head -1)  SDK iphoneos $(xcrun --sdk iphoneos --show-sdk-version)"

"$UNITY" -batchmode -quit -nographics -projectPath "$PROJECT" -buildTarget iOS \
  -executeMethod BuildScript.BuildIOS -logFile "$OUT/unity-build-device.log"

PLIST="$PROJECT/Builds/iOS/Info.plist"
echo "CFBundleVersion=$(/usr/libexec/PlistBuddy -c 'Print :CFBundleVersion' "$PLIST")"

rm -rf "$OUT/Picofon.xcarchive" "$OUT/export"
(cd "$PROJECT/Builds/iOS" && xcodebuild -workspace Unity-iPhone.xcworkspace -scheme Unity-iPhone \
  -configuration Release -destination 'generic/platform=iOS' -archivePath "$OUT/Picofon.xcarchive" \
  -allowProvisioningUpdates DEVELOPMENT_TEAM="$TEAM" CODE_SIGN_STYLE=Automatic archive) \
  > "$OUT/xcodebuild-archive.log" 2>&1
APP_PLIST="$OUT/Picofon.xcarchive/Products/Applications/Picofon.app/Info.plist"
for k in DTSDKName DTXcode CFBundleVersion ITSAppUsesNonExemptEncryption FIREBASE_ANALYTICS_COLLECTION_DEACTIVATED; do
  echo "$k=$(/usr/libexec/PlistBuddy -c "Print :$k" "$APP_PLIST")"
done

if [[ "${1:-}" == "--upload" ]]; then
  xcodebuild -exportArchive -archivePath "$OUT/Picofon.xcarchive" \
    -exportOptionsPlist "$OUT/ExportOptions.plist" -exportPath "$OUT/export" \
    -allowProvisioningUpdates > "$OUT/xcodebuild-upload.log" 2>&1
  grep -E "Upload succeeded|EXPORT (SUCCEEDED|FAILED)" "$OUT/xcodebuild-upload.log"
fi

# Unity suele quitar un preload de Adaptive Performance (Android) y borrar
# AddressableAssetsData/link.xml al cambiar a iOS: revisar `git status` y
# revertir esos cambios antes de commitear.
