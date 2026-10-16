#!/bin/sh
# M6 (PLAN §5; DP-06): Free1X2.app en Release, firmada y en un .dmg con acceso directo a Aplicaciones.
#   Sin variables:  firma ad hoc; el .dmg solo sirve para probar en este Mac (Gatekeeper lo frena en otros).
#   FREE1X2_IDENTIDAD="Developer ID Application: Nombre (EQUIPO)"  firma con runtime endurecido y sello de tiempo.
#   FREE1X2_PERFIL_NOTARIA=<perfil>  además notariza y grapa el ticket; el perfil se crea una vez con
#     `xcrun notarytool store-credentials <perfil> --apple-id <correo> --team-id <EQUIPO>` (pide la contraseña de app).
#   FREE1X2_APP=<ruta>/Free1X2.app  empaqueta esa app tal cual (p. ej. la que exporta Xcode ▸ Organizer ▸ Distribute App ▸
#     Direct Distribution, ya firmada y notarizada con la cuenta de Xcode): no compila ni vuelve a firmar.
# Salida: Free1X2.Mac/.build/dmg/Free1X2-<versión>.dmg
set -eu
cd "$(dirname "$0")/.."

VERSION=$(awk -F': ' '/MARKETING_VERSION/ {print $2}' project.yml)
SALIDA=.build/dmg
RAIZ="$SALIDA/raiz"
IDENTIDAD="${FREE1X2_IDENTIDAD:--}"

rm -rf "$SALIDA"
mkdir -p "$RAIZ"
if [ -n "${FREE1X2_APP:-}" ]; then
    ditto "$FREE1X2_APP" "$RAIZ/Free1X2.app"
else
    xcodegen generate >/dev/null
    xcodebuild -project Free1X2.xcodeproj -scheme Free1X2 -configuration Release -destination 'platform=macOS' \
        -derivedDataPath .build/DerivedData build CODE_SIGNING_ALLOWED=NO -quiet
    ditto .build/DerivedData/Build/Products/Release/Free1X2.app "$RAIZ/Free1X2.app"
    if [ "$IDENTIDAD" = "-" ]; then
        codesign --force --sign - "$RAIZ/Free1X2.app"
    else
        codesign --force --options runtime --timestamp --sign "$IDENTIDAD" "$RAIZ/Free1X2.app"
    fi
fi
codesign --verify --strict --verbose=2 "$RAIZ/Free1X2.app"

ln -s /Applications "$RAIZ/Aplicaciones"
DMG="$SALIDA/Free1X2-$VERSION.dmg"
# `diskutil image` sustituye a `hdiutil create` desde macOS 26; con macOS anteriores se usa el segundo.
diskutil image create from --format UDZO --volumeName "Free1X2 $VERSION" "$RAIZ" "$DMG" >/dev/null 2>&1 ||
    hdiutil create -volname "Free1X2 $VERSION" -srcfolder "$RAIZ" -ov -format UDZO "$DMG" >/dev/null
rm -rf "$RAIZ"

if [ "$IDENTIDAD" != "-" ]; then
    codesign --force --timestamp --sign "$IDENTIDAD" "$DMG"
fi
if [ -n "${FREE1X2_PERFIL_NOTARIA:-}" ]; then
    xcrun notarytool submit "$DMG" --keychain-profile "$FREE1X2_PERFIL_NOTARIA" --wait
    xcrun stapler staple "$DMG"
    spctl --assess --type open --context context:primary-signature --verbose "$DMG"
fi
echo "$DMG"
