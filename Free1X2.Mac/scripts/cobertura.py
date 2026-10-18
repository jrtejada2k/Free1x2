"""Cobertura de líneas del paquete por módulo y umbral de ESPEC_14 §3 (C6 del plan de pruebas).

Uso: python3 scripts/cobertura.py <Free1X2Kit.json de `swift test --enable-code-coverage --show-codecov-path`>
Falla si Free1X2CasosDeUso baja del 85 %.
"""
import collections
import json
import sys

UMBRALES = {"Free1X2CasosDeUso": 85.0}

datos = json.load(open(sys.argv[1]))
modulos = collections.defaultdict(lambda: [0, 0])
for f in datos["data"][0]["files"]:
    if "/Sources/" not in f["filename"]:
        continue
    modulo = f["filename"].split("/Sources/")[1].split("/")[0]
    lineas = f["summary"]["lines"]
    modulos[modulo][0] += lineas["covered"]
    modulos[modulo][1] += lineas["count"]
fallo = False
for modulo, (cubiertas, total) in sorted(modulos.items()):
    pct = 100 * cubiertas / total
    minimo = UMBRALES.get(modulo)
    marca = "" if minimo is None else (" OK" if pct >= minimo else f" < {minimo} %")
    fallo |= minimo is not None and pct < minimo
    print(f"{modulo:22s} {pct:5.1f} %{marca}")
sys.exit(1 if fallo else 0)
