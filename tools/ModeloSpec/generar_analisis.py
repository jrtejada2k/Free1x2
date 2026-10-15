# Genera Golden/motor/analisis.jsonl (sobrescribe): rutas C/D. Uso: python3 -I tools/ModeloSpec/generar_analisis.py
import os, sys, json
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from nucleo import *
G = os.path.join(os.path.dirname(os.path.abspath(__file__)), '../../Free1X2.Mac/Packages/Free1X2Kit/Sources/Free1X2Pruebas/Golden/')
N = 14
TODOS = ','.join(str(i) for i in range(N + 1))
J = lambda d: json.dumps(d, ensure_ascii=False, separators=(',', ':'))
casos = []
TR = ['Free1X2.Domain/MotorCalculo/Analizador.cs:137-165,256-349,351-408', 'Free1X2.Domain/MotorCalculo/Grupo.cs:226-300', 'Free1X2.Domain/Analisis/ContenedorAnalisis.cs:109-123', 'tools/ModeloSpec/nucleo.py (analizar)']


def caso(id_, entrada, grupos, controles=None, conjuntos=None, ifthen=None, traza=(), fuente='modelo'):
    an, ac, g = analizar(N, entrada['pronosticos'], lineas=entrada.get('lineas'), grupos=grupos, controles=controles, conjuntos=conjuntos, ifthen=ifthen,
                         externo=entrada['ruta'] == 'D', vx2='vx2' in entrada.get('analisis', []))
    salida = dict(analizadas=an, aceptadas=ac, vx2=g['vx2'], porFallosGrupos=g['porFallosGrupos'], porFallosConjuntos=g['porFallosConjuntos'])
    casos.append(dict(id=id_, fuente=fuente, componente='MotorCalculo.analizar', entrada=entrada, salida=salida, traza=TR + list(traza)))
    return salida


def gnv(var, eq=TODOS, do=TODOS, activa=True):
    return dict(activa=activa, variantes=var, equis=eq, doses=do)


# G-IFTHEN-OVR en ruta C: If-Then hace AND y no hay criba de pronóstico (comparar con IFT-L1: 3/2 en la ruta A).
ovr = dict(pronosticos=['1'] * N, lineas=['1' * N, 'X' * N, '2' * N], ruta='C', analisis=['vx2'], grupo0=dict(noVariantes=gnv('0')),
           ifThen=dict(condiciones=[dict(si='Cantidad de Variantes: 14', entonces='Cantidad de X: 14')], rangoCondiciones='1'))
s = caso('ANA-L1', ovr, [Grupo(N, filtro=NoVariantes(N, '0', TODOS, TODOS), base=True)], ifthen=([('Cantidad de Variantes: 14', 'Cantidad de X: 14')], '1', [], ''),
         traza=['ESPEC_01 §2.2 P1: la misma combinación da 2 aceptadas contando (A) y 1 analizando (C)'], fuente='lectura')
assert (s['analizadas'], s['aceptadas']) == (3, 1) and s['vx2'][0][0] == 1
seis = ['1,X,2'] * 6 + ['1'] * 8
# G-GR-1: grupo base y subgrupo {1,2,3,4}; ruta C con la estadística de variantes del grupo base.
e = dict(pronosticos=seis, ruta='C', analisis=['vx2'], grupos=[dict(base=True, noVariantes=gnv('2,3,4')), dict(partidos='1,2,3,4', noVariantes=gnv('1,2'))])
caso('ANA-M1', e, [Grupo(N, filtro=NoVariantes(N, '2,3,4', TODOS, TODOS), base=True), Grupo(N, [1, 2, 3, 4], NoVariantes(N, '1,2', TODOS, TODOS))])
# Sin la bandera de análisis: nada se registra.
e = dict(e, analisis=[])
caso('ANA-M2', e, [Grupo(N, filtro=NoVariantes(N, '2,3,4', TODOS, TODOS), base=True), Grupo(N, [1, 2, 3, 4], NoVariantes(N, '1,2', TODOS, TODOS))])
# Filtro inactivo con análisis: se analiza y registra (estadística de todas las aceptadas).
e = dict(pronosticos=seis, ruta='C', analisis=['vx2'], grupos=[dict(base=True, noVariantes=gnv('0', activa=False))])
caso('ANA-M3', e, [Grupo(N, filtro=NoVariantes(N, '0', TODOS, TODOS, activa=False), base=True)])
# Control con fallos permitidos: columnas por nº de grupos fallados.
sub = [dict(partidos='1,2', noVariantes=gnv('0')), dict(partidos='3,4', noVariantes=gnv('0')), dict(partidos='5,6', noVariantes=gnv('0'))]
gsub = lambda: [Grupo(N, base=True), Grupo(N, [1, 2], NoVariantes(N, '0', TODOS, TODOS)), Grupo(N, [3, 4], NoVariantes(N, '0', TODOS, TODOS)), Grupo(N, [5, 6], NoVariantes(N, '0', TODOS, TODOS))]
for i, fallos in enumerate(['0,1', '1-2'], 4):
    e = dict(pronosticos=seis, ruta='C', analisis=['vx2'], grupos=[dict(base=True)] + sub, controles=[dict(grupos='0'), dict(grupos='1,2,3', fallos=fallos)])
    caso(f'ANA-M{i}', e, gsub(), controles=[Control([0]), Control([1, 2, 3], fallos)], traza=['Free1X2.Domain/MotorCalculo/ControlGrupos.cs:105-143'])
# Conjuntos: columnas por nº de controles fallados.
e = dict(pronosticos=seis, ruta='C', analisis=['vx2'], grupos=[dict(base=True)] + sub, controles=[dict(grupos='0'), dict(grupos='1'), dict(grupos='2'), dict(grupos='3')],
         conjuntos=[dict(controles='0', fallos='0'), dict(controles='1,2,3', fallos='0-2')])
caso('ANA-M6', e, gsub(), controles=[Control([0]), Control([1]), Control([2]), Control([3])], conjuntos=[([0], '0'), ([1, 2, 3], '0-2')],
     traza=['Free1X2.Domain/MotorCalculo/ControladorGrupos.cs:201-269'])
# Archivo: la ruta C recorta sin rellenar; la D ni recorta ni rellena (una línea de 17 signos queda fuera de los 48 bits).
lineas = ['1X2', '1' * 14, 'X' * 17, '2X' * 7, '']
for ruta, i in (('C', 7), ('D', 8)):
    e = dict(pronosticos=['1,X,2'] * N, lineas=lineas, ruta=ruta, analisis=['vx2'], grupo0=dict(noVariantes=gnv('0', activa=False)))
    caso(f'ANA-M{i}', e, [Grupo(N, filtro=NoVariantes(N, '0', TODOS, TODOS, activa=False), base=True)],
         traza=['Free1X2.Domain/MotorCalculo/GeneradorColumnas.cs:300-336'])
open(G + 'motor/analisis.jsonl', 'w').write('\n'.join(J(c) for c in casos) + '\n')
for c in casos: print(c['id'], c['salida']['analizadas'], c['salida']['aceptadas'], c['salida']['porFallosGrupos'], c['salida']['porFallosConjuntos'], c['salida']['vx2'][0])
