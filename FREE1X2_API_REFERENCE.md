# Free1X2 API Reference Documentation

## Core API Components

### Analizador Class - Analysis Engine API

**Namespace**: `Free1X2.MotorCalculo`  
**Purpose**: Central analysis engine for processing betting combinations and applying statistical analysis.

```csharp
public class Analizador
{
    // Constructors
    public Analizador()
    public Analizador(int numeroPartidos)
    
    // Core Analysis Methods
    public void AnalizaColumna(long columna)
    public bool CompruebaPronostico(long columna)
    public void SetPronostico(int partido, string pronostico)
    public string GetPronostico(int partido)
    
    // Properties
    public GrupoPartidos GruposPartidos { get; set; }
    public ControladorGrupos ControladorGrupos { get; }
    public string ArchivoColumnasBase { get; set; }
    public string CompletarCon { get; set; }
    public int NoColsAnalizadas { get; }
    public int NoColsAceptadas { get; }
    public ControladorIfThen IfThen { get; set; }
    
    // Analysis Configuration
    public bool GuardarCols { get; set; }
    public bool AnalizarCols { get; set; }
    public IArchivoColumnas ArchivoCols { get; set; }
}
```

**Method Details**:

#### `AnalizaColumna(long columna)`
Analyzes a single betting combination against all configured filters and statistical models.

**Parameters**:
- `columna`: Long integer representing the betting combination

**Process Flow**:
1. Validates the combination against predictions
2. Recalculates group controllers
3. Applies all active filters
4. Processes If-Then conditional logic
5. Updates analysis statistics

**Usage Example**:
```csharp
Analizador analizador = new Analizador();
analizador.SetPronostico(0, "1"); // Team 1 wins
analizador.SetPronostico(1, "X"); // Draw
analizador.AnalizaColumna(123456789L); // Analyze specific combination
```

#### `SetPronostico(int partido, string pronostico)`
Sets the prediction for a specific match.

**Parameters**:
- `partido`: Match index (0-based)
- `pronostico`: Prediction value ("1", "X", "2")

---

### VariablesGlobales Class - Global Configuration API

**Namespace**: `Free1X2`  
**Purpose**: Manages application-wide configuration and settings.

```csharp
public class VariablesGlobales
{
    // Configuration Properties
    public static int NumeroPartidos { get; }
    public static int PuntosFijos { get; }
    public static int PuntosDobles { get; }
    public static int PuntosTriples { get; }
    public static string[] Separador { get; }
    public static string Idioma { get; }
    
    // Analysis Configuration
    public static bool AnalizarTodo { get; }
    public static bool AnalizarVX2 { get; }
    public static bool AnalizarSeguidos { get; }
    public static bool AnalizarDibujos { get; }
    public static bool AnalizarInterrupciones { get; }
    
    // UI Configuration
    public static bool MostrarTsFree { get; }
    public static bool MostrarTsFiltros { get; }
    public static bool MostrarTsCombinacion { get; }
    
    // Methods
    public static string GetTexto(string clave)
    public static void InicializarVariables()
    public static string GetConfigPath()
}
```

**Key Methods**:

#### `GetTexto(string clave)`
Retrieves localized text for the specified key.

**Parameters**:
- `clave`: Text key for localization

**Returns**: Localized string or key if not found

**Usage Example**:
```csharp
string welcomeText = VariablesGlobales.GetTexto("welcome_message");
```

---

### IFiltro Interface - Filter System API

**Namespace**: `Free1X2.MotorCalculo`  
**Purpose**: Base interface for all filter implementations.

```csharp
public interface IFiltro
{
    // Properties
    bool EsVacio { get; }
    bool EsActivo { get; set; }
    
    // Core Methods
    bool CompruebaPronostico(long columna);
    string ObtenInformacion();
    void InicializarFiltro();
}
```

**Implementation Requirements**:

#### `CompruebaPronostico(long columna)`
Core filter logic that determines if a combination passes the filter criteria.

**Parameters**:
- `columna`: Betting combination to analyze

**Returns**: `true` if combination passes filter, `false` otherwise

#### `ObtenInformacion()`
Provides human-readable information about the filter's current configuration.

**Returns**: String describing filter settings and criteria

**Filter Implementations**:

### FiltroContactos - Contact Pattern Filter

