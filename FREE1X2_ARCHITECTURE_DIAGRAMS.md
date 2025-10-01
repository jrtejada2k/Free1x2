# Free1X2 Application Architecture Diagrams

## High-Level System Architecture

```mermaid
graph TB
    subgraph "Free1X2 Application Architecture"
        subgraph "Presentation Tier"
            A[MainForm.cs<br/>Primary UI Controller]
            B[Filter Forms<br/>ContactosFrm, DistanciasFrm, etc.]
            C[Modern UI<br/>ModernMainForm, ModernBancoPruebasForm]
            D[Custom Controls<br/>Pronosticos, CtrSemaforo]
            E[Analysis Forms<br/>CalculaColumnasFrm, RentabilidadFrm]
        end
        
        subgraph "Business Logic Tier"
            F[Analizador.cs<br/>Core Analysis Engine]
            G[Filter System<br/>IFiltro implementations]
            H[ControladorGrupos<br/>Group Management]
            I[ColumnaProbable<br/>Statistical Analysis]
            J[Reduction Algorithms<br/>Mathematical Optimization]
        end
        
        subgraph "Data Access Tier"
            K[ArchivoCombinacion<br/>File Management]
            L[AConfiguracion<br/>Settings Management]
            M[ArchivoColumnas<br/>Data Persistence]
            N[EntradaSalida<br/>I/O Operations]
        end
        
        subgraph "Utility Layer"
            O[VariablesGlobales<br/>Global Configuration]
            P[CompresorZip<br/>Data Compression]
            Q[Utils<br/>Helper Functions]
            R[Compatibility<br/>Legacy Support]
        end
        
        subgraph "External Resources"
            S[(Configuration Files<br/>aidomnou.cfg, parametros.free1x2)]
            T[(Team Data<br/>equipos1.dat, equipos2.dat)]
            U[(Language Files<br/>en-US.lang)]
            V[(Combination Files<br/>*.comb, *.xml)]
        end
    end
    
    %% Connections
    A --> F
    B --> G
    C --> F
    D --> A
    E --> F
    F --> H
    F --> I
    G --> J
    H --> N
    I --> M
    J --> M
    K --> S
    L --> S
    M --> V
    N --> T
    O --> L
    P --> Q
    Q --> R
    
    %% Styling
    classDef presentation fill:#e1f5fe
    classDef business fill:#f3e5f5
    classDef data fill:#e8f5e8
    classDef utility fill:#fff3e0
    classDef external fill:#fce4ec
    
    class A,B,C,D,E presentation
    class F,G,H,I,J business
    class K,L,M,N data
    class O,P,Q,R utility
    class S,T,U,V external
```

## Component Interaction Flow

```mermaid
sequenceDiagram
    participant User
    participant MainForm
    participant Analizador
    participant FilterSystem
    participant DataLayer
    participant FileSystem
    
    Note over User,FileSystem: Application Startup
    User->>MainForm: Launch Application
    MainForm->>DataLayer: Load Configuration
    DataLayer->>FileSystem: Read Config Files
    FileSystem-->>DataLayer: Config Data
    DataLayer-->>MainForm: Initialized Settings
    
    Note over User,FileSystem: Load Combination
    User->>MainForm: Open Combination File
    MainForm->>DataLayer: Load Combination
    DataLayer->>FileSystem: Read .comb/.xml
    FileSystem-->>DataLayer: Match & Team Data
    DataLayer-->>MainForm: Combination Loaded
    
    Note over User,FileSystem: Configure Analysis
    User->>MainForm: Configure Filters
    MainForm->>FilterSystem: Set Filter Parameters
    FilterSystem-->>MainForm: Filters Ready
    
    Note over User,FileSystem: Run Analysis
    User->>MainForm: Start Analysis
    MainForm->>Analizador: Initialize Analysis
    
    loop For Each Combination
        Analizador->>FilterSystem: Apply Filters
        FilterSystem->>FilterSystem: Process Filter Logic
        FilterSystem-->>Analizador: Filter Results
        Analizador->>Analizador: Calculate Statistics
        Analizador->>DataLayer: Store Results (if needed)
    end
    
    Analizador-->>MainForm: Analysis Complete
    MainForm-->>User: Display Results
```

