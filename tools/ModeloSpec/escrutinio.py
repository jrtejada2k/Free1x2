# Modelo por lectura de Free1X2.Domain/Escrutinio/Escrutador.cs (no ejecuta el original).
import os
F=os.path.join(os.path.dirname(__file__),'../../Free1X2.Mac/Packages/Free1X2Kit/Sources/Free1X2Pruebas/Fixtures/')
def lineas(p):  # StreamReader.ReadLine + Trim + ToUpper (ASCII aquí)
    t=open(F+p).read().replace('\r\n','\n').replace('\r','\n')
    ls=t.split('\n')
    if ls and ls[-1]=='': ls=ls[:-1]
    return [l.strip().upper() for l in ls]
N=14
def escruta(gan, cols, premios=None, add=False, jornada=1, fichero=''):
    nc=[0]*(N+1); lista=[]; gan=gan.replace('x','X')
    for num,c in enumerate(cols,1):
        m=min(len(c),len(gan)); na=sum(1 for i in range(m) if gan[i]=='*' or gan[i]==c[i])
        if add and na in premios: lista.append(dict(columna=c,fichero=fichero,jornada=jornada,noColumna=num,premio=na,noBoleto=num//8+(1 if num%8 else 0)))
        nc[na]+=1
    return nc,lista
def hist(nc): return {str(i):v for i,v in enumerate(nc) if v}
a,b,r=lineas('cols_a.txt'),lineas('cols_b.txt'),lineas('cols_con_repetidas.txt')
g=a[0]
