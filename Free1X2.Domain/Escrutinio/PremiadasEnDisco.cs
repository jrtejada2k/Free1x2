// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Free1X2.Escrutinio
{
    /// <summary>
    /// Las columnas premiadas de un escrutinio, en un fichero temporal y no en memoria (#5): 14 triples
    /// contra 1.000 ganadoras dan 19 millones (3,7 GB en memoria en la Mac; más en .NET, un objeto por
    /// premiada). Una premiada por línea (columna⇥fichero⇥jornada⇥premio⇥nº columna⇥nº boleto, el fichero
    /// por su posición en una lista aparte); se guarda dónde empieza cada página para leerlas por páginas.
    /// Mismo formato que la Mac (PremiadasEnDisco.swift).
    /// </summary>
    public sealed class PremiadasEnDisco
    {
        public const int PorPagina = 1000;

        // El byte donde empieza cada página y, al final, el tamaño del fichero.
        private readonly long[] inicios;
        private readonly List<string> ficheros;

        internal PremiadasEnDisco(string ruta, int total, long[] inicios, List<string> ficheros)
        {
            Ruta = ruta;
            Total = total;
            this.inicios = inicios;
            this.ficheros = ficheros;
        }

        public string Ruta { get; }

        public int Total { get; }

        public int Paginas => inicios.Length - 1;

        /// <summary>Las premiadas de la página <paramref name="p"/> (desde 0), en el orden del escrutinio.</summary>
        public List<ColumnasPremiadas> Pagina(int p)
        {
            var r = new List<ColumnasPremiadas>();
            if (p < 0 || p >= Paginas) return r;
            byte[] datos = new byte[inicios[p + 1] - inicios[p]];
            using (var f = new FileStream(Ruta, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                f.Seek(inicios[p], SeekOrigin.Begin);
                f.ReadExactly(datos);
            }
            foreach (string linea in Encoding.UTF8.GetString(datos).Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                // La columna va tal cual (puede llevar tabuladores): los 5 últimos campos son los números.
                string[] c = linea.Split('\t');
                int n = c.Length - 5;
                r.Add(new ColumnasPremiadas
                {
                    Columna = string.Join('\t', c, 0, n),
                    Fichero = ficheros[int.Parse(c[n], CultureInfo.InvariantCulture)],
                    Jornada = int.Parse(c[n + 1], CultureInfo.InvariantCulture),
                    Premio = int.Parse(c[n + 2], CultureInfo.InvariantCulture),
                    NoColumna = int.Parse(c[n + 3], CultureInfo.InvariantCulture),
                    NoBoleto = int.Parse(c[n + 4], CultureInfo.InvariantCulture),
                });
            }
            return r;
        }

        /// <summary>«Guardar todas»: las columnas en el orden del escrutinio, leídas página a página.</summary>
        public IEnumerable<string> Columnas()
        {
            for (int p = 0; p < Paginas; p++)
                foreach (var premiada in Pagina(p))
                    yield return premiada.Columna;
        }

        public void Borrar() => BorrarFichero(Ruta);

        internal static void BorrarFichero(string ruta)
        {
            try { File.Delete(ruta); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }

    /// <summary>
    /// Escribe las premiadas en el fichero temporal mientras se escruta, de MB en MB (#5).
    /// Si se descarta sin <see cref="Cerrar"/> (error), borra el fichero.
    /// </summary>
    public sealed class EscritorPremiadas : IDisposable
    {
        private readonly string ruta = Path.Combine(Path.GetTempPath(), "Free1X2-premiadas-" + Guid.NewGuid().ToString("N") + ".txt");
        private readonly FileStream fichero;
        private readonly MemoryStream buffer = new MemoryStream();
        private readonly List<long> inicios = new List<long> { 0 };
        private readonly List<string> ficheros = new List<string>();
        private long escritos;
        private int total;
        private bool cerrado;

        public EscritorPremiadas()
        {
            fichero = new FileStream(ruta, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
        }

        /// <summary>Añade al fichero las premiadas (<see cref="ColumnasPremiadas"/>), en su orden.</summary>
        public void Anadir(IEnumerable premiadas)
        {
            foreach (ColumnasPremiadas p in premiadas)
            {
                if (total > 0 && total % PremiadasEnDisco.PorPagina == 0) inicios.Add(escritos + buffer.Length);
                // Las premiadas llegan por fichero: basta mirar el último.
                if (ficheros.Count == 0 || ficheros[ficheros.Count - 1] != p.Fichero) ficheros.Add(p.Fichero);
                string linea = string.Create(CultureInfo.InvariantCulture,
                    $"{p.Columna}\t{ficheros.Count - 1}\t{p.Jornada}\t{p.Premio}\t{p.NoColumna}\t{p.NoBoleto}\n");
                buffer.Write(Encoding.UTF8.GetBytes(linea));
                total++;
            }
            if (buffer.Length >= 1 << 20) Volcar();
        }

        public PremiadasEnDisco Cerrar()
        {
            Volcar();
            fichero.Dispose();
            cerrado = true;
            if (total > 0) inicios.Add(escritos);
            return new PremiadasEnDisco(ruta, total, total == 0 ? new long[] { 0 } : inicios.ToArray(), ficheros);
        }

        public void Dispose()
        {
            fichero.Dispose();
            if (!cerrado) PremiadasEnDisco.BorrarFichero(ruta);
        }

        private void Volcar()
        {
            fichero.Write(buffer.GetBuffer(), 0, (int)buffer.Length);
            escritos += buffer.Length;
            buffer.SetLength(0);
        }
    }
}
