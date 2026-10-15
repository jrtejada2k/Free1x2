#!/bin/sh
# Nunca abortar (ESPEC_11 §5.2; ESPEC_17 NF-07). `!`, `try!` e IUO los vigila swift-format (.swift-format).
# Una línea se admite solo con el comentario `// invariante: <por qué es inalcanzable>`.
# Free1X2Bench queda fuera: es un ejecutable de medida y su salida es `print`.
cd "$(dirname "$0")/.." || exit 1
if grep -rnE 'fatalError\(|preconditionFailure\(|as!|print\(' App Packages/Free1X2Kit/Sources --include='*.swift' --exclude-dir=Free1X2Bench |
    grep -v '// invariante:'; then
    echo "Patrones prohibidos encontrados." >&2
    exit 1
fi