## Filter System Architecture

```mermaid
graph TD
    subgraph "Filter System Architecture"
        A[IFiltro Interface<br/>Base Filter Contract]
        
        subgraph "Core Filters"
            B[FiltroContactos<br/>Contact Analysis]
            C[FiltroDistancias<br/>Distance Patterns]
            D[FiltroFormatos<br/>Format Analysis]
            E[FiltroSimetrias<br/>Symmetry Patterns]
            F[FiltroInterrupciones<br/>Interruption Patterns]
        end
        
        subgraph "Advanced Filters"
            G[FiltroSignosSeguidos<br/>Consecutive Signs]
            H[FiltroPesosNumericos<br/>Numeric Weights]
            I[FiltroValoracion<br/>Valuation Analysis]
            J[FiltroNoVariantes<br/>Non-Variant Analysis]
        end
        
        subgraph "Filter Controllers"
            K[ControladorGrupos<br/>Group Management]
            L[ControladorTol<br/>Tolerance Control]
            M[ControladorIfThen<br/>Conditional Logic]
        end
        
        subgraph "Filter Data"
            N[FiltroDatosBase<br/>Base Data Handler]
            O[FContactosData<br/>Contact Data]
            P[FDistanciasData<br/>Distance Data]
            Q[FSimetriasData<br/>Symmetry Data]
        end
    end
    
    %% Interface Implementation
    A -.-> B
    A -.-> C
    A -.-> D
    A -.-> E
    A -.-> F
    A -.-> G
    A -.-> H
    A -.-> I
    A -.-> J
    
    %% Controller Management
    K --> B
    K --> C
    L --> D
    M --> E
    
    %% Data Relationships
    N --> O
    N --> P
    N --> Q
    B --> O
    C --> P
    E --> Q
    
    %% Styling
    classDef interface fill:#ffebee
    classDef core fill:#e3f2fd
    classDef advanced fill:#e8f5e8
    classDef controller fill:#fff3e0
    classDef data fill:#f3e5f5
    
    class A interface
    class B,C,D,E,F core
    class G,H,I,J advanced
    class K,L,M controller
    class N,O,P,Q data
```

## Data Flow Architecture

```mermaid
graph LR
    subgraph "Input Sources"
        A[User Predictions<br/>1, X, 2 selections]
        B[Team Data<br/>equipos*.dat files]
        C[Configuration<br/>Settings & Parameters]
        D[Historical Data<br/>Past results & statistics]
    end
    
    subgraph "Processing Pipeline"
        E[Data Validation<br/>Input verification]
        F[Combination Generation<br/>Mathematical combinations]
        G[Filter Application<br/>Pattern matching]
        H[Statistical Analysis<br/>Probability calculations]
        I[Reduction Algorithms<br/>Optimization]
    end
    
    subgraph "Output Generation"
        J[Analysis Results<br/>Statistical reports]
        K[Filtered Combinations<br/>Optimized betting slips]
        L[Performance Metrics<br/>Success predictions]
        M[Export Data<br/>File outputs]
    end
    
    %% Flow connections
    A --> E
    B --> E
    C --> E
    D --> E
    
    E --> F
    F --> G
    G --> H
    H --> I
    
    I --> J
    I --> K
    H --> L
    J --> M
    K --> M
    L --> M
    
    %% Styling
    classDef input fill:#e1f5fe
    classDef process fill:#f3e5f5
    classDef output fill:#e8f5e8
    
    class A,B,C,D input
    class E,F,G,H,I process
    class J,K,L,M output
```

## UI Component Hierarchy

