// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views;

/// <summary>
/// Pantalla principal de la app WinUI (réplica de <c>Free1X2.UI.MainForm</c>):
/// boleto base editable a la izquierda y, a la derecha, la rejilla de condiciones
/// (con semáforos), la navegación de grupos, el filtro de columnas y las acciones
/// (Calcular / Reducir / Nueva / Abrir / Guardar combinación). Página de inicio del NavigationView.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPageViewModel ViewModel { get; } = new();

    public MainPage()
    {
        this.InitializeComponent();

        // El boleto crea su propio ViewModel; lo enlazamos al de la página para coordinar
        // (volcado de pronósticos, recarga al cambiar de grupo, etc.).
        ViewModel.Boleto = Boleto.ViewModel;

        // La VM navega a través del Frame de la página (no conoce la UI directamente).
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        // Suscribe el VM a AppState.Cambiado mientras la página está en pantalla (B-02). Se hace
        // ANTES de ejecutar la acción de la barra, que puede disparar ese mismo evento.
        ViewModel.Activar();

        // Al volver de una página de filtro, refresca los semáforos de condiciones
        // (equivale a MainForm.MainFormActivated → ActualizaGrupoPantalla).
        ViewModel.RefrescarPantalla();

        // Acción solicitada por un botón de la barra de herramientas (Nueva/Abrir/Guardar
        // combinación, Guardar/Abrir equipos, Borrar temporales/informes). MainWindow navega
        // aquí con el token y la página invoca el comando equivalente del MainForm original.
        if (e.Parameter is AccionInicio accion && accion != AccionInicio.Ninguna)
        {
            // B-06: la Task se OBSERVA. Antes era `_ = ViewModel.EjecutarAccionAsync(accion);`
            // y una excepción (p. ej. abrir una combinación corrupta) se perdía con la Task
            // descartada: la acción fallaba en silencio, sin log ni mensaje al usuario.
            Services.AppServices.EjecutarObservandoErrores(
                () => ViewModel.EjecutarAccionAsync(accion),
                "MainPage.OnNavigatedTo → " + accion);
        }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        // Desuscribe el VM: sin esto, AppState (singleton) retenía este VM y esta página para
        // siempre, y refrescaba todos los zombis en cada cambio del motor (B-02).
        ViewModel.Desactivar();
        base.OnNavigatedFrom(e);
    }
}
