import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroPesosNumericos.cs (02 §1).

final class FiltroPesosNumericos: FiltroColumna {
    static let indicesUnos = [7, 5, 3, 1, 8, 6, 4, 2, 9, 7, 5, 3, 1, 8, 6, 4]
    static let indicesEquis = [5, 1, 6, 2, 7, 3, 8, 4, 9, 5, 1, 6, 2, 7, 3, 8]
    static let indicesDoses = [1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5, 6, 7]

    let condicion: CondicionPesosNumericos
    let analisisActivo: Bool
    /// Pesos en el orden de `CondicionPesosNumericos.Peso`: global, variantes, unos, equis, doses.
    private(set) var pesos = [0, 0, 0, 0, 0]
    private var fallos = [false, false, false, false, false]
    private var noFallosActual = 0
    private(set) var figura: Int64 = 0

    init(_ condicion: CondicionPesosNumericos, analisisActivo: Bool) {
        self.condicion = condicion
        self.analisisActivo = analisisActivo
    }

    var tipo: TipoFiltro { .pesosNumericos }
    var activa: Bool { condicion.activa }
    private var usaFiguras: Bool { !condicion.figuras.isEmpty }

    /// `CalculaPeso` (:226-241): raíz digital (0 solo para 0; 9 es estable).
    static func raiz(_ v: Int) -> Int {
        var n = v
        while n > 9 { n = n / 10 + n % 10 }
        return n
    }

    /// `AnalizaColumna` (:113-163): el índice solo avanza en celdas 1, X o 2 (compacta los huecos).
    private func calcular(_ columna: Columna) throws(ErrorDominio) {
        var c = columna
        var (s1, sX, s2, k) = (0, 0, 0, 0)
        let o = "FiltroPesosNumericos.cs:127-145"
        while c != 0 {
            switch c & 7 {
            case 4:
                s1 += try Self.indicesUnos.leer(k, o)
                k += 1
            case 2:
                sX += try Self.indicesEquis.leer(k, o)
                k += 1
            case 1:
                s2 += try Self.indicesDoses.leer(k, o)
                k += 1
            default: break
            }
            c >>= 3
        }
        let unos = Self.raiz(s1)
        let equis = Self.raiz(sX)
        let doses = Self.raiz(s2)
        let variantes = Self.raiz(equis + doses)
        pesos = [Self.raiz(variantes + unos), variantes, unos, equis, doses]
        fallos = [false, false, false, false, false]
        noFallosActual = 0
    }

    /// `esColumnaValida` (:243-278): marca **todos** los que fallan (sin cortocircuito).
    private func marcar() throws(ErrorDominio) -> Bool {
        var ok = true
        for i in 0..<5 where !(try condicion.permitidos[i].permite(pesos[i], "FiltroPesosNumericos.cs:243-278")) {
            fallos[i] = true
            ok = false
        }
        return ok
    }

    /// `SonToleranciasValidas` (:331-395): orden G, V, U, E, D; muta `noFallosActual`.
    private func toleranciasValidas() throws(ErrorDominio) -> Bool {
        for i in 0..<5 where fallos[i] {
            guard try condicion.tolerados[i].permite(pesos[i], "FiltroPesosNumericos.cs:331-395") else { return false }
            noFallosActual += 1
        }
        return try condicion.tolerancias.permite(noFallosActual, "FiltroPesosNumericos.cs:394")
    }

    /// `AnalizaFiguras` (:164-172): con 5 figuras acepta sin calcular.
    private func figurasValidas() -> Bool {
        if condicion.figuras.count == 5 { return true }
        obtenerFigura()
        return condicion.figuras.contains(figura)
    }

    /// `ObtenerFiguraLong` (:174-224): por el producto de repeticiones de los 5 pesos (4+1 colisiona con 2+2+1; 5 iguales ⇒ 0).
    func obtenerFigura() {
        var cuenta = [Int](repeating: 0, count: 10)
        for p in pesos { cuenta[p] += 1 }  // invariante: cada peso es una raíz digital, 0…9
        let producto = cuenta.filter { $0 != 0 }.reduce(1, *)
        figura =
            switch producto {
            case 1: 69905
            case 2: 8465
            case 3: 785
            case 4: 545
            case 6: 50
            default: 0
            }
    }

    func analizar(_ columna: Columna) throws(ErrorDominio) -> Bool {
        try calcular(columna)
        // `CumpleCondiciones` (:397-425).
        var valida = false
        if try marcar(), try condicion.tolerancias.permite(0, "FiltroPesosNumericos.cs:404") { valida = true }
        if !valida { valida = try toleranciasValidas() }
        if valida && usaFiguras { valida = figurasValidas() }
        return valida
    }

    /// (:280-329,427-485): con fallos tolerados prefija `!` y añade el recuento; las figuras van aparte, al final.
    func analizarFallos(_ columna: Columna) throws(ErrorDominio) -> [String]? {
        try calcular(columna)
        let nombres = ["global", "de variantes", "de unos", "de equis", "de doses"]
        var texto = ""
        for i in 0..<5 where !(try condicion.permitidos[i].permite(pesos[i], "FiltroPesosNumericos.cs:280-329")) {
            fallos[i] = true
            texto += "Fallo en peso \(nombres[i])  (\(pesos[i]))#"
        }
        var figuras: [String]?
        if usaFiguras && !figurasValidas() {
            figuras = ["Fallo en Figuras  (\(try UtilidadesEntradasValores.obtenerTextoFiguraFromLong(figura)))"]
        }
        var lista = Self.fallos(texto)
        if let l = lista, try toleranciasValidas() {
            lista = l.map { "!" + $0 } + ["*\(noFallosActual) tolerancias aceptadas."]
        }
        switch (lista, figuras) {
        case (nil, nil): return nil
        case (nil, let f?): return f
        case (let l?, nil): return l
        case (let l?, let f?): return l + f
        }
    }

    func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int { 0 }
}
