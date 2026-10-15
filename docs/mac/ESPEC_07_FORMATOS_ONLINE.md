# ESPEC 07 — Formatos de fichero, datos iniciales, idioma y online

> Fase **M0** de [`../PLAN_MIGRACION_MAC.md`](../PLAN_MIGRACION_MAC.md). Sigue
> [`ESPEC_00_CONVENCIONES.md`](ESPEC_00_CONVENCIONES.md) (plantilla §2, trampas T1–T12) y cubre
> M-R3 («los `.txt`, `.comb`, condiciones, `parametros.free1x2`, `.dat`, `.lang`, `.xml` existentes se
> leen y escriben igual»). Objetivo: reescribir en Swift **sin mirar el C#** y leer/escribir los
> mismos ficheros byte a byte. El C# **leído** sigue siendo el oráculo (no se ejecuta: ESPEC_12).
>
> Criterio de evidencia (R4): cada afirmación lleva `fichero:línea`. «**[bytes]**» = comprobado
> inspeccionando el fichero real del repo (`od`/`python -I`). «**[.NET]**» = comportamiento del
> framework .NET 8 que el repo no puede probar (va también a §8 «Abiertas» si afecta a bytes).
> Lo no verificado **no** se afirma: está en §8.

**Alias de rutas** (para abreviar; todas relativas a la raíz del repo):

| Alias | Ruta |
|-------|------|
| `ES/` | `Free1X2.Domain/EntradaSalida/` |
| `CP/` | `Free1X2.Domain/EntradaSalida/GenerarCPs/` |
| `ON/` | `Free1X2.Domain/Online/` |
| `MC/` | `Free1X2.Domain/MotorCalculo/` |
| `UT/` | `Free1X2.Domain/Utils/` |
| `SV/` | `Free1X2.WinUI/Services/` |
| `DT/` | `Free1X2.WinUI/Datos/` |
| `VM/` | `Free1X2.WinUI/Views/Ported/` |
| `VW/` | `Free1X2.WinUI/Views/` |
| `CT/` | `Free1X2.WinUI/Controls/` |
| `TS/` | `Free1X2.Domain.Tests/` |

---

## 0. Resumen ejecutivo

### 0.1 Índice de formatos (ver §1 para la gramática de cada uno)

| ID | Formato | Ruta / extensión | Lo lee / escribe |
|----|---------|------------------|------------------|
| F01 | Configuración global (XML) | `parametros.free1x2` | `AConfiguracion` |
| F02 | Combinación completa (XML) | `*.comb` (y `*_tmp.comb`) | `ArchivoCombinacion` |
| F03 | Condición de un filtro / grupo / If-Then (XML) | `*.vx2 .seg .int .dbj .pes .valor .dist .cont .geq .fmt .123 .sim .dif .cps .if` + `*.xml` + `Temp/tmp.*` | `ArchivoCondiciones` + `F*Data` |
| F04 | Columnas de apuesta (texto) | `*.txt` / `*.cols` | `ArchivoColumnasTexto` |
| F05 | Tipos/configuración de CPs (DataSet XML) | `ColumnasProbables.xml` | `DatosHelper` (`DataSet.ReadXml/WriteXml`) |
| F06 | CPs generadas (texto) | `Condiciones/{Tipo}_j{n}.txt` | `GenerarCPsViewModel` + `IO` |
| F07 | Valoraciones/porcentajes (texto, 5 variantes + histórico) | `*.txt` | `Porcentajes` |
| F08 | Histórico L.A.E. (TSV) | `Jornadas/InfoJornadasLAE.txt` | `LAE`, `TramificarFormViewModel`, `EstimadorPremiosFrmViewModel` |
| F09 | Resultados históricos (TSV) | `Jornadas/Resultados.txt` | `EscrutiniosFrmViewModel`, `EscrutarCombinacionesFrmViewModel` |
| F10 | Equipos por categoría (texto Latin-1) | `Equipos/equipos{1,2,2b,Int}.dat` | `GestorEquiposFrmViewModel`, `AgregarEquipoFrmViewModel` |
| F11 | Traducciones (texto `CLAVE#VALOR`) | `Idioma/*.lang` | `ArchivoIdioma` |
| F12 | Catálogo de impresoras (XML) | `Impresion/impresoras.cfg` | `ControladoresImpresion` |
| F13 | Config. de impresión del boleto (`clave=valor`) | `Impresion/imprebol.cfg` | `ImprimirBoletoFrmViewModel` |
| F14 | Rangos de «aidomnou» (texto con `//`) | `aidomnou.cfg` | solo legacy WinForms |
| F15 | Notificaciones (`a#b#c`) | `Comunicaciones/comunicaciones.free1x2` | `AConfiguracion.ObtenEstadoNotificaciones` (sin llamadores) |
| F16 | Bitmap de columnas comprimido (ZIP) | `*.z3q` | `CompresorZip` |
| F17 | Equipos del boleto (texto) | `*.txt` (Archivo → Guardar/Abrir equipos) | `MainPageViewModel` |
| F18 | Jornada online (JSON) | HTTP `…/quiniela/{pais}/actual` y caché `jornada-{es,mx}.json` | `JornadaQuinielaParser`, `JornadaCache` |
| F19 | Catálogo de equipos online (JSON) | HTTP `…/equipos/{pais}` (sin caché) | `CatalogoEquiposParser` |
| F20 | Preferencia de idioma (JSON) | `idioma.json` | `IdiomaApp` |
| F21 | Preferencia de tema (JSON) | `tema.json` | `TemaApp` |
| F22 | Log (texto) | `log.txt` / `log.1.txt` | `Log` |
| F23 | Recursos WinUI (RESW/XML) | `Strings/{es,en-US}/Resources.resw` | sistema PRI (`x:Uid`) |
| F24 | Formatos de pantalla (texto) | `.lst` (2 variantes), `.cnd` (3), `.par`, `.jb7`, `.tri`, `.clm` | ViewModels (ver F24) |

**24 familias, 52 variantes** contando las sub-variantes de F03 (14 filtros + grupo + If-Then = 16), F07 (formatos 1, 3, 42, 43, 44 + histórico = 6) y F24 (9).

### 0.2 Las 3 trampas de compatibilidad que más afectan

1. **Codificación y fin de línea no son uniformes** (T8): `.dat` y `.lang` son **Latin-1** (no UTF-8) [bytes];
   el boleto `.txt` es UTF-8 **con BOM** en WinUI pero lo escribía en ANSI el legacy; XML y columnas son UTF-8 **sin** BOM;
   y `ObtenNumCols` calcula el nº de columnas como `bytes/(longitud+2)`, es decir **asume CRLF** (`ES/ArchivoColumnasTexto.cs:326-328`):
   con un `.txt` terminado en LF devuelve un valor menor (p. ej. `Fixtures/cols_a.txt` → 9 en vez de 10).
2. **Cultura** (T7): `parametros.free1x2` guarda decimales con **coma** (`PrecioApuesta="0,50"`) y se lee con
   `float.Parse(…, es-ES)` → **`Float` y después `Double`** (pierde precisión), y con `es-ES` el `.` es separador de **miles** (`"0.50"` ⇒ 50).
   Otros 10 puntos (C3–C12 de §2.1) usan la **cultura actual del SO** sin fijarla. El oráculo asume `es-ES`.
3. **XML escrito por `XmlTextWriter`**: raíz literal `<xml>`, **sin** declaración en `.comb`/condiciones, elementos vacíos como `" />"`,
   sangría de 2 espacios, sin salto final, y los lectores son **frágiles** (`Attributes["x"].Value` ⇒ `NullReference` si falta;
   `GetFiltro(id)` con `id` desconocido devuelve **el último filtro**; `values=""` en NoVariantes lanza excepción). Hay que
   replicar tanto el formato como el modo de fallo (F02, F03).

---

## 1. Catálogo de formatos

### 1.0 Convenciones y notación

- **bool** en XML: se escribe `bool.ToString()` ⇒ `True` / `False`; se lee con `Convert.ToBoolean(string)` ⇒ `bool.Parse`:
  acepta `true/True/TRUE…` (con espacios laterales) y lanza `FormatException` con cualquier otra cosa (`"1"`, `"si"`) **[.NET]**.
- **int** en XML: escritura `int.ToString()` (dígitos ASCII, `-` si negativo); lectura `Convert.ToInt16/ToInt32(string)`
  (espacios laterales y signo admitidos; desbordan con `OverflowException`).
- **cadena opaca**: atributo que el motor guarda y devuelve **tal cual** (p. ej. `grupos="0,1,2"`, `fallos="0-2"`). Se serializa
  byte a byte; su gramática semántica pertenece a la espec. del motor (ESPEC_01…04).
- `NRE` = `NullReferenceException` por atributo/elemento ausente; el lector **no** lo captura (el llamador de UI sí: muestra un diálogo).
- Todos los lectores XML usan `XmlDocument.Load(new XmlTextReader(ruta))` (`ES/AConfiguracion.cs:51-53`, `ES/ArchivoCombinacion.cs:45-46`,
  `ES/ArchivoCondiciones.cs:39-42`): detectan codificación por BOM/declaración (UTF-8 por defecto), **descartan nodos de solo
  espacios** (`PreserveWhitespace=false`) **[.NET]**, son **sensibles a mayúsculas** y navegan por `GetElementsByTagName(nombre)[0]`
  (primer descendiente con ese nombre en orden de documento) y por índice de hijo (`ChildNodes[i]`, que **incluye comentarios**).
- Todos los escritores XML usan `new XmlTextWriter(ruta, null)` con `Formatting.Indented` (`ES/AConfiguracion.cs:217-218`,
  `ES/ArchivoCombinacion.cs:302-303`, `ES/ArchivoCondiciones.cs:253-254`). Salida (ver §1.0.1).

#### 1.0.1 Serialización exacta de `XmlTextWriter` (`Indented`, codificación `null`)

| Aspecto | Regla | Evidencia |
|---------|-------|-----------|
| Codificación | UTF-8 **sin BOM** (`encoding == null` ⇒ `StreamWriter` por defecto) | **[.NET]**; coherente con `DT/parametros.free1x2` [bytes: sin BOM] |
| Declaración XML | **Solo** si el documento tiene un nodo `XmlDeclaration` (parametros y `ColumnasProbables.xml`). `.comb` y condiciones se escriben con `WriteStartElement("xml")` directo ⇒ **sin declaración** | `ES/ArchivoCombinacion.cs:320-323`; `ES/ArchivoCondiciones.cs:286-289` |
| Sangría | 2 espacios por nivel; cada elemento hijo en línea nueva | [bytes] `DT/parametros.free1x2` |
| Elemento sin contenido | `<x a="1" />` (**espacio antes de `/>`**) | [bytes] `DT/parametros.free1x2:4` |
| Elemento con solo texto (`WriteString`) | `<dibujo>3+2</dibujo>` en una línea; con texto vacío ⇒ `<dibujo />` | `ES/FDibujosData.cs:72-74`, `ES/FValoracionData.cs:96-99`; **[.NET]** |
| Atributos | orden de escritura; comillas dobles; escape `& < > "` ⇒ `&amp; &lt; &gt; &quot;`; TAB/CR/LF ⇒ `&#x9; &#xD; &#xA;`; `'` y no-ASCII **sin** escapar | **[.NET]** |
| Comentario | `<!-- texto -->` con los espacios exactamente pasados (`WriteComment(" grupo 0 es el boleto base ")`) | `ES/ArchivoCombinacion.cs:375` |
| Fin de línea | `Environment.NewLine` (**CRLF en Windows**; el repo está normalizado a LF por `.gitattributes:2` [bytes]) | **[.NET]** |
| Final de fichero | **sin** salto de línea tras el último `>`; `Close()` cierra los elementos abiertos | [bytes: `parametros.free1x2` no acaba en `\n`] |

> Política Swift propuesta: escribir **CRLF** para ficheros que se intercambian con Windows (XML, `.txt`), leer LF/CRLF/CR indistintamente.
> Decisión del dueño (R2) en §8-A1.

---

### F01 · `parametros.free1x2` — configuración global (XML)

- **Ruta**: `<BaseDir>/parametros.free1x2` (`ES/AConfiguracion.cs:35-47`); semilla en `DT/parametros.free1x2`.
  `VariablesGlobales` prueba 5 rutas candidatas (`VariablesGlobales.cs:119-146`); `new AConfiguracion()` usa `AppContext.BaseDirectory` sin pruebas.
- **Lee**: `AConfiguracion` (un getter por sección). **Escribe**: 11 `Guardar*` (`ES/AConfiguracion.cs:209-400`); **cada uno re-serializa el documento entero**
  tras modificar solo su atributo ⇒ el resto del fichero (incluidos nodos desconocidos y comentarios) se **conserva**.
- **Codificación**: lectura autodetectada; escritura UTF-8 sin BOM, sangría 2, sin salto final (§1.0.1). Declaración `<?xml version="1.0"?>` (sin `encoding`) [bytes].
  La semilla del repo: 40 líneas (39 saltos), **LF**, sin BOM, sin salto final, `€` = `E2 82 AC` [bytes].
- **Cultura**: `LAE` fija `es-ES` (lectura y escritura); `notificaciones/fecha` usa la cultura actual (vestigial). Resto: enteros/bool/cadenas.
- **Navegación**: `GetElementsByTagName(tag)[0].ChildNodes[0].Attributes[...]` — p. ej. `partidos` (externo) → `ChildNodes[0]` = `partidos` (interno).

| Ruta (hijos de `<xml>`) | Atributos | Tipo / formato | Lee | Escribe | Si falta o es inválido |
|-------------------------|-----------|----------------|-----|---------|------------------------|
| `condicionesFiltros/CP/puntos` | `fijos` `dobles` `triples` | Int32 | `:58-67` | `:209-221` | NRE |
| `configuracionONLAE/LAE/OrdenarApuestas` | `PrecioApuesta` `PorcentajeDestinadoAl14` `Recaudacion` `Moneda` | decimal con **coma** (es-ES); `Moneda` cadena | `:402-413` | `:367-385` | NRE / `FormatException` |
| `utilidades/separadorJB` | `valores` | `"15,22,29,36,48,61"` cadena opaca | `:448-453` | `:349-365` | NRE |
| `partidos/partidos` | `numero` (Int16), `separador` (`"5,9,12"`) | `separador.Split(',')` ⇒ `[String]` | `:145-159` | `:288-299` | NRE / `Overflow` |
| `desplazamiento/desplazamiento` | `valor` (Int16) | | `:160-170` | `:301-315` | **silencio** (try/catch vacío): conserva el valor previo |
| `toolbars/Activadas` | `tsFree tsFiltros tsCombinacion tsOperaciones tsArchivo tsUtilidades` | bool | `:68-98` | `:222-239` | **`true`** por atributo/nodo ausente o no parseable (`bool.TryParse`) |
| `actualizador/Opciones` | `actualizarAlInicio` | bool | `:99-107` | `:240-253` | NRE |
| `analisis/Opciones` | 20 bool: `aTodo aVX2 aSeguidos aFigurasV1X2 aDibujos aInterrupciones aSimetrias aDiferencias aFormatos aFormatos123 aDistancias aContactos aFigurasContactos aPesos aFigurasPesos aValoracion aCPs aGruposEquipos aControlGrupos aControlConjuntos` | bool | `:117-143` | `:254-286` | NRE (un atributo ausente aborta toda la carga, ver abajo) |
| `idioma/idioma` | `valor` (`"es-ES"`) | cadena | `:172-177` | `:332-346` | NRE (lectura) / silencio (escritura) |
| `salir/Opciones` | `confirmacionSalir` | bool | `:108-116` | `:387-400` | NRE |
| `notificaciones/fecha` | `valor` (`"03/02/2010"`) | `DateTime` cultura actual | `:198-208` | `:316-330` | silencio; **vestigial** (sin escritor llamado) |
| `equipos/*` (`division`) | — | texto | `:426-446` | — | **vestigial**: el nodo no existe en la semilla |

Mapeo no obvio: `aDiferencias` ⇒ `VariablesGlobales.AnalizarSimetriasII` (`VariablesGlobales.cs:100`, `ES/AConfiguracion.cs:142`).

**Carga en `VariablesGlobales`** (estático, una vez; `VariablesGlobales.cs:62-114`): orden idioma → CP → desplazamiento → partidos → toolbars →
actualizador → análisis → LAE → diccionario idioma → salir → fecha. **Cualquier excepción** ⇒ `SetDefaultValues()` (`:149-175`) que sobrescribe:
`numPartidos=14, puntos=10/15/20, desplazamiento=0, separador={"1","X","2"}, precioApuesta=0.75, porcentaje14=0.55, recaudacion=1000000, moneda="€", idioma="es-ES"`,
las 6 toolbars `true`, `analizarTodo=true`, `actualizarAlInicio=false`, `confirmacionSalir=true`, `fecha=ahora` y marca `inicializado=true`.
Los demás `analizar*` conservan lo leído hasta el fallo (o `false`). Como `SetDefaultValues` pone `inicializado=true`, **`ReinicializarVariables()` es un no-op**
tras la primera carga (`:78,174,177-180`): los cambios de configuración no se releen en la misma sesión (los puntos CP sí: `Analizador.InicializaParametros`, `MC/Analizador.cs:424-436`).
`new Grupo()` **relee el fichero** en cada construcción (`MC/Grupo.cs:57-63`) ⇒ si falta, la creación de un grupo lanza (sin captura).

Ejemplo real [bytes] (la línea `analisis` abreviada con `…`; semilla `DT/parametros.free1x2`):

```xml
<?xml version="1.0"?>
<xml>
  <condicionesFiltros>
    <CP>
      <puntos fijos="6" dobles="3" triples="1" />
    </CP>
  </condicionesFiltros>
  <configuracionONLAE>
    <LAE>
      <OrdenarApuestas PrecioApuesta="0,50" PorcentajeDestinadoAl14="12" Recaudacion="14000000" Moneda="€" />
    </LAE>
  </configuracionONLAE>
  <utilidades>
    <separadorJB valores="15,22,29,36,48,61" />
  </utilidades>
  <partidos>
    <partidos numero="14" separador="5,9,12" />
  </partidos>
  <desplazamiento>
    <desplazamiento valor="3" />
  </desplazamiento>
  <toolbars>
    <Activadas tsFree="True" tsFiltros="True" tsCombinacion="True" tsOperaciones="True" tsArchivo="True" tsUtilidades="True" />
  </toolbars>
  <actualizador>
    <Opciones actualizarAlInicio="False" />
  </actualizador>
  <analisis>
    <Opciones aTodo="True" aVX2="True" … aControlConjuntos="True" />
  </analisis>
  <idioma>
    <idioma valor="es-ES" />
  </idioma>
  <salir>
    <Opciones confirmacionSalir="True" />
  </salir>
  <notificaciones>
    <fecha valor="03/02/2010" />
  </notificaciones>
</xml>
```

**Formato numérico de `LAE`** (T3, T7):
- Lectura: `float.Parse(texto, es-ES)` ⇒ NumberStyles `Float|AllowThousands`: espacios laterales, signo, `.` = **separador de miles** (ignorado en cualquier posición), `,` = decimal, exponente `E`. El resultado es **`Single`** y se asigna a `double` (`ES/AConfiguracion.cs:407-410`): `"0,55"` ⇒ `Double(Float(0.55))` = `0.550000011920929…`.
- Escritura: `PrecioApuesta.ToString("0.00", es-ES)`; `Porcentaje`/`Recaudacion` con `Double.ToString(es-ES)` (**ida y vuelta más corta**, coma decimal, sin miles; notación científica `E+15`/`E-05` si exponente ≥15 o <−5) (`:373-375`) **[.NET]**.

---

### F02 · `.comb` — combinación completa (XML)

- **Lee**: `ArchivoCombinacion` (`ES/ArchivoCombinacion.cs:41-297`) + `ArchivoCondiciones.LeeIfThen` para el If-Then (`:89-95`). **Escribe**: `GuardaArchivo()` (`:299-318`).
  Extensión `.comb` (también se ofrece `.xml` al abrir: `VW/MainPageViewModel.cs:580`); `*_tmp.comb` en `Temp/` son combinaciones temporales (`MainPageViewModel.cs:749-754`). `.gitattributes:*.comb binary`.
