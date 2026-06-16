using System.Data;
using System.IO;
using Free1X2.MotorCalculo;
using Free1X2.Reduccion;
using Free1X2.Escrutinio;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Validación end-to-end de los 3 flujos clave (Calcular → Reducir → Escrutar)
    /// ejecutando el motor real exactamente como lo invocan los ViewModels de WinUI.
    /// Confirma que el comportamiento coincide con el del WinForms original.
    /// </summary>
    public class FlujosClaveTests
    {
        // ---- Flujo 1: Calcular ----
        // Boleto de 10 fijos + 4 triples = 3^4 = 81 columnas. Réplica de
        // CalculaColumnasFrmViewModel: SetPronostico x14 + AnalizaCombinacion(false).
        [Fact]
        public void Calcular_BoletoCon4Triples_Genera81Columnas()
        {
            var a = new Analizador();
            for (int i = 0; i < 14; i++)
                a.SetPronostico(i, i < 10 ? "1" : "1,X,2");

            a.AnalizaCombinacion(false);

            Assert.Equal(81, a.ColsAnalizadas);
            Assert.Equal(81, a.ColsAceptadas); // sin condiciones, todas pasan
        }

        // ---- Flujo 1 (grabar a fichero) usado como entrada de los otros 2 flujos ----
        private static string GenerarFichero81()
        {
            var a = new Analizador();
            for (int i = 0; i < 14; i++)
                a.SetPronostico(i, i < 10 ? "1" : "1,X,2");

            string ruta = Path.Combine(Path.GetTempPath(), "free1x2_flujo_in.txt");
            if (File.Exists(ruta)) File.Delete(ruta);
            a.AnalizaCombinacion(ruta); // modo grabar
            return ruta;
        }

        [Fact]
        public void Calcular_ModoGrabar_EscribeLas81ColumnasAFichero()
        {
            string ruta = GenerarFichero81();
            Assert.True(File.Exists(ruta));
            int lineas = File.ReadAllLines(ruta).Length;
            Assert.Equal(81, lineas);
        }

        // ---- Flujo 2: Reducir ----
        // Réplica de ReductorFrmViewModel: new JDC(false) + ComienzaReduccion(...).
        [Fact]
        public void Reducir_JDC_ProduceSubconjuntoNoVacio()
        {
            string entrada = GenerarFichero81();
            string salida = Path.Combine(Path.GetTempPath(), "free1x2_flujo_out.txt");
            if (File.Exists(salida)) File.Delete(salida);

            IReduccion reductor = new JDC(false);
            reductor.ComienzaReduccion(entrada, salida, 13, 0, 100);

            Assert.Equal(81, reductor.NoColumnasIniciales);
            Assert.True(reductor.NoColumnasFinales > 0, "la reducción no produjo columnas");
            Assert.True(reductor.NoColumnasFinales <= reductor.NoColumnasIniciales,
                "la reducción no puede tener más columnas que la entrada");
        }

        // ---- Flujo 3: Escrutar ----
        // Réplica de EscrutiniosFrmViewModel: crea la tabla "Resultados", Escrutador(rangos),
        // EscrutaCombConColumna(ganadora, ds, fichero), lee PremiosTotales.
        [Fact]
        public void Escrutar_ContraColumnaGanadora_PuntuaCorrectamente()
        {
            string fichero = GenerarFichero81();
            string ganadora = File.ReadAllLines(fichero)[0].Replace(",", "");

            var ds = CrearDataSetResultados();
            var escrutador = new Escrutador(new[] { 14, 13, 12, 11, 10 })
            {
                ArchivoColumnas = fichero,
            };
            escrutador.EscrutaCombConColumna(ganadora, ds, Path.GetFileName(fichero));

            int[] premios = escrutador.PremiosTotales;

            // La ganadora está en el fichero, así que exactamente 1 columna acierta 14.
            Assert.Equal(1, premios[14]);
            // Cada una de las 81 columnas se puntúa: la suma de aciertos cubre el total.
            int totalPuntuadas = 0;
            for (int i = 0; i <= 14; i++) totalPuntuadas += premios[i];
            Assert.Equal(81, totalPuntuadas);
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
