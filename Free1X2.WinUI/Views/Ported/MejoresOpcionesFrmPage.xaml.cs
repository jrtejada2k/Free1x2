using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "MejoresOpcionesFrm" ("Mis Mejores Opciones").
///
/// A partir de una columna ganadora conocida (posiblemente con comodines), el usuario
/// marca que partidos pueden variar ("Partidos Involucrados", legacy ckb1..ckb16) e indica
/// cuantos resultados mostrar (legacy txtLimite). Al pulsar "Calcular" (legacy button1_Click)
/// se generan las columnas ganadoras posibles, se escrutan las columnas jugadas y se muestra
/// el resumen ordenado por premios (legacy txtResumen).
/// </summary>
public sealed partial class MejoresOpcionesFrmPage : Page
{
    public MejoresOpcionesFrmViewModel ViewModel { get; } = new();

    public MejoresOpcionesFrmPage()
    {
        InitializeComponent();

        // TODO[dominio]: en el form legacy, OnLoad llamaba a
        //   AdaptarInterfaz(ColumnaGanadora.Length) para ocultar las casillas de
        //   partidos inexistentes. Aqui, cuando se porte la entrada de ColumnaGanadora,
        //   invocar ViewModel.AdaptarInterfaz(longitud) con la longitud real.
    }
}