```mermaid
graph TD
    subgraph "UI Component Architecture"
        A[MainForm<br/>Application Entry Point]
        
        subgraph "Primary Forms"
            B[CalculaColumnasFrm<br/>Analysis Interface]
            C[BancoPruebasFrm<br/>Test Bench]
            D[TramificarForm<br/>Stratification]
            E[RentabilidadFrm<br/>Profitability Analysis]
        end
        
        subgraph "Filter Forms"
            F[ContactosFrm<br/>Contact Filter UI]
            G[DistanciasFrm<br/>Distance Filter UI]
            H[FormatosFrm<br/>Format Filter UI]
            I[SimetriasFrm<br/>Symmetry Filter UI]
        end
        
        subgraph "Custom Controls"
            J[Pronosticos<br/>Prediction Input]
            K[CtrSemaforo<br/>Status Indicator]
            L[SignoBoletoBase<br/>Betting Slip Control]
            M[CtrlSimetria<br/>Symmetry Control]
        end
        
        subgraph "Modern UI"
            N[ModernMainForm<br/>Updated Main Interface]
            O[ModernBancoPruebasForm<br/>Modern Test Bench]
        end
        
        subgraph "Utility Forms"
            P[AyudaFrm<br/>Help System]
            Q[AcercaDeFrm<br/>About Dialog]
            R[ActualizadorFrm<br/>Update Manager]
            S[SalirFrm<br/>Exit Confirmation]
        end
    end
    
    %% Hierarchy connections
    A --> B
    A --> C
    A --> D
    A --> E
    A --> F
    A --> G
    A --> H
    A --> I
    A --> P
    A --> Q
    A --> R
    A --> S
    
    %% Control embedding
    B --> J
    B --> K
    C --> L
    F --> M
    
    %% Modern alternatives
    N -.-> A
    O -.-> C
    
    %% Styling
    classDef main fill:#ffebee
    classDef primary fill:#e3f2fd
    classDef filter fill:#e8f5e8
    classDef control fill:#fff3e0
    classDef modern fill:#f3e5f5
    classDef utility fill:#fce4ec
    
    class A main
    class B,C,D,E primary
    class F,G,H,I filter
    class J,K,L,M control
    class N,O modern
    class P,Q,R,S utility
```

## Class Relationship Diagram

```mermaid
classDiagram
    class Analizador {
        -GeneradorColumnas gc
        -string[] pronosticos
        -ControladorGrupos ctrlGrupos
        +AnalizaColumna(long columna)
        +SetPronostico(int partido, string pronostico)
        +CompruebaPronostico(long columna)
    }
    
    class ControladorGrupos {
        -GrupoPartidos gruposPartidos
        +RecalcularControladorGrupos()
        +AnalizaColumna(long columna)
        +AddGrupo(Grupo grupo)
    }
    
    class IFiltro {
        <<interface>>
        +bool EsVacio
        +bool CompruebaPronostico(long columna)
        +string ObtenInformacion()
    }
    
    class FiltroContactos {
        +bool CompruebaPronostico(long columna)
        +string ObtenInformacion()
        -AnalizeContactPattern()
    }
    
    class VariablesGlobales {
        -int numPartidos$
        -string[] separador$
        -Dictionary~string,string~ diccionarioIdioma$
        +InicializarVariables()$
        +GetConfigPath()$
    }
    
    class MainForm {
        -Analizador analizador
        -string nombreArchivoComb
        +MCalcular(object sender, EventArgs e)
        +MAbrirCombClick(object sender, EventArgs e)
        +MSalir(object sender, EventArgs e)
    }
    
    class ArchivoCombinacion {
        -XmlDocument combFile
        -string[] pronosticos
        +AbrirArchivoCombinacion(string fileName)
        +LeeEquipos()
        +LeePronosticos()
    }
    
    %% Relationships
    Analizador --> ControladorGrupos : uses
    ControladorGrupos --> IFiltro : manages
    FiltroContactos ..|> IFiltro : implements
    MainForm --> Analizador : contains
    MainForm --> ArchivoCombinacion : uses
    Analizador --> VariablesGlobales : accesses
    
    %% Styling
    classDef core fill:#e3f2fd
    classDef ui fill:#f3e5f5
    classDef data fill:#e8f5e8
    classDef interface fill:#ffebee
    
    class Analizador,ControladorGrupos core
    class MainForm ui
    class ArchivoCombinacion,VariablesGlobales data
    class IFiltro interface
```

