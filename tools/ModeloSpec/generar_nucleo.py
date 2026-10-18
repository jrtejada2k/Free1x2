# Genera Golden/motor/nucleo.jsonl (sobrescribe). Uso: python3 -I tools/ModeloSpec/generar_nucleo.py
import os, sys, json, hashlib
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from nucleo import *
G = os.path.join(os.path.dirname(os.path.abspath(__file__)), '../../Free1X2.Mac/Packages/Free1X2Kit/Sources/Free1X2Pruebas/Golden/')
N = 14
TODOS = ','.join(str(i) for i in range(N + 1))
J = lambda d: json.dumps(d, ensure_ascii=False, separators=(',', ':'))
casos = []
TR_GEN = ['Free1X2.Domain/MotorCalculo/GeneradorColumnas.cs:149-169,199-228 preorden: raíz; por partido ascendente, la X con su subárbol y después el 2', 'ESPEC_01 §2.1 traza de 27 columnas']

# --- lectura: trazas de ESPEC_01 ---
traza27 = '111, X11, XX1, XXX, XX2, X21, X2X, X22, X1X, X12, 211, 2X1, 2XX, 2X2, 221, 22X, 222, 21X, 212, 1X1, 1XX, 1X2, 121, 12X, 122, 11X, 112'.split(', ')
p1 = ['1'] * 11 + ['1,X,2'] * 3
an, ac, cols = calcular(N, p1)
assert [texto_de_col(c, N) for c in cols] == ['1' * 11 + t for t in traza27]
casos.append(dict(id='GEN-L1', fuente='lectura', componente='MotorCalculo.calcular', entrada=dict(pronosticos=p1, ruta='B'), salida=dict(analizadas=27, aceptadas=27, columnas=['1' * 11 + t for t in traza27]), traza=TR_GEN))
traza8 = '111, X11, XX1, XXX, X1X, 1X1, 1XX, 11X'.split(', ')
p2 = ['1,X'] * 3 + ['1'] * 11
an, ac, cols = calcular(N, p2)
assert [texto_de_col(c, N)[:3] for c in cols] == traza8
casos.append(dict(id='GEN-L2', fuente='lectura', componente='MotorCalculo.calcular', entrada=dict(pronosticos=p2, ruta='B'), salida=dict(analizadas=8, aceptadas=8, columnas=[t + '1' * 11 for t in traza8]), traza=TR_GEN))
lineas = ['1X2', '', '1X2' + '1' * 14, '11111111111111']
an, ac, cols = calcular(N, ['1,X,2'] * N, lineas=lineas)
assert cols[0] == 2513169434708
casos.append(dict(id='GEN-L3', fuente='lectura', componente='MotorCalculo.calcular', entrada=dict(pronosticos=['1,X,2'] * N, lineas=lineas, ruta='B'), salida=dict(analizadas=4, aceptadas=4, valores=[str(c) for c in cols]),
                  traza=['Free1X2.Domain/MotorCalculo/GeneradorColumnas.cs:272-299 recorta a N o rellena con CompletarCon ("" ⇒ \'1\')', 'ESPEC_01 §2.1: "1X2" ⇒ 2513169434708; línea vacía ⇒ 14 unos = 2513169434916']))
# G-IFTHEN-OVR (ESPEC_01 §6)
ovr = dict(pronosticos=['1'] * N, lineas=['1' * N, 'X' * N, '2' * N], ruta='A', grupo0=dict(noVariantes=dict(activa=True, variantes='0', equis=TODOS, doses=TODOS)),
           ifThen=dict(condiciones=[dict(si='Cantidad de Variantes: 14', entonces='Cantidad de X: 14')], rangoCondiciones='1'))
an, ac, _ = calcular(N, ovr['pronosticos'], lineas=ovr['lineas'], grupos=[Grupo(N, filtro=NoVariantes(N, '0', TODOS, TODOS), base=True)], ifthen=([('Cantidad de Variantes: 14', 'Cantidad de X: 14')], '1', [], ''))
assert (an, ac) == (3, 2)
casos.append(dict(id='IFT-L1', fuente='lectura', componente='MotorCalculo.calcular', entrada=ovr, salida=dict(analizadas=3, aceptadas=2),
                  traza=['Free1X2.Domain/MotorCalculo/Analizador.cs:97-103 ruta A: valida = IfThen (asigna, no AND)', 'ESPEC_01 §6 G-IFTHEN-OVR: la línea X…X viola pronóstico y grupo pero el If-Then la acepta; 2…2 falla el Then']))

# --- modelo ---
def caso(id_, componente, entrada, grupos, controles=None, conjuntos=None, ifthen=None, columnas=False, traza=()):
    an, ac, cols = calcular(N, entrada['pronosticos'], lineas=entrada.get('lineas'), grupos=grupos, controles=controles, conjuntos=conjuntos, ifthen=ifthen)
    salida = dict(analizadas=an, aceptadas=ac)
    if columnas:
        salida['sha256'] = hashlib.sha256('\n'.join(texto_de_col(c, N) for c in cols).encode()).hexdigest()
    casos.append(dict(id=id_, fuente='modelo', componente=componente, entrada=entrada, salida=salida, traza=list(traza) + ['tools/ModeloSpec/nucleo.py']))

seis = ['1,X,2'] * 6 + ['1'] * 8
def gnv(var, eq=TODOS, do=TODOS):
    return dict(activa=True, variantes=var, equis=eq, doses=do)
