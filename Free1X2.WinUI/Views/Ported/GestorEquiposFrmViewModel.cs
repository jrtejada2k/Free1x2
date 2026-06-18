// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Free1X2.Online;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Gestión de Equipos".
/// Replica el WinForms <c>GestorEquiposFrm</c>: cuatro listas de equipos por
/// categoría (1ª, 2ª, 2ªB e Internacionales) que se cargan desde los ficheros
/// <c>equipos1.dat</c>, <c>equipos2.dat</c>, <c>equipos2b.dat</c> y
/// <c>equiposInt.dat</c>. Permite mover equipos entre categorías, eliminarlos,
/// dar de alta nuevos y guardar los cambios.
///
/// Persistencia idéntica al legacy: lectura/escritura línea a línea con
/// StreamReader/StreamWriter (Encoding.Latin1 ≡ Encoding.Default de WinForms en
/// el Windows español). El directorio base equivale a Application.StartupPath
/// (AppContext.BaseDirectory en WinUI).
/// </summary>
public partial class GestorEquiposFrmViewModel : ObservableObject
{
    // Legacy: archivoEquiposPrimera/Segunda/SegundaB/Int = StartupPath + "/Equipos/equiposX.dat".
    private static string DirEquipos =>
        AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

    private static readonly string ArchivoEquiposPrimera = DirEquipos + "/Equipos/equipos1.dat";
    private static readonly string ArchivoEquiposSegunda = DirEquipos + "/Equipos/equipos2.dat";
    private static readonly string ArchivoEquiposSegundaB = DirEquipos + "/Equipos/equipos2b.dat";
    private static readonly string ArchivoEquiposInt = DirEquipos + "/Equipos/equiposInt.dat";

    // ===== Listas por categoría (lbEquipos1 / lbEquipos2 / lbEquipos2B / lbEquiposInt) =====
    public ObservableCollection<string> EquiposPrimera { get; } = new();
    public ObservableCollection<string> EquiposSegunda { get; } = new();
    public ObservableCollection<string> EquiposSegundaB { get; } = new();
    public ObservableCollection<string> EquiposInt { get; } = new();

    // ===== Selección actual de cada lista (ListView.SelectedItem) =====
    [ObservableProperty]
    private string? _seleccionPrimera;

    [ObservableProperty]
    private string? _seleccionSegunda;

    [ObservableProperty]
    private string? _seleccionSegundaB;

    [ObservableProperty]
    private string? _seleccionInt;

    // ===== Alta de equipo en línea (legacy: AgregarEquipoFrm dentro del gestor) =====
    [ObservableProperty]
    private string _nuevoNombre = "";

    /// <summary>Categorías para el alta en línea (RadioButtons del AgregarEquipoFrm legacy).</summary>
    public string[] Categorias { get; } = new[] { "1ª", "2ª", "2ªB", "Int" };

    [ObservableProperty]
    private string _nuevaCategoria = "1ª";

    // ===== Importación online del catálogo de equipos (clubprogol.com) =====
    // Cliente HTTP de la integración OPCIONAL (mismo servicio que "Descarga de boleto").
    private readonly QuinielaOnlineService _servicioOnline = new();

    /// <summary>Opción de país para el selector online (texto visible + código "es"/"mx").</summary>
    public sealed record OpcionPais(string Nombre, string Codigo)
    {
        public override string ToString() => Nombre;
    }

    /// <summary>
    /// Países disponibles para importar el catálogo (España / México). Reutiliza exactamente el
    /// mismo patrón de selector que <c>DescargaBoletoFrmViewModel.Paises</c>.
    /// </summary>
    public IReadOnlyList<OpcionPais> Paises { get; } = new List<OpcionPais>
    {
        new OpcionPais("España", "es"),
        new OpcionPais("México", "mx"),
    };

    [ObservableProperty]
    private OpcionPais _paisSeleccionado;

