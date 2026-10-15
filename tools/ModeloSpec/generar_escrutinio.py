import os, sys, json, hashlib, random
sys.path.insert(0,'.')
from net import *
from escrutinio import escruta, lineas, hist, a, b, r
G=os.path.join(os.path.dirname(__file__),'../../Free1X2.Mac/Packages/Free1X2Kit/Sources/Free1X2Pruebas/Golden/')
def J(d): return json.dumps(d, ensure_ascii=False, separators=(',',':'))
TE=['Free1X2.Domain/Escrutinio/Escrutador.cs:159-203 EscrutaColumna','Free1X2.Domain/Escrutinio/Escrutador.cs:205-226 AñadeAGanadoras/ObtenBoleto','Free1X2.Domain/Escrutinio/Escrutador.cs:243-276 PonerPremios(4)/AñadirPremiosGlobales']
esc=[]
g=a[0]
for i,(fich,cols) in enumerate([('Fixtures/cols_a.txt',a),('Fixtures/cols_b.txt',b),('Fixtures/cols_con_repetidas.txt',r)],1):
    nc,_=escruta(g,cols)
    esc.append(dict(id=f'GE-01-{i}',fuente='modelo',componente='Escrutador.columna',entrada=dict(fichero=fich,ganadora=g,categorias=[14,13,12,11,10]),salida=dict(premios=nc,acTotales=str(sum(v*k for k,v in enumerate(nc)))),traza=TE))
for i,gg in enumerate(['21122X1X11112*','21122x1x111122','***********122','2112'],1):
    nc,_=escruta(gg,a)
    esc.append(dict(id=f'GE-02-{i}',fuente='modelo',componente='Escrutador.columna',entrada=dict(fichero='Fixtures/cols_a.txt',ganadora=gg,categorias=[14,13,12,11,10]),salida=dict(premios=nc,acTotales=str(sum(v*k for k,v in enumerate(nc)))),traza=TE))
_,lista=escruta(g,a,[8,7],True,1,'Fixtures/cols_a.txt')
esc.append(dict(id='GE-03',fuente='modelo',componente='Escrutador.premiadas',entrada=dict(fichero='Fixtures/cols_a.txt',ganadora=g,categorias=[8,7]),salida=dict(premiadas=lista),traza=TE))
filas=[]; tot=[0]*15
for j,gan in enumerate(a,1):
    nc,_=escruta(gan,b); filas.append(dict(lineaID=j,columna=gan,archivo='cols_b.txt',premios=nc,acTotales=str(sum(v*k for k,v in enumerate(nc)))))
    tot=[x+y for x,y in zip(tot,nc)]
esc.append(dict(id='GE-04',fuente='modelo',componente='Escrutador.temporada',entrada=dict(ganadoras='Fixtures/cols_a.txt',fichero='Fixtures/cols_b.txt',categorias=[14,13,12,11,10]),salida=dict(filas=[dict(f,aciertosTotales=f.pop('acTotales'),seleccionado=False) for f in filas],premiosTotales=tot),traza=TE+['Free1X2.Domain/Escrutinio/Escrutador.cs:117-157 EscrutaCombConTemporada/LeeArchivoTemporada']))
esc.append(dict(id='GE-05',fuente='lectura',componente='Escrutador.filaTotales',entrada=dict(globales=tot),salida=dict(fila=dict(lineaID=0,columna='----- TOTALES:',archivo='-------------',premios=tot,aciertosTotales=' ',seleccionado=False)),traza=['Free1X2.Domain/Escrutinio/Escrutador.cs:262-276']))
def bits(s):
    v=0
    for i,p in enumerate(s.split('|')):
        for ch in p: v|={'1':4,'X':2,'2':1}[ch]<<(3*i)
    return v
gan=bits('1|2|X')
ap=[bits('1X|12|X2'),bits('1X2|1X2|1X2'),bits('2|1|1')]
esc.append(dict(id='GE-06',fuente='lectura',componente='Escrutador.apuestaMultiple',entrada=dict(ganadora=str(gan),apuestas=[str(x) for x in ap]),salida=dict(aciertos=[bin(gan&x).count('1') for x in ap]),traza=['Free1X2.Domain/Escrutinio/Escrutador.cs:104-108','Free1X2.Domain/Utils/UtilColumnas.cs ContarBitsA1']))
assert [gan]+ap==[140,238,511,289]
# GE-07 / NET-15: candidatas de EscrutadorComb y orden de Hashtable.Keys
POT=[3**i for i in range(14)]
def num(col):
    v=0
    for i,ch in enumerate(col): v+= 0 if ch=='1' else (2 if ch=='2' else 1)*POT[i]
    return v