# Máscara de grupo
e = dict(pronosticos=seis, ruta='B', grupos=[dict(base=True, noVariantes=gnv('2,3,4')), dict(partidos='1,2,3,4', noVariantes=gnv('1,2'))])
caso('GRP-M1', 'MotorCalculo.calcular', e, [Grupo(N, filtro=NoVariantes(N, '2,3,4', TODOS, TODOS), base=True), Grupo(N, [1, 2, 3, 4], NoVariantes(N, '1,2', TODOS, TODOS))], columnas=True,
     traza=['Free1X2.Domain/MotorCalculo/Grupo.cs:165-225 col & máscara si no son todos los partidos'])
# Tolerancias (G-TOL)
for i, fallos in enumerate(['', '1', '0-1'], 1):
    e = dict(pronosticos=seis, ruta='A', grupos=[dict(base=True, noVariantes=gnv('0,1A,2,3B,4B', '0,1,2A,3,4,5,6', '0,1,2,3B,4,5,6'), tolerancias=[dict(letras='a', aciertos='0,1'), dict(letras='B', aciertos='0-1')], fallosTolerancias=fallos)])
    caso(f'TOL-M{i}', 'MotorCalculo.calcular', e, [Grupo(N, filtro=NoVariantes(N, '0,1A,2,3B,4B', '0,1,2A,3,4,5,6', '0,1,2,3B,4,5,6'), tolerancias=[('a', '0,1'), ('B', '0-1')], fallos_tol=fallos, base=True)],
         traza=['Free1X2.Domain/MotorCalculo/Grupo.cs:331-408 SonTolGrupoValidas / …ConFallos', 'Free1X2.Domain/MotorCalculo/ToleranciaFiltros.cs:43-114'])
# Control con fallos permitidos (G-CTL-FALLOS)
sub = [dict(partidos='1,2', noVariantes=gnv('0')), dict(partidos='3,4', noVariantes=gnv('0')), dict(partidos='5,6', noVariantes=gnv('0'))]
gsub = lambda: [Grupo(N, base=True), Grupo(N, [1, 2], NoVariantes(N, '0', TODOS, TODOS)), Grupo(N, [3, 4], NoVariantes(N, '0', TODOS, TODOS)), Grupo(N, [5, 6], NoVariantes(N, '0', TODOS, TODOS))]
for i, fallos in enumerate(['0,1', '2', '0-3'], 1):
    e = dict(pronosticos=seis, ruta='A', grupos=[dict(base=True)] + sub, controles=[dict(grupos='0'), dict(grupos='1,2,3', fallos=fallos)])
    caso(f'CTL-M{i}', 'MotorCalculo.calcular', e, gsub(), controles=[Control([0]), Control([1, 2, 3], fallos)],
         traza=['Free1X2.Domain/MotorCalculo/ControlGrupos.cs:61-90 cuenta los grupos que fallan, sin corte'])
# Conjuntos
e = dict(pronosticos=seis, ruta='A', grupos=[dict(base=True)] + sub, controles=[dict(grupos='0'), dict(grupos='1'), dict(grupos='2'), dict(grupos='3')],
         conjuntos=[dict(controles='0', fallos='0'), dict(controles='1,2,3', fallos='1')])
caso('CNJ-M1', 'MotorCalculo.calcular', e, gsub(), controles=[Control([0]), Control([1]), Control([2]), Control([3])], conjuntos=[([0], '0'), ([1, 2, 3], '1')],
     traza=['Free1X2.Domain/MotorCalculo/ControladorGrupos.cs:95-153 libres + presupuesto por conjunto'])
# If-Then de grupos y de condiciones con (NO)
e = dict(pronosticos=seis, ruta='A', grupos=[dict(base=True)] + sub, ifThen=dict(grupos=[dict(si=1, entonces=2, noSi=False, noEntonces=True)], rangoGrupos='1'))
caso('IFT-M1', 'MotorCalculo.calcular', e, gsub(), controles=[Control([0, 1, 2, 3])], ifthen=([], '', [(1, 2, False, True)], '1'),
     traza=['Free1X2.Domain/MotorCalculo/GrupoIfThen.cs:67-81'])
# Memo obsoleta: grupos fuera de todo control conservan el veredicto de la primera columna (ESPEC_01 §1.4 [PRESERVAR])
e = dict(pronosticos=seis, ruta='A', grupos=[dict(base=True)] + sub, controles=[dict(grupos='0')],
         ifThen=dict(grupos=[dict(si=1, entonces=2, noSi=False, noEntonces=True)], rangoGrupos='1'))
caso('IFT-M3', 'MotorCalculo.calcular', e, gsub(), controles=[Control([0])], ifthen=([], '', [(1, 2, False, True)], '1'),
     traza=['Free1X2.Domain/MotorCalculo/ControladorGrupos.cs:270-278 solo recalcula los grupos controlados', 'ESPEC_01 §1.4: un grupo no controlado nunca se invalida'])
e = dict(pronosticos=seis, ruta='B', grupos=[dict(base=True)], ifThen=dict(condiciones=[dict(si='(NO) Cantidad de 2: 0', entonces='Cantidad de X: 2'), dict(si='Cantidad de X: 1', entonces='(NO) Cantidad de Variantes: 3')], rangoCondiciones='1-2'))
caso('IFT-M2', 'MotorCalculo.calcular', e, [Grupo(N, base=True)], ifthen=([('(NO) Cantidad de 2: 0', 'Cantidad de X: 2'), ('Cantidad de X: 1', '(NO) Cantidad de Variantes: 3')], '1-2', [], ''), columnas=True,
     traza=['Free1X2.Domain/MotorCalculo/CondicionIfThen.cs:95-115,284-298', 'Free1X2.Domain/MotorCalculo/ControladorIfThen.cs:277-298'])
open(G + 'motor/nucleo.jsonl', 'w').write('\n'.join(J(c) for c in casos) + '\n')
for c in casos: print(c['id'], c['salida'].get('analizadas'), c['salida'].get('aceptadas'))
