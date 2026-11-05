import Free1X2Base
import Free1X2Motor
import Testing

/// #21 (dueño, 4-nov-2026): las cantidades de las condiciones admiten rangos; D-CT2 (letra en mayúsculas) y D-CT3/H11
/// (lista ordenada sin repetidos).
@Suite("Rangos en las listas")
struct RangosListaTests {
    @Test(arguments: [
        ("0,3,5", "0,3,5"), ("2-6", "2,3,4,5,6"), ("0,2-4,9", "0,2,3,4,9"), ("6-4", "4,5,6"), ("1-3a,7", "1A,2A,3A,7"),
        ("8a", "8A"), ("", ""),
    ])
    func expandir(_ entrada: String, _ esperado: String) {
        #expect(RangosHelper.expandirLista(entrada) == esperado)
    }

    @Test(arguments: [("0,2-3", "0,2,3"), ("3,1", "1,3"), ("7,9-11", "7,9,10,11"), ("0-1", "0,1"), ("2", "2")])
    func ordenada(_ entrada: String, _ esperado: String) throws {
        #expect(try RangosHelper.listaOrdenada(entrada) == esperado)
    }

    @Test func variantesConRangoIgualQueConLista() throws {
        var conRango = CondicionNoVariantes(numeroPartidos: 14)
        try conRango.setNoVariantes("4-7")
        var conLista = CondicionNoVariantes(numeroPartidos: 14)
        try conLista.setNoVariantes("4,5,6,7")
        #expect(conRango == conLista)
    }
}