- **Codificación/EOL/cultura**: §1.0.1; sin declaración XML; sin cultura (solo enteros y cadenas).
- **Raíz**: elemento literal `<xml>` (no es la declaración). Orden de escritura (`:305-315`):

| # | Elemento | Atributos | Escribe | Lee | Notas |
|---|----------|-----------|---------|-----|-------|
| 1 | `<file version="0.1" />` | `version` fijo `"0.1"` | `:330-335` | **nunca se lee** | la versión no se valida |
| 2 | `<equipos>` ⊃ N× `<partido id="i" equipos="LOCAL - VISITANTE" />` | `id` = índice 0-based (no se lee); `equipos` | `:337-349` | `LeeEquipos` `:50-62` (`ChildNodes.Count`, `Attributes["equipos"]`) | texto `"Local - Visitante"`, vacío ⇒ `"? - ?"` (`CT/BoletoBaseViewModel.cs:87-125`) |
| 3 | `<columnaBase>` ⊃ N× `<partido id="i" pronostico="1,X,2" />` | `pronostico` | `:351-363` | `LeePronosticos` `:76-87` | signos presentes en orden `1`,`X`,`2` separados por coma: `"1,X,2"`, `"1,X"`, `"X"`; ninguno ⇒ `""`. El motor lee un signo cada 2 caracteres (`UT/UtilColumnas.cs:78-92`) |
| 4 | `<filtroColumnas nombreArchivo="…" />` | ruta del filtro de columnas (cadena, puede ser `""`) | `:365-370` | `LeeFiltroColumnas` `:64-74` (ausente ⇒ `""`) | **ruta absoluta de Windows** si la hay (ver §3.1) |
| 5 | `<gruposPartidos>` | | `:372-399` | `LeeGrupos` `:147-172` | primer hijo = comentario `<!-- grupo 0 es el boleto base -->` |
| 6 | `<controlGrupos>` ⊃ N× `<control id="i" grupos="…" fallos="…" />` | `grupos`, `fallos` cadenas opacas | `:461-479` | `LeeControlesGrupos` `:183-215` | `i==0` (índice **de hijo**) = control base: se reutiliza `ControlesGrupos[0]` y `UsaControlGrupos=false` |
| 7 | `<controlConjuntos>` ⊃ N× `<control id conjuntos fallos />` | cadenas opacas | **solo si `ControlesConjuntos.Count>1`** `:438-459` | `:109-145` (ausente ⇒ nada) | `i==0` = base; el resto se añade y `UsaControlConjuntos=true` |
| 8 | `<If-Then>` | | **solo si `IfThen!=null`** `:481-524` | `ArchivoCondiciones.LeeIfThen` `ES/ArchivoCondiciones.cs:85-146` | ver abajo |

**`<grupo>`** (`:379-396` / `GetGrupoFromXmlNode` `:217-261`): atributos `id` (índice, no se lee) y `nombre` (cadena; **su lectura va en `try/catch` vacío**, `:162-168`); hijos en este orden:

1. `<partidosActivos noPartidos="1,2,…,14" />` — lista 1-based separada por comas (`MC/Grupo.cs:433-452`); vacío ⇒ ningún partido. Lectura: si `noPartidos != ""` ⇒ `PonerPartidosActivos` (`:243-246`).
2. `<filtroParcial archivo="…" />` — **solo** si `!grupo.EsGrupoBase && grupo.NombreGrupo != "0"` (`:386-389`).
3. `<condicion …>` × cada filtro con `ContieneDatos`, **en el orden de la lista del grupo**: NoVariantes, ValoracionSignos, SignosSeguidos, NoInterrupciones, Dibujos, PesosNumericos, Distancias, GruposEquipos, Contactos, FormatosSignos, ColProbables, Formatos123, Simetrias, Diferencias (`MC/Grupo.cs:94-134`; escritura `ES/DatosFiltroHelper.cs:83-137`). Contenido: §F03.
4. `<tolerancias>` ⊃ N× `<control id="i" letras="…" noAciertos="…" />` + un `<controlFallos noFallos="…" />` (`:413-435`). Lectura `PonerToleranciasGrupo` `:263-290`: `control` ⇒ `ToleranciaFiltros{Aciertos=noAciertos, LetrasTol=letras}`; `controlFallos` ⇒ `FallosPermitidos` (por defecto `""`).

El lector de `<grupo>` **ignora** el orden: despacha por `Name` (`condicion`, `partidosActivos`, `tolerancias`, `filtroParcial`), salta comentarios (`NodeType != Comment`) y el resto de nodos.

**`<If-Then>`** (`:481-524`; lectura `ES/ArchivoCondiciones.cs:85-146`):

```xml
<If-Then>
  <Condiciones>
    <Condicion id="0" If="…" Then="…" />   <!-- N≥0 -->
    <Rangos valor="…" />                    <!-- solo si N>0 -->
  </Condiciones>
  <Grupos>
    <Grupo id="0" if="1" then="2" no-if="False" no-then="False" />   <!-- M≥0 -->
    <Rangos valor="…" />                    <!-- solo si M>0 -->
  </Grupos>
</If-Then>
```
- `If`/`Then`: **líneas de texto en castellano** generadas por `ControladorIfThen.getLineaTexto` (`MC/ControladorIfThen.cs:158-263`, p. ej. `"Cantidad de Variantes: 3"`, `"Dibujo: 3+2"`) y re-interpretadas por palabras clave en `CondicionIfThen.getFiltro` (`MC/CondicionIfThen.cs:95-`); contienen la marca `(NO)` para negar (`MC/CondicionIfThen.cs:63,76`). **Cadena opaca aquí; gramática en ESPEC_01.** No localizarlas jamás.
- `if`/`then`: `Convert.ToInt16`; `no-if`/`no-then`: `Convert.ToBoolean` (`True`/`False`).
- **Lectura frágil**: el **último hijo** de `Condiciones` y de `Grupos` se interpreta siempre como `Rangos` (`:108-112`, `:128-132`); si el fichero no lo trae, el último elemento real se malinterpreta (NRE en `Attributes["valor"]`). `LeeIfThen` devuelve `null` si no hay nodo `If-Then` (`:94`).

**Peculiaridades a preservar**
- `EsGrupoBase` tras leer = *«el `<grupo>` tiene exactamente 1 hijo»* (`:223-232`: se reasigna en cada iteración; queda el valor del último índice) ⇒ en la práctica `false` para todos los grupos, incluido el 0. `ControlGrupos` lo vuelve a fijar a `true` en el análisis (`MC/ControlGrupos.cs:113,148`), pero **al re-guardar** un `.comb` recién abierto, el grupo 0 (nombre `""`, no `"0"`) saldrá con `<filtroParcial archivo="" />`. Deducción del código, no observada en un fichero real (§8).
- `LeeGrupos` crea un `GrupoPartidos` nuevo; `AddGrupo` pone cada grupo en el control libre `ControlesGrupos[0]` (`MC/GrupoPartidos.cs:37-46`) **antes** de que `LeeControlesGrupos` lo sobrescriba (orden: `CargaControladorGrupos` `:97-102` = grupos → controles de grupos → conjuntos).
- `CargaIfThen` reabre el fichero con `ArchivoCondiciones` (lectura doble).
- `GetFiltro(id)` con `id` desconocido devuelve el **último** filtro del grupo (`MC/Grupo.cs:418-433`) ⇒ se aplicaría el `F*Data` de `Diferencias` a un XML ajeno (excepción o basura).
- Bucle de escritura de condiciones (`ES/DatosFiltroHelper.cs:85,135`): `filtroDatos` se declara **fuera** del `foreach`; un filtro sin `case` reutilizaría el `F*Data` anterior.

**Ejemplo mínimo** (derivado del escritor; **no hay ningún `.comb` real en el repo**, §8):

```xml
<xml>
  <file version="0.1" />
  <equipos>
    <partido id="0" equipos="Madrid - Barcelona" />
    <!-- … 14 partidos -->
  </equipos>
  <columnaBase>
    <partido id="0" pronostico="1,X,2" />
  </columnaBase>
  <filtroColumnas nombreArchivo="" />
  <gruposPartidos>
    <!-- grupo 0 es el boleto base -->
    <grupo id="0" nombre="">
      <partidosActivos noPartidos="1,2,3,4,5,6,7,8,9,10,11,12,13,14" />
      <tolerancias>
        <controlFallos noFallos="" />
      </tolerancias>
    </grupo>
  </gruposPartidos>
  <controlGrupos>
    <control id="0" grupos="0" fallos="" />
  </controlGrupos>
  <If-Then>
    <Condiciones />
    <Grupos />
  </If-Then>
</xml>
```

---

### F03 · Condición de un filtro, de un grupo o If-Then (XML)

- **Lee**: `ArchivoCondiciones` (`ES/ArchivoCondiciones.cs`). **Escribe**: tres sobrecargas de `GuardaArchivo` (`:251-284`).
- **Extensiones** (cada pantalla de filtro ofrece la suya + `.xml`; el contenido es idéntico): `.vx2` (NoVariantes) `.seg` (SignosSeguidos) `.int` (Interrupciones) `.dbj` (Dibujos) `.pes` (Pesos) `.valor` (Valoración) `.dist` (Distancias) `.cont` (Contactos) `.geq` (GruposEquipos) `.fmt` (FormatosSignos) `.123` (Formatos123) `.sim` (Simetrías) `.dif` (Diferencias, copia en `tmp.rep`) `.cps` (ColProbables, desde el visor) `.if` (If-Then) — `VM/*FrmViewModel.cs` (`ExtensionesAbrir`/`TiposGuardar`).
  Copiar/Pegar entre pantallas usa el mismo formato en `Temp/tmp.{vx2,seg,int,dbj,pes,valor,dist,cont,geq,fmt,123,sim,rep,if}` (`VM/_Helpers/FiltroArchivoViewModelBase.cs:45-47,110-112`).
- **Tres envolturas** (raíz `<xml>` + `<file version="0.1"/>` siempre):

| Sobrecarga | Cuerpo | Lectura |
|------------|--------|---------|
| `GuardaArchivo(IFiltro)` `:262-273` | `<Condiciones>` ⊃ `<condicion …>` (uno) | `LeeCondicion()` `:77-83`: `GetElementsByTagName("Condiciones")[0]` → `GetGrupoFromXmlNode` |
| `GuardaArchivo(Grupo)` `:251-260,303-314` | `<gruposPartidos>` ⊃ `<grupo id="0" nombre="…">` ⊃ `partidosActivos`, `condicion*`, `tolerancias` | `LeeGrupos(ref Grupo)` `:53-75`: procesa **todos** los `grupo` y se queda con el **último** |
| `GuardaArchivo(ControladorIfThen)` `:275-284,329-374` | `<If-Then>` (ver F02) | `LeeIfThen()` |

- **Tolerancia al leer**: `AbrirArchivoCombinacion` devuelve `false` si el XML está mal formado (`:44-47`; nota: no cierra el lector), pero los errores de contenido (NRE, `FormatException`) **propagan**.
  `GetGrupoFromXmlNode` aquí **no** trata `filtroParcial` (`:148-177`).

#### F03.a — Contenido de `<condicion>` por filtro

Común: `<condicion id="{enum Filtro}" activa="True|False">`; `id` ∈ `NoVariantes, SignosSeguidos, Dibujos, ColProbables, PesosNumericos, ValoracionSignos, NoInterrupciones, Distancias, GruposEquipos, Contactos, FormatosSignos, Formatos123, Simetrias, Diferencias` (`EnumFiltros.cs:22-37`). Solo se escribe si `ContieneDatos`.
Lectura común: `activa` obligatorio (NRE) y `ContieneDatos=true`. Los `id=` de los **hijos** (`id="0"`…) se escriben pero **no se leen** (posición).
`values="…"` es el texto canónico que producen los *getters* del filtro (ESPEC_02/03); aquí es **cadena opaca**, salvo donde se indica.

| Filtro | Estructura (orden de escritura) | Lectura / peculiaridades | Ev. |
|--------|---------------------------------|---------------------------|-----|
| **NoVariantes** | `standard` ⊃ 3× `valores tipo="var\|equis\|doses" values` | recorre **todos** los hijos de `condicion` y, de cada uno, sus hijos (no exige `standard`); `tipo` desconocido se ignora. `values` = `n[L],n[L],…` (`n` 0..N, `L` letra de tolerancia opcional, sin espacios); **`values=""` ⇒ `IndexOutOfRange`** en `SetNoVariantes` (`MC/FiltroNoVariantes.cs:231-254`) | `ES/FNoVariantesData.cs` lee `:31-69` · escribe `:71-107` |
| **SignosSeguidos** | `standard` ⊃ 4× `valores tipo="var\|unos\|equis\|doses"`; luego 0..4 de `figurasVariantes \| figurasUnos \| figurasEquis \| figurasDoses` ⊃ N× `figura value="a-b-c"` (solo si la lista no está vacía) | despacha por `Name`. **Figura**: nibbles decimales separados por `-` ⇒ `long` (`<<4 \|`); texto = nibbles de MSB a LSB sin ceros a la izquierda, `0` ⇒ `"0"` (`UT/UtilidadesEntradasValores.cs:220-253`) | `ES/FSignosSeguidosData.cs` lee `:32-165` · escribe `:167-292` |
| **Interrupciones** | `standard` ⊃ 10× `valores tipo=` `global,var,unos,equis,doses,globalSeg,varSeg,unosSeg,equisSeg,dosesSeg` | **compat. 0.23**: si no aparece ningún `*Seg`, los 5 `Seg` se rellenan con `"0,1,…,14"` (`:61,125-134`) | `ES/FInterrupcionesData.cs` lee `:35-134` · escribe `:136-217` |
| **Dibujos** | N× `dibujo` con texto `"nX+n2"` (p. ej. `3+2`) | **todos** los hijos (¡también comentarios!) se leen con `InnerText`; luego `dibs[nX,n2]=true` (`MC/FiltroDibujos.cs:113-135`) | `ES/FDibujosData.cs` lee `:32-47` · escribe `:49-77` |
| **PesosNumericos** | `standard` ⊃ 5× `valores tipo="global\|variantes\|unos\|equis\|doses" values="1,2,3"`; `tolerancias noTolerancias="…"` ⊃ 5× `valores` (mismos `tipo`); `figuras` ⊃ N× `figura value` | `values` se parsea con `GetValores`: `""` ⇒ `null` (se omite), si no `Convert.ToInt32` por elemento (`ES/FiltroDatosBase.cs:33-50`). Despacha `standard/tolerancias/figuras` por `Name` | `ES/FPesosNumericosData.cs` lee `:32-154` · escribe `:156-266` |
| **ValoracionSignos** | atributos extra en `condicion`: `tipoValoracion valorGlobal valorUnos valorEquis valorDoses`; N× `valores id="i"` con **texto** `"v1,vX,v2"` | texto: decimales con **punto**; lectura `Convert.ToDouble(x.Replace(".", ","))` con **cultura actual** (⇒ solo correcta con coma decimal); escritura `double.ToString().Replace(",", ".")` (T7). Atributos extra obligatorios (NRE) | `ES/FValoracionData.cs` lee `:29-60` · escribe `:62-102` |
| **Distancias** | `standard` ⊃ 4× `valores tipo="var\|unos\|equis\|doses"` | solo atiende `standard` | `ES/FDistanciasData.cs` lee `:35-87` · escribe `:89-132` |
| **GruposEquipos** | 5 comentarios, uno tras otro, con estos textos (espacio inicial y final incluidos): `<!-- Para cada partido:-->`, `<!-- 0 => ningun equipo seleccionado-->`, `<!-- 1 => solo equipo casa seleccionado-->`, `<!-- 2 => solo equipo fuera seleccionado-->`, `<!-- 3 => equipos casa y fuera seleccionados-->` (el texto pasado a `WriteComment` empieza por un espacio y **no** termina en espacio, `ES/FGruposEquiposData.cs:143-147`); N× `grupoEquipos` ⊃ `equipos value`, `victorias value`, `empates value`, `derrotas value`, `sumaPuntos value`; M× `Relacion1` ⊃ `ge values`, `victorias values`, `empates values`, `derrotas values`, `puntos values` | `Name.ToUpper()` (`GRUPOEQUIPOS`, `RELACION1`; cultura actual, usar mayúsculas ASCII). `equipos value`: **1 carácter `0..3` por partido** (`ObtenEquipos` = copia de `char`). `CalcularLongPronosticos()` tras leer | `ES/FGruposEquiposData.cs` lee `:33-129` · escribe `:131-250` |
| **Contactos** | `standard` ⊃ 10× `valores tipo="1X\|12\|X2\|11\|XX\|22\|1V\|XV\|2V\|VV"`; `figuras` ⊃ N× `figura value` | despacha por `Name` | `ES/FContactosData.cs` lee `:35-137` · escribe `:139-235` |
| **FormatosSignos** | N× `grupoFormato id lineas global` ⊃ M× `lineaFormato id formato rango` (**sin** `standard`) | itera **todos** los hijos de `condicion`; `lineas`, `global`, `formato`, `rango` cadenas opacas | `ES/FFormatosSignosData.cs` lee `:33-70` · escribe `:72-118` |
| **Formatos123** | un único `grupoformato id="0" aciertos repeticiones valoracion` ⊃ N× `formatos id formato aciertosMin aciertosMax` | `aciertos` `"a,b,c"` (vacío ⇒ `"0"`); `repeticiones` bool; `valoracion` = **42 decimales** separados por `/` (14×3, **fijo 14 partidos**), `Convert.ToDouble` / `double.ToString()` con **cultura actual** (T7); al leer se deriva `ValoracionTranformada` (byte[14,3] con 4/2/1 por orden) | `ES/FFormatos123Data.cs` lee `:18-123` · escribe `:125-200` |
| **Simetrias** | un único `gruposimetrias id="0" aciertos` ⊃ N× `simetrias id simetria` | `aciertos` vacío ⇒ `"0"`; `simetria` cadena opaca (`Simetria.Partidos`) | `ES/FSimetriasData.cs` lee `:16-47` · escribe `:49-100` |
| **Diferencias** | N× `grupoDiferencias id` ⊃ M× `partidos id valores` y luego un `aciertos aciertosV aciertosX aciertos2 aciertosDib aciertosInt aciertosFormatos` | **orden relevante**: `aciertos` usa `PartidosSimetricos.Count+1` de los `partidos` ya leídos (`ES/FDiferenciasData.cs:33-38`). Listas `"0,1,3-5"` (`UT/UtilidadesEntradasValores.cs:115-181` admite rangos `a-b`); `""` ⇒ `null` ⇒ «no analizar» (`AnalizaVX2Dib/Interrupciones/Formatos=false`, resto todo-true). Escritura: `ObtenerTextoFromBool` = índices `true` separados por coma. Parseo erróneo ⇒ lista vacía ⇒ `ArgumentOutOfRange` (`UT/UtilidadesEntradasValores.cs:261-263`) | `ES/FDiferenciasData.cs` lee `:14-109` · escribe `:111-185` |
| **ColProbables** | ver F03.b | | `ES/FColProbablesData.cs` lee `:32-95` · escribe `:372-712` |

> Las 14 clases `F*Data` son DTO sin estado de dominio: leen el XML a un `IFiltro` y viceversa. Nombre → clase en `ES/DatosFiltroHelper.cs:29-81`.

#### F03.b — `<condicion id="ColProbables">`

Orden de escritura (`ES/FColProbablesData.cs:384-388`): `cp*`, `relacion1*`, `relacion2*`, `fallosCP*` + un `controlFallos`, `relacion3*`.
El lector despacha por `Name.ToUpper()` (`CP RELACION1 FALLOSCP CONTROLFALLOS RELACION2 RELACION3`, `:54-84`); **`relacion2`/`relacion3` necesitan que sus `cp` ya estén leídos** (usan la lista acumulada, `:75-84,206-212`).