```csharp
public class FiltroContactos : IFiltro
{
    public int ContactosMinimos { get; set; }
    public int ContactosMaximos { get; set; }
    public bool PermitirSinContactos { get; set; }
    
    public bool CompruebaPronostico(long columna)
    {
        // Analyzes contact patterns between consecutive matches
        int contactos = CalcularContactos(columna);
        return contactos >= ContactosMinimos && contactos <= ContactosMaximos;
    }
}
```

### FiltroDistancias - Distance Pattern Filter

```csharp
public class FiltroDistancias : IFiltro
{
    public int DistanciaMinima { get; set; }
    public int DistanciaMaxima { get; set; }
    
    public bool CompruebaPronostico(long columna)
    {
        // Analyzes distance patterns between similar outcomes
        int distancia = CalcularDistanciaMedia(columna);
        return distancia >= DistanciaMinima && distancia <= DistanciaMaxima;
    }
}
```

---

### MainForm Class - UI Controller API

**Namespace**: `Free1X2.UI`  
**Purpose**: Primary application window and UI coordinator.

```csharp
public partial class MainForm : Form
{
    // Core Properties
    public Analizador MotorCalculo { get; }
    public string BoletoOnline { get; set; }
    public int NoGrupoPantalla { get; }
    
    // Event Handlers (Menu System)
    void MCalcular(object sender, EventArgs e)           // Start analysis
    void MAbrirCombClick(object sender, EventArgs e)     // Open combination file
    void MSalir(object sender, EventArgs e)              // Exit application
    void MAyuda(object sender, EventArgs e)              // Show help
    void mAcercaDe(object sender, EventArgs e)           // About dialog
    
    // Analysis Management
    protected void AbreCalculoColumnasFrm(bool guardaColumnas)
    protected void ObtenDatosGruposPronosticos()
    protected bool ComprobarPronosticos()
    
    // File Operations
    protected void AbreCombinacion()
    protected void GrabarCombinacion()
}
```

**Key Event Handlers**:

#### `MCalcular(object sender, EventArgs e)`
Initiates the main analysis process.

**Process**:
1. Validates current predictions
2. Configures analysis parameters
3. Opens calculation form
4. Updates UI with results

#### `MAbrirCombClick(object sender, EventArgs e)`
Opens and loads a combination file.

**Supported Formats**:
- `.comb` - Native combination format
- `.xml` - XML-based combination format

---

### ArchivoCombinacion Class - File Management API

**Namespace**: `Free1X2.EntradaSalida`  
**Purpose**: Manages combination file operations and data persistence.

```csharp
public class ArchivoCombinacion
{
    // File Operations
    public void AbrirArchivoCombinacion(string fileName)
    public void GrabarArchivoCombinacion(string fileName, string[] equipos, 
                                        string[] pronosticos, GrupoPartidos grupos)
    
    // Data Retrieval
    public string[] LeeEquipos()
    public string[] LeePronosticos()
    public GrupoPartidos LeeGruposPartidos()
    public ControladorGrupos LeeControladorGrupos()
    public string LeeArchivoColumnasFiltro()
    public ControladorIfThen LeeControladorIfThen()
    
    // Data Writing
    public void EscribeEquipos(string[] equipos)
    public void EscribePronosticos(string[] pronosticos)
    public void EscribeGruposPartidos(GrupoPartidos grupos)
}
```

**Usage Example**:
```csharp
ArchivoCombinacion archivo = new ArchivoCombinacion();
archivo.AbrirArchivoCombinacion("MiCombinacion.comb");
string[] equipos = archivo.LeeEquipos();
string[] pronosticos = archivo.LeePronosticos();
```

---

### ControladorGrupos Class - Group Management API

**Namespace**: `Free1X2.MotorCalculo`  
**Purpose**: Manages groups of matches for complex betting strategies.

```csharp
public class ControladorGrupos
{
    // Properties
    public GrupoPartidos GruposPartidos { get; set; }
    public bool EsVacio { get; }
    public bool EsActivo { get; set; }
    
    // Group Management
    public void RecalcularControladorGrupos()
    public bool AnalizaColumna(long columna)
    public void AddGrupo(Grupo grupo)
    public void RemoveGrupo(int indice)
    public Grupo GetGrupo(int indice)
    
    // Analysis Methods
    public bool CompruebaGrupo(long columna, int indiceGrupo)
    public int CalcularAciertos(long columna, int indiceGrupo)
}
```

---

### Custom Controls API

### Pronosticos Control - Prediction Input

**Namespace**: `Free1X2.UI.Controls`  
**Purpose**: Custom control for match prediction input.

