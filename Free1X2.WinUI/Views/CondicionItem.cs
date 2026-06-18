// Free1X2 · WinUI 3 — WIN3
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Controls;

namespace Free1X2.WinUI.Views;

/// <summary>
/// Una condición (filtro) de la rejilla de la MainPage: equivale a un botón + semáforo del
/// MainForm WinForms. Mantiene el nombre del filtro (clave del enum <c>Filtro</c>), la página
/// portada a la que navega y el estado de semáforo calculado desde el <see cref="Grupo"/>.
/// </summary>
public partial class CondicionItem : ObservableObject
{
    /// <summary>
    /// </summary>
    /// <param name="titulo">Texto visible del botón.</param>
    /// <param name="claveFiltro">
    /// Clave del filtro (p. ej. <c>Filtro.NoVariantes.ToString()</c>) usada en
    /// <c>grupo.GetFiltro(clave)</c>. <c>null</c> para condiciones especiales sin IFiltro
    /// (If-Then y Control de grupos), cuyo semáforo se gestiona aparte.
    /// </param>
    /// <param name="pagina">Página portada a la que navega al pulsar.</param>
    public CondicionItem(string titulo, string? claveFiltro, Type pagina, string glifo = "")
    {
        Titulo = titulo;
        ClaveFiltro = claveFiltro;
        Pagina = pagina;
        Glifo = glifo;
    }

    public string Titulo { get; }
    public string? ClaveFiltro { get; }
    public Type Pagina { get; }

    /// <summary>Glifo (Segoe Fluent/MDL2) del icono de la condición, como en las imágenes del original.</summary>
    public string Glifo { get; }

    /// <summary>Estado del semáforo del filtro (gris/verde/rojo). Réplica de PonerColorBotonCondicion.</summary>
    [ObservableProperty]
    private EstadoSemaforo _estado = EstadoSemaforo.Neutro;

    partial void OnEstadoChanged(EstadoSemaforo value) => OnPropertyChanged(nameof(Fondo));

    /// <summary>Color de fondo del tile según el estado (tinte verde=activa, rojo=con datos, gris=vacía).</summary>
    public Microsoft.UI.Xaml.Media.Brush Fondo => Estado switch
    {
        EstadoSemaforo.Verde => new Microsoft.UI.Xaml.Media.SolidColorBrush(
            Windows.UI.Color.FromArgb(0x3D, 0x2E, 0x7D, 0x32)),  // verde (activa)
        EstadoSemaforo.Rojo => new Microsoft.UI.Xaml.Media.SolidColorBrush(
            Windows.UI.Color.FromArgb(0x3D, 0xC0, 0x39, 0x2B)),  // rojo (con datos, inactiva)
        _ => new Microsoft.UI.Xaml.Media.SolidColorBrush(
            Windows.UI.Color.FromArgb(0x29, 0xEE, 0x6C, 0x4D)),  // coral tenue (vacía) — acento terciario de la paleta
    };

    /// <summary>
    /// Recalcula el semáforo desde el grupo, igual que <c>PonerColorBotonCondicion</c> (MainForm):
    /// IsActive → Verde; en su defecto ContieneDatos → Rojo; en su defecto → Neutro (gris).
    /// </summary>
    public void RefrescarDesdeGrupo(Grupo grupo)
    {
        if (ClaveFiltro is null) return; // condiciones especiales: ver MainPageViewModel
        IFiltro filtro = grupo.GetFiltro(ClaveFiltro);
        if (filtro is null)
        {
            Estado = EstadoSemaforo.Neutro;
        }
        else if (filtro.IsActive)
        {
            Estado = EstadoSemaforo.Verde;
        }
        else if (filtro.ContieneDatos)
        {
            Estado = EstadoSemaforo.Rojo;
        }
        else
        {
            Estado = EstadoSemaforo.Neutro;
        }
    }
}