| Elemento | Atributos / hijos | Lectura |
|----------|-------------------|---------|
| `cp id` | `pronostico values="1X,,,X,1X2,,X2,,,"` (14 tokens separados por coma; token vacío = sin signo); `standard` ⊃ `aciertos`, `aciertosSeguidos`, `fallosSeguidos`, `puntos` (`values` opacos); `tolerancias noTolerancias` ⊃ `aciertosTol`, `aciertosSeguidosTol`, `fallosSeguidosTol` **solo si `ToleranciaLocalActiva`** | `ObtenCP` `:97-121`; `ObtenPronosticosCP` `:342-370` convierte `"1X"`⇒`"1,X"`; escritura quita las comas internas y une con `,` (`:640-659`) |
| `relacion1 id` | `columnas values`, `sumas values`, `recorridos values`, `gruposCP cantidadCP noAC` | `ObtenRelacionCP1` `:123-152` |
| `relacion2 id` | `columnasA`, `columnasB`, `aciertos`, `concepto`(`value`), `cantidad`(`value`), `columnasA2`, `columnasB2`, `aciertos2`, `concepto2`, `cantidad2` | `ObtenRelacionCP2` `:154-204`; `columnas*` se re-parsean con `ObtenerListaFromTxt`, `aciertos*` con `ObtenerListaFromTxtAciertos` |
| `fallosCP id` | `columnas values`, `tolerancias values`, `aciertos values` | `ObtenCPCrtlFallos` `:264-290` |
| `controlFallos noFallos` | cadena | último gana |
| `relacion3 id` | `columnas`, `sandwichs`, `escalerasAsc`, `escalerasDesc`, `escaleras`, `concepto` (todos `values`), `agrupacionesPasoFijo values="a+n+b#…"`, `agrupacionesSolapadas values="…"` | `ObtenRelacionCP3` `:206-262`; agrupaciones separadas por `#`, cada una `elementos+n+aciertos` (3 partes) con `n < nCP+1` y `≤15` aciertos, si no se descarta; si no queda ninguna ⇒ `null`; escritura omite elementos `null` y quita el último `#` |

**Peculiaridad a preservar** (`:714-722`): `ObtenerColumnasImplicadasRelCP3` toma `grupoCP[i]` con `i` = posición en la lista de **índices** (no el índice indicado): usa siempre las **primeras N** CPs.

**Ejemplo mínimo** — fichero de condición de NoVariantes (derivado del escritor):

```xml
<xml>
  <file version="0.1" />
  <Condiciones>
    <condicion id="NoVariantes" activa="True">
      <standard>
        <valores tipo="var" values="0,1,2,3,4,5,6,7,8,9,10,11,12,13,14" />
        <valores tipo="equis" values="0,1,2,3,4,5,6,7,8,9,10,11,12,13,14" />
        <valores tipo="doses" values="0,1,2,3,4,5,6,7,8,9,10,11,12,13,14" />
      </standard>
    </condicion>
  </Condiciones>
</xml>
```

---

### F04 · Columnas de apuesta — `*.txt` (texto)

- **Lee/escribe**: `ArchivoColumnasTexto` (`ES/ArchivoColumnasTexto.cs`), interfaz `IArchivoColumnas` (`ES/IArchivoColumnas.cs:24-49`). Es el formato de intercambio de todos los flujos (filtro de columnas, cálculo, reducción, escrutinio, operaciones, ganadoras, histórico L.A.E.). `.cols` se ofrece solo como alias de apertura (`VM/AnalizarFicheroFrmViewModel.cs:58`).
- **Gramática**: una columna por línea; cada línea = `n` signos `1`,`X`,`2` (14 ó 15), **con o sin comas** (`1,X,2,…`).
  El lector `LeeColumnaSinComas` devuelve `Trim().ToUpper()` **conservando** las comas (`:214-217`); `LeerTodasColsSeparadasPorComas` las quita (`:243-257`); `LeerTodasColsANumero`/`ABitArray` esperan la forma sin comas (`:259-288`).
- **Codificación**: escritura **UTF-8 sin BOM** (`new UTF8Encoding(false, true)`, buffer 64 KiB) (`:63-68`), `WriteLine` ⇒ `\r\n` en Windows; lectura `StreamReader(ruta, UTF8, detectBOM=true, 64 KiB)` (`:191`): `\r`, `\n` y `\r\n` válidos como fin de línea; detecta BOM UTF-8/16/32.
- **Escritura**: `GuardarCols(string)` quita **todas** las comas (`:70-80`); `GuardarColsComa(string)` escribe tal cual (`:92-102`); `GuardarCols(int)` convierte con `ConvertidorDeBases.ConvNumAColumna` (`:82-87`); **`GuardarCols(long)` es un no-op** («Implementar», `:88-91`; preservar). `GuardarTodasCols(BitArray)` recorre los bits a `true` y convierte el índice (con `numPartidos` si ≠0) (`:156-181`). La primera escritura crea/trunca el fichero y pone `NumColumnas=0`.
- **`ObtenNumCols`** (`:314-340`): si el fichero existe y tiene primera línea ⇒ `archivo.Length / (longitud(Trim(primera línea)) + 2)` (**división entera de `long`**; el `+2` supone `\r\n`); si no, 0. Se invoca **en el constructor** (`:50,56`) y deja `NumColumnas`. `ObtenNumSignos` = longitud de la primera línea recortada (`:342-366`).
- **Tolerancia**: línea vacía intermedia ⇒ columna `""` (rompe conversiones numéricas); fichero inexistente al leer ⇒ excepción de `StreamReader`; `LeeColumna()` sobre fin de fichero ⇒ NRE (`:204-206`); `DarFormatoColumna` devuelve `""` si la línea **ya trae comas** (`:290-312`, preservar) y `LeeColumna` aplica `ToUpper()` (cultura actual; usar mayúsculas ASCII).
- **Ejemplo real** [bytes] `TS/Fixtures/cols_a.txt` (150 B, 10 líneas, **LF**, sin BOM; `ObtenNumCols` ⇒ 150/16 = **9**; con CRLF serían 160 B ⇒ 10):

```
21122X1X111122
1X1X12X1X111X1
12122X1XX211X1
```

---

### F05 · `ColumnasProbables.xml` — tipos y configuración de CPs (DataSet)

- **Ruta**: `<BaseDir>/ColumnasProbables.xml` (`CP/DatosHelper.cs:16-17`). **Lee**: `DataSet.ReadXml(ruta, XmlReadMode.Auto)` solo si existe (`:24-26`; la primera vez no existe). **Escribe**: `DataSet.WriteXml(ruta)` (`:34`). DataSet tipado autogenerado `ColumnasProbables` (`CP/ColumnasProbables.cs:23-`), esquema `CP/ColumnasProbables.xsd` (solo diseño; `.xsx` es el layout del diseñador).
- **Esquema lógico**: espacio de nombres `http://tempuri.org/ColumnasProbables.xsd`; atributos **cualificados**; nombres de elementos/atributos con espacios codificados `_x0020_`.

| Tabla (elemento) | Columnas (atributos, orden) | Restricciones |
|------------------|-----------------------------|---------------|
| `Tipos de CPs` (`Tipos_x0020_de_x0020_CPs`) | `Tipo` int (clave única, no nula), `Nombre` string | `CP/ColumnasProbables.cs:278-287` |
| `Configuracion de CPs` (`Configuracion_x0020_de_x0020_CPs`) | `Tipo` int, `Desde` int, `Hasta` int, `Forzar Fijos` bool, `Num Fijos` int, `Forzar Dobles` bool, `Num Dobles` int, `Forzar Triples` bool | FK `Configuración` → `Tipos.Tipo` (cascade) `:145-160`; columnas **todas** nulables salvo la clave |

- **Salida de `WriteXml`** [bytes de `DT/ColumnasProbables.xml`]: `<?xml version="1.0" standalone="yes"?>`, raíz `<ColumnasProbables xmlns="…">`, tablas **planas** (relación no anidada) en orden de tablas y filas, solo atributos **no nulos**, prefijo `d2p1` redeclarado en cada elemento, booleanos en minúscula (`true/false`), sangría 2:

```xml
<?xml version="1.0" standalone="yes"?>
<ColumnasProbables xmlns="http://tempuri.org/ColumnasProbables.xsd">
  <Tipos_x0020_de_x0020_CPs d2p1:Tipo="1" d2p1:Nombre="Normal" xmlns:d2p1="http://tempuri.org/ColumnasProbables.xsd" />
  <Configuracion_x0020_de_x0020_CPs d2p1:Desde="0" d2p1:Hasta="100" d2p1:Forzar_x0020_Fijos="true" d2p1:Num_x0020_Fijos="14" xmlns:d2p1="http://tempuri.org/ColumnasProbables.xsd" />
  <Configuracion_x0020_de_x0020_CPs d2p1:Tipo="1" d2p1:Desde="1" d2p1:Hasta="100" d2p1:Forzar_x0020_Fijos="true" d2p1:Num_x0020_Fijos="14" xmlns:d2p1="http://tempuri.org/ColumnasProbables.xsd" />
</ColumnasProbables>
```
  Semilla: 626 B, 6 líneas, **LF**, sin BOM, sin salto final [bytes]. Obsérvese que la primera fila de configuración **no tiene `Tipo`** (nulo) [bytes]. Los atributos van **antes** de `xmlns:d2p1` en cada elemento.
- **Uso**: `CPs.CrearCPs` accede a la fila por **posición de columna** (`Tipo`=0, `Desde`=1, `Hasta`=2, `Forzar Fijos`=3, `Num Fijos`=4, `Forzar Dobles`=5, `Num Dobles`=6, `Forzar Triples`=7) (`CP/CPs.cs:24-31`) tras filtrar `RowFilter="Tipo = N"` (`VM/GenerarCPsViewModel.cs:118-120`). Editor: `VM/ConfigCPsFrmViewModel.cs:91-201`.
- **Tolerancia**: fichero ausente ⇒ DataSet vacío (sin filas ⇒ no se genera nada). Valores `null` en `Forzar*` se tratan como `false` (`CPs.cs:42-43`: `ToString().Length>0`).

---

### F06 · CPs generadas — `Condiciones/{Nombre}_j{jornada}.txt`

- **Escribe**: `GenerarCPsViewModel.Generar` (`VM/GenerarCPsViewModel.cs:84-136,279-302`) con `IO.GuardarTexto`; un fichero por fila de `Tipos de CPs`. **El `Replace(" ", "_")` del nombre no se asigna** (literal del legacy, `VM/GenerarCPsViewModel.cs:125`): el nombre conserva los espacios.
- **Gramática**: `LlenarTxtColumnas` emite, por cada CP (columna `i` de la matriz), una línea con los 14 tokens separados por `,` y terminada en `"\r\n"` **explícito** (`VM/GenerarCPsViewModel.cs:279-300`); `IO.GuardarTexto` añade además `WriteLine` ⇒ el fichero **acaba en `\r\n\r\n`** (una línea vacía final) (`CP/IO.cs:23-31`). Token = subconjunto ordenado de `1`,`X`,`2` (`"1X"`, `"X2"`, `"1X2"`, …) o `" "` (un espacio) si no hay signo (`columnas[i,j] ?? " "`, `VM/GenerarCPsViewModel.cs:267`).
- **Codificación**: `File.CreateText` ⇒ UTF-8 sin BOM (`CP/IO.cs:27`).
- Los demás miembros de `IO` (`LeeColumna`, `DarFormatoColumna`, `ColumnaAPartidos`, `ContarValoracion`, `LeeLineaValoracion`, `CP/IO.cs:47-157`) **no tienen llamadores** (solo `new IO` + `GuardarTexto` + `Cerrar`, `VM/GenerarCPsViewModel.cs:126-128`; legacy `Free1X2/UI/GenerarCPs.cs:378`): código muerto, **no portar**.

---

### F07 · Valoraciones / porcentajes — `*.txt`

- **Lee**: `UT/Porcentajes.cs:81-322` (`new Porcentajes(ruta)` detecta el formato). **Escribe**: `Guardar` (`:354-400`) y `GuardarValoraciones` (`:401-498`).
- **Cultura (T7)**: todas las lecturas hacen `texto.Replace(".", ",")` y `Convert.ToDouble` con la **cultura actual** (⇒ correcto solo con coma decimal: oráculo `es-ES`); `EsNumero` usa `Convert.ToDouble` **sin** `Replace` (`:324-328`). Escritura: `double.ToString()` (cultura actual) + `Replace(",", ".")` (formatos 3 y 42), `ToString()` crudo (formato 1), `Replace(".", ",")` (histórico).
- **Detección** (`TestFichero` `:288-310`): lee la **primera línea** recortada y prueba separador `,` ⇒ ` ` ⇒ TAB; `TestSeparador` (`:248-286`) clasifica por nº de campos, **todos numéricos** salvo los de prefijo:

| Formato (`FormatoFichero`) | Forma de la línea | Campos | Lector |
|----------------------------|-------------------|--------|--------|
| **1** | un valor por línea (3·N líneas, orden 1,X,2 por partido) | 1 | `:102-119` |
| **3** | `v1 vX v2` por partido (N líneas) | 3 | `:121-148` |
| **42 / 45** | una línea con 42 valores (14×3); 45 admite 3 extra que se ignoran | 42/45 | `:149-176` |
| **43 / 46** | `descripción v1 vX v2 …` (1+42) | 43/46 | `:177-212` (filtra por `_DescripcionBuscada`) |
| **44 / 47** | `temporada jornada v1…` (2+42) — **histórico por jornada** | 44/47 | `:213-247` (filtra por `_Temporada`+`_Jornada`) |

  `TestSeparador` solo devuelve 3, 42, 43 ó 44; las líneas con 45/46/47 campos se aceptan en esos mismos formatos (los 3 campos extra se ignoran) y los `case 45/46/47` de `Leer` (`:92-97`) no son alcanzables desde `TestFichero`. Separador: `,`, espacio o TAB según la primera línea. `LeerLinia` hace `Trim()`; sus `Replace` no se asignan (`:317-319`, no-ops).
- **Errores**: formato no reconocido ⇒ diálogo `"Fichero Incorrecto"`; valor inválido ⇒ `"El fichero seleccionado no es de valoraciones o está corrupto"` (`UserDialogs.ShowError`) y se abandona la lectura (los valores ya leídos quedan).
- **Histórico (formato 44, escritura)** `GuardarValoraciones(nombre, sep, valores, temporada, jornada)` (`:422-481`): lee el fichero con `ArchivoColumnasTexto` (cada línea `Trim().ToUpper()`), reemplaza la línea cuya `temporada`/`jornada` coincida o añade al final, y reescribe **todo** con `GuardarColsComa` (UTF-8 sin BOM, CRLF). `MontaLinea` (`:482-498`): `temporada{sep}jornada` + por cada partido y signo `{sep}{valor.ToString().Replace(".", ",")}`.
- **Portapapeles**: `FormatearParaPortapapeles` = 14 líneas `v1\tvX\tv2` unidas por `\r\n` (sin final) con `ToString()` de cultura actual (`:505-517`).
- `IO.LeeLineaValoracion` (`CP/IO.cs:132-157`) es código muerto (sin llamadores); sus `Replace(".", ",")` no se asignan (strings inmutables) y por tanto `Convert.ToDouble` actuaría sobre el texto original con la cultura actual. No portar.

**Ejemplo (formato 3, derivado del lector)**: `1,5 2,0 3,5` por línea (separador espacio, coma decimal).

---

### F08 · `Jornadas/InfoJornadasLAE.txt` (TSV)

- **Escribe**: `UT/LAE.cs:GrabarJornada` (`:77-118`) y `TramificarFormViewModel.cs:1040-1090`; **lee**: `EstimadorPremiosFrmViewModel.cs:262-340`. Se accede mediante `ArchivoColumnasTexto` ⇒ cada línea `Trim().ToUpper()` y se reescribe **el fichero entero** con `GuardarColsComa` (UTF-8 sin BOM, CRLF).
- **Gramática**: 9 campos separados por TAB (`(char)9`): `[0]` columna premiada; `[1]` temporada; `[2]` jornada **de 2 dígitos** (`PadLeft(2,'0')`, `UT/LAE.cs:35`); `[3]` recaudación en **nº de apuestas** (`Recaudacion/PrecioApuesta`); `[4..8]` premios por categoría 14,13,12,11,10 (`UT/LAE.cs:120-139`). Decimales: `double.ToString().Replace(".", ",")` ⇒ coma (`"0,00"`, `"1234,5"`); lectores usan `ParseEsp` (cultura actual y, si falla, invariante con `,`→`.`; `EstimadorPremiosFrmViewModel.cs:352-359`).
- Clave de registro: `[1]`+`[2]`; si existe se **reemplaza** la línea, si no se **añade** al final. `Tramificar` usa temporada `"{inicio}/{fin}"` (`:1058`).
- Tolerancia: línea con <9 campos se ignora al leer (`VM/EstimadorPremiosFrmViewModel.cs:295`); en `LAE.GrabarJornada` una línea con <3 campos lanza `IndexOutOfRange` (`UT/LAE.cs:98`).
- Cálculo: `Premios[i] = Math.Round(Recaud/Precio*Pct[i]/100/Acertantes[i] + 0.005, 2)` (`UT/LAE.cs:141-151`; redondeo bancario, T4; **división por cero** ⇒ `Infinity`/`NaN` en `double`).

### F09 · `Jornadas/Resultados.txt` (TSV)

- **Lee**: `VM/EscrutiniosFrmViewModel.cs:318-370`, `VM/EscrutarCombinacionesFrmViewModel.cs:108-150`; el fichero **no se distribuye** (no está en `DT/`); si no existe ⇒ lista vacía, sin error.
- **Gramática**: `Temp<TAB>Jorn<TAB>Quiniela` — `Temp` y `Jorn` `Int16` (`Convert.ToInt16`), `Quiniela` ≤15 caracteres (`MaxLength=15`). Líneas con <3 campos se saltan. Las temporadas se listan de la **última fila a la primera** (`Rows[nr-1]` → `Rows[0]`, decrementando) ⇒ se supone fichero **ordenado por temporada ascendente** (`:359-362`).
- Codificación: `File.ReadLines` (UTF-8 con BOM autodetectado); cualquier excepción de parseo se traga (`:366-369`).

---

### F10 · `Equipos/equipos{1,2,2b,Int}.dat` (texto Latin-1)

- **Rutas**: `Equipos/equipos1.dat` (1ª), `equipos2.dat` (2ª), `equipos2b.dat` (2ªB), `equiposInt.dat` (Internacionales) (`VM/GestorEquiposFrmViewModel.cs:37-40`). Semilla en `DT/Equipos/` (20 / 22 / 80 / 73 líneas [bytes]).
- **Lee**: `StreamReader(ruta, Encoding.Latin1)` línea a línea mientras `Peek()!=-1` (`:230-251`); **escribe**: `StreamWriter(ruta, false, Encoding.Latin1)` + `WriteLine` por equipo, creando el directorio (`:336-348`; igual en `VM/AgregarEquipoFrmViewModel.cs:103-139`).
- **Codificación**: **ISO-8859-1** sin BOM [bytes: p. ej. `Almería` = `41 6C 6D 65 72 ED 61`; los 4 ficheros son UTF-8 inválido]. Fin de línea: la semilla del repo es **LF** [bytes]; el escritor emite `\r\n`. El lector acepta ambos. Caracteres > U+00FF se escriben como `?` (reemplazo por defecto del codificador Latin1) **[.NET]** ⇒ un catálogo online con caracteres fuera de Latin-1 se **degrada** al guardar.
- **Gramática**: un nombre de equipo por línea; sin cabecera; sin comentarios; las líneas vacías se conservan como equipos vacíos; orden = orden del fichero (la semilla está ordenada alfabéticamente, el código **no** ordena).
- **Reglas de lista** (UI, no de formato): duplicado = `OrdinalIgnoreCase` sobre `Trim()` (`GestorEquiposFrmViewModel.cs:206-223`); importar online solo añade (`:159-170`); nada se escribe hasta «Guardar» (`:314-331`).
- Ejemplo [bytes] `DT/Equipos/equipos1.dat`: `Almería⏎At. Madrid⏎Ath. Bilbao⏎Barcelona⏎Betis⏎Dep. Coruña⏎…` (la `í` es el byte `ED`).

### F11 · `Idioma/*.lang` (texto `CLAVE#VALOR`)

