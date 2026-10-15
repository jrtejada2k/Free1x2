import Free1X2Base
import Free1X2Pruebas
import Testing
import os

/// ESPEC_17 NF-06: como mucho un evento de progreso cada 100 ms, más el final.
@Suite("Progreso")
struct ProgresoTests {
    final class Recolector: Sendable {
        private let eventos = OSAllocatedUnfairLock<[Int]>(initialState: [])
        var procesadas: [Int] { eventos.withLock { $0 } }
        func anotar(_ p: Progreso) { eventos.withLock { $0.append(p.procesadas) } }
    }

    @Test func limitaLaFrecuenciaYSiempreEmiteElFinal() {
        let reloj = RelojManual()
        let recolector = Recolector()
        let reportador = ReportadorProgreso(reloj: reloj, emitir: recolector.anotar)

        reportador.reportar(Progreso(procesadas: 1))  // el primero sale
        reloj.avanzar(.milliseconds(99))
        reportador.reportar(Progreso(procesadas: 2))  // demasiado pronto
        reloj.avanzar(.milliseconds(1))
        reportador.reportar(Progreso(procesadas: 3))  // 100 ms después del primero
        reportador.terminar(Progreso(procesadas: 4))  // el final sale siempre

        #expect(recolector.procesadas == [1, 3, 4])
    }

    @Test func ningunoNoEmite() {
        ReportadorProgreso.ninguno.reportar(Progreso(procesadas: 1))
        ReportadorProgreso.ninguno.terminar(Progreso(procesadas: 2))
    }
}
