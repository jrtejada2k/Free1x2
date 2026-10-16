import Free1X2CasosDeUso
import SwiftUI

// Formularios de configuración (plantilla T6): pantallas 25, 24 y 16.

/// «Configuración» (`ConfiguracionFrmPage`): no vuelve atrás al guardar.
struct VistaConfiguracion: View {
    @Environment(Sesion.self) private var sesion
    @State private var e = ConfiguracionEditable()

    var body: some View {
        Pagina(destino: .configuracion) {
            Text("Ajusta los parámetros del boleto, la puntuación y las preferencias del programa.")
                .foregroundStyle(Color.textoSecundario)
            Tarjeta("Configuración del boleto") {
                CampoNumero(etiqueta: "Nº de partidos en boleto", valor: $e.numeroPartidos, minimo: 1, maximo: 15)
                CampoTexto(etiqueta: "Separador de partidos", texto: $e.separador, ancho: 110)
                CampoNumero(etiqueta: "Desplazamiento de grupo/condición", valor: $e.desplazamiento, minimo: 0, maximo: 99)
            }
            Tarjeta("Puntuación en CPs") {
                HStack(spacing: 16) {
                    CampoNumero(etiqueta: "Fijos", valor: $e.fijos, ancho: 70)
                    CampoNumero(etiqueta: "Dobles", valor: $e.dobles, ancho: 70)
                    CampoNumero(etiqueta: "Triples", valor: $e.triples, ancho: 70)
                }
            }
            Tarjeta("Separador de porcentajes JB") {
                Text("Seis límites en orden estrictamente creciente.").font(.callout).foregroundStyle(Color.textoSecundario)
                HStack {
                    ForEach(0..<6, id: \.self) { i in CampoNumero(etiqueta: "", valor: $e.jb[i], ancho: 60) }
                }
            }
            Tarjeta("Parámetros LAE") {
                HStack(spacing: 16) {
                    CampoNumero(etiqueta: "Recaudación", valor: $e.recaudacion, ancho: 120)
                    LabeledContent("Símbolo moneda") {
                        TextField("", text: $e.moneda).frame(width: 50)
                            .onChange(of: e.moneda) { if e.moneda.count > 3 { e.moneda = String(e.moneda.prefix(3)) } }
                    }
                }
                HStack(spacing: 16) {
                    CampoNumero(etiqueta: "Precio apuesta", valor: $e.precioApuesta, ancho: 80)
                    CampoNumero(etiqueta: "% al 14", valor: $e.porcentajeAl14, minimo: 0, maximo: 100, ancho: 70)
                }
            }
            Tarjeta("Preferencias") {
                Toggle(isOn: $e.actualizarAlInicio) {
                    Text("Comprobar actualizaciones al inicio")
                    Text("Buscar nuevas versiones automáticamente al abrir el programa.")
                }
                Toggle(isOn: $e.confirmarAlSalir) {
                    Text("Pedir confirmación al salir")
                    Text("Mostrar una advertencia antes de cerrar la aplicación.")
                }
                Picker("Idioma", selection: $e.idioma) {
                    ForEach(e.idiomas, id: \.self) { Text($0).tag(Optional($0)) }
                }
                .frame(width: 220)
            }
            HStack {
                Button("Guardar") {
                    do throws(ErrorDominio) {
                        sesion.alertas.info(try CasosAjustes.guardarConfiguracion(e, entorno: sesion.entorno))
                    } catch {
                        sesion.alertas.error(error)
                    }
                }
                .buttonStyle(EstiloPrincipal())
                Button("Volver") { sesion.router.atras() }
            }
        }
        .onAppear { e = CasosAjustes.leerConfiguracionEditable(entorno: sesion.entorno) }
    }
}

/// «Configurar Análisis» (`ConfiguracionAnalisisFrmPage`): «Guardar» y «Salir» vuelven siempre atrás.
struct VistaConfiguracionAnalisis: View {
    @Environment(Sesion.self) private var sesion
    @State private var a = OpcionesAnalisis()

    /// Apagar el padre apaga su «Figuras…» (`OnSeguidosChanged`…); encenderlo solo la habilita.
    private func padre(_ t: String, _ v: WritableKeyPath<OpcionesAnalisis, Bool>, _ hijo: WritableKeyPath<OpcionesAnalisis, Bool>)
        -> some View
    {
        Toggle(
            t,
            isOn: Binding(
                get: { a[keyPath: v] },
                set: {
                    a[keyPath: v] = $0; if !$0 { a[keyPath: hijo] = false }
                }))
    }