- **Ruta**: `Idioma/{idioma}.lang` (`ES/ArchivoIdioma.cs:14-16`); el nombre sin extensión debe **coincidir exactamente** con `<idioma valor>` de F01 (`:21`). Única semilla: `DT/Idioma/en-US.lang`. `es-ES` no tiene fichero (es el texto base).
- **Lee**: `ObtenerIdioma` (`:11-45`): `StreamReader(ruta, Encoding.Default)`, por línea `Split('#')`; `diccionario[linea[0]] = linea[1]` **si la clave no existe** (**gana la primera**). Cualquier excepción (p. ej. línea vacía ⇒ `linea[1]` fuera de rango) **corta la lectura sin error**, conservando lo ya leído (`:23-39`). `ObtenerListaDeIdiomasDisponibles` devuelve los nombres de `*.lang` o **`null`** si no hay ninguno (`:46-60`). Si falta la carpeta `Idioma/`, `DirectoryInfo.GetFiles` lanza (`:16`, **fuera** del `try`) ⇒ §8-A21.
- **Codificación**: `Encoding.Default` = **UTF-8 en .NET 8** (T8), pero el fichero es **Latin-1** [bytes: `Ó`=`D3`, `Á`=`C1`; UTF-8 inválido] (heredado de .NET Framework/ANSI). En .NET 8 los acentos se leen como `U+FFFD` ⇒ las claves acentuadas **no coincidirían**. Swift: leer como **ISO-8859-1**. EOL de la semilla: LF [bytes].
- **Gramática**: `CLAVE_EN_MAYÚSCULAS#Traducción`, una por línea; sin comentarios. La clave es el texto español del control en mayúsculas (`Text.ToUpper()`). Un segundo `#` trunca el valor (`linea[1]`).
- **Contenido de la semilla** [bytes]: 143 líneas ⇒ **142 claves únicas**; 1 duplicada (`ANALIZAR`: gana `Analize`, se ignora `Analyze`); **14 valores vacíos** (sin traducir: `ACIERTOS`, `ANÁLISIS DE COLUMNAS`, `ANÁLISIS DE FALLOS`, `ANÁLISIS DE SIGNOS`, `COLS PROBABLES`, `COMBINAR FILTROS`, `CONTROL DE CONJUNTOS`, `EL PRIMER PROGRAMA DE QUINIELAS LIBRE Y GRATUITO`, `FILTRO COINCIDENCIAS`, `GENERADOR COLUMNAS PROBABLES`, `ORDENAR POR PROBABILIDAD`, `RECAUDACIÓN`, `SIMULADOR DE ESCRUTINIOS`, `TRAMIFICAR`); 1 clave no mayúscula (`Figuras`); 1 valor con espacio final (`Analysis of Combination Groups `).
- **¿Se aplica?** No: `FormulariosHelper.Traducir` está **comentado** (`Free1X2/UI/FormulariosHelper.cs:30-58`) y `Free1X2.WinUI` no referencia `DiccionarioIdioma` (búsqueda sin resultados). Ver §4.
- Ejemplo [bytes]: `ABRIR COMBINACIÓN#Open Combination⏎ACERCA DE#About⏎ACIERTOS#⏎` (la `Ó` es `D3`).

### F12 · `Impresion/impresoras.cfg` (XML)

- **Lee**: `MC/ControladoresImpresion.cs:26-61` (`XmlTextReader` + `GetElementsByTagName("impresoras")[0]`); no se escribe desde código (solo a mano; cabecera «No editar…»). Semilla `DT/Impresion/impresoras.cfg`: 18 impresoras, 26 líneas, LF, sin salto final [bytes].
- **Gramática**: `<xml><impresoras><Controladores modelo="…" ms="N" mi="N" girar="true|false"/>…</impresoras></xml>`; `ms`/`mi` `Int32` (margen superior / izquierdo), `girar` `bool`; **todos los hijos** de `impresoras` se leen (un comentario interior fallaría). `impresoras.Contains(controlador)` compara por referencia (no hay `Equals`) ⇒ **no deduplica** (`:43-46`; `MC/ControladorImpresion.cs`). El XML puede llevar comentario antes de la raíz (permitido).
- Ejemplo [bytes]: `<Controladores modelo="HP OFFICEJET 5510" ms="102" mi="32" girar="false" />`.

### F13 · `Impresion/imprebol.cfg` (`clave=valor`)

- **Lee/escribe**: `VM/ImprimirBoletoFrmViewModel.cs:275-355`. 4 líneas fijas **en este orden**: `margen superior=N`, `margen izquierdo=N`, `modelo=TEXTO`, `girar=si|no`. Escritura con `StreamWriter` (UTF-8 sin BOM, CRLF); `N` = `(int)valor` entero. Lectura `File.ReadAllLines`: exige ≥4 líneas; valor = lo que sigue al **primer `=`**; `double.TryParse` (cultura actual) con valor previo por defecto; `girar=="si"`.
- Semilla [bytes]: `margen superior=100⏎margen izquierdo=44⏎modelo=Por Defecto⏎girar=no⏎` (LF, termina en salto).

### F14 · `aidomnou.cfg`

- **Solo legacy**: `Free1X2/UI/Aidomnou.cs:156,181-224`; el port WinUI **no lo lee ni lo escribe** (`VM/GeneraPimViewModel.cs:24,116`). Semilla `DT/aidomnou.cfg` [bytes]: UTF-8 (acentos `C3 xx`), LF, 10 líneas.
- **Gramática**: 4 líneas de cabecera descartadas; luego `L0   // texto`, `L1,L2,L3   // texto`, línea comentario, `L4   // texto`, `L5,L6,L7   // texto`, línea comentario. El lector hace `Substring(0, IndexOf("//"))` y `Convert.ToInt32` (con los espacios finales, que `Convert` admite). El escritor (`SetConfig`) emite textos fijos y `{lims}   // …`. `GetConfig` termina con `return false` siempre (`:208`) y el llamador hace `if (!GetConfig()) SetConfig();` (`:156`) ⇒ el legacy **reescribe** el fichero cada vez que abre la pantalla.

### F15 · `Comunicaciones/comunicaciones.free1x2`

- Formato deducido **solo del lector** (`ES/AConfiguracion.cs:183-197`): líneas `id#campo2#campo3`; `SortedList<int,string>[id] = campo2 + "#" + campo3`, ignorando `id` repetido. **Sin llamadores** y el fichero no existe en el repo (la carpeta se crea vacía, `App.xaml.cs:107-109`) ⇒ **no portar**.

### F16 · `*.z3q` — bitmap de columnas comprimido (ZIP)

- **Lee/escribe**: `UT/CompresorZip.cs:41-137` (SharpZipLib 1.4.2); UI: `VM/CompresorViewModel.cs`. Contenedor **ZIP con una única entrada** (nombre = `fichBin`, método *Deflated*, `Size` y `Crc32` fijados, nivel `nivel`).
- **Payload** (descomprimido): `(tamano/8)+1` bytes; el bit `i` del bitmap ⇒ `byte[i/8] |= 1 << (i%8)` (**LSB primero**) (`:46-62`). Índice `i` = nº de columna en base 3 (`ConvertidorDeBases`; 3^14 = 4 782 969 bits, 3^15 = 14 348 907).
- **Descomprimir** (`:88-137`): `BitArray(14348907)`; recorre la primera entrada; `count` = bytes×8; tamaño final **4 782 969** salvo `count > 14 348 907` ⇒ 14 348 907 (con 14 partidos `count`=4 782 976 ⇒ 3^14). Test: `TS/CompresorZipTests.cs`.
- Para Swift basta **inflar** (cualquier librería) y reproducir el orden de bits; no se exige igualdad byte a byte del ZIP (deflate no determinista entre librerías): comparar el payload descomprimido y el CRC.

### F17 · Equipos del boleto — `*.txt`

- **Lee/escribe**: `VW/MainPageViewModel.cs:674-732`. **Escribe** `File.WriteAllLines(ruta, equipos, Encoding.UTF8)` ⇒ UTF-8 **con BOM** (`EF BB BF`), una línea `"LOCAL - VISITANTE"` por partido, cada una con `\r\n` (incluida la última). **Lee** `File.ReadAllLines` (UTF-8 + BOM autodetectado): las `NumeroPartidos` primeras líneas; faltantes ⇒ `"? - ?"`.
- **Legacy**: `StreamWriter/Reader(…, Encoding.Default)` = ANSI (cp1252) (`Free1X2/UI/Controls/Pronosticos.cs:508-570`): ficheros antiguos de usuario están en **Windows-1252**, que el lector WinUI interpreta mal (U+FFFD). Swift: leer UTF-8 estricto y **recaer en Windows-1252** si hay bytes inválidos (§8-A7).

---

### F18 · Jornada online (JSON) y su caché

- Contrato HTTP y validaciones en §5. **Caché**: `jornada-es.json` / `jornada-mx.json` (`SV/JornadaCache.cs:52`) = **el cuerpo HTTP recibido, texto idéntico** (`File.WriteAllText`, UTF-8 sin BOM, sin EOL añadido; `:79`). Se escribe en `jornada-xx.json.tmp` y se renombra con *overwrite* (`:75-80`). Fecha de guardado = *mtime* UTC del fichero (`:104,134`).
- **Formato real del backend** [bytes `TS/Fixtures/online/real-es.json`]: una línea sin EOL final, `/` escapado (`"2026\/27"`), no-ASCII como `\uXXXX`, `jornada` **entero grande** (`20260621` en ES, `2338` en MX), `actualizado` con `+00:00`:

```json
{"pais":"ES","temporada":"2026\/27","jornada":20260621,"fecha":"2026-06-21","actualizado":"2026-06-18T16:42:16+00:00","partidos":[{"n":1,"local":"Estados Unidos","visitante":"Australia"},…,{"n":14,"local":"Finalista 2 Playoff","visitante":"Finalista 1 Playoff"}],"pleno15":{"n":15,"local":"España","visitante":"Arabia Saudi"}}
```
- Ejemplos con formato legible (LF, 2 espacios): `docs/ejemplos-api/quiniela-{es,mx}-actual.json` [bytes: sin BOM].

### F19 · Catálogo de equipos online (JSON)

- `{ "pais":"ES", "actualizado":"…", "divisiones":[ { "id":"all", "nombre":"Equipos (jornadas recientes)", "equipos":["Real Madrid", …] } ] }` (`docs/ejemplos-api/equipos-es.json` [bytes]). **No se cachea** (`SV/QuinielaOnlineService.cs:195-227`). Validaciones: §5.4.

### F20 · `idioma.json` · F21 · `tema.json` (JSON mínimo)

- Rutas: `%LocalAppData%\Free1X2\idioma.json` y `tema.json` (`SV/IdiomaApp.cs:47-50`, `SV/TemaApp.cs:46-49`).
- **Escritura** (cadena literal, sin EOL): `{"idioma":"Espanol"}` / `{"idioma":"Ingles"}`; `{"tema":"Sistema"}` / `"Claro"` / `"Oscuro"` — el valor es el **nombre del enum** `Opcion` (`IdiomaApp.cs:147`, `TemaApp.cs:165`) con escritura atómica (`.tmp` + `Move` *overwrite*). UTF-8 sin BOM.
- **Lectura** (`SV/IdiomaApp.cs:66-92`, `SV/TemaApp.cs:65-91`): fichero ausente ⇒ defecto (`Espanol` / `Sistema`); JSON inválido o sin la propiedad o no-string ⇒ defecto + traza; valor: `Enum.TryParse(texto, ignoreCase: true)` ⇒ acepta nombres sin distinguir mayúsculas **y también cifras** (`"1"`) incluso fuera de rango (no valida con `IsDefined`) **[.NET]**. `IdiomaApp.CodigoBcp47` = `"en"` solo si `Ingles`, si no `"es"` (`:60`).
- **Per-máquina, no portables**: en Mac se sustituyen por `UserDefaults` (§3, §4.3); no hay que leerlos.

### F22 · `log.txt` / `log.1.txt`

- `SV/Log.cs`. Línea: `{UTC yyyy-MM-dd HH:mm:ss.fff}Z [ERROR|WARN|INFO] mensaje` + `Environment.NewLine` (`:88-90,104`); `File.AppendAllText(…, Encoding.UTF8)` ⇒ el **BOM solo se escribe al crear** el fichero **[.NET]**. Excepciones: `Tipo: mensaje` encadenado con ` <- ` hasta 5 niveles, y la pila en nueva línea (`:63-80`). Rotación: si `Length >= 1 048 576` antes de escribir ⇒ `log.txt` → `log.1.txt` (sobrescribe) (`:120-132`). Nunca lanza; `lock` global.

### F23 · `Strings/{es,en-US}/Resources.resw`

- RESW (XML tipo `.resx`) UTF-8 [bytes]. **Solo 6 entradas** (pantalla piloto `CreditosFrmPage`): `CreditosTituloEquipo.Text`, `CreditosWebsiteLink.[using:Microsoft.UI.Xaml.Automation]AutomationProperties.Name`, `CreditosWebUrl.Text`, `CreditosColaboradores.Text`, `CreditosGraciasNota.Text`, `CreditosGracias.Text`. Clave = `x:Uid.Propiedad` (`VM/CreditosFrmPage.xaml:26-85`). Ver §4.

### F24 · Formatos definidos en ViewModels (solo para compatibilidad de lectura/escritura)

Texto UTF-8 sin BOM salvo indicación (`StreamWriter(ruta)` / `File.WriteAllLines`), CRLF. Lectura con `ReadLine`/`ReadAllLines` (se acepta cualquier EOL). Los campos «cadena» son texto libre de la pantalla.

| Ext. | Variante / quién | Gramática | Evidencia |
|------|------------------|-----------|-----------|
| `.lst` | DiFiltros | líneas `Ruta;Nombre;True\|False;Difs;Mínimo(int)` (≥5 campos; vacías se saltan); `FileIO.WriteLinesAsync` (codificación WinRT, ver §8-A9) | `VM/DiFiltrosViewModel.cs:204-262` |
| `.lst` | CombinarFiltros | una **ruta** por línea; al cargar se abre cada fichero con `ArchivoColumnasTexto.ObtenNumSignos` y todos deben tener el mismo nº de signos | `VM/CombinarFiltrosViewModel.cs:181-250` |
| `.cnd` | Coincidencias | líneas `Grupos;Rango` | `VM/CoincidenciasViewModel.cs:392-440` |
| `.cnd` | SelecJM | 6 líneas: 14 caracteres de grupos, `LimiteGrupo1`, `2`, `3`, `LimiteTotal`, modo (`0`=valor, `1`=corte) | `VM/SelecJMViewModel.cs:196-250` |
| `.cnd` | GeneraPim/aidomnou | 8 líneas: `Rango1..7` y `Recorrido` (`min-max`) | `VM/GeneraPimViewModel.cs:264-340`, `VM/aidomnouViewModel.cs:290-340` |
| `.par` | Parejas | 14 líneas `P1,P2,V11,V1X,V12,VX1,VXX,VX2,V21,V2X,V22` + 7 líneas `min,max` | `VM/ParejasFrmViewModel.cs:108-200` |
| `.jb7` | FiltroPorcenJB | 7 rangos, `Recorrido`, y `l1,…,l6` (enteros, cultura invariante) | `VM/FiltroPorcenJBViewModel.cs:232-300` |
| `.tri` | Trios | 12 líneas de 30 enteros separados por coma + 5 líneas `min,max` | `VM/TriosFrmViewModel.cs:293-372` |
| `.clm` | Exportador/Importador CPs | `columna#aciertos#aciertosSeguidos#fallosSeguidos` (exactamente 4 partes; `aciertos…` en el formato de `ColumnaProbable`) | `VM/ExportadorCPsFrmViewModel.cs:106-135`, `VM/ImportadorCPsFrmViewModel.cs:112-160` |

Además: `Informe_{fecha}_{hora}.txt` en `Informes/` solo lo escribe el legacy (`Free1X2/Program.cs:130`); WinUI únicamente los **borra** (`Informe*`, `VW/MainPageViewModel.cs:770-785`).

---

## 2. Inventarios transversales (T7 cultura · T8 texto)

### 2.1 T7 — todos los puntos con cultura (oráculo = `es-ES`)

La app **nunca** fija la cultura (sin `DefaultThreadCurrentCulture` ni `InvariantGlobalization`, búsqueda en `Free1X2.Domain`, `Free1X2.WinUI`): «cultura actual» = ajustes regionales de Windows del usuario.
La app está pensada para `es-ES` (coma decimal, `.` = miles). **Swift no debe depender del `Locale` del Mac**: implementar un módulo `FormatoNET` con las reglas de la columna «Regla Swift».

| # | Punto | Operación | Cultura en C# | Semántica oráculo (es-ES) | Regla Swift |
|---|-------|-----------|---------------|---------------------------|-------------|
| C1 | `ES/AConfiguracion.cs:373-375` | `double`→texto (`"0.00"`; `ToString()`) | **es-ES fija** | `0,50`; ida y vuelta más corta con `,`; `E+15`/`E-05` | `formatearDouble(v, .corto)` / `.dosDecimales` |
| C2 | `ES/AConfiguracion.cs:407-410` | texto→`float`→`double` | **es-ES fija** | `.` = miles (ignorado), `,` = decimal; **precisión `Float`** | `parsearEs(s) -> Float` y luego `Double(f)` |
| C3 | `ES/AConfiguracion.cs:205,322` | `DateTime` ↔ `"03/02/2010"` | **actual** | `ToShortDateString()`/`ToDateTime` ⇒ `dd/MM/yyyy` en es-ES | fijar `es_ES`; fallo ⇒ conservar valor previo (vestigial) |
| C4 | `ES/FValoracionData.cs:50-52` (lee) · `:92-94` (escribe) | `Convert.ToDouble(x.Replace(".", ","))` · `ToString().Replace(",", ".")` | **actual** | lee: punto→coma y parse con coma decimal; escribe: punto decimal | lee: `parsearEs(s.replacing(".", with: ","))`; escribe: `formatearDouble(v).replacing(",", with: ".")` |
| C5 | `ES/FFormatos123Data.cs:42` (lee) · `:181-194` (escribe; `:188`) | `Convert.ToDouble` / `double.ToString()` sin `Replace` | **actual** | `"0,5/0,3/…"` (coma); `"0.5"` se leería como 5 | `parsearEs` / `formatearDouble` |
| C6 | `UT/Porcentajes.cs:109-111,132-134,162-164,195-197,230-232` | `Replace(".", ",")` + `Convert.ToDouble` | **actual** | `.`→`,` y parse con coma | como C4 (lectura) |
| C7 | `UT/Porcentajes.cs:324-328` (`EsNumero`) | `Convert.ToDouble` **sin** `Replace` | **actual** | `"0.5"`⇒5 válido; `"abc"`⇒no | `parsearEs(s) != nil` |
| C8 | `UT/Porcentajes.cs:369,380,390` (escribe) · `:494` · `:511-514` | `ToString()` (+`Replace(",", ".")`) · `Replace(".", ",")` · `ToString()` | **actual** | formato 3/42: punto; formato 1 y portapapeles: coma; histórico: coma | por formato |
| C9 | `UT/LAE.cs:131,136` | `ToString().Replace(".", ",")` | **actual** | coma | `formatearDouble(v)` (ya trae coma) |
| C10 | `ES/ArchivoColumnasTexto.cs:211,216` · `ES/FColProbablesData.cs:54` · `ES/FGruposEquiposData.cs:46` | `ToUpper()` | **actual** | solo ASCII en la práctica | mayúsculas ASCII |
| C11 | `VM/DescargaBoletoFrmViewModel.cs:145-146` | `ToLocalTime().ToString("dd/MM/yyyy HH:mm")` | **actual** (`/` y `:` son separadores de cultura) | `03/02/2026 17:45` | `DateFormatter` `en_US_POSIX`, patrón `dd/MM/yyyy HH:mm`, zona local |
| C12 | `VM/ImprimirBoletoFrmViewModel.cs:352-355` | `double.TryParse` | **actual** | enteros escritos con `(int)` | `Double(s)` |
| C13 | enteros (`Convert.ToInt16/32`, `int.ToString()`) | | actual | solo cambia el signo negativo | `Int(s)` con recorte de espacios |