## Deployment Architecture

```mermaid
graph TB
    subgraph "Free1X2 Application Deployment"
        subgraph "Application Executable"
            A[Free1X2.exe<br/>.NET 8.0 Windows Application]
            B[Free1X2.dll<br/>Main Application Assembly]
            C[Free1X2.dll.config<br/>Application Configuration]
        end
        
        subgraph "Dependencies"
            D[ICSharpCode.SharpZipLib.dll<br/>Compression Library]
            E[Microsoft.Windows.Compatibility<br/>.NET Compatibility Pack]
            F[System.Data.*.dll<br/>Data Access Libraries]
        end
        
        subgraph "Configuration Files"
            G[aidomnou.cfg<br/>Main Configuration]
            H[parametros.free1x2<br/>Application Parameters]
            I[imprebol.cfg<br/>Print Configuration]
            J[impresoras.cfg<br/>Printer Settings]
        end
        
        subgraph "Data Directories"
            K[Equipos/<br/>Team Data Files]
            L[Jornadas/<br/>Match Day Data]
            M[Ganadoras/<br/>Historical Winners]
            N[Combinaciones/<br/>User Combinations]
            O[Filtros/<br/>Filter Configurations]
        end
        
        subgraph "Language Support"
            P[Idioma/<br/>Language Files]
            Q[en-US.lang<br/>English Translations]
        end
        
        subgraph "Resources"
            R[imagenes/<br/>UI Images & Icons]
            S[Documentacion/<br/>Help Files & Documentation]
        end
    end
    
    %% Dependencies
    A --> B
    A --> C
    B --> D
    B --> E
    B --> F
    
    %% Configuration
    A --> G
    A --> H
    A --> I
    A --> J
    
    %% Data Access
    A --> K
    A --> L
    A --> M
    A --> N
    A --> O
    
    %% Localization
    A --> P
    P --> Q
    
    %% Resources
    A --> R
    A --> S
    
    %% Styling
    classDef executable fill:#ffebee
    classDef dependency fill:#e3f2fd
    classDef config fill:#e8f5e8
    classDef data fill:#fff3e0
    classDef resource fill:#f3e5f5
    
    class A,B,C executable
    class D,E,F dependency
    class G,H,I,J config
    class K,L,M,N,O,P,Q data
    class R,S resource
```

---

## Architecture Analysis Summary

### Strengths
1. **Clear Separation of Concerns**: Well-defined layers with specific responsibilities
2. **Modular Filter System**: Extensible filter architecture with common interface
3. **Comprehensive Configuration**: Flexible configuration management system
4. **Modern UI Options**: Both legacy and modern UI components available
5. **Robust File Handling**: Multiple file format support with error handling

### Design Patterns Implemented
1. **Strategy Pattern**: Filter system with IFiltro interface
2. **Facade Pattern**: MainForm as central UI coordinator
3. **Singleton Pattern**: VariablesGlobales for global state management
4. **Template Method**: Base classes for data handlers and UI forms
5. **Observer Pattern**: Event-driven UI updates and notifications

### Scalability Considerations
1. **Filter Extension**: Easy to add new filter types
2. **UI Modernization**: Modern UI components can gradually replace legacy forms
3. **Algorithm Enhancement**: Analysis algorithms can be improved without UI changes
4. **Data Format Support**: New file formats can be added through interface implementations

### Performance Optimizations
1. **Lazy Loading**: Configuration and data loaded on demand
2. **Efficient Algorithms**: Mathematical optimizations in reduction algorithms
3. **Memory Management**: Proper disposal of resources and large data structures
4. **Background Processing**: Analysis can run without blocking UI

**Documentation Created**: September 30, 2025  
**Architecture Version**: 1.0 (.NET 8.0 Migration)