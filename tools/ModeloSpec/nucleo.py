# Modelo del núcleo del motor por lectura de ESPEC_01 (rutas A/B) y FiltroNoVariantes (ESPEC_03 §2).
# No ejecuta el C# original. Cubre: generador DFS, AND de pronóstico, grupos con máscara, NoVariantes con letras,
# tolerancias (con y sin fallos), controles con fallos, conjuntos e If-Then de condiciones «Cantidad» y de grupos.

SIG = {'1': 4, 'X': 2, '2': 1}


def col_de_pronosticos(pron):
    v = 0
    for p in reversed(pron):
        b = 0
        for ch in p[::2]:
            b |= SIG[ch]
        v = (v << 3) | b
    return v


def col_de_texto(s):
    return sum(SIG[c] << (3 * i) for i, c in enumerate(s))


def texto_de_col(c, n):
    return ''.join('2X1'[((c >> (3 * i)) & 7) >> 1] for i in range(n))


def popcount(x):
    return bin(x & (2 ** 64 - 1)).count('1')


class NoVariantes:
    MX, M2 = 80421421917330, 5026338869833

    def __init__(self, n, var='', eq='', do='', activa=True):
        self.n = n
        self.activa = activa
        self.analisis = False  # AnalisisActivo (AnalizarVX2)
        self.sets = []
        for txt in (var, eq, do):
            flags, tol = [False] * (n + 1), [' '] * (n + 1)
            if txt:
                for t in txt.split(','):
                    if t[-1].isalpha():
                        k, letra = int(t[:-1]), t[-1]
                    else:
                        k, letra = int(t), ' '
                    flags[k] = True
                    tol[k] = letra
            self.sets.append((flags, tol))

    def analizar(self, c):
        ne, nd = popcount(c & self.MX), popcount(c & self.M2)
        self.cont = (ne + nd, ne, nd)
        return all(f[k] for (f, _), k in zip(self.sets, self.cont))

    def tolerancias(self, letras):
        return sum(1 for l in letras for (_, t), k in zip(self.sets, self.cont) if t[k] == l)


def array_tol(s):
    sep = next((ch for ch in s if ch in ',-'), '')
    if sep == ',':
        p = s.split(',')
        a = [False] * (int(p[-1]) + 2)
        for x in p:
            a[int(x)] = True
    elif sep == '-':
        lo, hi = map(int, s.split('-')[:2])
        a = [False] * (hi + 2)
        for i in range(hi - lo + 1):
            a[lo + i] = True
    else:
        a = [False] * (int(s) + 2)
        a[int(s)] = True
    return a


class Grupo:
    def __init__(self, n, partidos=None, filtro=None, tolerancias=(), fallos_tol='', base=False):
        self.todos = base
        self.base = base
        act = [False] * n
        for p in partidos or []:
            act[p - 1] = True
        self.mascara = 0
        for a in reversed(act):
            self.mascara = (self.mascara << 3) | (7 if a else 0)
        # Todo Grupo tiene sus 14 filtros (Grupo.cs:94-134): sin condición, un NoVariantes inactivo, que en las rutas C/D
        # se analiza y registra igualmente si AnalizarVX2 está activo.
        self.filtro = filtro or NoVariantes(n, activa=False)
        self.tols = [(l.upper(), array_tol(a)) for l, a in tolerancias]
        self.fallos_tol = fallos_tol
        self.arr_fallos = array_tol(fallos_tol) if fallos_tol else None
        self.recalc, self.memo = True, True

    def analiza(self, c):
        if not self.recalc:
            return self.memo
        col = c if self.todos else c & self.mascara
        ok = True
        if self.filtro and self.filtro.activa and not self.filtro.analizar(col):
            ok = False
        if ok:
            ok = self.tol_validas()
        self.memo, self.recalc = ok, False
        return ok

    def analiza_an(self, c, reg):
        """Grupo.AnalizaColumna(long, ContenedorAnalisis) (Grupo.cs:226-300): rutas C/D."""
        if not self.recalc:
            return self.memo
        col = (c & self.mascara) or c
        ok = True
        f = self.filtro
        if f:
            if f.analisis:
                if f.activa:
                    ok = f.analizar(col)
                else:
                    f.analizar(col)
                if ok and self.base:
                    reg.anadir(f)
            elif f.activa:
                ok = f.analizar(col)
        if ok:
            ok = self.tol_validas()
        self.memo, self.recalc = ok, False
        return ok

    def tol_validas(self):
        activos = [self.filtro] if self.filtro and self.filtro.activa else []
        if self.fallos_tol == '' and not self.tols:
            return True
        acc = [sum(f.tolerancias(l) for f in activos) for l, _ in self.tols]
        cumple = [k < len(a) and a[k] for k, (_, a) in zip(acc, self.tols)]
        if self.fallos_tol == '':
            return all(cumple)
        nf = cumple.count(False)
        return nf < len(self.arr_fallos) and self.arr_fallos[nf]


