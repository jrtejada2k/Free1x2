using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Free1X2.Escrutinio;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// #9: el escrutinio por tramos en varios núcleos da lo mismo que el bucle de un hilo de antes
    /// (aquí <see cref="Referencia"/>): mismos aciertos y mismas premiadas en el mismo orden. 200.000
    /// columnas son varios tramos (32.768 como mínimo cada uno).
    /// </summary>
    public class EscrutadorParaleloTests
    {
        private static readonly int[] Premios = { 14, 13, 12, 11, 10 };

        [Fact]
        public void EscrutarSimple_IgualQueEnUnHilo()
        {
            var lineas = Lineas(200_000, 14, semilla: 1);
            string ganadora = "1X2*1x21X2*1X2";
            string ruta = Escribir(lineas);
            try
            {
                var e = new Escrutador(Premios) { ArchivoColumnas = ruta, AñadirAGanadoras = true };
                e.EscrutaCombConColumna(ganadora, CrearDataSetResultados(), Path.GetFileName(ruta));

                var esperadas = new List<ColumnasPremiadas>();
                int[] nc = Referencia(lineas, ganadora, ruta, 1, esperadas, out var error);
                Assert.Null(error);
                Assert.Equal(nc, e.PremiosTotales);
                Iguales(esperadas, e.ListaPremiadas.Cast<ColumnasPremiadas>().ToList());
            }
            finally { File.Delete(ruta); }
        }

        [Fact]
        public void EscrutarTemporada_IgualQueEnUnHilo()
        {
            var lineas = Lineas(200_000, 14, semilla: 2);
            string[] ganadoras = { "11111111111111", "X2X2X2X2X2X2X2", lineas[123_456] };
            string ruta = Escribir(lineas);
            string rutaRef = Escribir(ganadoras.ToList());
            try
            {
                var e = new Escrutador(Premios) { ArchivoColumnas = ruta, AñadirAGanadoras = true };
                var ds = CrearDataSetResultados();
                var porJornada = new List<ColumnasPremiadas>();
                e.EscrutaCombConTemporada(rutaRef, ds, Path.GetFileName(ruta), l => porJornada.AddRange(l.Cast<ColumnasPremiadas>()));

                var esperadas = new List<ColumnasPremiadas>();
                var total = new int[15];
                for (int j = 0; j < ganadoras.Length; j++)
                {
                    // LeeArchivoTemporada lee las ganadoras como las columnas (Trim + ToUpper).
                    int[] nc = Referencia(lineas, ganadoras[j].Trim().ToUpper(), ruta, j + 1, esperadas, out var error);
                    Assert.Null(error);
                    for (int i = 0; i < nc.Length; i++) total[i] += nc[i];
                    Assert.Equal(nc[14].ToString(), ds.Tables["Resultados"].Rows[j]["P14"].ToString());
                }
                Assert.Equal(3, ds.Tables["Resultados"].Rows.Count);
                Assert.Equal(total, e.PremiosTotales);
                Iguales(esperadas, porJornada);
            }
            finally { File.Delete(ruta); File.Delete(rutaRef); }
        }

        [Fact]
        public void FueraDeRango_MismoErrorYLasPremiadasDeAntes()
        {
            // 15 signos contra una ganadora de 15: la columna igual da 15 aciertos y no cabe (0..14).
            var lineas = Lineas(200_000, 15, semilla: 3);
            string ganadora = lineas[150_000];
            string ruta = Escribir(lineas);
            try
            {
                var e = new Escrutador(Premios) { ArchivoColumnas = ruta, AñadirAGanadoras = true };
                Assert.Throws<IndexOutOfRangeException>(() =>
                    e.EscrutaCombConColumna(ganadora, CrearDataSetResultados(), Path.GetFileName(ruta)));

                var esperadas = new List<ColumnasPremiadas>();
                Referencia(lineas, ganadora, ruta, 1, esperadas, out var error);
                Assert.IsType<IndexOutOfRangeException>(error);
                Iguales(esperadas, e.ListaPremiadas.Cast<ColumnasPremiadas>().ToList());
            }
            finally { File.Delete(ruta); }
        }

        /// <summary>El EscrutaColumna de un hilo de antes (Escrutador.cs, antes de #9).</summary>
        private static int[] Referencia(List<string> lineas, string ganadora, string fichero, int jornada,
            List<ColumnasPremiadas> premiadas, out Exception error)
        {
            error = null;
            var nc = new int[15];
            ganadora = ganadora.Replace('x', 'X');
            int numCol = 0;
            try
            {
                foreach (string linea in lineas)
                {
                    numCol++;
                    string columna = linea.Trim().ToUpper();
                    string colTst = columna.Replace('x', 'X');
                    int na = 0;
                    int maxSignos = Math.Min(columna.Length, ganadora.Length);
                    for (int nr = 0; nr < maxSignos; nr++)
                        if (ganadora[nr] == '*' || ganadora[nr] == colTst[nr]) na++;
                    if (Array.IndexOf(Premios, na) >= 0)
                        premiadas.Add(new ColumnasPremiadas
                        {
                            Columna = colTst, Fichero = fichero, Jornada = jornada, NoColumna = numCol, Premio = na,
                            NoBoleto = numCol / 8 + (numCol % 8 > 0 ? 1 : 0),
                        });
                    nc[na]++;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                error = e;
            }
            return nc;
        }

        private static void Iguales(List<ColumnasPremiadas> esperadas, List<ColumnasPremiadas> reales)
        {
            Assert.True(esperadas.Count > 100, "pocas premiadas para comparar");
            Assert.Equal(esperadas.Count, reales.Count);
            for (int i = 0; i < esperadas.Count; i++)
            {
                Assert.Equal(esperadas[i].Columna, reales[i].Columna);
                Assert.Equal(esperadas[i].Fichero, reales[i].Fichero);
                Assert.Equal(esperadas[i].Jornada, reales[i].Jornada);
                Assert.Equal(esperadas[i].NoColumna, reales[i].NoColumna);
                Assert.Equal(esperadas[i].Premio, reales[i].Premio);
                Assert.Equal(esperadas[i].NoBoleto, reales[i].NoBoleto);
            }
        }

        /// <summary>Columnas al azar, algunas con «x» minúscula y blancos alrededor.</summary>
        private static List<string> Lineas(int n, int signos, int semilla)
        {
            var r = new Random(semilla);
            var lineas = new List<string>(n);
            var sb = new StringBuilder();
            for (int i = 0; i < n; i++)
            {
                sb.Clear();
                for (int k = 0; k < signos; k++) sb.Append("1Xx2"[r.Next(4)]);
                if (i % 7 == 0) sb.Insert(0, ' ').Append('\t');
                lineas.Add(sb.ToString());
            }
            return lineas;
        }

        private static string Escribir(List<string> lineas)
        {
            string ruta = Path.Combine(Path.GetTempPath(), "free1x2_esc_" + Guid.NewGuid().ToString("N") + ".txt");
            File.WriteAllLines(ruta, lineas);
            return ruta;
        }

        private static DataSet CrearDataSetResultados()
        {
            var ds = new DataSet();
            var t = new DataTable("Resultados");
            t.Columns.Add("Seleccionado", typeof(bool));
            t.Columns.Add("LineaID", typeof(int));
            t.Columns.Add("Columna", typeof(string));
            t.Columns.Add("Archivo", typeof(string));
            for (int i = 0; i <= 14; i++) t.Columns.Add("P" + i, typeof(int));
            t.Columns.Add("Ac. Totales", typeof(string));
            ds.Tables.Add(t);
            return ds;
        }
    }
}