**`parsearEs` (reglas .NET `NumberStyles.Float | AllowThousands`, símbolos es-ES) [.NET]**: se ignoran espacios `U+0009…U+000D`/`U+0020` laterales; signo `+`/`-` opcional al inicio; dígitos con `.` como separador de **miles permitido solo tras al menos un dígito y antes de la coma** (`"1.234,5"`⇒1234,5; `"1.5"`⇒15; `".5"` ⇒ error); `,` decimal (una vez); exponente `e|E[+-]dígitos`; cadena vacía o con otros caracteres ⇒ `FormatException`. `Float` y `Double` se redondean al más cercano (IEEE), idéntico a `strtof/strtod`.
**`formatearDouble` [.NET Core 3.0+]**: dígitos de ida y vuelta más cortos (los mismos que `Double.description` de Swift) con **coma** decimal; formato fijo si el exponente decimal está en `[-5, 15)`, si no científico `d,ddE+XX` (mínimo 2 dígitos de exponente); entero sin decimales (`12`, no `12.0`). `"0.00"`: redondeo a 2 decimales (§8-A12).

### 2.2 T8 — codificación, BOM y fin de línea por escritor/lector

| Formato | C# escribe (Windows) | C# lee | Swift lee | Swift escribe (propuesta) |
|---------|----------------------|--------|-----------|---------------------------|
| F01 `parametros`, F02 `.comb`, F03, F05, F12 (XML) | UTF-8 sin BOM, EOL `\r\n` en sangrías, sin salto final (`XmlTextWriter(ruta,null)`) | autodetección BOM/declaración | idem (usar `XMLParser`/libxml con encoding declarado; sin declaración ⇒ UTF-8) | UTF-8 sin BOM, `\r\n`, sin salto final, **edición in-place del DOM** (F01, F05) |
| F04 columnas `.txt` | UTF-8 sin BOM (`UTF8Encoding(false,true)`), `\r\n` | UTF-8 + autodetección BOM | `\r`, `\n`, `\r\n`; BOM UTF-8 | UTF-8 sin BOM, **`\r\n`** (por `ObtenNumCols`) |
| F06, F07, F08 | `File.CreateText`/`ArchivoColumnasTexto`: UTF-8 sin BOM, `\r\n` | UTF-8 | idem | idem |
| F09 `Resultados.txt` | (no lo escribe la app) | `File.ReadLines` UTF-8+BOM | idem | n/a |
| F10 `.dat` | **Latin-1**, `\r\n`, sin BOM | Latin-1 | `String(data:, encoding: .isoLatin1)` | Latin-1 + `\r\n`; carácter > U+00FF ⇒ `?` |
| F11 `.lang` | (no se escribe) | `Encoding.Default` (UTF-8 en .NET 8; el fichero es Latin-1) | **Latin-1** | n/a |
| F13 `imprebol.cfg` | UTF-8 sin BOM, `\r\n` | UTF-8 | idem | idem |
| F14 `aidomnou.cfg` | (legacy) `StreamWriter` UTF-8 sin BOM | UTF-8 | UTF-8 (LF en la semilla) | n/a (no portar) |
| F17 equipos `.txt` | **UTF-8 con BOM**, `\r\n` (WinUI); ANSI/cp1252 (legacy) | UTF-8+BOM | UTF-8 estricto; si inválido ⇒ **Windows-1252** | UTF-8 con BOM, `\r\n` |
| F18–F21 JSON (caché, prefs) | UTF-8 sin BOM, sin EOL añadido | UTF-8+BOM | idem; quitar `U+FEFF` inicial | UTF-8 sin BOM |
| F22 log | UTF-8; BOM al crear; `\r\n` | — | — | `~/Library/Logs`, UTF-8, `\n` (no es un formato de intercambio) |
| F24 | `StreamWriter(ruta)` UTF-8 sin BOM, `\r\n` (`FileIO.WriteLinesAsync` en `.lst`: §8-A9) | `ReadLine/ReadAllLines` | cualquier EOL | UTF-8 sin BOM, `\r\n` |

> `.gitattributes:2` (`* text=auto`) normaliza a LF los ficheros de texto **del repositorio** (semillas y fixtures están en LF [bytes]); el despliegue Windows puede reconvertirlos a CRLF según `core.autocrlf`. Los lectores deben aceptar ambos.

---

## 3. Rutas y carpetas de trabajo

### 3.1 Estado actual (Windows)

Base = `AppContext.BaseDirectory` (carpeta del `.exe`; equivale a `Application.StartupPath` del legacy; `Free1X2.Domain/Abstractions/IAppPaths.cs:19-22`). La semilla `DT/**` se copia a la raíz de salida (`Free1X2.WinUI.csproj` `<Content Include="Datos\**\*">`, bloque «Datos semilla»). En el arranque se crean **vacías** `Columnas, Combinaciones, Condiciones, Filtros, Ganadoras, Informes, Jornadas, Temp, Comunicaciones, Documentacion` (`App.xaml.cs:102-119`; si falla por permisos solo se registra).

| Ruta (relativa a Base) | Contenido | Quién | Evidencia |
|------------------------|-----------|-------|-----------|
| `parametros.free1x2` | F01 (R/W, usuario) | `AConfiguracion` | `ES/AConfiguracion.cs:35-56` |
| `ColumnasProbables.xml` | F05 (R/W) | `DatosHelper` | `CP/DatosHelper.cs:16-34` |
| `aidomnou.cfg` | F14 (solo legacy) | `Free1X2/UI/Aidomnou.cs` | `Free1X2/UI/Aidomnou.cs:181-224` |
| `Idioma/*.lang` | F11 (R, semilla) | `ArchivoIdioma` | `ES/ArchivoIdioma.cs:14-16` |
| `Equipos/equipos{1,2,2b,Int}.dat` | F10 (R/W, usuario) | Gestor/Agregar Equipo | `VM/GestorEquiposFrmViewModel.cs:34-40` |
| `Impresion/impresoras.cfg` · `imprebol.cfg` | F12 (R) · F13 (R/W) | | `MC/ControladoresImpresion.cs:35`; `VM/ImprimirBoletoFrmViewModel.cs:118-122` |
| `Documentacion/{licencia,GPL}.txt` | texto de licencia (R) | «Acerca de» | `VM/AcercaDeFrmPage.xaml.cs:102,110` |
| `Comunicaciones/` | vacía; F15 sin uso | | `ES/AConfiguracion.cs:185` |
| `Columnas/` | salida de «Calcular varias combinaciones» (`{nombre}.txt`) | | `VM/CalculaColumnasMultipleFrmViewModel.cs:136-165` |
| `Combinaciones/` | carpeta de `.comb` (legacy `InitialDirectory`) | | `VM/CalculaColumnasMultipleFrmViewModel.cs:109`; `Free1X2/UI/Controls/Pronosticos.cs:503` |
| `Condiciones/` | salida del Generador de CPs (F06) | | `VM/GenerarCPsViewModel.cs:108-111` |
| `Filtros/` | creada pero **sin usos** en el código | | `App.xaml.cs:107` |
| `Ganadoras/` | columnas ganadoras de usuario (F04) leídas por 12 pantallas de filtro | | p. ej. `VM/Formatos123FrmViewModel.cs:125-127` |
| `Informes/` | `Informe*` (legacy escribe; WinUI solo borra) | | `Free1X2/Program.cs:130`; `VW/MainPageViewModel.cs:766-785` |
| `Jornadas/InfoJornadasLAE.txt` · `Resultados.txt` | F08 (R/W) · F09 (R) | | §F08, §F09 |
| `Temp/tmp.*` · `Temp/*_tmp.comb` | portapapeles de condiciones (F03) · temporales | | `VM/_Helpers/FiltroArchivoViewModelBase.cs:45-47`; `VW/MainPageViewModel.cs:743-761` |
| `Assets/…` | iconos | | `MainWindow.xaml.cs:72` |
| `%LocalAppData%\Free1X2\` | `jornada-{es,mx}.json` (F18), `idioma.json` (F20), `tema.json` (F21), `log.txt`/`log.1.txt` (F22) | servicios | `SV/JornadaCache.cs:38-45` |
| `%TEMP%\free1x2_smoke.log` | solo prueba de humo | | `MainWindow.xaml.cs:821` |

**Rutas absolutas dentro de ficheros**: `.comb` guarda `filtroColumnas/@nombreArchivo` y `filtroParcial/@archivo` con la **ruta tal cual la devolvió el selector** (absoluta de Windows) (`VW/MainPageViewModel.cs:363-385,508-525,603`), y los lectores solo comprueban `File.Exists` para el semáforo (`:248-250,270`). F24 (`.lst`) también guarda rutas.

### 3.2 Equivalentes macOS propuestos (decisión final del dueño, R2)

| Windows | macOS | Notas |
|---------|-------|-------|
| Recursos de solo lectura en la carpeta del exe (`Idioma/`, `Impresion/impresoras.cfg`, `Documentacion/`, `Assets/`) | **Bundle**: `Free1X2.app/Contents/Resources/Datos/…` (`Bundle.main.url(forResource:)`) | el bundle no es escribible |
| Semillas editables: `parametros.free1x2`, `ColumnasProbables.xml`, `Equipos/*.dat` | Copia **la primera vez** (si no existe) desde `Resources/Datos/` a `~/Library/Application Support/Free1X2/` y se trabaja ahí | nunca sobrescribir un fichero existente; migración de versiones: §8-A13 |
| `Jornadas/`, `Temp/`, `Filtros/`, `Comunicaciones/` | `~/Library/Application Support/Free1X2/{Jornadas,Temp,…}` | `Temp/tmp.*` mejor en memoria o `FileManager.temporaryDirectory` |
| Entregables del usuario: `Columnas/`, `Combinaciones/`, `Condiciones/`, `Ganadoras/` | `~/Documents/Free1X2/{Columnas,Combinaciones,Condiciones,Ganadoras}` (carpeta inicial de `NSOpenPanel`/`fileExporter`) | visibles en Finder; `PickerHelper` ya usa «Documentos» (`SV/PickerHelper.cs:32,58`) |
| `%LocalAppData%\Free1X2\jornada-*.json` | `~/Library/Application Support/Free1X2/jornada-{es,mx}.json` | mismos nombres y contenido |
| `idioma.json`, `tema.json` | `UserDefaults` (`idioma` = `"es"\|"en"`, `tema` = `"sistema"\|"claro"\|"oscuro"`) | per-máquina, no portables |
| `log.txt` / `log.1.txt` | `~/Library/Logs/Free1X2/log.txt` (+ `log.1.txt`), rotación a 1 MiB | |
| `Informes/` | `~/Library/Logs/Free1X2/Informes/` (si se mantiene el informe de errores) | |
| `aidomnou.cfg`, `Comunicaciones/` | **no portar** | WinUI no lo usa |

Resolución de rutas Windows heredadas (`.comb`, `.lst`): ante una ruta que no existe, normalizar `\`→`/`, probar el *basename* en la carpeta del propio fichero y luego en `~/Documents/Free1X2/Columnas`; si no se encuentra ⇒ estado «no encontrado» (equivalente al semáforo rojo), **sin fallar** la apertura (§8-A14).

---

## 4. Idioma

### 4.1 Mecanismos existentes (hay tres, solo uno aplica)

| Mecanismo | Datos | ¿Se aplica? | Evidencia |
|-----------|-------|-------------|-----------|
| **A. Diccionario legacy** `.lang` | `parametros/idioma/@valor` (`es-ES`) elige `Idioma/{valor}.lang` ⇒ `VariablesGlobales.DiccionarioIdioma` (`Dictionary<string,string>`, clave en MAYÚSCULAS) | **No**: `FormulariosHelper.Traducir` está comentado y WinUI no lo lee | `ES/ArchivoIdioma.cs:11-45`; `VariablesGlobales.cs:88,104`; `Free1X2/UI/FormulariosHelper.cs:30-58` |
| **B. Recursos WinUI** `.resw` | `idioma.json` (`Espanol`/`Ingles`) ⇒ `ApplicationLanguages.PrimaryLanguageOverride = "es"\|"en"` ⇒ PRI resuelve `Strings\es` / `Strings\en-US` por `x:Uid` | **Sí, pero solo 6 claves** (pantalla `CreditosFrmPage`) | `SV/IdiomaApp.cs:60,98-103,123-133`; `App.xaml.cs:52-58`; `Strings/*/Resources.resw` |
| **C. Texto español embebido** en XAML/C# | literales | el resto de las 107 páginas (comentario del propio `.resw`: «El resto de las 107 páginas siguen con su texto en español embebido») | `Strings/es/Resources.resw:2-9` |

Cambio de idioma en WinUI: menú *Ver → Idioma* (`Español`/`English`, `RadioMenuFlyoutItem`); `IdiomaApp.Cambiar` persiste y `RecargarPaginaActual` re-navega porque los `x:Uid` se resuelven **al cargar la página** (`MainWindow.xaml.cs:650-705`); el estado en memoria de la página se pierde (límite documentado en el propio código).
En `ConfiguracionFrm` el combo de idiomas es `{"es-ES"}` ∪ `{nombres de .lang}` (`es-ES` no tiene fichero) y guarda `parametros/idioma/@valor` (`VM/ConfiguracionFrmViewModel.cs:87,145-149,195`) — es independiente del mecanismo B.

### 4.2 Formato `.lang` (resumen de F11)

`CLAVE#VALOR` por línea, Latin-1, clave = texto español en mayúsculas, la primera aparición gana, las líneas sin `#` abortan la lectura silenciosamente. Contenido: 143 líneas, 142 claves, 14 sin traducir (lista en F11).

### 4.3 Propuesta Swift: String Catalog

1. **Catálogo principal** `Localizable.xcstrings` con `sourceLanguage = "es"`, idiomas `es`, `en`. Claves **simbólicas** (`menu.archivo`, `online.error.timeout`) definidas durante el diseño de UI (M5); el texto español es el valor `es`.
2. **Importación de `.lang`** (herramienta de build, no en tiempo de ejecución): `tools/lang2xcstrings` (script) que
   - lee `DT/Idioma/en-US.lang` como **ISO-8859-1**, parte por el **primer** `#`, gana la primera clave repetida (`ANALIZAR`⇒`Analize`),
   - los **14 valores vacíos** se importan con `state: "needs_review"`/sin valor `en` (no `translated`),
   - mapea `CLAVE_MAYÚSCULAS` a la clave simbólica mediante una tabla de correspondencia que se redacta con las pantallas (UI_A/B/C); las claves sin correspondencia van a un catálogo `Legacy.xcstrings` con la clave tal cual,
   - conserva el texto con su espacio final (`Analysis of Combination Groups `) hasta que se decida recortarlo.
3. **Formato `.xcstrings`** (JSON, `version` `1.0`):

