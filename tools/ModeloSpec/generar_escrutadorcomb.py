# Genera Golden/motor/escrutinio/escrutadorcomb.jsonl (GE-08, DEC-501). Uso: python3 -I tools/ModeloSpec/generar_escrutadorcomb.py
import os, sys, json
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from net import Hashtable
G = os.path.join(os.path.dirname(os.path.abspath(__file__)), '../../Free1X2.Mac/Packages/Free1X2Kit/Sources/Free1X2Pruebas/Golden/')
J = lambda d: json.dumps(d, ensure_ascii=False, separators=(',', ':'))
POT = [3 ** i for i in range(14)]
BYTE = {'1': 4, 'X': 2, '2': 1, '1X': 6, '12': 5, 'X2': 3, '1X2': 7}


def num(col):
    return sum((0 if ch == '1' else 2 if ch == '2' else 1) * POT[i] for i, ch in enumerate(col))


def posibles(col, cats, N=14):
    """EscrutadorComb.cs:209-247 (igual que generar_escrutinio.py)."""
    t = Hashtable(); cats = sorted(cats, reverse=True); prof = cats[0] & 0xFF; c = num(col)
    if cats[0] == N:
        t.insert(('i64', c), ('byte', cats[0] & 0xFF), False); prof -= 1

    def vec(ini, pos, ult, act):
        for p in range(pos, N):
            sig = (ini // POT[p]) % 3
            for z in range(3):
                if z == sig: continue
                idx = ini + POT[p] * (z - sig)
                if t.contains(('i32', idx)):
                    if t.get(('i32', idx))[1] < act: t.insert(('i32', idx), ('int', act), False)
                else:
                    t.insert(('i32', idx), ('int', act), True)
                if act > ult: vec(idx, p + 1, ult, act - 1)
    vec(c, 0, cats[-1] & 0xFF, prof)
    return t


def acepta(k, pron, nv_variantes=None):
    """AnalizadorColumnaCombinacion.cs:38-134: pronóstico del grupo base (la clave base 3 leída como 3 bits) y, si hay,
    NoVariantes activo del grupo base sobre la columna 0 (máscara 0)."""
    c, p = k, 0
    while c:
        s = c & 7
        if s & BYTE.get(pron[p].replace(',', ''), 0) != s: return False
        c >>= 3; p += 1
    if nv_variantes is not None and 0 not in nv_variantes: return False
    return True


def escrutar(gan, cats, pron, nv=None):
    t = posibles(gan, cats)
    for k in t.keys():
        if acepta(k[1], pron, nv):
            v = t.get(('i64', k[1]))
            if v is None: return 'NullReferenceException'
            return 'InvalidCastException' if v[0] == 'byte' else None
    return None


TR = ['Free1X2.Domain/Escrutinio/EscrutadorComb.cs:100-134', 'Free1X2.Domain/Analisis/AnalizadorColumnaCombinacion.cs:38-134', 'docs/mac/DECISIONES.md DEC-501']
casos = []
def caso(id_, fuente, gan, cats, pron, nv=None, traza=()):
    err = escrutar(gan, cats, pron, nv)
    e = dict(ganadora=gan, categorias=cats, pronosticos=pron)
    if nv is not None: e['noVariantes'] = ','.join(map(str, nv))
    s = dict(error=err) if err else dict(premiosTotales=[0] * len(cats))
    casos.append(dict(id=id_, fuente=fuente, componente='EscrutadorComb.escrutarCombinacion', entrada=e, salida=s, traza=TR + list(traza) + (['tools/ModeloSpec/generar_escrutadorcomb.py'] if fuente == 'modelo' else [])))
    return err

U = '1' * 14
assert caso('GE-08-1', 'modelo', U, [14, 13], ['1,X,2'] * 14) in ('NullReferenceException', 'InvalidCastException')
assert caso('GE-08-2', 'modelo', '21122X1X111122', [14, 13, 12, 11, 10], ['1,X,2'] * 14) == 'NullReferenceException'
# Solo la ganadora «1…1» (índice 0) pasa el pronóstico «1»: los vecinos 3^P·z tienen algún dígito octal ∉ {0, 4}.
assert caso('GE-08-L1', 'lectura', U, [14], ['1'] * 14, traza=['índice 0 ⇒ sin grupos de 3 bits ⇒ sin fallos de pronóstico; aciertos[0L] es un byte ⇒ InvalidCast']) == 'InvalidCastException'
# NoVariantes del grupo base sobre la columna 0 (máscara 0): V = 0 no permitido ⇒ ninguna candidata ⇒ premios a 0.
assert caso('GE-08-L2', 'lectura', U, [14, 13], ['1,X,2'] * 14, nv=[1], traza=['ColumnaGrupo(col) = col & 0 ⇒ el filtro del grupo base ve la columna 0']) is None
open(G + 'motor/escrutinio/escrutadorcomb.jsonl', 'w').write('\n'.join(J(c) for c in casos) + '\n')
for c in casos: print(c['id'], c['salida'])