```csharp
public partial class Pronosticos : UserControl
{
    // Properties
    public int NumPartidos { get; set; }
    public string NombreGrupo { get; set; }
    
    // Methods
    public string[] DevolverEquipos()
    public string this[int partido] { get; set; }  // Indexer for predictions
    public void EstablecerEquipos(string[] equipos)
    public void LimpiarPronosticos()
    public int ObtenPartidosGrupo(int grupo)
    
    // Events
    public event EventHandler PronosticoChanged;
}
```

### CtrSemaforo Control - Status Indicator

**Namespace**: `Free1X2.UI.Controls`  
**Purpose**: Traffic light-style status indicator.

```csharp
public partial class CtrSemaforo : UserControl
{
    public enum NombreEstado { Rojo, Amarillo, Verde }
    
    // Properties
    public NombreEstado Estado { get; set; }
    public string Mensaje { get; set; }
    
    // Methods
    public void CambiarEstado(NombreEstado nuevoEstado)
    public void MostrarMensaje(string mensaje, NombreEstado estado)
}
```

---

## Error Handling API

### Global Exception Handling

```csharp
// In Program.cs
public static class Program
{
    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        HandleException(e.Exception, "Application Thread Exception");
    }
    
    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        HandleException((Exception)e.ExceptionObject, "Unhandled Domain Exception");
    }
    
    private static void HandleException(Exception ex, string source)
    {
        // Log exception and show user-friendly error dialog
        LogException(ex, source);
        ShowErrorDialog(ex);
    }
}
```

---

## Configuration API

### AConfiguracion Class - Settings Management

```csharp
public class AConfiguracion
{
    public AConfiguracion(string configPath)
    
    // Configuration Reading Methods
    public void ObtenNumPartidos(ref int numPartidos, ref string[] separador)
    public void ObtenPuntosCP(ref int fijos, ref int dobles, ref int triples)
    public void ObtenDesplazamiento(ref int desplazamiento)
    public void ObtenInfoIdioma(ref string idioma)
    public void ObtenToolBarsVisibles(ref bool tsFree, ref bool tsFiltros, /* ... */)
    public void ObtenConfiguracionAnalisis(ref bool analizarTodo, /* ... */)
    
    // Configuration Writing Methods
    public void GrabarNumPartidos(int numPartidos, string[] separador)
    public void GrabarPuntosCP(int fijos, int dobles, int triples)
    public void GrabarConfiguracionAnalisis(bool analizarTodo, /* ... */)
}
```

---

## Usage Examples

### Complete Analysis Workflow

```csharp
// Initialize analyzer
Analizador analizador = new Analizador();

// Load combination file
ArchivoCombinacion archivo = new ArchivoCombinacion();
archivo.AbrirArchivoCombinacion("combination.comb");

// Set up predictions
string[] pronosticos = archivo.LeePronosticos();
for (int i = 0; i < pronosticos.Length; i++)
{
    analizador.SetPronostico(i, pronosticos[i]);
}

// Configure filters
FiltroContactos filtroContactos = new FiltroContactos();
filtroContactos.ContactosMinimos = 2;
filtroContactos.ContactosMaximos = 8;
filtroContactos.EsActivo = true;

// Run analysis on combinations
long combination = 123456789L;
analizador.AnalizaColumna(combination);

// Check results
int analyzed = analizador.NoColsAnalizadas;
int accepted = analizador.NoColsAceptadas;
```

### Custom Filter Implementation

```csharp
public class MiNuevoFiltro : IFiltro
{
    public bool EsVacio => !EsActivo || !ConfiguradoCorrectamente();
    public bool EsActivo { get; set; }
    
    // Custom properties
    public int MiParametro { get; set; }
    
    public bool CompruebaPronostico(long columna)
    {
        // Implement custom filter logic
        return AnalizeCustomPattern(columna);
    }
    
    public string ObtenInformacion()
    {
        return $"Mi Filtro: Parámetro={MiParametro}, Activo={EsActivo}";
    }
    
    public void InicializarFiltro()
    {
        // Initialize filter state
        MiParametro = 0;
        EsActivo = false;
    }
    
    private bool AnalizeCustomPattern(long columna)
    {
        // Custom analysis implementation
        return true; // Placeholder
    }
    
    private bool ConfiguradoCorrectamente()
    {
        return MiParametro > 0;
    }
}
```

---

**API Documentation Version**: 1.0  
**Last Updated**: September 30, 2025  
**Compatible with**: .NET 8.0 Version