```json
{
  "sourceLanguage" : "es",
  "strings" : {
    "ABRIR COMBINACIÓN" : {
      "localizations" : {
        "en" : { "stringUnit" : { "state" : "translated", "value" : "Open Combination" } }
      }
    }
  },
  "version" : "1.0"
}
```
4. **`.resw` → catálogo**: las 6 entradas pasan a claves `creditos.tituloEquipo`, `creditos.webUrl` (no se traduce), `creditos.colaboradores`, `creditos.graciasNota`, `creditos.gracias`, `creditos.enlaceWebAccesibilidad`, con el `<comment>` del `.resw` como comentario del catálogo (textos exactos en `Strings/{es,en-US}/Resources.resw:68-90`).
5. **Mensajes de dominio/servicios** (hoy literales en español en C#): pasar a `String(localized:)` con claves estables. Lista exacta en §5.7 (los literales son parte del contrato de pruebas: `TS/JornadaQuinielaParserTests.cs` exige `Contains("JSON")` y `Contains("14")`).
6. **Selección de idioma en la app**: `UserDefaults["idioma"]` ∈ `{"es","en"}` (equivale a `IdiomaApp.Opcion`); aplicar con `.environment(\.locale, Locale(identifier: …))` para cambio inmediato (sin re-navegar). Además, al guardar la configuración, escribir `parametros/idioma/@valor` = `"es-ES"` / `"en-US"` para mantener la compatibilidad de F01.
7. **No localizar nunca** los textos que viajan en ficheros: las líneas `If`/`Then` del `.comb` (F02) y los nombres de enum de `condicion/@id` (F03) son parte del formato.

---

## 5. Online (clubprogol.com)

Contrato original: [`../API_CLUBPROGOL.md`](../API_CLUBPROGOL.md). Aquí se fija lo que **hace el cliente C#** (oráculo).

### 5.1 Contrato HTTP (cliente)

| Aspecto | Valor | Evidencia |
|---------|-------|-----------|
| Base URL | `FREE1X2_API_BASE` si no está vacía (tras `Trim()`), si no `https://clubprogol.com`. Se aplica `TrimEnd('/')` | `SV/QuinielaOnlineService.cs:65,71,112-119,163,214` |
| Prefijo | `/wp-json/clubprogol/v1` | `:68` |
| Jornada | `GET {base}/wp-json/clubprogol/v1/quiniela/{pais}/actual` | `:163` |
| Catálogo | `GET {base}/wp-json/clubprogol/v1/equipos/{pais}` | `:214` |
| `{pais}` | `Trim().ToLowerInvariant()`; solo `es` o `mx`, si no `QuinielaOnlineException("País no soportado: '<original>' (use 'es' o 'mx').")` (usa el valor **original**) | `:132-136,207-211` |
| Cabeceras | solo `Accept: application/json`; **sin** `User-Agent` explícito, sin autenticación | `:103` |
| Timeout | `HttpClient.Timeout = 10 s` (total de la petición, incluida la lectura del cuerpo: `ResponseContentRead`) | `:101,244` |
| Redirecciones/decompresión/cookies | valores por defecto de `HttpClient` | **[.NET]** (§8-A10) |
| Éxito | `IsSuccessStatusCode` (200–299); cuerpo = `ReadAsStringAsync` (charset del `Content-Type`, UTF-8 por defecto) | `:247-250` |
| Errores | cualquier no-2xx: se lee el cuerpo como **dato**; ver 5.2 | `:252-279` |
| Cancelación del llamador | se propaga (`OperationCanceledException`) sin envolver | `:285-289` |
| Timeout (cancelación no pedida) | `QuinielaOnlineException("Se agotó el tiempo de espera al contactar con el servicio online.")` | `:290-295` |
| Fallo de red/TLS/DNS | `QuinielaOnlineException("No se pudo contactar con el servicio online (sin conexión o servidor no disponible).")` | `:296-300` |
| Otras excepciones (URL base inválida…) | **no se capturan**: llegan a la UI como `"No se pudo descargar la jornada: " + mensaje` | `:235-302`; `VM/DescargaBoletoFrmViewModel.cs:127` |

Códigos del backend (`docs/API_CLUBPROGOL.md:41-45`): `404 sin_jornada`, `429 rate_limited` (+`Retry-After: 60`), `503 no_listo`; cuerpo de error `{"error":"…","mensaje":"…"}`.

### 5.2 Algoritmo del cliente (`DescargarConReintentoAsync`, `:235-302`)

```
reintentoUsado = false
bucle:
  resp = GET url                                   // 10 s
  si 200...299: devolver cuerpo (texto)
  cuerpoError = cuerpo de resp (texto)
  si status == 429:
      ra = leerRetryAfter(resp)                    // Int? segundos
      si !reintentoUsado && ra != nil && ra > 0 && ra <= 3:     // RetryAfterMaxSegundos = 3
          reintentoUsado = true; esperar ra s; continuar        // UN solo reintento
      espera = ra ?? 60
      lanzar Error( mensajeDelCuerpo(cuerpoError) ?? "Demasiadas solicitudes (límite 60/min). Intenta de nuevo en ~\(espera) s." )
  lanzar Error( mensajeDelCuerpo(cuerpoError) ?? "El servicio respondió con código \(status) (\(NombreHttpStatusCode)).")
```
- `leerRetryAfter` (`:309-332`): (1) valor *delta-seconds* ⇒ `Int(ceil(segundos))`; (2) si es *HTTP-date* ⇒ `ceil(fecha − ahoraUTC)` y, si ≤ 1 s o pasada, **1**; (3) último recurso: `int.TryParse` de los valores crudos de la cabecera; si nada ⇒ `nil`. Con `Retry-After: 0` ⇒ `0` ⇒ no reintenta y el mensaje dice «~0 s».
- `mensajeDelCuerpo` (`:339-365`): JSON objeto; devuelve `mensaje` si es string no vacío/blanco; si no, `error` (idem); si no, `nil`. Cuerpo no JSON ⇒ `nil`. (Arrays/`mensaje` no string ⇒ `nil`.)
- `NombreHttpStatusCode`: nombre del enum `System.Net.HttpStatusCode` (`NotFound`, `ServiceUnavailable`, `InternalServerError`, `BadGateway`, `GatewayTimeout`, `Forbidden`, `Unauthorized`, `BadRequest`, …); para códigos sin nombre, .NET imprime el número. **[.NET]** — tabla completa a fijar con el dueño (§8-A11).

### 5.3 Jornada: `ObtenerJornadaAsync` (`:130-192`) y guarda anti-doble-petición

```
p = normalizar(pais)
si ultimaDescargaUtc[p] existe && (ahoraUTC − ultima) < 60 s && cache.fechaGuardado(p) != nil:
     si (j, g) = cache.tryCargar(p) válido  →  devolver (j, desdeCache: true, guardadoUtc: g)
     // si la caché resulta ilegible se continúa a la red
cuerpo = descargarConReintento(url)
jornada = parse(cuerpo)   // FormatException ⇒ Error("La respuesta del servicio no es una jornada válida: " + detalle)
cache.guardar(p, cuerpo)  // best-effort, nunca lanza
ultimaDescargaUtc[p] = ahoraUTC          // SOLO tras éxito (B-05)
devolver (jornada, desdeCache: false, guardadoUtc: cache.fechaGuardado(p))
```
`ultimaDescargaUtc` es un diccionario **estático en memoria** (se pierde al cerrar). Catálogo (`ObtenerEquiposAsync`, `:205-227`): misma descarga/errores, **sin** guarda ni caché; error de parseo ⇒ `"La respuesta del servicio no es un catálogo de equipos válido: " + detalle`.

### 5.4 Parsers JSON (dominio puro)

Entrada: cadena UTF-8 ya decodificada. Se trata como **dato no confiable**: solo se leen campos conocidos; los extra (`actualizado`, `revancha`…) se **ignoran** (`ON/JornadaQuinielaParser.cs`, `docs/API_CLUBPROGOL.md:203-206`). Todos los errores son `FormatException` con mensaje en español.

**Jornada** (`ON/JornadaQuinielaParser.cs:30-95`; modelo `ON/JornadaQuiniela.cs:18-53`):

| Paso | Regla | Mensaje (exacto) |
|------|-------|------------------|
| 1 | `null`, vacío o solo espacios | `La respuesta de la jornada está vacía.` |
| 2 | JSON mal formado (`JsonDocument.Parse`: sin comentarios, sin comas finales, profundidad ≤64) | `La respuesta de la jornada no es un JSON válido: {detalle .NET}` |
| 3 | La raíz debe ser objeto | `La respuesta de la jornada no es un objeto JSON.` |
| 4 | `pais` **string obligatorio** (cualquier texto, también `""`; `null`/ausente ⇒ error) | `Falta el campo obligatorio 'pais'.` / `El campo 'pais' debe ser texto.` |
| 5 | `temporada` string obligatorio | idem con `'temporada'` |
| 6 | `jornada` **entero Int32 obligatorio**: debe ser un número JSON que `TryGetInt32` acepte (sin parte decimal ni exponente; `38.0` y `1e2` fallan; el backend real envía `20260621`) | `Falta el campo obligatorio 'jornada'.` / `El campo 'jornada' debe ser un número entero.` |
| 7 | `fecha` string opcional (`null`/ausente ⇒ `""`; no string ⇒ error) | `El campo 'fecha' debe ser texto.` |
| 8 | `partidos` array obligatorio | `Falta el array 'partidos' en la jornada.` |
| 9 | Cada elemento, **en orden** (posición esperada 1, 2, …): objeto; `n` Int32 obligatorio y `== posición`; `local` y `visitante` strings obligatorios (se leen ambos: errores de ausencia/tipo, `local` primero) y luego no vacíos/blancos (`local` primero); los nombres **no se recortan** | `El partido nº {pos} no es un objeto JSON.` · `Los partidos deben venir en orden: se esperaba n={pos} y se recibió n={n}.` · `El equipo local del partido nº {n} está vacío.` · `El equipo visitante del partido nº {n} está vacío.` |
| 10 | Tras recorrer todos: **exactamente 14** | `La jornada debe tener exactamente 14 partidos; se recibieron {k}.` |
| 11 | `pleno15`: solo si existe **y es objeto** (si es otra cosa se ignora en silencio); se valida con `n==15` y mismas reglas | (mismos mensajes con `n` 15) |

Notas: propiedad duplicada ⇒ §8-A5. Un 15.º partido en `partidos` produce el error del paso 9 solo si su `n` no es 15 (esperado=15): `n:15` pasa y luego falla el paso 10 con «se recibieron 15».

**Catálogo** (`ON/CatalogoEquiposParser.cs:30-122`; modelo `ON/CatalogoEquipos.cs`):

| Paso | Regla | Mensaje |
|------|-------|---------|
| 1–3 | vacío / JSON inválido / raíz no objeto | `La respuesta del catálogo de equipos está vacía.` · `… no es un JSON válido: {detalle}` · `… no es un objeto JSON.` |
| 4 | `pais` string obligatorio | `Falta el campo obligatorio 'pais'.` |
| 5 | `divisiones` array obligatorio | `Falta el array 'divisiones' en el catálogo de equipos.` |
| 6 | Por división (en orden): objeto; `id` string obligatorio (puede ser `""`); `nombre` string opcional; `equipos` array obligatorio; cada equipo string no blanco; ≥1 equipo | `Una división del catálogo no es un objeto JSON.` · `Falta el array 'equipos' en la división '{id}'.` · `Los equipos de la división '{id}' deben ser texto.` · `La división '{id}' contiene un nombre de equipo vacío.` · `La división '{id}' no tiene equipos.` |
| 7 | ≥1 división (se comprueba **después** del bucle) | `El catálogo de equipos debe tener al menos una división.` |

`EquiposPlano()` (`ON/CatalogoEquipos.cs:35-52`): aplana todas las divisiones **en orden**, salta blancos, usa como clave y como valor devuelto `Trim()`, y deduplica con `OrdinalIgnoreCase` conservando la **primera** aparición. Swift: clave = escalares pasados a su mayúscula simple (`ToUpperInvariant`), **sin** plegado completo (`ß`≠`SS`).

### 5.5 Caché offline-first (`SV/JornadaCache.cs`)

| Operación | Comportamiento | Línea |
|-----------|----------------|-------|
| Carpeta | `%LocalAppData%\Free1X2` (`Environment.GetFolderPath(LocalApplicationData)`) | `:38-45` |
| Ruta | `jornada-{es\|mx}.json`; país no válido ⇒ `nil` | `:48-59` |
| `Guardar(pais, raw)` | ignora vacío/blanco; crea carpeta; escribe `…json.tmp` y `Move(overwrite)`; ante error: traza, borra el `.tmp`, **no lanza** | `:66-89` |
| `FechaGuardado` | *mtime* UTC del fichero o `nil`; no lee el contenido | `:98-111` |
| `TryCargar` | `File.ReadAllText` + `JornadaQuinielaParser.Parse`; cualquier excepción ⇒ `false` (traza); devuelve *mtime* UTC | `:119-146` |
| `PaisMasReciente` | itera `["es","mx"]`; elige el de mayor *mtime* (`>` estricto ⇒ empate = `es`); **no comprueba validez** | `:153-180` |

Arranque (`App.xaml.cs:52-58,76-95`): `AsegurarCarpetasDeTrabajo` → `IdiomaApp.Aplicar` → `SembrarJornadaDesdeCache` (`PaisMasReciente` → `TryCargar` → `AppState.JornadaActual`; si el más reciente está corrupto **no** se prueba el otro país) → ventana. **Sin red**. La pantalla «Descarga de boleto» arranca en `PaisesOnline.PorCodigo(PaisMasReciente())` (`VM/DescargaBoletoFrmViewModel.cs:53-66`) y su acción «Actualizar jornada» llama al servicio; si falla, cae a `TryCargar` (`:109-135`).
El boleto y *Grupos de Equipos* copian `Partidos[i].Local/Visitante` de la jornada cuando existe (`CT/BoletoViewModel.cs:96-109`, `VM/GruposEquiposFrmViewModel.cs:255-273`).

### 5.6 Propuesta Swift: `URLSession` async

```swift
public actor QuinielaOnlineService {
    public static let shared = QuinielaOnlineService()
    public static let baseUrlPorDefecto = "https://clubprogol.com"
    static let prefijoApi = "/wp-json/clubprogol/v1"
    static let retryAfterMaxSegundos = 3, ventanaAntiDobleSegundos = 60.0

    private let session: URLSession = {
        let c = URLSessionConfiguration.ephemeral          // sin caché ni cookies persistentes
        c.requestCachePolicy = .reloadIgnoringLocalCacheData
        c.urlCache = nil
        c.timeoutIntervalForRequest = 10                   // HttpClient.Timeout = 10 s (total)
        c.timeoutIntervalForResource = 10
        c.httpAdditionalHeaders = ["Accept": "application/json"]
        return URLSession(configuration: c)
    }()
    private var ultimaDescarga: [String: Date] = [:]       // solo en memoria

    public func obtenerJornada(pais: String) async throws -> ResultadoJornada { … }   // §5.3
    public func obtenerEquipos(pais: String) async throws -> CatalogoEquipos { … }
}
public struct ResultadoJornada: Sendable { let jornada: JornadaQuiniela; let desdeCache: Bool; let guardadoUTC: Date? }
public enum ErrorOnline: Error, LocalizedError { case mensaje(String) }   // = QuinielaOnlineException
```
Reglas de implementación:
- **Base URL**: `ProcessInfo.processInfo.environment["FREE1X2_API_BASE"]`, y para apps lanzadas desde Finder, `UserDefaults.standard.string(forKey: "FREE1X2_API_BASE")` (solo desarrollo; **no** documentar en el manual). Trim y `TrimEnd("/")`.
- **Estado de red**: `URLError.timedOut` ⇒ mensaje de tiempo de espera; cualquier otro `URLError` (`.notConnectedToInternet`, `.cannotFindHost`, `.cannotConnectToHost`, `.networkConnectionLost`, `.dnsLookupFailed`, `.secureConnectionFailed`, …) ⇒ mensaje «No se pudo contactar…»; `CancellationError`/`URLError.cancelled` con `Task.isCancelled` ⇒ se propaga sin envolver.
- **Decodificación**: `String(decoding: data, as: UTF8.self)` (o `textEncodingName` si el servidor lo declara), eliminando un `U+FEFF` inicial; el **mismo texto** se guarda en la caché (el C# guarda el `string` decodificado, no los bytes).
- **Parsers**: no usar `Codable` directo; recorrer `JSONSerialization` (o un parser propio) para respetar las validaciones y mensajes de §5.4. `jornada`/`n` deben ser enteros: rechazar números con fracción/exponente (`NSNumber` no distingue `38` de `38.0` con `JSONDecoder`; usar `JSONSerialization` y `CFNumberIsFloatType`, o validar el token en texto) y comprobar rango `Int32`.
- **Caché**: `Data.write(to:options:.atomic)` (equivale a `.tmp`+`Move`); *mtime* con `attributesOfItem[.modificationDate]`.
- **Retry-After**: `Int(valor.trimmed)` o fecha IMF (`EEE, dd MMM yyyy HH:mm:ss 'GMT'`, `en_US_POSIX`, GMT).
- **ATS/sandbox**: HTTPS por defecto; para un stub `http://localhost:8080` añadir `NSAppTransportSecurity › NSAllowsLocalNetworking = YES`; si se activa App Sandbox (decisión pendiente del plan §7.3) añadir `com.apple.security.network.client`.
- **UI**: llamar desde un `@MainActor` ViewModel con `await`; botón deshabilitado mientras `descargando` (las 3 capas que cita el C#: botón, comando sin concurrencia y guard).

### 5.7 Mensajes de usuario que dependen del servicio (para el catálogo, §4.3)

| Origen | Texto exacto |
|--------|--------------|
| `VM/DescargaBoletoFrmViewModel.cs:84,102-106,118,127-128` | `Conectando con el servicio online…` · `Jornada {n} · {k} partidos cargados` + (`— mostrando la última guardada ({dd/MM/yyyy HH:mm}).` ∣ ` · Última actualización: {fecha}`) · `Sin conexión: mostrando la jornada guardada (actualizada {fecha}).` · `{detalle} El boleto sigue en modo manual.` con `detalle` = mensaje del servicio o `No se pudo descargar la jornada: {msg}` |
| `VM/GestorEquiposFrmViewModel.cs:147,172-174,180-183` | `Conectando con el servicio online…` · `{n} equipos importados (online) a {categoría}. Revisa y pulsa «Guardar archivos».` · `El catálogo online no añadió equipos nuevos a {categoría} (ya estaban todos).` · `{detalle} Los equipos no se modificaron.` (`detalle` = mensaje o `No se pudo importar el catálogo: {msg}`) |
| `SV/QuinielaOnlineService.cs` | los 7 literales de §5.1/§5.2 |
| Parsers | los 22 literales de §5.4 |

---

## 6. Componentes (plantilla ESPEC_00 §2)

> Las gramáticas están en §1 (se citan como «F0x»); aquí se fija el modelo Swift, el algoritmo y los casos de prueba.
> Nombres Swift propuestos: tipos `UpperCamelCase`, miembros `lowerCamelCase`, términos de dominio en español. Todo lo de este módulo va en `Free1X2Formats` y `Free1X2Online` (plan §2) y es `Sendable`, sin estado global.

### 6.1 `AConfiguracion` — `Free1X2.Domain/EntradaSalida/AConfiguracion.cs`

**Propósito.** Acceso tipado a `parametros.free1x2` (F01): lee secciones por nombre de elemento y, en cada `Guardar*`, cambia un atributo y reserializa el documento completo.

**Tipos y datos.**

| Dato | C# | Swift |
|------|----|-------|
| `numero` de partidos, `desplazamiento` | `Convert.ToInt16` → `short` (asignado a `int`) | `Int16` (validar rango; desbordar = error) |
| `fijos/dobles/triples` | `Convert.ToInt32` | `Int32` |
| `PrecioApuesta`, `PorcentajeDestinadoAl14`, `Recaudacion` | `float.Parse(es-ES)` → `double` | `Double(Float)` (T3) |
| `Moneda`, `idioma`, `separadorJB`, `separador` | `string` (`separador.Split(',')` ⇒ `string[]`) | `String`, `[String]` (conservar vacíos) |
| 6 toolbars, `actualizarAlInicio`, 20 `analisis`, `confirmacionSalir` | `bool` | `Bool` |
| `fecha` notificaciones | `DateTime` (cultura actual) | `Date?` (vestigial) |
| Documento | `XmlDocument` | `DocumentoXML` propio (árbol con **orden de atributos**, comentarios y nodos desconocidos preservados) |

Defaults: los de `VariablesGlobales.SetDefaultValues` (F01, `VariablesGlobales.cs:149-175`).

**Entradas / salidas.** `init(directorio:)` lee `<dir>/parametros.free1x2` (error si no existe o está mal formado: `FileNotFoundException`/`XmlException`). `Obten*` devuelven valores; `Guardar*` escriben a `archConfig` (mismo fichero).

**Algoritmo.**
```
cargar: doc = DOM(contenido)                         // descarta nodos de solo espacios
elemento(tag) = primer descendiente (preorden) con nombre == tag            // sensible a mayúsculas
hijo0(tag)    = elemento(tag).hijos[0]                                      // hijos = elementos/comentarios (sin espacios)
atributo(h,n) = h.atributos[n] ?? falla(NRE)
ObtenToolBarsVisibles: todos = true; si elemento("toolbars")==nil || sin hijos ⇒ return;
                       cada tsX = parseBoolTolerante(atributo) ?? true
ObtenDesplazamiento / GuardarDesplazamiento / GuardarIdioma / fecha: try { … } catch {}   // silencian
Guardar*: h = hijo0(tag); h.atributos[n] = String; escribir(doc → archConfig)  // §1.0.1, UTF-8 sin BOM
GuardarValoresLAE: Precio="\(formato(precio,"0.00"))" (es-ES); Pct/Recaud = shortest es-ES; Moneda tal cual
```
**Estado global.** Lo leen `VariablesGlobales` (44 miembros, ESPEC_01 §estado), `Grupo.configuraBoleto` (nº de partidos, `MC/Grupo.cs:57-63`), `Grupo.ActualizaParametrosFiltros` (puntos CP, `MC/Grupo.cs:140-147`), `LAE` (`UT/LAE.cs:48-49`) y `Analizador.InicializaParametros` (`MC/Analizador.cs:424-436`). En Swift: `Sesion.configuracion` inyectada; recarga explícita.

**Peculiaridades a preservar.**
- `PrecioApuesta` etc. son `double` obtenidos de un `float` (`:407-410`).
- `bool.TryParse` solo en `toolbars`; el resto `Convert.ToBoolean` (falla con `"1"`).
- Los `Guardar*` no releen el fichero: usan el documento **en memoria** del objeto; la UI crea un `AConfiguracion` nuevo por operación (`MainWindow.xaml.cs:722`, `VM/CambioPuntosFrmViewModel.cs:63`), de modo que el fichero actual manda.
- `ConfiguracionFrm` guarda hasta 8 secciones con un solo objeto: hasta 8 escrituras completas seguidas (`VM/ConfiguracionFrmViewModel.cs:188-196`).
- `ObtenEquipos`, `ObtenEstadoNotificaciones`, `GuardarFechaUltimaComprobacionNotificaciones`: sin llamadores (no portar).

**Trampas Swift.** T3 (`Float`), T7 (`parsearEs`, `formatearDouble`), T8 (UTF-8 sin BOM, EOL), `Int16` con desbordamiento como error (no `&+`), `split(omittingEmptySubsequences: false)`, `Bool("true")` de Swift es sensible a mayúsculas (usar `parseBoolNET`), edición in-place del DOM (no re-codificar desde una `struct`, perdería nodos desconocidos).

**Casos golden propuestos.**
1. Leer `DT/parametros.free1x2`: puntos 6/3/1; LAE 0,5 / 12 / 14 000 000 / `€`; JB `15,22,29,36,48,61`; partidos 14 y `["5","9","12"]`; desplazamiento 3; 6 toolbars `true`; actualizador `false`; 20 `analisis` `true`; idioma `es-ES`; salir `true`; fecha `03/02/2010`.
2. `GuardarValoresLAE(0.55, 12.5, 14000000, "$")` ⇒ `PrecioApuesta="0,55" PorcentajeDestinadoAl14="12,5" Recaudacion="14000000" Moneda="$"`; resto del fichero idéntico.
3. Lecturas `PrecioApuesta` = `"0,55"` (⇒ 0.550000011920929), `"0.50"` (⇒ 50), `"1.234,5"` (⇒ 1234,5), `" 7 "` (⇒ 7), `"abc"` (error).
4. `toolbars` con atributo ausente / `"1"` / `"false"` / `" True "`.
5. Cadena de los 11 `Guardar*` sobre la semilla ⇒ fichero final (comparar bytes con EOL normalizado, §7).
6. XML sin `aDiferencias` ⇒ error de lectura (y fallback global descrito en F01).

**Evidencia.** `ES/AConfiguracion.cs:35-455`; `VariablesGlobales.cs:62-180`; `DT/parametros.free1x2`.

---

### 6.2 `ArchivoCombinacion` — `Free1X2.Domain/EntradaSalida/ArchivoCombinacion.cs`

**Propósito.** (De)serializar la combinación completa (F02): equipos, pronósticos de la columna base, filtro de columnas, grupos con sus condiciones y tolerancias, controles de grupos/conjuntos e If-Then.

**Tipos y datos (capa de formato; el motor las traduce a `Grupo`/`ControladorGrupos`, ESPEC_01).**
```swift
public struct CombinacionXML: Sendable, Equatable {
    var equipos: [String]                 // "LOCAL - VISITANTE" | "? - ?"
    var pronosticos: [String]             // "1,X,2" | "1,X" | "X" | ""
    var filtroColumnas: String            // ruta tal cual ("" = ninguno)
    var grupos: [GrupoXML]
    var controlGrupos: [ControlXML]       // (grupos: String, fallos: String)
    var controlConjuntos: [ControlXML]    // solo se serializa si count > 1
    var ifThen: IfThenXML?                // nil ⇒ no se escribe <If-Then>
}
public struct GrupoXML { var nombre: String; var partidosActivos: String
                          var filtroParcial: String?        // nil ⇒ sin elemento
                          var condiciones: [CondicionXML]   // orden de la lista del grupo (F02)
                          var tolerancias: [(letras: String, noAciertos: String)]
                          var fallosTolerancias: String }
```
**Entradas / salidas.** `leer(url) throws -> CombinacionXML`; `escribir(_ , a: url) throws` (`GuardaArchivo`). Sin estado global propio, pero la carga original **muta** `ControladorGrupos` (reemplaza `GruposPartidos`, reutiliza `ControlesGrupos[0]`/`ControlesConjuntos[0]`).

**Algoritmo.** Escritura = orden y elementos de F02 (`:299-524`); lectura = `LeeEquipos`, `LeeFiltroColumnas`, `LeePronosticos`, `CargaControladorGrupos` (= `LeeGrupos` → `LeeControlesGrupos` → `LeeControlesConjuntos`, `:97-215`), `CargaIfThen` (`:89-95`):
```
LeeGrupos: para cada hijo i de gruposPartidos (saltando comentarios):
     g = GetGrupoFromXmlNode(hijo); try { g.nombre = hijo.@nombre } catch {}; AddGrupo(g)
GetGrupoFromXmlNode: g.esGrupoBase = (i == 0) en cada iteración (queda: nHijos == 1)
     por hijo no comentario: condicion → g.GetFiltro(@id) + F*Data.leer ; partidosActivos (si @noPartidos != "") ;
                            tolerancias ; filtroParcial → g.archivoFiltroGrupo = @archivo, reinicializar caché
LeeControlesGrupos: i == 0 (índice de hijo, comentarios incluidos) ⇒ ControlesGrupos[0] ; resto ⇒ new + AddControlGrupo
```
**Peculiaridades a preservar.** Las de F02: `EsGrupoBase` por nº de hijos; `filtroParcial archivo=""` tras un ciclo abrir→guardar en el grupo 0; `GetFiltro` devuelve el último filtro si el `id` no existe; último hijo de `Condiciones`/`Grupos` = `Rangos`; `file/@version` nunca se lee; `controlConjuntos` solo si hay >1; `equipos`/`pronostico` sin validar.

**Trampas Swift.** T8 (sin declaración XML, sin BOM, sin salto final); escapes de atributo (`&amp; &lt; &gt; &quot; &#xD; &#xA; &#x9;`); **no** habilitar resolución de entidades/DTD externas en `XMLParser` (`shouldResolveExternalEntities=false`; `XmlTextReader` de .NET procesa DTD por defecto, §8-A22); orden de atributos de `<grupo>` = `id`, `nombre`; `Int` de `id` generados por posición.

**Casos golden propuestos.**
1. Analizador con 3 grupos (g1: NoVariantes + ColProbables + GruposEquipos + Diferencias; g2: tolerancias `(ABC,"0,1")` + `filtroParcial`), `controlGrupos` con 2 controles, `controlConjuntos` con 2, If-Then con 1 condición + 1 grupo ⇒ escribir ⇒ bytes; leer ⇒ re-escribir ⇒ **idempotente salvo** lo descrito arriba.
2. Equipos con `&`, `"`, `<`, `ñ`, un salto de línea ⇒ escapes.
3. Pronósticos `"1,X,2"`, `"1,X"`, `"X"`, `""`.
4. Errores: sin `columnaBase` (NRE), sin `If-Then` (`ifThen==nil`), `Condiciones` sin `Rangos`.
5. Archivo con comentario `<!-- -->` dentro de `controlGrupos` (índice 0 desplazado).

**Evidencia.** `ES/ArchivoCombinacion.cs:41-564`; `ES/DatosFiltroHelper.cs:29-137`; `MC/Grupo.cs:94-134,415-433`; `MC/GrupoPartidos.cs:37-46`; `MC/ControladorGrupos.cs:27-48`; `MC/Analizador.cs:57-80`.

---

### 6.3 `ArchivoCondiciones` — `Free1X2.Domain/EntradaSalida/ArchivoCondiciones.cs`

**Propósito.** Ficheros de condición sueltos (un filtro, un grupo o un If-Then) y lector del If-Then de las combinaciones.
**Tipos y datos.** `CondicionXML` (14 casos, §6.4), `GrupoXML`, `IfThenXML { condiciones: [(si: String, entonces: String)]; rangoCondiciones: String; grupos: [(si: Int16, entonces: Int16, noSi: Bool, noEntonces: Bool)]; rangoGrupos: String }`.
**Entradas / salidas.** `abrir(url) -> Bool` (`false` solo si el XML no se puede cargar); `leeCondicion()`; `leeGrupos()` (último grupo); `leeIfThen() -> IfThenXML?` (nil si no hay `If-Then`); `guardar(filtro)`, `guardar(grupo)`, `guardar(ifThen)`.
**Algoritmo.** Ver F03 y F02 (`If-Then`). `GuardaArchivo(Grupo)` no cierra `gruposPartidos` explícitamente: lo cierra `FinalizaArchivo` y el `Close()` cierra `xml` (`:303-314,291-294,259`); salida bien formada.
**Peculiaridades.** `GetGrupoFromXmlNode` aquí ignora `filtroParcial` (`:148-177`); `AbrirArchivoCombinacion` no cierra el `XmlTextReader` si `Load` falla; `ObtenFiltro`/`AddCondicionesGrupo`/`GetFiltroFromXmlNode` (`:179-249`) no tienen llamadores.
**Trampas Swift.** Las de 6.2; `Convert.ToInt16` en `if`/`then` (rango Int16).
**Casos golden.** Un fichero por filtro (14) con datos no triviales; If-Then vacío (`<Condiciones />`, `<Grupos />`); If-Then con rangos; fichero de grupo; XML mal formado ⇒ `abrir` = false.
**Evidencia.** `ES/ArchivoCondiciones.cs:35-415`.

---

### 6.4 `DatosFiltroHelper`, `IFiltroDatos`, `FiltroDatosBase` y las 14 `F*Data`

**Propósito.** Patrón DTO: cada `F*Data` traduce entre `<condicion>` y el estado de un `IFiltro`. `DatosFiltroHelper` elige la clase por `filtro.NombreFiltro` (`switch`, `ES/DatosFiltroHelper.cs:29-81` lectura; `:83-137` escritura sobre `grupo.Filtros`).
**Tipos y datos.** `enum Filtro` (14 nombres, `EnumFiltros.cs:22-37`); `protocol CondicionCodec { static func leer(_ nodo: ElementoXML, en: inout FiltroEstado) throws; static func escribir(_ f: FiltroEstado, en: inout EscritorXML) }`. `FiltroDatosBase.GetValores("1,2,3") -> [Int32]?` (`nil` si `""`; `Convert.ToInt32` por elemento) (`ES/FiltroDatosBase.cs:33-50`).
**Algoritmo.** Gramática, orden, lectores y fallos de cada clase: **F03.a** y **F03.b**. Escritura: solo si `ContieneDatos`; `id`=`rawValue` del enum; `activa`=`True|False`.
**Peculiaridades a preservar.**
- Los `if`/`else if` encadenados (`ES/FInterrupcionesData.cs:75-121`, `ES/FContactosData.cs:73-115`) equivalen a un `switch` porque los `tipo` son exclusivos; con `tipo` repetido gana la última aparición (cada `Set*` sobrescribe).
- `NoVariantes`: lee los hijos de **cualquier** hijo de `condicion`; `Dibujos`: lee `InnerText` de **todos** los hijos (un comentario añadiría un «dibujo» basura).
- `Diferencias`: orden `partidos` → `aciertos` obligatorio; `""` ⇒ campo no analizado.
- `Formatos123`: 14×3 valoraciones fijas; `Valoracion` en cultura actual; deriva `ValoracionTranformada` (nota: `byte[14,3]` con valores 4/2/1 según el orden descendente de los 3 valores de cada partido; empates resueltos por la cascada `>=`/`>` de `ES/FFormatos123Data.cs:69-122`).
- `GruposEquipos`: 5 comentarios fijos tras `<condicion>`.
- `ColProbables`: `ObtenerColumnasImplicadasRelCP3` usa las primeras N CPs.
**Trampas Swift.** Atributo ausente ⇒ error **tipado** (`ErrorFormato.atributoAusente(nombre)`) en el mismo punto que el NRE de C#; `Convert.ToBoolean` (`parseBoolNET`); `Name.ToUpper()` ⇒ comparar en mayúsculas ASCII; `ObtenerTextoFiguraFromLong` con negativos no termina en C# (`fig >>= 4` aritmético): no se produce con figuras válidas, proteger con `UInt64` y 16 nibbles.
**Casos golden propuestos.** Por filtro: (a) lectura→escritura de un fichero real de Windows; (b) construcción programática con datos en los extremos (todas las casillas, ninguna, tolerancias con letras, figuras `0`, `1-2-3`, `15-15`); (c) fallos: atributo ausente, `values=""` en NoVariantes, `id` desconocido, `tipo` desconocido (ignorado).
**Evidencia.** `ES/F*Data.cs` (rangos de línea en F03.a); `UT/UtilidadesEntradasValores.cs:115-302`.

---

### 6.5 `ArchivoColumnasTexto` / `IArchivoColumnas` — `ES/ArchivoColumnasTexto.cs`, `ES/IArchivoColumnas.cs`

**Propósito.** E/S secuencial de ficheros de columnas (F04) y utilidades de conversión (texto ⇄ número ⇄ `BitArray`).
**Tipos y datos.** `numColumnas: Int32` (privado; `Int(ObtenNumCols())`), `numPartidos: Int` (setter), `TamBuffer = 1<<16`; escritor `UTF8Encoding(false, throwOnInvalid: true)`; Swift: `final class ArchivoColumnasTexto` con `FileHandle`/`InputStream` y buffer de 64 KiB (`@unchecked Sendable` o `actor`).
**Entradas / salidas.** Métodos de `IArchivoColumnas` (`ES/IArchivoColumnas.cs:24-49`). Escritura: se abre perezosamente en la primera llamada (`FileMode.Create`). Lectura: perezosa en `SiguienteColumna`.
**Algoritmo.**
```
init(nombre[, noPartidos]): numColumnas = Int(obtenNumCols())
obtenNumCols(): si !existe ⇒ 0 ; abrir lector; si siguienteColumna(): t = leeColumnaSinComas(); L = t.count + 2; n = bytesDelFichero / L (división entera Int64) ; cerrar lector (lector = nil) ; devolver n
siguienteColumna(): si lector==nil ⇒ abrirlo ; devolver lector.peek() >= 0     // false en EOF
leeColumnaSinComas(): readLine().trim().uppercasedASCII()
leeColumna(): l = readLine(); si l != "" ⇒ l = darFormato(l)   // "" si ya trae comas
              devolver l.uppercasedASCII()
darFormato(l): si l contiene ',' ⇒ ""; si no ⇒ caracteres unidos por ','
guardarCols(s): si escritor==nil ⇒ crear (trunca), numColumnas=0 ; numColumnas++ ; writeLine(s.sinComas)
guardarColsComa(s): igual sin quitar comas
guardarTodasCols([Int]): por cada n ⇒ guardarCols(conv.convNumAColumna(n))     // conComa: guardarColsComa
guardarTodasCols(BitArray): conv = numPartidos != 0 ? Conv(numPartidos) : Conv(); por i con bit ⇒ guardar…(conv.convNumAColumna(i))
guardarCols(Int64): NO-OP
leerTodasColsABitArray(n): max = 3^n; por cada columna c (sin comas): bits[conv.convColumnaANumero(c, n)] = true
```
**Peculiaridades.** `ObtenNumCols` estima por **bytes** y asume `\r\n`; `NumColumnas` tras escribir es exacto. `LeeColumna` devuelve `""` si la línea tiene comas. `LeeColumna()` lanza NRE tras EOF; `LeerTodasColsANumero` lanza con líneas vacías. Constructor lee el fichero aunque se vaya a escribir.
**Trampas Swift.** T8; T9 (división entera `Int64`); `ToUpper`/`Trim` ⇒ ASCII; `String.count` (grafemas) ≠ `.Length` de .NET (UTF-16): para columnas ASCII coincide, usar `utf16.count` si se generalizan; **EOL de salida CRLF** para conservar `ObtenNumCols`; cerrar y volcar el buffer al hacer `Cerrar()` (el destructor C# también cierra).
**Casos golden propuestos.** `TS/Fixtures/cols_a.txt` (LF): `numColumnas`=9, `obtenNumSignos`=14, `leerTodasCols(false)` 10 líneas; el mismo contenido en CRLF (160 B) ⇒ 10; con BOM UTF-8; con comas (`1,X,2,…`: `leeColumna`=`""`, `leerTodasColsSeparadasPorComas` sin comas); línea en blanco intermedia; fichero vacío (0 y `siguienteColumna`=false); escritura `guardarCols("1,X,2")` ⇒ bytes `31 58 32 0D 0A`; `guardarCols(Int64(5))` no crea fichero.
**Evidencia.** `ES/ArchivoColumnasTexto.cs:29-391`; `TS/Fixtures/*.txt` [bytes].

---

### 6.6 `ArchivoIdioma` — `ES/ArchivoIdioma.cs`

**Propósito.** Cargar el diccionario del idioma seleccionado (F11) y listar idiomas disponibles.
**Tipos y datos.** `[String: String]` (en Swift conviene `[(clave, valor)]` + diccionario para reproducir «gana la primera»).
**Algoritmo.**
```
obtenerIdioma(idioma): para cada fichero *.lang de <Base>/Idioma/ (orden del sistema de ficheros): si nombreSinExt == idioma:
      try { para cada línea (Latin-1): p = línea.split('#') ; si !dic.contains(p[0]) ⇒ dic[p[0]] = p[1] } catch { }   // corta en la 1ª línea sin '#'
listaIdiomas(): nombres sin extensión de *.lang, o nil si no hay ninguno
```
**Peculiaridades.** `DirectoryInfo.GetFiles` lanza si `Idioma/` no existe (fuera del `try`) ⇒ `VariablesGlobales` cae a defaults (§8-A21). Dos ficheros con el mismo nombre no pueden coexistir (un único `idioma`). El diccionario **no se usa** (§4).
**Trampas Swift.** Latin-1 (no UTF-8); `Split('#')` conserva vacíos; la lectura parcial tras error es **comportamiento**, no se debe validar el fichero entero.
**Casos golden.** `en-US.lang` ⇒ 142 claves; `ANALIZAR`⇒`Analize`; `ACIERTOS`⇒`""`; `Figuras`; clave `ANÁLISIS DE FALLOS` presente solo si se decodifica Latin-1; `obtenerIdioma("es-ES")` ⇒ vacío; fichero con línea vacía en medio ⇒ diccionario truncado.
**Evidencia.** `ES/ArchivoIdioma.cs:11-61`; `DT/Idioma/en-US.lang` [bytes].

---

### 6.7 `GenerarCPs/*` — `CP/ColumnasProbables.cs`, `DatosHelper.cs`, `CPs.cs`, `IO.cs`, `Valoracion.cs`, `ColumnasProbables.xsd/.xsx`

**Propósito.** Configuración persistente de los «tipos de CPs» y generación de columnas probables desde una valoración 1/X/2 por partido.
**Tipos y datos.**
```swift
struct Valoracion { var uno: Double; var equis: Double; var dos: Double }          // CP/Valoracion.cs
struct TipoCP { var tipo: Int32; var nombre: String }                                // tabla "Tipos de CPs"
struct ConfigCP { var tipo: Int32?; var desde: Int32?; var hasta: Int32?
                  var forzarFijos: Bool?; var numFijos: Int32?
                  var forzarDobles: Bool?; var numDobles: Int32?; var forzarTriples: Bool? }   // "Configuracion de CPs"
```
`.xsd`/`.xsx`: solo diseño (no se cargan en tiempo de ejecución); el esquema efectivo es el del DataSet tipado (`CP/ColumnasProbables.cs:130-160,278-287,560-583`).
**Entradas / salidas.** `DatosHelper.obtenerDatos()` (vacío si no existe el fichero), `guardarDatos(ds)` (F05); `CPs.crearCPs(config, valoracion) -> [[String?]]` (matriz `N × nConfigs`); salida a F06.
**Algoritmo `CrearCPs`** (`CP/CPs.cs:16-90`; `N = NumeroPartidos`):
```
para j en 0..<filas:
   tipo = 3
   si ForzarFijos[j] no vacío y Convert.ToBoolean ⇒ tipo = 1
   si ForzarDobles[j] no vacío y Convert.ToBoolean ⇒ tipo = 2          // gana sobre fijos (se evalúa después)
   desde = Int16(row.Desde) ; hasta = Int16(row.Hasta)                    // null ⇒ excepción (no se capturan)
   caso 1: col[k,j] = ObtenerFijos(Int16(NumFijos), valor)[k]  ∀k
   caso 2: col[k,j] = ObtenerDobles(Int16(NumDobles), valor)[k] ∀k
   caso 3: ∀k: s = ""; si desde ≤ valor[k].uno ≤ hasta ⇒ s+="1"; idem equis ⇒ "X"; dos ⇒ "2"
               si s.isEmpty y ForzarTriples ⇒ s = "1X2" ; col[k,j] = s
ObtenerFijos(max, v):  si max == N ⇒ ObtenerTodosFijos(v)
   t = copia(v) ; m = sortAsc([t0.uno,t0.equis,t0.dos,t1.uno,…]) ; m = reversed(m)
   para i en 0..<max: para j en 0..<t.count:                                   // primer partido con ese valor
        si t[j].uno == m[i] ⇒ s[j]="1" ; t[j].uno = 0 ; break
        si t[j].equis == m[i] ⇒ s[j]="X" ; t[j].equis = 0 ; break
        si t[j].dos == m[i] ⇒ s[j]="2" ; t[j].dos = 0 ; break
ObtenerDobles(max, v): igual con m ascendente; s[j]="X2"(uno) | "12"(equis) | "1X"(dos) ; el valor usado pasa a 99.99
ObtenerTodosFijos: s[i]="1" si uno > equis && uno > dos ; "X" si equis > uno && equis > dos ; "2" si dos > …   // estrictos: empate ⇒ nil
ObtenerTodosDobles: "X2" si uno < equis && uno < dos ; "12" si equis < uno && equis < dos ; "1X" si dos < equis && dos < uno
```
Salida F06: cada CP = línea de `N` tokens (`nil`→`" "`) separados por `,`, más `\r\n`; el fichero acaba en línea vacía (`WriteLine` extra).
**Estado global.** `VariablesGlobales.NumeroPartidos` (tamaño de matrices).
**Peculiaridades a preservar.** Empates en `ObtenerFijos/Dobles`: se asigna al **primer partido** (por índice) y con prioridad `1`,`X`,`2`; un valor `0` ya usado puede volver a coincidir si `m[i]==0`; `99.99` como centinela; `OrdenarValoracion` rellena `N*3` posiciones aunque `valor.Length < N` (ceros); `valor[k]` nulo ⇒ NRE; `Array.Sort` de `double` (NaN primero); `signos` puede quedar con `nil`.
**Trampas Swift.** Comparación exacta de `Double` (`==`), no usar tolerancias; `Array.Sort` ⇒ `sorted()` (para `Double` sin NaN equivalente); `Convert.ToInt16` ⇒ `Int16(exactly:)` con error; el DataSet requiere un lector/escritor XML propio con **prefijos `d2p1`** y orden de atributos (F05).
**Casos golden propuestos.** (1) `ColumnasProbables.xml` de la semilla ⇒ 1 tipo (`Normal`), 2 configuraciones; leer→escribir ⇒ idéntico (EOL normalizado). (2) Valoración sintética de 14 partidos con empates y ceros, configuraciones `{desde:0,hasta:100,ForzarFijos:true,NumFijos:14}`, `{NumFijos:5}`, `{ForzarDobles:true,NumDobles:3}`, `{desde:40,hasta:60,ForzarTriples:true}`. (3) Fichero F06 completo para 2 tipos.
**Evidencia.** `CP/CPs.cs:16-221`; `CP/DatosHelper.cs`; `CP/IO.cs:13-45`; `VM/GenerarCPsViewModel.cs:84-302`; `DT/ColumnasProbables.xml` [bytes].

---

### 6.8 Clases con E/S fuera de `EntradaSalida/` (formato en §1; algoritmos en ESPEC_05/06)

| Clase | Formato | Resumen del contrato de E/S | Evidencia |
|-------|---------|------------------------------|-----------|
| `Porcentajes` | F07 | detección por la primera línea (coma ⇒ espacio ⇒ TAB), 5 lectores, 3 escritores, histórico 44, portapapeles; cultura §2.1 | `UT/Porcentajes.cs:81-547` |
| `LAE` | F08 | `GrabarJornada(acertantes)`: upsert por `(temporada, jornada)` y `Premios = Round(Recaud/Precio·Pct/100/Acertantes + 0.005, 2)` | `UT/LAE.cs:30-151` |
| `CompresorZip` | F16 | `Comprimir`/`Descomprimir` (bitmap LSB-first, ZIP de una entrada) | `UT/CompresorZip.cs:41-137`; `TS/CompresorZipTests.cs` |
| `ControladoresImpresion` | F12 | lista de impresoras soportadas | `MC/ControladoresImpresion.cs:26-61` |

**Casos golden.** `CompresorZip`: bitmap con bits {0,1,7,8,9,1234,100000,2391484,4782968} ⇒ `payload.count == 597 872`, descomprimir ⇒ mismos bits (test existente); histórico L.A.E.: upsert de una jornada nueva y de una existente; `Porcentajes`: un fichero por formato (1, 3, 42, 43, 44) con `.` y `,` decimales.

---

### 6.9 Dominio online — `ON/JornadaQuiniela.cs`, `JornadaQuinielaParser.cs`, `CatalogoEquipos.cs`, `CatalogoEquiposParser.cs`

**Propósito.** Modelos puros y parsers defensivos de las respuestas JSON (F18/F19); sin red ni UI.
**Tipos y datos.**
```swift
public struct PartidoJornada: Sendable, Equatable { public var n: Int32; public var local: String; public var visitante: String }
public struct JornadaQuiniela: Sendable, Equatable {
    public var pais = "", temporada = "" ; public var jornada: Int32 = 0 ; public var fecha = ""
    public var partidos: [PartidoJornada] = [] ; public var pleno15: PartidoJornada? }
public struct DivisionEquipos: Sendable, Equatable { public var id = "", nombre = "" ; public var equipos: [String] = [] }
public struct CatalogoEquipos: Sendable, Equatable { public var pais = "" ; public var divisiones: [DivisionEquipos] = []
    public func equiposPlano() -> [String] }
public struct ErrorFormato: Error, Equatable { public let mensaje: String }      // = FormatException
public enum JornadaQuinielaParser { public static let numeroPartidos = 14 ; public static func parse(_ json: String) throws(ErrorFormato) -> JornadaQuiniela }
public enum CatalogoEquiposParser { public static func parse(_ json: String) throws(ErrorFormato) -> CatalogoEquipos }
```
**Algoritmo.** Tablas de §5.4 (orden de validación = orden de las filas). `EquiposPlano`: ver §5.4.
**Peculiaridades.** `pais`/`temporada` no se validan más allá de «string presente»; `fecha` opcional; campos extra ignorados; `pleno15` no objeto ⇒ ignorado; recorte solo en `EquiposPlano`; los mensajes `FormatException` de JSON mal formado llevan texto de `System.Text.Json` (no reproducible): basta que contengan `JSON`.
**Trampas Swift.** Enteros estrictos (§5.6); `String` de JSON con `\uXXXX`/`\/` ya decodificados por el parser; `JSONSerialization` acepta `NaN`/fragmentos según opciones: usar opciones por defecto y rechazar profundidad > 64 si se quiere paridad; propiedades duplicadas (§8-A5).
**Casos golden.** Los 6 JSON (`quiniela-{es,mx}-actual`, `equipos-{es,mx}`, `real-{es,mx}`) ⇒ campos esperados de `TS/JornadaQuinielaParser*Tests.cs`, `TS/CatalogoEquiposParserTests.cs` (14 partidos; ES con pleno 15 `Cádiz`/`Tenerife`; real-ES `jornada`=20260621 y `Pleno15.Local`=`España`; MX sin pleno). Errores sintéticos (12 + 10 mensajes de §5.4): `""`, basura `{ esto no es json válido ][ `, raíz array, 2 partidos, `n` desordenado, `local:""`, `jornada:"38"` (string), `jornada:38.0`, `jornada:3000000000`, 15 partidos con `n=15` en el último, `pleno15:null`, catálogo `divisiones:[]`, `equipos:[]`, equipo `""`, dedupe `["Real Madrid","Sevilla"] + ["sevilla ","Real Madrid","Eibar"]` ⇒ `["Real Madrid","Sevilla","Eibar"]`.
**Evidencia.** `ON/*.cs`; `TS/*Tests.cs`; `TS/Fixtures/online/*` [bytes].

---

### 6.10 `QuinielaOnlineService` — `SV/QuinielaOnlineService.cs`

**Propósito.** Cliente HTTP opcional con guarda anti-doble-petición, reintento acotado y traducción de errores a `QuinielaOnlineException`.
**Tipos y datos.** `ResultadoJornada(Jornada, DesdeCache: Bool, GuardadoUtc: Date?)` (`:31`); constantes de §5.1; `ErrorOnline` (mensaje en español).
**Entradas / salidas.** `obtenerJornada(pais:)`, `obtenerEquipos(pais:)`, `baseUrl` (estático).
**Algoritmo.** §5.2 y §5.3.
**Estado global.** `ultimaDescargaUtc` (estático, concurrente) y el `HttpClient` único; instancia única `Instancia` (`:60`). Swift: `actor` singleton.
**Peculiaridades.** La marca anti-doble se pone solo tras éxito (`:184-189`); la guarda solo corta si la caché es **válida**; los reintentos no acumulan: un solo reintento por llamada; el timeout de 10 s se aplica a cada intento.
**Trampas Swift.** URLSession cachea por defecto (desactivar); `timeoutIntervalForRequest` es inactividad, usar también `…ForResource`; ATS; `Task.sleep` cancelable; los mensajes literales.
**Casos golden.** Con un servidor local falso: 200 (jornada y catálogo), 404 con `{"error":"sin_jornada","mensaje":"No hay jornada publicada para ES."}`, 429 `Retry-After: 2` + 200 (un reintento), 429 `Retry-After: 60` (error sin reintento; mensaje del cuerpo o `~60 s`), 429 sin cabecera (`~60 s`), 429 `Retry-After: 0` (`~0 s`), 503 con HTML (⇒ `El servicio respondió con código 503 (ServiceUnavailable).`), cuerpo no JSON, timeout >10 s, servidor caído, doble llamada <60 s con y sin caché, país `"ES "` (se normaliza), país `"fr"` (error con el original).
**Evidencia.** `SV/QuinielaOnlineService.cs:53-366`; `docs/API_CLUBPROGOL.md`; `scripts/stub-api.ps1` (`?sim=404|429`).

### 6.11 `JornadaCache` — `SV/JornadaCache.cs`

**Propósito.** Caché local por país del JSON crudo de la última jornada.
**Tipos y datos.** Paths §5.5; `FechaGuardado: Date?` = *mtime*.
**Algoritmo.** Tabla §5.5.
**Peculiaridades.** `PaisMasReciente` no valida contenido; escritura atómica; todo best-effort con traza (`Log`).
**Trampas Swift.** Resolución del *mtime* (HFS+/APFS: nanosegundos; el empate `>` estricto casi nunca ocurre); permisos/sandbox; no confundir `Caches/` (purgable) con `Application Support`.
**Casos golden.** Escribir→releer→parsear (test `TS/JornadaCacheRoundTripTests.cs`); caché corrupta ⇒ `tryCargar`=false; los dos países con *mtime* distinto/iguales; `.tmp` huérfano; país inválido ⇒ nil.
**Evidencia.** `SV/JornadaCache.cs:26-181`; `TS/JornadaCacheRoundTripTests.cs`.

### 6.12 `PaisesOnline` — `SV/PaisesOnline.cs`

`OpcionPais(Nombre, Codigo)` (`ToString()` = nombre); `Todos` = `[("España","es"), ("México","mx")]`; `PorDefecto` = España; `PorCodigo(c)`: `c` nulo/blanco ⇒ por defecto; si no, comparación `OrdinalIgnoreCase` **sin `Trim`**; sin coincidencia ⇒ por defecto (`SV/PaisesOnline.cs:25-49`). Swift: `struct OpcionPais: Identifiable, Hashable`.

### 6.13 `IdiomaApp` / 6.14 `TemaApp` — `SV/IdiomaApp.cs`, `SV/TemaApp.cs`

**Propósito.** Preferencia persistente de idioma (`Espanol`|`Ingles`, defecto `Espanol`) y de tema (`Sistema`|`Claro`|`Oscuro`, defecto `Sistema`) con tolerancia total a errores.
**Algoritmo.** `Cargar()`: fichero ausente ⇒ defecto; `JsonDocument.Parse(ReadAllText)`; raíz objeto con propiedad string y `Enum.TryParse(ignoreCase)` ⇒ valor; si no ⇒ defecto (+traza WARN); cualquier excepción ⇒ defecto (+traza ERROR). `Cambiar(op)`: sin cambio si igual; fija estado, aplica (`PrimaryLanguageOverride` / `RequestedTheme` de la raíz) y `Guardar` (atómico). `CodigoBcp47` = `"en"` ⇔ `Ingles`, si no `"es"`.
**Mac.** `UserDefaults` + `@AppStorage`; tema vía `NSApp.appearance` (`nil`=sistema, `.aqua`, `.darkAqua`); idioma §4.3.
**Casos golden.** `{"idioma":"Ingles"}`, `{"idioma":"ingles"}`, `{"idioma":"1"}`, `{"idioma":"99"}`, `{"idioma":3}`, `{}`, `[]`, basura, vacío; análogo para tema.
**Evidencia.** `SV/IdiomaApp.cs:34-157`, `SV/TemaApp.cs:30-175`.

### 6.15 `AppState` — `SV/AppState.cs`

**Propósito.** Estado compartido del proceso (equivale a campos de `MainForm`): `Analizador` vivo, grupo en pantalla, rutas de la combinación y del filtro de columnas, jornada online actual.
**Tipos y datos.** `analizador` (inicial `new Analizador()`), `grupoPantalla: Int = 0`, `nombreArchivoComb = ""`, `archivoFiltroCols = ""`, `jornadaActual: JornadaQuiniela? = nil`; eventos `Cambiado` y `JornadaCambiada`; estáticos de traspaso `GrupoEnEdicion: Grupo?`, `UsarAnalizadorCompartido: Bool`.
**Algoritmo.** Cada setter notifica `Cambiado` (el de `GrupoPantalla` solo si cambia); `JornadaActual` notifica `JornadaCambiada` y `Cambiado`; `GrupoActual`: si el índice está fuera de rango lo **restablece a 0** y devuelve el grupo 0; `NuevaCombinacion()`: nuevo `Analizador`, `grupoPantalla=0`, rutas `""`, **una** notificación, **no** toca `jornadaActual`.
**Mac.** `@MainActor @Observable final class Sesion` (plan M-R4: sin singleton global; inyectar por `Environment`); los dos estáticos de traspaso se sustituyen por parámetros de navegación.
**Casos golden.** Secuencia de mutaciones ⇒ conteo de notificaciones; `GrupoActual` con índice 7 y 1 grupo ⇒ 0.
**Evidencia.** `SV/AppState.cs:24-167`.

### 6.16 `Log` — `SV/Log.cs`

**Propósito.** Traza mínima a fichero, nunca lanza. Formato y rotación en F22.
**Algoritmo.** `Escribir(nivel, msg)`: `linea = "{UTC yyyy-MM-dd HH:mm:ss.fff, InvariantCulture}Z [{nivel}] {msg}"`; `Debug.WriteLine`; bajo `lock`: crear carpeta, `RotarSiProcede` (si existe y `Length >= 1 048 576` ⇒ mover a `log.1.txt`, sobrescribiendo), `AppendAllText(linea + NewLine)`. `Error(ctx, ex)` ⇒ `ctx :: Tipo: msg <- Tipo: msg … (≤5)` + `NewLine` + pila.
**Mac.** `os.Logger` + fichero en `~/Library/Logs/Free1X2/`; mismo formato de línea (útil para soporte). Fecha en UTC con formato `yyyy-MM-dd HH:mm:ss.SSS` + `Z`.
**Evidencia.** `SV/Log.cs:27-134`.

---

## 7. Verificación y generación de golden

| Qué | Cómo | Comparador |
|-----|------|-----------|
| Lectores (F01–F03, F05, F09–F13, F17–F21, F24) | el modelo esperado de cada fichero del corpus (ESPEC_12 §6) se escribe a mano leyendo el lector C#; el test Swift lee el fichero y compara el modelo | igualdad exacta (`Double` por bits) |
| Escritores (F01–F06, F08, F10, F13, F16, F17, F20–F21) | los bytes esperados se escriben a mano leyendo el escritor C# (CRLF, ESPEC_12 §1.1); Swift escribe el mismo modelo | **bytes**; F16: payload descomprimido |
| Errores de lectura | pares (entrada defectuosa → tipo de fallo) | mismo punto de fallo (error tipado) |
| Online | los 6 JSON + casos sintéticos de 6.9; servidor local falso para 6.10 | mensajes exactos |
| Corpus real | ≥3 `.comb`, ≥1 fichero de cada extensión F03, `parametros.free1x2` de usuarios, `.dat`/`.txt` con acentos, `InfoJornadasLAE.txt` real | round-trip lectura→escritura |

Casos mínimos adicionales: ficheros con **BOM**, con **CRLF/LF/CR**, con **declaración XML y `encoding="ISO-8859-1"`**, con atributos que contienen `& < > " \n`, y con nombres de equipo con `ñ á é ü ç` y caracteres fuera de Latin-1 (`ł`, `ő`) para F10.

---

## 8. Abiertas — estado y decisión de cada entrada en [`DECISIONES.md`](DECISIONES.md) §3

> Cosas **no verificadas** o que requieren decisión del dueño (R2). Cada una afecta a bytes o a comportamiento observable.

| ID | Tema | Detalle |
|----|------|---------|
| **A1** | Política de EOL/BOM al escribir | Windows escribe `\r\n` y el repo/semillas están en LF (`.gitattributes:2`). `ObtenNumCols` depende de CRLF (F04). Propuesta: CRLF en `.txt` y XML, UTF-8 sin BOM salvo equipos `.txt` (con BOM, como WinUI). Cerrada: los golden llevan CRLF escrito a mano (ESPEC_12 §1.1). |
| **A2** | No existe ningún `.comb` real en el repo | `git ls-files` no lista ninguno; la gramática de F02/F03 sale de lector+escritor. Hay que aportar `.comb` reales (con If-Then, tolerancias, ColProbables, FormatosSignos, Formatos123, Diferencias) para validar. El ejemplo de F02 es **derivado**. |
| **A3** | Exactitud de `XmlTextWriter` | Ausencia de declaración en `.comb`, `\r\n` en la sangría, `" />"`, escapes y ausencia de salto final están confirmados solo en parte por los bytes de `parametros.free1x2`/`ColumnasProbables.xml` (que además pasaron por `XmlDocument`/`DataSet`). Verificar con un `.comb` generado en Windows. |
| **A4** | Gramáticas delegadas | El texto canónico de `values` de cada filtro (getters `Get*`) y las líneas `If`/`Then` están en ESPEC_01–04; aquí solo se trataron como cadenas opacas, salvo NoVariantes (`n[L],…`) y Dibujos (`a+b`). |
| **A5** | JSON con propiedades duplicadas | `JsonElement.TryGetProperty` (qué valor devuelve) y `JSONSerialization` (el último) pueden diferir. Sin evidencia en el repo. |
| **A6** | Enteros JSON y `Enum.TryParse` | `TryGetInt32` rechaza `38.0`/`1e2` y números > Int32 **[.NET]**; `Enum.TryParse` acepta cifras no definidas (`"99"`) **[.NET]**. Sin fixture que lo pruebe. |
| **A7** | Equipos `.txt` antiguos | El legacy los escribía en ANSI (cp1252, `Pronosticos.cs:508-570`); el lector WinUI (`File.ReadAllLines`) los interpreta como UTF-8 y corrompe acentos. Propuesta: leer UTF-8 estricto con *fallback* a Windows-1252, escribir UTF-8 con BOM. Confirmar con el dueño. |
| **A8** | `.lang` | ¿Se importa a `Legacy.xcstrings` o se descarta (no se aplica hoy)? Tabla de claves simbólicas pendiente de las espec. de UI (08–10). |
| **A9** | `FileIO.WriteLinesAsync` (WinRT) | Codificación, BOM y EOL final de los `.lst` de DiFiltros/CombinarFiltros/Tramos no se pueden deducir del repo. |
| **A10** | Valores por defecto de `HttpClient` | Redirecciones (hasta 50), descompresión automática desactivada, cookies, ausencia de `User-Agent`: conocimiento de .NET, no del repo. Un WAF de WordPress podría tratar distinto el `User-Agent` de `URLSession`. |
| **A11** | Nombres de `HttpStatusCode` | El mensaje genérico imprime el nombre del enum .NET (`ServiceUnavailable`). Decidir si se conserva literalmente (tabla de ~40 nombres) o se sustituye por `HTTPURLResponse.localizedString(forStatusCode:)`. |
| **A12** | `PrecioApuesta.ToString("0.00")` | Redondeo de .NET Core 3.0+ para valores con >2 decimales (p. ej. `0.125`) y representación binaria; los precios habituales (0,50/0,75) no se ven afectados. |
| **A13** | Semillas ya editadas | Si una versión futura añade atributos a `parametros.free1x2`/`Equipos/*.dat`, no hay estrategia de fusión (la copia inicial no sobrescribe). |
| **A14** | Rutas absolutas heredadas | `.comb` y `.lst` contienen rutas de Windows; falta decidir si en Mac se escriben absolutas (como hoy) o relativas al fichero, y la heurística de §3.2. |
| **A15** | Cultura del oráculo | Se asume `es-ES` para todos los puntos «cultura actual» (C3–C9). Si los ficheros de ejemplo del dueño se crearon con otra cultura (p. ej. `Formatos123.valoracion` con punto), la interpretación cambia. Pedir muestras reales. |
| **A16** | App Sandbox | Plan §7.3 pendiente: condiciona `~/Documents/Free1X2`, rutas heredadas (necesitarían *security-scoped bookmarks*) y el entitlement de red. |
| **A17** | Impresión | ¿Se conservan `imprebol.cfg`/`impresoras.cfg` con `NSPrintOperation`? Probablemente obsoletos. |
| **A18** | `Jornadas/Resultados.txt` | No se distribuye ni hay ejemplo; el consumo real está en el escrutinio (ESPEC_05, vía `DataSet`). Solo se documentó el lector de las pantallas. |
| **A19** | Lectura de `ColumnasProbables.xml` | `DataSet.ReadXml(…, Auto)` con el esquema tipado: comportamiento ante elementos/atributos desconocidos o atributos sin cualificar no verificado. |
| **A20** | Documentación desfasada | `docs/API_CLUBPROGOL.md` cita `App.xaml.cs:45-61` y `JornadaCache.cs:89-114`; hoy son `App.xaml.cs:76-95` y `JornadaCache.cs:119-146`. Menor. |
| **A21** | Carpeta `Idioma/` ausente | `DirectoryInfo.GetFiles` lanza fuera del `try` (`ES/ArchivoIdioma.cs:16`) y `VariablesGlobales` cae a `SetDefaultValues` (accidente). En Mac la carpeta siempre estará en el bundle; decidir si se replica el fallo (no). |
| **A22** | DTD/entidades en XML | `XmlTextReader` resuelve DTD por defecto en .NET; en Swift se desactiva. No hay ficheros del repo con DTD. |
| **A23** | Espacios en blanco en XML | Se asume `PreserveWhitespace=false` (nodos de solo espacios descartados) por el comportamiento por defecto de `XmlDocument` **[.NET]**; los lectores indexan `ChildNodes[0]`, así que un fichero con espacios mixtos importa. |
| **A24** | `NoVariantes values=""` | `SetNoVariantes("")` lanza `IndexOutOfRange` (`MC/FiltroNoVariantes.cs:231-254`); no se verificó si la UI puede producir ese valor al guardar. |