    /// <summary>True mientras se descarga el catálogo (deshabilita el botón en la vista).</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PuedeImportar))]
    private bool _importando;

    /// <summary>
    /// Inverso de <see cref="Importando"/>: el botón de importar está habilitado salvo mientras hay
    /// una descarga en curso. (La página enlaza IsEnabled aquí; mismo enfoque sin convertidores que
    /// el resto de pantallas.)
    /// </summary>
    public bool PuedeImportar => !Importando;

    // ===== Estado / feedback =====
    [ObservableProperty]
    private string _estado = "Preparado";

    /// <summary>Acción para navegar a otra página (la cablea la página con Frame.Navigate(tipo)).</summary>
    public Action<Type>? Navegar { get; set; }

    /// <summary>
    /// Equivale a <c>btnNuevoEquipo_Click</c> del WinForms (GestorEquiposFrm.cs líneas 120-124):
    /// <c>new AgregarEquipoFrm(...).ShowDialog()</c>. Abre el formulario de alta de equipo como
    /// página independiente (AgregarEquipoFrmPage), que persiste el equipo al .dat de su categoría.
    /// El alta rápida en línea (<see cref="NuevoEquipo"/>) sigue disponible.
    /// </summary>
    [RelayCommand]
    private void NuevoEquipoFormulario()
    {
        Navegar?.Invoke(typeof(AgregarEquipoFrmPage));
    }

    public GestorEquiposFrmViewModel()
    {
        // Legacy: ctor -> CargaEquipos(lbEquiposX, archivoEquiposX) para cada categoría.
        CargaEquipos(EquiposPrimera, ArchivoEquiposPrimera);
        CargaEquipos(EquiposSegunda, ArchivoEquiposSegunda);
        CargaEquipos(EquiposSegundaB, ArchivoEquiposSegundaB);
        CargaEquipos(EquiposInt, ArchivoEquiposInt);

        // País inicial del selector online: España por defecto (la quiniela "clásica" de la app).
        _paisSeleccionado = Paises[0];
    }

    /// <summary>
    /// Importa el catálogo de equipos ONLINE (clubprogol.com,
    /// <c>GET .../equipos/{pais}</c>) y FUSIONA la lista plana de equipos en la categoría destino
    /// elegida (<see cref="NuevaCategoria"/>, el mismo selector que usa el alta en línea).
    ///
    /// Importante (offline-first / no destructivo):
    ///   · El backend sirve una ÚNICA lista plana ("all", equipos de jornadas recientes), no una
    ///     por división; por eso se fusiona en la categoría que el usuario tenga seleccionada.
    ///   · Solo AÑADE los que falten (dedup, ignora may/min y espacios); NO borra ni reordena los
    ///     existentes y NO guarda en disco: el usuario revisa y pulsa "Guardar" como siempre.
    ///   · Sin conexión / 429 / dato inválido → mensaje claro y NO cambia nada (modo manual).
    /// </summary>
    [RelayCommand]
    private async Task ImportarEquiposOnline()
    {
        if (Importando) return;

        string pais = PaisSeleccionado?.Codigo ?? "es";
        Importando = true;
        Estado = "Conectando con el servicio online…";
        try
        {
            CatalogoEquipos catalogo = await _servicioOnline
                .ObtenerEquiposAsync(pais, CancellationToken.None)
                .ConfigureAwait(true); // continúa en el hilo de UI para tocar las ObservableCollection

            // Categoría destino: el MISMO selector que el alta en línea (1ª/2ª/2ªB/Int).
            ObservableCollection<string> destino = ListaDeCategoria(NuevaCategoria);

            int importados = 0;
            foreach (string equipo in catalogo.EquiposPlano())
            {
                if (!ContieneEquipo(destino, equipo))
                {
                    destino.Add(equipo);
                    importados++;
                }
            }

            Estado = importados > 0
                ? $"{importados} equipos importados (online) a {NuevaCategoria}. Revisa y pulsa «Guardar archivos»."
                : $"El catálogo online no añadió equipos nuevos a {NuevaCategoria} (ya estaban todos).";
        }
        catch (Exception ex) when (ex is QuinielaOnlineException || ex is not OperationCanceledException)
        {
            // Fallo de red/HTTP/parseo: no se cambia NADA. Mensaje amigable + modo manual.
            string detalle = ex is QuinielaOnlineException
                ? ex.Message
                : "No se pudo importar el catálogo: " + ex.Message;
            Estado = detalle + " Los equipos no se modificaron.";
        }
        finally
        {
            Importando = false;
        }
    }

