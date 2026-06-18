using System;
using System.Collections.Generic;
using Free1X2.WinUI.Views.Ported;

namespace Free1X2.WinUI.Navigation;

/// <summary>Una pantalla portada desde WinForms, para la navegacion data-driven.</summary>
public record PortedPage(string Title, string Glyph, Type PageType, string Category);

/// <summary>
/// Registro de las 111 pantallas portadas a WinUI 3. Generado desde Views/Ported/.
/// El NavigationView se puebla solo desde aqui - sin tocar el XAML del shell.
/// </summary>
public static class PortedPagesRegistry
{
    public static readonly IReadOnlyList<PortedPage> All = new[]
    {
        // ===== Filtros =====
        new PortedPage("Analisis Formatos 123",               "", typeof(AnalisisFormatos123FrmPage), "Filtros"),
        new PortedPage("Calculo Formatos",                    "", typeof(CalculoFormatosFrmPage), "Filtros"),
        new PortedPage("Combinar Filtros",                    "", typeof(CombinarFiltrosPage), "Filtros"),
        new PortedPage("Contactos",                           "", typeof(ContactosFrmPage), "Filtros"),
        new PortedPage("Control Tol",                         "", typeof(ControlTolFrmPage), "Filtros"),
        new PortedPage("Di Filtros",                          "", typeof(DiFiltrosPage), "Filtros"),
        new PortedPage("Dialogo Filtrar Por Limites",         "", typeof(DialogoFiltrarPorLimitesFrmPage), "Filtros"),
        new PortedPage("Dibujos",                             "", typeof(DibujosFrmPage), "Filtros"),
        new PortedPage("Diferencias",                         "", typeof(DiferenciasFrmPage), "Filtros"),
        new PortedPage("Distancias",                          "", typeof(DistanciasFrmPage), "Filtros"),
        new PortedPage("Figuras Filtros",                     "", typeof(FigurasFiltrosFrmPage), "Filtros"),
        new PortedPage("Filtro Porcen JB",                    "", typeof(FiltroPorcenJBPage), "Filtros"),
        new PortedPage("Formatos",                            "", typeof(FormatosFrmPage), "Filtros"),
        new PortedPage("Formatos 123",                        "", typeof(Formatos123FrmPage), "Filtros"),
        new PortedPage("Frm Dependencia Lineal",              "", typeof(FrmDependenciaLinealPage), "Filtros"),
        new PortedPage("Generador CPSDiferencias",            "", typeof(GeneradorCPSDiferenciasPage), "Filtros"),
        new PortedPage("GEPT",                                "", typeof(GEPTFrmPage), "Filtros"),
        new PortedPage("If Then",                             "", typeof(IfThenFrmPage), "Filtros"),
        new PortedPage("Interrupciones",                      "", typeof(InterrupcionesFrmPage), "Filtros"),
        new PortedPage("Listado Condiciones",                 "", typeof(ListadoCondicionesFrmPage), "Filtros"),
        new PortedPage("No Variantes",                        "", typeof(NoVariantesFrmPage), "Filtros"),
        new PortedPage("Parejas",                             "", typeof(ParejasFrmPage), "Filtros"),
        new PortedPage("Rotacion De Signos",                  "", typeof(RotacionDeSignosFrmPage), "Filtros"),
        new PortedPage("Signos Seguidos",                     "", typeof(SignosSeguidosFrmPage), "Filtros"),
        new PortedPage("Simetrias",                           "", typeof(SimetriasFrmPage), "Filtros"),
        new PortedPage("Trios",                               "", typeof(TriosFrmPage), "Filtros"),
        new PortedPage("VSignos",                             "", typeof(VSignosFrmPage), "Filtros"),
        // ===== Boleto e impresion =====
        new PortedPage("Boleto",                              "", typeof(BoletoFrmPage), "Boleto e impresion"),
        new PortedPage("Col Ganadora",                        "", typeof(ColGanadoraFrmPage), "Boleto e impresion"),
        new PortedPage("Columnas Premiadas",                  "", typeof(ColumnasPremiadasFrmPage), "Boleto e impresion"),
        new PortedPage("Descarga Boleto",                     "", typeof(DescargaBoletoFrmPage), "Boleto e impresion"),
        new PortedPage("Estimador Premios",                   "", typeof(EstimadorPremiosFrmPage), "Boleto e impresion"),
        new PortedPage("Imprimir Boleto",                     "", typeof(ImprimirBoletoFrmPage), "Boleto e impresion"),
        new PortedPage("Lista Impresoras",                    "", typeof(ListaImpresorasPage), "Boleto e impresion"),
        new PortedPage("Mejores Opciones",                    "", typeof(MejoresOpcionesFrmPage), "Boleto e impresion"),
        new PortedPage("Posibles Premios",                    "", typeof(PosiblesPremiosFrmPage), "Boleto e impresion"),
        new PortedPage("Premiadas",                           "", typeof(PremiadasFrmPage), "Boleto e impresion"),
        new PortedPage("Rentabilidad",                        "", typeof(RentabilidadFrmPage), "Boleto e impresion"),
        new PortedPage("Subir Categoria",                     "", typeof(SubirCategoriaFrmPage), "Boleto e impresion"),
        new PortedPage("Ver Boletos",                         "", typeof(VerBoletosPage), "Boleto e impresion"),
        new PortedPage("Ver Boletos En Editor",               "", typeof(VerBoletosEnEditorFrmPage), "Boleto e impresion"),
        new PortedPage("Visor Posibles Premios",              "", typeof(VisorPosiblesPremiosPage), "Boleto e impresion"),
        // ===== Analisis =====
        new PortedPage("Ana Combi",                           "", typeof(AnaCombiPage), "Analisis"),
        new PortedPage("Analizador JPM",                      "", typeof(AnalizadorJPMPage), "Analisis"),
        new PortedPage("Analizar Combinacion",                "", typeof(AnalizarCombinacionFrmPage), "Analisis"),
        new PortedPage("Analizar Fichero",                    "", typeof(AnalizarFicheroFrmPage), "Analisis"),
        new PortedPage("Anastatics",                          "", typeof(AnastaticsPage), "Analisis"),
        new PortedPage("Coincidencias",                       "", typeof(CoincidenciasPage), "Analisis"),
        new PortedPage("Col Probables",                       "", typeof(ColProbablesFrmPage), "Analisis"),
        new PortedPage("Grafico Columnas",                    "", typeof(GraficoColumnasFrmPage), "Analisis"),
        new PortedPage("Guardar Valoracion",                  "", typeof(GuardarValoracionFrmPage), "Analisis"),
        new PortedPage("Historia Valoraciones",               "", typeof(HistoriaValoracionesFrmPage), "Analisis"),
        new PortedPage("Ordenar Por Probabilidad",            "", typeof(OrdenarPorProbabilidadFrmPage), "Analisis"),
        new PortedPage("Probabilidad Premios",                "", typeof(ProbabilidadPremiosPage), "Analisis"),
        new PortedPage("Valoracion",                          "", typeof(ValoracionFrmPage), "Analisis"),
        new PortedPage("Visor Analisis Columnas",             "", typeof(VisorAnalisisColumnasFrmPage), "Analisis"),
        new PortedPage("Visor Analisis Columnas Abdon",       "", typeof(VisorAnalisisColumnasAbdonFrmPage), "Analisis"),
        // ===== Estadisticas =====
        new PortedPage("Pesos Num",                           "", typeof(PesosNumFrmPage), "Estadisticas"),
        new PortedPage("Sta Inter",                           "", typeof(StaInterFrmPage), "Estadisticas"),
        new PortedPage("Sta SS",                              "", typeof(StaSSFormPage), "Estadisticas"),
        new PortedPage("Tramificar Graficas",                 "", typeof(TramificarGraficasFrmPage), "Estadisticas"),
        new PortedPage("Visor Estadisticas",                  "", typeof(VisorEstadisticasPage), "Estadisticas"),
        // ===== Columnas y combinacion =====
        new PortedPage("Algebra Columnas",                    "", typeof(AlgebraColumnasFrmPage), "Columnas y combinacion"),
        new PortedPage("Busca Lims",                          "", typeof(BuscaLimsFrmPage), "Columnas y combinacion"),
        new PortedPage("Calcula Columnas",                    "", typeof(CalculaColumnasFrmPage), "Columnas y combinacion"),
        new PortedPage("Calcula Columnas Multiple",           "", typeof(CalculaColumnasMultipleFrmPage), "Columnas y combinacion"),
        new PortedPage("Compresor",                           "", typeof(CompresorPage), "Columnas y combinacion"),
        new PortedPage("Control Grupos",                      "", typeof(ControlGruposFrmPage), "Columnas y combinacion"),
        new PortedPage("Crear Grupos",                        "", typeof(CrearGruposFrmPage), "Columnas y combinacion"),
        new PortedPage("Dif Cols",                            "", typeof(DifColsPage), "Columnas y combinacion"),
        new PortedPage("Escrutar Combinaciones",              "", typeof(EscrutarCombinacionesFrmPage), "Columnas y combinacion"),
        new PortedPage("Escrutinios",                         "", typeof(EscrutiniosFrmPage), "Columnas y combinacion"),
        new PortedPage("Fraccionador",                        "", typeof(FraccionadorFrmPage), "Columnas y combinacion"),
        new PortedPage("Frm Reducidas Perfectas",             "", typeof(FrmReducidasPerfectasPage), "Columnas y combinacion"),
        new PortedPage("Genera Pim",                          "", typeof(GeneraPimPage), "Columnas y combinacion"),
        new PortedPage("Modificador",                         "", typeof(ModificadorFrmPage), "Columnas y combinacion"),
        new PortedPage("Multiplicador",                       "", typeof(MultiplicadorFrmPage), "Columnas y combinacion"),
        new PortedPage("Reductor",                            "", typeof(ReductorFrmPage), "Columnas y combinacion"),
        new PortedPage("Resultados Calculo Multiple",         "", typeof(ResultadosCalculoMultipleFrmPage), "Columnas y combinacion"),
        new PortedPage("Selec JM",                            "", typeof(SelecJMPage), "Columnas y combinacion"),
        new PortedPage("Selector MS",                         "", typeof(SelectorMSPage), "Columnas y combinacion"),
        new PortedPage("Tramificar",                          "", typeof(TramificarFormPage), "Columnas y combinacion"),
        new PortedPage("Transposicion",                       "", typeof(TransposicionFrmPage), "Columnas y combinacion"),
        // ===== Equipos =====
        new PortedPage("Agregar Equipo",                      "", typeof(AgregarEquipoFrmPage), "Equipos"),
        new PortedPage("Gestor Equipos",                      "", typeof(GestorEquiposFrmPage), "Equipos"),
        new PortedPage("Grupos Equipos",                      "", typeof(GruposEquiposFrmPage), "Equipos"),
        // ===== Cuadros (CPs) =====
        new PortedPage("Cambio Puntos",                       "", typeof(CambioPuntosFrmPage), "Cuadros (CPs)"),
        new PortedPage("Config CPs",                          "", typeof(ConfigCPsFrmPage), "Cuadros (CPs)"),
        new PortedPage("Copiar CP",                           "", typeof(CopiarCPFrmPage), "Cuadros (CPs)"),
        new PortedPage("Copiar Datos CP",                     "", typeof(CopiarDatosCPFrmPage), "Cuadros (CPs)"),
        new PortedPage("Exportador CPs",                      "", typeof(ExportadorCPsFrmPage), "Cuadros (CPs)"),
        new PortedPage("Generar CPs",                         "", typeof(GenerarCPsPage), "Cuadros (CPs)"),
        new PortedPage("Importador CPs",                      "", typeof(ImportadorCPsFrmPage), "Cuadros (CPs)"),
        // ===== Ajustes =====
        new PortedPage("Configuracion",                       "", typeof(ConfiguracionFrmPage), "Ajustes"),
        new PortedPage("Configuracion Analisis",              "", typeof(ConfiguracionAnalisisFrmPage), "Ajustes"),
        new PortedPage("Import Export",                       "", typeof(ImportExportFrmPage), "Ajustes"),
        // ===== Ayuda =====
        new PortedPage("Acerca De",                           "", typeof(AcercaDeFrmPage), "Ayuda"),
        new PortedPage("Ayuda",                               "", typeof(AyudaFrmPage), "Ayuda"),
        new PortedPage("Creditos",                            "", typeof(CreditosFrmPage), "Ayuda"),
        // ===== Otras pantallas =====
        new PortedPage("Agrega P 15",                         "", typeof(AgregaP15FrmPage), "Otras pantallas"),
        new PortedPage("aidomnou",                            "", typeof(aidomnouPage), "Otras pantallas"),
        new PortedPage("Banco Pruebas",                       "", typeof(BancoPruebasFrmPage), "Otras pantallas"),
        new PortedPage("Dialogo Analisis Multiple De Tramos", "", typeof(DialogoAnalisisMultipleDeTramosFrmPage), "Otras pantallas"),
        new PortedPage("Dialogo Grabar Banco Pruebas",        "", typeof(DialogoGrabarBancoPruebasFrmPage), "Otras pantallas"),
        new PortedPage("Dialogo Grabar Tramos",               "", typeof(DialogoGrabarTramosFrmPage), "Otras pantallas"),
        new PortedPage("Dib",                                 "", typeof(DibFormPage), "Otras pantallas"),
        new PortedPage("Dib Rep",                             "", typeof(DibRepFrmPage), "Otras pantallas"),
        new PortedPage("Estucol",                             "", typeof(EstucolFrmPage), "Otras pantallas"),
    };
}
