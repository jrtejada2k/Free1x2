# ModeloSpec

Modelos independientes en Python que calculan los valores `modelo` y `bcl` del oráculo (ESPEC_12 §1).
Se escriben **leyendo** la spec o el fuente público de .NET 8; nunca ejecutan el C# original.

| Script | Transcribe | Genera |
|--------|-----------|--------|
| `net.py` | `ArraySortHelper.cs`, `SortUtils.MoveNansToFront`, `Hashtable.cs`, `HashHelpers.cs` (dotnet/runtime `release/8.0`) | — (biblioteca) |
| `escrutinio.py` | `Escrutador.cs` (ESPEC_05 §4.1) | — (biblioteca) |
| `nucleo.py` | ESPEC_01 (rutas A/B) y `FiltroNoVariantes` (ESPEC_03 §2) | — (biblioteca) |
| `generar_nucleo.py` | — | `motor/nucleo.jsonl` (sobrescribe) |
| `generar_escrutinio.py` | `EscrutadorComb.cs`, `PosiblesPremiosComparer.cs` | GE-01…09 de `motor/escrutinio/escrutador.jsonl` y `net/colecciones.jsonl` (añade al final: regenerar sobre ficheros limpios) |

Uso: `python3 -I tools/ModeloSpec/generar_escrutinio.py`.