    /// <summary>Mapea el nombre de categoría (1ª/2ª/2ªB/Int) a su lista en memoria.</summary>
    private ObservableCollection<string> ListaDeCategoria(string categoria) => categoria switch
    {
        "2ª" => EquiposSegunda,
        "2ªB" => EquiposSegundaB,
        "Int" => EquiposInt,
        _ => EquiposPrimera, // "1ª" por defecto
    };

    /// <summary>
    /// Comprueba si la lista ya contiene el equipo, ignorando may/min y espacios extremos
    /// (la lista plana del catálogo ya viene recortada y de-duplicada, pero la lista local
    /// pudo escribirse a mano).
    /// </summary>
    private static bool ContieneEquipo(ObservableCollection<string> lista, string equipo)
    {
        foreach (string existente in lista)
        {
            if (string.Equals(existente?.Trim(), equipo?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Carga una categoría desde su .dat (legacy: CargaEquipos).
    /// El legacy usaba StreamReader(Encoding.Default); en WinUI el equivalente del
    /// code page ANSI español es Encoding.Latin1 (ISO-8859-1).
    /// </summary>
    private void CargaEquipos(ObservableCollection<string> equipos, string archivoEquipos)
    {
        equipos.Clear();
        try
        {
            using var sr = new StreamReader(archivoEquipos, Encoding.Latin1);
            while (sr.Peek() != -1)
            {
                string? linea = sr.ReadLine();
                if (linea is not null)
                {
                    equipos.Add(linea);
                }
            }
        }
        catch (Exception ex)
        {
            // El legacy no protegía la carga; en WinUI evitamos tumbar la página si
            // falta el .dat (entorno de desarrollo) y dejamos la categoría vacía.
            Estado = $"No se pudo cargar {Path.GetFileName(archivoEquipos)}: {ex.Message}";
        }
    }

    // ===== Mover 1ª -> 2ª (btnASegunda_Click) =====
    [RelayCommand]
    private void MoverAPrimeraASegunda() => Mover(EquiposPrimera, EquiposSegunda, SeleccionPrimera);

    // ===== Mover 2ª -> 1ª (btnAPrimera_Click) =====
    [RelayCommand]
    private void MoverASegundaAPrimera() => Mover(EquiposSegunda, EquiposPrimera, SeleccionSegunda);

    // ===== Mover 2ª -> 2ªB (btnASegundaB_Click) =====
    [RelayCommand]
    private void MoverASegundaASegundaB() => Mover(EquiposSegunda, EquiposSegundaB, SeleccionSegunda);

    // ===== Mover 2ªB -> 2ª (btnASegundaSube_Click) =====
    [RelayCommand]
    private void MoverASegundaBASegunda() => Mover(EquiposSegundaB, EquiposSegunda, SeleccionSegundaB);

    // ===== Eliminar de cada categoría (btnEliminaDeX_Click) =====
    [RelayCommand]
    private void EliminarDePrimera() => Eliminar(EquiposPrimera, SeleccionPrimera);

    [RelayCommand]
    private void EliminarDeSegunda() => Eliminar(EquiposSegunda, SeleccionSegunda);

    [RelayCommand]
    private void EliminarDeSegundaB() => Eliminar(EquiposSegundaB, SeleccionSegundaB);

    [RelayCommand]
    private void EliminarDeInt() => Eliminar(EquiposInt, SeleccionInt);

    /// <summary>
    /// Alta de equipo en línea (legacy: btnNuevoEquipo_Click -> AgregarEquipoFrm).
    /// El AgregarEquipoFrm legacy recibía las cuatro ListBox y, según el RadioButton,
    /// añadía el nombre a la categoría destino si no existía ya.
    /// </summary>
    [RelayCommand]
    private void NuevoEquipo()
    {
        if (string.IsNullOrWhiteSpace(NuevoNombre))
        {
            Estado = "Introduce un nombre de equipo.";
            return;
        }

        // Legacy AgregarEquipoFrm.btnNuevoEquipo_Click: mapeo categoría -> lista destino.
        var destino = NuevaCategoria switch
        {
            "2ª" => EquiposSegunda,
            "2ªB" => EquiposSegundaB,
            "Int" => EquiposInt,
            _ => EquiposPrimera, // "1ª" (rdbPrimera por defecto)
        };

        if (!destino.Contains(NuevoNombre))
        {
            destino.Add(NuevoNombre);
            Estado = $"Equipo \"{NuevoNombre}\" añadido a {NuevaCategoria}.";
        }
        else
        {
            Estado = $"El equipo \"{NuevoNombre}\" ya existe en {NuevaCategoria}.";
        }

        NuevoNombre = "";
    }

    // ===== Guardar todas las categorías (btnGuardar_Click) =====
    [RelayCommand]
    private void Guardar()
    {
        // Legacy: GuardarEquiposTodasCategorias -> GuardarEquipos(archivoX, lbX) para cada categoría.
        try
        {
            GuardarEquipos(ArchivoEquiposPrimera, EquiposPrimera);
            GuardarEquipos(ArchivoEquiposSegunda, EquiposSegunda);
            GuardarEquipos(ArchivoEquiposSegundaB, EquiposSegundaB);
            GuardarEquipos(ArchivoEquiposInt, EquiposInt);
            Estado = "Equipos guardados.";
        }
        catch (Exception ex)
        {
            Estado = $"Error al guardar: {ex.Message}";
            Services.AppServices.MostrarError($"No se pudieron guardar los equipos: {ex.Message}");
        }
    }

    /// <summary>
    /// Escribe una categoría a su .dat (legacy: GuardarEquipos, StreamWriter Encoding.Default).
    /// </summary>
    private static void GuardarEquipos(string archivoEquipos, ObservableCollection<string> equipos)
    {
        string? dir = Path.GetDirectoryName(archivoEquipos);
        if (dir is not null && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        using var sw = new StreamWriter(archivoEquipos, false, Encoding.Latin1);
        foreach (string equipo in equipos)
        {
            sw.WriteLine(equipo);
        }
    }

    // ----- Helpers de manipulación de listas (idénticos a los btnA*_Click del WinForms) -----

    /// <summary>
    /// Mueve el equipo seleccionado de <paramref name="origen"/> a
    /// <paramref name="destino"/> sin duplicar (igual que los btnA*_Click del
    /// WinForms, que comprobaban Items.Contains antes de Add y luego Remove).
    /// </summary>
    private void Mover(ObservableCollection<string> origen, ObservableCollection<string> destino, string? seleccion)
    {
        if (string.IsNullOrEmpty(seleccion)) { Estado = "Selecciona un equipo."; return; }
        if (!destino.Contains(seleccion)) destino.Add(seleccion);
        origen.Remove(seleccion);
        Estado = $"Movido \"{seleccion}\".";
    }

    private void Eliminar(ObservableCollection<string> lista, string? seleccion)
    {
        if (string.IsNullOrEmpty(seleccion)) { Estado = "Selecciona un equipo."; return; }
        lista.Remove(seleccion);
        Estado = $"Eliminado \"{seleccion}\".";
    }
}
