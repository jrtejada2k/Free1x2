using System;
using System.Collections.Generic;
using System.Text;
using Free1X2.MotorCalculo;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Golden-master de <see cref="RelacionCP1"/> (Relaciones I entre Columnas Probables).
    /// Fija el resultado COMPLETO de recorrer las 243 columnas de un universo
    /// determinista (5 triples + 9 fijos) con tres relaciones configuradas:
    ///   · la cadena de validez columna a columna (243 caracteres, una por columna);
    ///   · el SHA-256 del volcado completo "indice;valido;textoDeFallos" de las 243
    ///     columnas, que fija cada byte del resultado, incluidos los textos de fallo;
    ///   · el conjunto completo de textos de fallo distintos.
    /// Las dos variantes estrechan alternativamente la Suma de Aciertos y el Recorrido,
    /// de modo que se ejercitan las dos ramas que leen la lista interna de CPs que
    /// cumplen la condición de grupos (antes un string CSV re-parseado por columna).
    ///
    /// Valores capturados del código original (v0.82.0, antes de las optimizaciones F3).
    /// </summary>
    public class RelacionCP1GoldenTests
    {
        private const string Todos = "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14";
        private const int NoColumnas = 243;

        private static List<ColumnaProbable> CrearCPs()
        {
            var lista = new List<ColumnaProbable>();
            for (int k = 0; k < 6; k++)
            {
                var cp = new ColumnaProbable();
                var pron = new string[14];
                for (int i = 0; i < 14; i++)
                    pron[i] = (i % (k + 2) == 0) ? "" : new[] { "1", "X", "2" }[(i + k) % 3];
                cp.Pronosticos = pron;
                cp.SetNoAciertos(Todos);
                cp.SetNoAciertosSeguidos(Todos);
                cp.SetNoFallosSeguidos(Todos);
                lista.Add(cp);
            }
            return lista;
        }

        private static ControladorRelacionesCP1 CrearControlador(List<ColumnaProbable> cps, bool variante)
        {
            var ctrl = new ControladorRelacionesCP1();
            ctrl.ColumnasProbables = cps;

            var r1 = new RelacionCP1();
            r1.Columnas = "1-6";
            r1.CantidadCP = "0-6";
            r1.CuantosAC = "2,3,4,5";
            r1.SumaAciertos = variante ? "0-56" : "5-15";
            r1.Recorridos = variante ? "0,1" : "0-3";
            ctrl.PonerRelacion(r1);

            var r2 = new RelacionCP1();
            r2.Columnas = "1,3,5";
            r2.SumaAciertos = "4-12";
            r2.Recorridos = "0-14";
            ctrl.PonerRelacion(r2);

            var r3 = new RelacionCP1();
            r3.Columnas = "2-5";
            r3.CantidadCP = "1-4";
            r3.CuantosAC = "0-6";
            r3.SumaAciertos = "0-56";
            r3.Recorridos = "0-14";
            ctrl.PonerRelacion(r3);

            return ctrl;
        }

        private static string[] Secuencia(bool variante)
        {
            var cps = CrearCPs();
            var ctrl = CrearControlador(cps, variante);
            var salida = new List<string>();

            for (int idx = 0; idx < NoColumnas; idx++)
            {
                long col = ColumnaSintetica(idx, 5);
                // Analizar(col) recalcula el NoAC de cada CP, que es lo que leen las relaciones.
                for (int c = 0; c < cps.Count; c++) cps[c].Analizar(col);

                bool valido = ctrl.Analiza();
                string txt = "";
                ctrl.Analiza(ref txt);
                salida.Add(idx + ";" + (valido ? "1" : "0") + ";" + txt);
            }
            return salida.ToArray();
        }

        // Columna con los `triples` primeros partidos variando en base 3 y el resto a "1".
        private static long ColumnaSintetica(int indice, int triples)
        {
            long col = 0;
            for (int p = 0; p < 14; p++)
            {
                int d;
                if (p < triples) { d = indice % 3; indice /= 3; } else d = 0;
                long bit = d == 0 ? 4L : (d == 1 ? 2L : 1L);
                col |= bit << (3 * p);
            }
            return col;
        }

        private static void Comprobar(bool variante, string validezEsperada, string shaEsperado, string[] textosEsperados)
        {
            string[] s = Secuencia(variante);
            Assert.Equal(NoColumnas, s.Length);

            var validez = new StringBuilder();
            var distintos = new SortedSet<string>();
            foreach (string linea in s)
            {
                string[] partes = linea.Split(new[] { ';' }, 3);
                validez.Append(partes[1]);
                if (partes[2] != "") distintos.Add(partes[2]);
            }

            Assert.Equal(validezEsperada, validez.ToString());

            string join = string.Join("\n", s);
            string sha;
            using (var h = System.Security.Cryptography.SHA256.Create())
                sha = BitConverter.ToString(h.ComputeHash(Encoding.UTF8.GetBytes(join))).Replace("-", "");
            Assert.Equal(shaEsperado, sha);

            Assert.Equal(textosEsperados, new List<string>(distintos));
        }

        [Fact]
        public void RelacionCP1_SumaEstrecha_SecuenciaCompletaExacta()
        {
            Comprobar(
                false,
                "000000000000000000000111000000000111000000000000000000000000000111000000000000000000000000000000000111111111000000111000000000000111000000000000111000000000111000000000111000000000000000000111000111000000111000000111000000111000000000000000000",
                "65B49894C684D1428B4EB1F1F2ED1E3BDC891BF8361B54D7BD5B853E6EE32DF8",
                new[]
                {
                    "Fallo en Suma de Aciertos de la Relación de Columnas 1  (16)#",
                    "Fallo en Suma de Aciertos de la Relación de Columnas 1  (19)#",
                    "Fallo en Suma de Aciertos de la Relación de Columnas 1  (20)#",
                    "Fallo en Suma de Aciertos de la Relación de Columnas 1  (21)#",
                    "Fallo en Suma de Aciertos de la Relación de Columnas 1  (22)#",
                    "Fallo en Suma de Aciertos de la Relación de Columnas 1  (23)#",
                });
        }

        [Fact]
        public void RelacionCP1_RecorridoEstrecho_SecuenciaCompletaExacta()
        {
            Comprobar(
                true,
                "000000000000000000000111000000000000000000000111000000000000000111000000000000111000000000000111000000111000000000000000000000000000000111000000000000111000000000000000000000000000000000000000000111000000000000000000000000111000000000111000000",
                "125522BFFE6D8AF86E6A718B80C06709F124F0F2E72D60401886D2D624B934DA",
                new[]
                {
                    "Fallo en Recorrido de la Relación de Columnas 1  (2)#",
                    "Fallo en Recorrido de la Relación de Columnas 1  (3)#",
                });
        }
    }
}
