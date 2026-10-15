# Genera Golden/net/aleatorio.jsonl (G-NET-RND). Uso: python3 -I tools/ModeloSpec/generar_aleatorio.py
import os, sys, json, struct
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from net import RandomNet
G = os.path.join(os.path.dirname(os.path.abspath(__file__)), '../../Free1X2.Mac/Packages/Free1X2Kit/Sources/Free1X2Pruebas/Golden/')
J = lambda d: json.dumps(d, ensure_ascii=False, separators=(',', ':'))
bits = lambda x: '0x%016X' % struct.unpack('<Q', struct.pack('<d', x))[0]
TR = ['dotnet/runtime release/8.0 src/libraries/System.Private.CoreLib/src/System/Random.Net5CompatImpl.cs (Net5CompatSeedImpl)', 'docs/mac/ESPEC_16_PIEZAS_TRANSVERSALES.md §5', 'tools/ModeloSpec/net.py RandomNet']
casos = []
for s in [0, 1, 42, -1, 2**31 - 1, -2**31]:
    r = [RandomNet(s) for _ in range(5)]
    casos.append(dict(id=f'NET-RND-{s}', fuente='bcl', componente='AleatorioNET', entrada=dict(semilla=s),
        salida=dict(next=[r[0].next() for _ in range(10)], next100=[r[1].next_max(100) for _ in range(10)],
                    nextMenos5a5=[r[2].next_rango(-5, 5) for _ in range(10)],
                    nextRangoGrande=[r[3].next_rango(-2**31, 2**31 - 1) for _ in range(6)],
                    nextDouble=[bits(r[4].next_double()) for _ in range(5)]), traza=TR))
open(G + 'net/aleatorio.jsonl', 'w').write('\n'.join(J(c) for c in casos) + '\n')
print(casos[0]['salida']['next'][:3], casos[2]['salida']['next100'][:5])