def posibles(col,cats,N=14):
    t=Hashtable(); cats=sorted(cats,reverse=True); prof=cats[0]&0xFF; c=num(col)
    if cats[0]==N: t.insert(('i64',c),('byte',cats[0]&0xFF),False); prof-=1
    def vec(ini,pos,ult,act):
        for p in range(pos,N):
            sig=(ini//POT[p])%3
            for z in range(3):
                if z==sig: continue
                idx=ini+POT[p]*(z-sig)
                if t.contains(('i32',idx)):
                    if t.get(('i32',idx))[1]<act: t.insert(('i32',idx),('int',act),False)
                else: t.insert(('i32',idx),('int',act),True)
                if act>ult: vec(idx,p+1,ult,act-1)
    vec(c,0,cats[-1]&0xFF,prof)
    return t
TC=['Free1X2.Domain/Escrutinio/EscrutadorComb.cs:209-247 columnasCon1SignoDiferente/ObtenerPosiblesPremios','dotnet/runtime release/8.0 Hashtable.cs Insert/rehash/putEntry/CopyKeys; HashHelpers.cs ExpandPrime/GetPrime']
for i,cats in enumerate([[14,13,12,11,10],[10,11,12],[14,13,10],[14]],1):
    t=posibles(g,cats); ks=t.keys()
    cnt={}
    for k in ks:
        v=t.get(k)[1]; cnt[str(v)]=cnt.get(str(v),0)+1
    txt=';'.join(f'{k[0]}:{k[1]}' for k in ks)
    esc.append(dict(id=f'GE-07-{i}',fuente='modelo',componente='EscrutadorComb.posiblesPremios',entrada=dict(ganadora=g,categorias=cats),
        salida=dict(etiquetas=cnt,claves=len(ks),cubos=len(t.b),primeras=[f'{k[0]}:{k[1]}' for k in ks[:8]],ultimas=[f'{k[0]}:{k[1]}' for k in ks[-4:]],sha256=hashlib.sha256(txt.encode()).hexdigest(),posicionGanadora=next((j for j,k in enumerate(ks) if k[0]=='i64'),-1)),traza=TC))
# GE-09: PosiblesPremiosComparer con empates (orden = introsort .NET)
def cmpPP(x,y):
    for k in range(6,-1,-1):  # Col16..Col10 descendente
        v=(y[1][k]>x[1][k])-(y[1][k]<x[1][k])
        if v: return v
    return 0
rnd=random.Random(2026)
for i,n in enumerate([12,40],1):
    conts=[(f'G{j:02d}',[rnd.choice([0,0,1,2]),rnd.choice([0,1,1,3]),rnd.choice([0,1]),0,0,0,0]) for j in range(n)]
    conts=[(c,[x[0],x[1],x[2],0,0,0,0]) for c,x in conts]  # recuentos de Col10, Col11, Col12
    s=list(conts); introsort(s,cmpPP)
    esc.append(dict(id=f'GE-09-{i}',fuente='modelo',componente='PosiblesPremiosComparer',entrada=dict(contenedores=[dict(colGanadora=c,cuentas=x) for c,x in conts]),salida=dict(orden=[c for c,_ in s]),traza=['Free1X2.Domain/Escrutinio/PosiblesPremiosComparer.cs:10-48','dotnet/runtime release/8.0 ArraySortHelper.cs IntrospectiveSort (DT-15)']))
open(G+'motor/escrutinio/escrutador.jsonl','a').write('\n'.join(J(x) for x in esc)+'\n')
# net/colecciones: NET-12, G-NET-SORT, NET-15 pequeños
net=[]
TS=['dotnet/runtime release/8.0 ArraySortHelper.cs IntrospectiveSort/IntroSort/PickPivotAndPartition/InsertionSort/HeapSort; SortUtils.MoveNansToFront']
def dg(x): return dict(bits='0x%016X'%struct.unpack('<Q',struct.pack('<d',x))[0],texto=repr(x))
for i,v in enumerate([[3.0,float('nan'),1.0],[float('nan'),2.0,-0.0,0.0,float('-inf'),float('nan'),1.5]],1):
    s=list(v); sort_doubles(s)
    net.append(dict(id=f'NET-12-{i}',fuente='bcl',componente='IntrosortNET.doubles',entrada=dict(dobles=[dg(x) for x in v]),salida=dict(dobles=[dg(x) for x in s]),traza=TS))
for i,n in enumerate([16,17,60,500],1):
    x=[(rnd.randint(0,3),j) for j in range(n)]; s=list(x); introsort(s,lambda p,q:(p[0]>q[0])-(p[0]<q[0]))
    net.append(dict(id=f'NET-SORT-{i}',fuente='bcl',componente='IntrosortNET.empates',entrada=dict(claves=[c for c,_ in x]),salida=dict(orden=[j for _,j in s]),traza=TS))
TH=['dotnet/runtime release/8.0 Hashtable.cs Insert/rehash/putEntry/ContainsKey/CopyKeys; HashHelpers.cs']
for i,ops in enumerate([[('i32',v) for v in range(10)],[('i32',v) for v in [5,12,19,26,33,-1,2147483647]],[('i64',5),('i32',5),('i64',-1),('i64',4294967296)],[('i32',v*7) for v in range(150)]],1):
    t=Hashtable()
    for k in ops: t.insert(k,k[1],True)
    net.append(dict(id=f'NET-15-{i}',fuente='bcl',componente='HashtableNET.claves',entrada=dict(claves=[f'{k[0]}:{k[1]}' for k in ops]),salida=dict(orden=[f'{k[0]}:{k[1]}' for k in t.keys()],cubos=len(t.b)),traza=TH))
open(G+'net/colecciones.jsonl','w').write('\n'.join(J(x) for x in net)+'\n')
print(len(esc),len(net))