class Control:
    def __init__(self, grupos, fallos=''):
        self.grupos = grupos
        self.usa = fallos != ''
        if self.usa:
            self.arr = [False] * (len(grupos) + 1)
            for x in expand(fallos):
                self.arr[x] = True
        self.recalc, self.memo = True, True

    def analiza(self, c, G):
        if self.usa:
            if not self.recalc:
                return self.memo
            nf = sum(1 for g in self.grupos if not G[g].analiza(c))
            self.memo, self.recalc = self.arr[nf], False
            return self.memo
        return all(G[g].analiza(c) for g in self.grupos)

    def analiza_an(self, c, G, reg):
        """ControlGrupos.cs:105-161: con fallos evalúa sus grupos sin análisis; sin fallos, con análisis y corte."""
        G[0].base = G[0].todos = True
        if self.usa:
            if not self.recalc:
                return self.memo
            nf = sum(1 for g in self.grupos if not G[g].analiza(c))
            ok = self.arr[nf]
            if ok:
                reg.global_['porFallosGrupos'][nf] += 1
            else:
                reg.vaciar()
            self.memo, self.recalc = ok, False
            return ok
        return all(G[g].analiza_an(c, reg) for g in self.grupos)


class Registro:
    """ContenedorAnalisisGlobal + FiltrosTemp (solo NoVariantes en este modelo)."""
    def __init__(self, n, ngrupos, ncontroles):
        self.global_ = dict(vx2=[[0] * (n + 1) for _ in range(3)], porFallosGrupos=[0] * ngrupos, porFallosConjuntos=[0] * ncontroles)
        self.temp = []

    def anadir(self, f):
        if all(f is not x for x in self.temp):
            self.temp.append(f)

    def vaciar(self):
        self.temp = []

    def incrementar(self):
        for f in self.temp:
            if f.analisis:
                for k in range(3):
                    self.global_['vx2'][k][f.cont[k]] += 1
        self.vaciar()


def expand(s):
    out = []
    for t in s.split(','):
        if '-' in t:
            a, b = map(int, t.split('-'))
            out += list(range(a, b + 1))
        else:
            out.append(int(t))
    return out


def lista_aciertos(s, longitud):
    vals = sorted(set(expand(s)))
    a = [False] * (max(longitud, vals[-1]) + 1)
    for v in vals:
        a[v] = True
    return a


def condicion_cantidad(texto, n):
    valor = int(texto[texto.rfind(' '):].strip())
    nv = NoVariantes(n)
    i = 0 if 'Variantes' in texto else 1 if 'X' in texto else 2 if '2' in texto else None
    if i is not None:
        nv.sets[i][0][valor] = True
    todos = ','.join(str(k) for k in range(n + 1))
    for j in range(3):
        if not any(nv.sets[j][0]):
            for k in range(n + 1):
                nv.sets[j][0][k] = True
    return nv


def comprueba_ifthen(ifthen, c, G, n):
    conds, rango_c, grs, rango_g = ifthen
    fallos = 0
    for si, entonces in conds:
        v = condicion_cantidad(si, n).analizar(c) != ('(NO)' in si)
        if v:
            w = condicion_cantidad(entonces, n).analizar(c) != ('(NO)' in entonces)
            fallos += 0 if w else 1
    ok = True
    if conds:
        arr = [False] * (len(conds) + 1)
        for x in expand(rango_c):
            arr[x] = True
        ok = arr[len(conds) - fallos]
    if ok and grs:
        fallos = 0
        for gi, gt, nif, nth in grs:
            v = G[gi].analiza(c) != nif
            if v and (G[gt].analiza(c) != nth) is False:
                fallos += 1
        arr = [False] * (len(grs) + 1)
        for x in expand(rango_g):
            arr[x] = True
        ok = arr[len(grs) - fallos]
    return ok


