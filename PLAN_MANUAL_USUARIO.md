# Plan - Manual de Usuario Free1X2
*Estado: DIFERIDO - ejecutar solo después de completar PLAN_UI_MODERNIZACION.md*

---

## Objetivo

Crear manual de usuario completo con capturas de pantalla de la interfaz modernizada.  
**Audiencia**: usuarios de Quiniela española que conocen el dominio pero no necesariamente el software.

---

## Estructura del manual

### Capítulo 1 — Introducción
- Qué es Free1X2 y para qué sirve
- Conceptos básicos: Quiniela, columnas, 1/X/2, triples, dobles, simples
- Requisitos del sistema (Windows 10/11, .NET 8)
- Instalación y primer arranque

### Capítulo 2 — La interfaz principal
- Captura: MainForm completo con anotaciones
- Descripción de cada barra de herramientas (tsFree, tsArchivo, tsOperaciones, tsCombinacion, tsFiltros, tsUtilidades)
- Descripción del menú principal
- Barra de estado: qué indica cada panel

### Capítulo 3 — Gestión de equipos
- Captura: GestorEquiposFrm
- Añadir un equipo nuevo (AgregarEquipoFrm)
- Editar datos de equipo
- Importar equipos desde archivo

### Capítulo 4 — Crear y gestionar columnas
- Qué es una "columna" en el contexto de Quiniela
- Captura: flujo de creación de columna
- Tipos de columnas: simples, dobles, triples
- Guardar y cargar columnas

### Capítulo 5 — Sistema de filtros
*Una sección por filtro con captura + explicación de cada parámetro*
- Filtro de Contactos
- Filtro de Diferencias
- Filtro de Figuras
- Filtro de Simetrías
- Filtro de Distancias
- Filtro de Interrupciones
- Filtro de Pesos / Valoraciones
- Filtro de Formatos 123
- (resto de los 20+ filtros)

### Capítulo 6 — Análisis de columnas
- Captura: cada panel de análisis
- Cómo interpretar el análisis de contactos
- Cómo interpretar el análisis de diferencias
- Análisis de simetrías
- Análisis de distancias
- Análisis de pesos/valoraciones

### Capítulo 7 — Banco de pruebas (BancoPruebasFrm)
- Captura: interfaz completa
- Paso 1: carga de histórico
- Paso 2: configuración de prueba
- Paso 3: ejecución
- Paso 4: interpretación de resultados
- Cómo leer las estadísticas de éxito

### Capítulo 8 — Combinaciones y reducción
- Concepto de reducción de columnas
- Captura: TramificarForm
- Algoritmos disponibles
- Configuración de parámetros
- Exportar resultado final

### Capítulo 9 — El boleto
- Captura: BoletoFrm
- Cómo leer el boleto generado
- Imprimir el boleto
- Exportar a PDF/imagen

### Capítulo 10 — Estadísticas históricas
- Captura: VisorEstadisticas
- Cómo importar histórico de resultados
- Interpretar gráficos
- Reportes disponibles (DibForm, DibRepFrm)

### Capítulo 11 — Configuración
- Captura: ConfiguracionFrm
- Opciones disponibles
- Rutas de datos
- Opciones de impresión

### Apéndice A — Glosario
- 1X2: victoria local, empate, victoria visitante
- Columna: combinación de pronósticos para los 14 partidos
- Reducción: algoritmo para cubrir resultados con menos columnas
- Tramificación: técnica para distribuir apuestas
- Escrutinio: verificación de aciertos contra resultado real
- (completar con términos del dominio)

### Apéndice B — Atajos de teclado
- Lista completa de atajos por función

### Apéndice C — Formatos de archivo
- Formatos de importación/exportación soportados
- Estructura de archivos .cp (columnas)
- Estructura de archivos de histórico

---

## Proceso de creación (instrucciones para el agente)

1. **Requisito previo**: la UI modernizada debe estar completa y compilando.
2. **Arrancar la app** en modo de desarrollo.
3. **Por cada sección**: navegar a la función, capturar screenshot, anotar controles clave.
4. **Herramienta de screenshots**: usar `screencapture` (macOS) o `PrintScreen` + anotaciones.
5. **Formato de salida**: Markdown + imágenes PNG en carpeta `docs/manual/img/`.
6. **Idioma**: español (castellano), lenguaje claro y sin jerga técnica de programación.
7. **Revisar con el usuario** cada capítulo antes de continuar al siguiente.

---

## Estado

**DIFERIDO** — Iniciar solo cuando el usuario apruebe la Fase 7 (Pulido Final) de la modernización UI.
