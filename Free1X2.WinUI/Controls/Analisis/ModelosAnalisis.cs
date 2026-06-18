// Free1X2 · WinUI 3 — WIN3
using System.Collections.Generic;

namespace Free1X2.WinUI.Controls.Analisis
{
    // Modelos de presentación reutilizables para los visores de análisis (WinUI).
    // Reproducen la información que el WinForms pintaba con los UserControls
    // CtrlAnalisis* (Free1X2/UI/Controls/Analisis/) usando rejillas nativas:
    //   - CtrlAnalisisVX2/SSeguidos/Interrupciones/Contactos/Pesos/Distancias:
    //       cabecera de índices (0..N) + filas de conceptos, cada celda = nº de columnas.
    //   - CtrlAnalisisSimetrias: cabecera "Aciertos" + fila "Columnas".
    //   - CtrlAnalisisDiferencias_Individual: cabecera (1..N) + 6 filas (Var/Equis/Dibujos/Doses/Interrup/Formatos).
    //   - CtrlDataGridViewCPs: tres matrices (Aciertos / Aciertos seguidos / Fallos seguidos).
    //   - CtrlAnalisisFormatosSignos: tablas de Líneas y Globales (clave -> nº columnas).
    // NUNCA se fabrican datos: todos los valores provienen del ContenedorAnalisis del dominio.

    /// <summary>Una celda de una matriz de análisis (valor numérico ya formateado a texto).</summary>
    public sealed class CeldaAnalisis
    {
        public CeldaAnalisis(string valor, bool esCabecera = false)
        {
            Valor = valor;
            EsCabecera = esCabecera;
        }

        /// <summary>Texto a mostrar (regla anti-crash: int -> string para TextBlock.Text).</summary>
        public string Valor { get; }

        /// <summary>Si la celda es un índice de cabecera (estilo resaltado, como Color.NavajoWhite del WinForms).</summary>
        public bool EsCabecera { get; }
    }

    /// <summary>
    /// Una fila de una matriz de análisis: una etiqueta de concepto + sus celdas
    /// (réplica de cada fila de CtrlCasilla del WinForms, p. ej. "Variantes 0:.. 1:..").
    /// </summary>
    public sealed class FilaMatrizAnalisis
    {
        public FilaMatrizAnalisis(string etiqueta, IReadOnlyList<CeldaAnalisis> celdas, bool esCabecera = false)
        {
            Etiqueta = etiqueta;
            Celdas = celdas;
            EsCabecera = esCabecera;
        }

        /// <summary>Etiqueta de la fila (concepto). Vacía en la fila de cabecera de índices.</summary>
        public string Etiqueta { get; }

        /// <summary>Celdas de la fila, ya alineadas por columna con la cabecera.</summary>
        public IReadOnlyList<CeldaAnalisis> Celdas { get; }

        /// <summary>Marca la fila como cabecera (índices) para diferenciarla visualmente.</summary>
        public bool EsCabecera { get; }
    }

    /// <summary>
    /// Una matriz de análisis completa: una cabecera de índices opcional + filas de conceptos.
    /// Reemplaza el volcado de texto plano previo por una rejilla alineada equivalente a
    /// los UserControls de análisis del WinForms.
    /// </summary>
    public sealed class MatrizAnalisis
    {
        public MatrizAnalisis(IReadOnlyList<FilaMatrizAnalisis> filas)
        {
            Filas = filas;
        }

        /// <summary>Filas de la matriz (la primera suele ser la cabecera de índices).</summary>
        public IReadOnlyList<FilaMatrizAnalisis> Filas { get; }
    }

    /// <summary>Una entrada (valor -> nº columnas) de una columna de valoración o de formatos.</summary>
    public sealed class EntradaValorAnalisis
    {
        public EntradaValorAnalisis(string clave, string columnas)
        {
            Clave = clave;
            Columnas = columnas;
        }

        /// <summary>Clave/valor (índice). Texto para TextBlock.Text.</summary>
        public string Clave { get; }

        /// <summary>Nº de columnas asociado a esa clave. Texto para TextBlock.Text.</summary>
        public string Columnas { get; }
    }

    /// <summary>
    /// Una columna de valoración (Global/Unos/Equis/Doses) con su lista de entradas no nulas
    /// (réplica de CtrlAnalisisValoraciones: sólo se muestran los valores con columnas != 0).
    /// </summary>
    public sealed class ColumnaValoracion
    {
        public ColumnaValoracion(string titulo, IReadOnlyList<EntradaValorAnalisis> entradas)
        {
            Titulo = titulo;
            Entradas = entradas;
        }

        public string Titulo { get; }
        public IReadOnlyList<EntradaValorAnalisis> Entradas { get; }
        public bool TieneEntradas => Entradas.Count > 0;
    }
}