def columnas_generadas(pron, n):
    """GeneradorColumnas: columna inicial y DFS en preorden (solo las que pasan el AND con el pronóstico)."""
    base = col_de_pronosticos(pron)
    np_ = len(pron)
    out = []

    def nueva(c):
        if base & c == c:
            out.append(c)

    def genera(v, pos, prof):
        m = 7 << (3 * pos)
        for p in range(pos, np_):
            for k in (3 * p + 1, 3 * p):
                x = (v & ~m) ^ (1 << k)
                nueva(x)
                if prof < np_:
                    genera(x, p + 1, prof + 1)
            m <<= 3

    nueva(4 * (8 ** n - 1) // 7)
    genera(4 * (8 ** n - 1) // 7, 0, 1)
    return out


def analizar(n, pron=None, lineas=None, grupos=None, controles=None, conjuntos=None, ifthen=None, externo=False, vx2=False):
    """Rutas C/D (Analizador.cs:137-165, 256-349): sin criba de pronóstico; If-Then hace AND."""
    G = grupos or [Grupo(n, base=True)]
    C = controles or [Control(list(range(len(G))))]
    for g in G:
        if g.filtro:
            g.filtro.analisis = vx2
    reg = Registro(n, len(G), len(C))
    res = {'an': 0, 'ac': 0}

    def analiza_columna(c):
        for ctl in C:
            ctl.recalc = True
            for g in ctl.grupos:
                G[g].recalc = True
        if conjuntos:
            valida = True
            for j, i in enumerate(conjuntos[0][0]):
                if j > 0:
                    reg.vaciar()
                if not C[i].analiza_an(c, G, reg):
                    valida = False
                    break
            if valida:
                for idx, fallos in conjuntos[1:]:
                    nf = sum(1 for i in idx if not C[i].analiza(c, G))
                    arr = lista_aciertos(fallos, len(idx))
                    if len(arr) < nf + 1 or not arr[nf]:
                        valida = False
                        break
                    reg.global_['porFallosConjuntos'][nf] += 1
                    reg.global_['porFallosGrupos'][nf] += 1
        else:
            valida = all(ctl.analiza_an(c, G, reg) for ctl in C)
        if ifthen and not comprueba_ifthen(ifthen, c, G, n):
            valida = False
        res['an'] += 1
        if valida:
            reg.incrementar()
            res['ac'] += 1

    if lineas is not None:
        for l in lineas:
            c = col_de_texto(l if externo or len(l) <= n else l[:n])
            if (2 ** 48 - 1) & c == c:
                analiza_columna(c)
    else:
        for c in columnas_generadas(pron, n):
            analiza_columna(c)
    return res['an'], res['ac'], reg.global_


def calcular(n, pron=None, lineas=None, completar='', grupos=None, controles=None, conjuntos=None, ifthen=None):
    """Ruta A/B. Devuelve (analizadas, aceptadas, columnas aceptadas en orden)."""
    G = grupos or [Grupo(n, base=True)]
    C = controles or [Control(list(range(len(G))))]
    pb = col_de_pronosticos(pron)
    res = {'an': 0, 'ac': 0, 'cols': []}

    def analiza_columna(c):
        valida = False
        if pb & c == c:
            # RecalcularControladorGrupos: solo los grupos listados en algún control (los demás conservan la memo).
            for ctl in C:
                ctl.recalc = True
                for g in ctl.grupos:
                    G[g].recalc = True
            if conjuntos:
                valida = all(C[i].analiza(c, G) for i in conjuntos[0][0])
                if valida:
                    for idx, fallos in conjuntos[1:]:
                        nf = sum(1 for i in idx if not C[i].analiza(c, G))
                        arr = lista_aciertos(fallos, len(idx))
                        if len(arr) < nf + 1 or not arr[nf]:
                            valida = False
                            break
            else:
                valida = all(ctl.analiza(c, G) for ctl in C)
        if ifthen:
            valida = comprueba_ifthen(ifthen, c, G, n)
        res['an'] += 1
        if valida:
            res['ac'] += 1
            res['cols'].append(c)

    if lineas is not None:
        base = 2 ** 48 - 1
        for l in lineas:
            if len(l) > n:
                c = col_de_texto(l[:n])
            elif len(l) < n:
                c = col_de_texto(l + (completar or '1' * (n - len(l))))
            else:
                c = col_de_texto(l)
            if base & c == c:
                analiza_columna(c)
    else:
        base = col_de_pronosticos(pron)
        np_ = len(pron)
        ini = 4 * (8 ** n - 1) // 7

        def nueva(c):
            if base & c == c:
                analiza_columna(c)

        def genera(v, pos, prof):
            m = 7 << (3 * pos)
            for p in range(pos, np_):
                for k in (3 * p + 1, 3 * p):
                    x = (v & ~m) ^ (1 << k)
                    nueva(x)
                    if prof < np_:
                        genera(x, p + 1, prof + 1)
                m <<= 3

        nueva(ini)
        genera(ini, 0, 1)
    return res['an'], res['ac'], res['cols']
