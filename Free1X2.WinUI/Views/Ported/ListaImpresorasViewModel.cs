using System.Collections.ObjectModel;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para ListaImpresorasPage.
/// Porta el WinForms legacy Free1X2.UI.ListaImpresoras.
/// Muestra la lista de impresoras conocidas y permite seleccionar una
/// (doble clic en el legacy) para aplicar su configuración al controlador
/// de impresión activo.
/// </summary>
public partial class ListaImpresorasViewModel : ObservableObject
{
    public ListaImpresorasViewModel()
    {
        // TODO[dominio]: cargar la lista real desde Free1X2.MotorCalculo.ControladoresImpresion.Impresoras
        //                (legacy: ListaImpresoras.CargarListaImpresoras). Aquí se deja vacía
        //                hasta integrar el dominio. Items.Add(arrayImpresoras[i].Modelo), Sorted = true.
        _impresoras = new ObservableCollection<string>();

        ActualizarMensaje();
    }

    /// <summary>Modelos de impresoras conocidas (legacy: listBox1, Sorted).</summary>
    [ObservableProperty]
    private ObservableCollection<string> _impresoras;

    /// <summary>Impresora seleccionada en la lista (legacy: listBox1.SelectedItem).</summary>
    [ObservableProperty]
    private string? _impresoraSeleccionada;

    /// <summary>
    /// Mensaje de estado.
    /// Legacy lblMensaje: "Doble clic sobre una impresora de la lista" /
    /// "No hay impresoras" / "No se han encontrado impresoras".
    /// </summary>
    [ObservableProperty]
    private string _mensaje = "Doble clic sobre una impresora de la lista";

    partial void OnImpresoraSeleccionadaChanged(string? value) => ActualizarMensaje();

    private void ActualizarMensaje()
    {
        if (Impresoras.Count == 0)
        {
            Mensaje = "No hay impresoras";
            return;
        }

        Mensaje = ImpresoraSeleccionada is { Length: > 0 }
            ? $"Impresora seleccionada: {ImpresoraSeleccionada}"
            : "Doble clic sobre una impresora de la lista";
    }

    /// <summary>
    /// Selecciona la impresora activa (legacy: listBox1_DoubleClick).
    /// </summary>
    [RelayCommand]
    private void SeleccionarImpresora(string? modelo)
    {
        if (modelo is null or { Length: 0 })
        {
            return;
        }

        ImpresoraSeleccionada = modelo;

        // TODO[dominio]: replicar ListaImpresoras.listBox1_DoubleClick:
        //   var controladorImp = BuscarImpresora(modelo);
        //   cont.Modelo          = controladorImp.Modelo;
        //   cont.MargenSuperior  = controladorImp.MargenSuperior;
        //   cont.MargenIzquierda = controladorImp.MargenIzquierda;
        //   cont.Rotar           = controladorImp.Rotar;
        //   Close();  // cerrar la página/diálogo tras aplicar la configuración.
    }
}