    var body: some View {
        Pagina(destino: .configuracionAnalisis) {
            Text("Selecciona qué tipos de análisis estadístico se ejecutarán sobre los históricos.")
                .foregroundStyle(Color.textoSecundario)
            HStack {
                Button("Marcar todo") { todo(true) }
                Button("Desmarcar todo") { todo(false) }
            }
            Tarjeta("Signos y secuencias") {
                Toggle("VX2", isOn: $a.vx2)
                padre("Signos seguidos", \.seguidos, \.figurasV1X2)
                Toggle("Figuras V1X2", isOn: $a.figurasV1X2).disabled(!a.seguidos).padding(.leading, 20)
                Toggle("Dibujos", isOn: $a.dibujos)
                Toggle("Interrupciones", isOn: $a.interrupciones)
                Toggle("Simetrías", isOn: $a.simetrias)
                Toggle("Diferencias", isOn: $a.diferencias)
            }
            Tarjeta("Formatos y grupos") {
                Toggle("Grupos equipos", isOn: $a.gruposEquipos)
                Toggle("Formatos", isOn: $a.formatos)
                Toggle("Formatos 123", isOn: $a.formatos123)
                Toggle("Control de grupos", isOn: $a.controlGrupos)
                Toggle("Control de conjuntos", isOn: $a.controlConjuntos)
            }
            Tarjeta("Pesos y valoración") {
                padre("Pesos numéricos", \.pesos, \.figurasPesos)
                Toggle("Figuras de pesos numéricos", isOn: $a.figurasPesos).disabled(!a.pesos).padding(.leading, 20)
                Toggle("Valoración", isOn: $a.valoracion)
                Toggle("Columnas probables", isOn: $a.cps)
                Toggle("Distancias", isOn: $a.distancias)
            }
            Tarjeta("Contactos") {
                padre("Contactos", \.contactos, \.figurasContactos)
                Toggle("Figuras de contactos", isOn: $a.figurasContactos).disabled(!a.contactos).padding(.leading, 20)
            }
            HStack {
                Button("Salir") { sesion.router.atras() }
                Button("Guardar") {
                    do throws(ErrorDominio) {
                        try CasosAjustes.guardarOpcionesAnalisis(a, entorno: sesion.entorno)
                    } catch {
                        sesion.alertas.error(error)
                    }
                    sesion.router.atras()
                }
                .buttonStyle(EstiloPrincipal())
            }
        }
        .toggleStyle(.switch)
        .onAppear { a = CasosAjustes.leerOpcionesAnalisis(entorno: sesion.entorno) }
    }

    /// `MarcarTodo`/`DesmarcarTodo`: los 20 y `aTodo`.
    private func todo(_ v: Bool) {
        for k: WritableKeyPath<OpcionesAnalisis, Bool> in [
            \.vx2, \.seguidos, \.figurasV1X2, \.dibujos, \.interrupciones, \.gruposEquipos, \.formatos, \.simetrias,
            \.diferencias, \.pesos, \.figurasPesos, \.valoracion, \.cps, \.distancias, \.contactos, \.figurasContactos,
            \.formatos123, \.controlGrupos, \.controlConjuntos,
        ] { a[keyPath: k] = v }
        a.todo = v
    }
}

/// «Cambiar Puntuación» (`CambioPuntosFrmPage`): «Aceptar» guarda, avisa y vuelve; «Cancelar» vuelve.
struct VistaCambioPuntos: View {
    @Environment(Sesion.self) private var sesion
    @State private var fijos = 0.0
    @State private var dobles = 0.0
    @State private var triples = 0.0

    var body: some View {
        Pagina(destino: .cambioPuntos) {
            Text("Define los puntos asignados a cada tipo de signo al evaluar columnas.").foregroundStyle(Color.textoSecundario)
            Tarjeta {
                CampoNumero(etiqueta: "Valor Fijos", valor: $fijos, minimo: 0)
                CampoNumero(etiqueta: "Valor Dobles", valor: $dobles, minimo: 0)
                CampoNumero(etiqueta: "Valor Triples", valor: $triples, minimo: 0)
            }
            HStack {
                Button("Cancelar") { sesion.router.atras() }
                Button("Aceptar") {
                    do throws(ErrorDominio) {
                        try CasosAjustes.guardarPuntosCP(fijos, dobles, triples, entorno: sesion.entorno)
                        sesion.alertas.info("Puntuación guardada.")
                    } catch {
                        sesion.alertas.error(error)
                    }
                    sesion.router.atras()
                }
                .buttonStyle(EstiloPrincipal())
                .keyboardShortcut(.defaultAction)
            }
        }
        .onAppear {
            let l = CasosAjustes.leerPuntosCP(entorno: sesion.entorno)
            (fijos, dobles, triples) = (Double(l.fijos), Double(l.dobles), Double(l.triples))
        }
    }
}
