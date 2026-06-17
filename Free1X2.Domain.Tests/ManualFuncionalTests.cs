using Free1X2;
using Free1X2.MotorCalculo;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Pruebas funcionales que siguen los ejemplos documentados en docs/MANUAL_USUARIO.md,
    /// ejercitando el motor real (Analizador + condiciones) exactamente como lo invoca la UI.
    /// Verifican que el comportamiento coincide con los números que promete el manual.
    /// </summary>
    public class ManualFuncionalTests
    {
        // "Acepta cualquiera": los setters de condición toman valores explícitos
        // separados por coma (no rangos "0-14"), igual que producen los controles WinForms.
        private const string TodosLosValores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14";

        // Boleto de 14 triples (universo completo 3^14).
        private static Analizador TodoTriples()
        {
            var a = new Analizador();
            for (int i = 0; i < 14; i++) a.SetPronostico(i, "1,X,2");
            return a;
        }

        // §6 "Las condiciones" — N.º de signos (variantes).
        // 0 variantes -> solo la columna 1,1,...,1 (1).
        // 1 variante  -> 14 posiciones × {X,2} = 28.
        // 2 variantes -> C(14,2) × 2² = 91 × 4 = 364.
        [Theory]
        [InlineData("0", 1)]
        [InlineData("1", 28)]
        [InlineData("2", 364)]
        public void Condicion_NoVariantes_FiltraComoElManual(string rango, int esperado)
        {
            var a = TodoTriples();
            var grupo = a.GruposPartidos[0];
            var f = (FiltroNoVariantes)grupo.GetFiltro(Filtro.NoVariantes.ToString());
            f.SetNoVariantes(rango);
            f.SetNoEquis(TodosLosValores);   // equis: acepta cualquiera
            f.SetNoDoses(TodosLosValores);   // doses: acepta cualquiera
            f.IsActive = true;

            a.AnalizaCombinacion(false);

            Assert.Equal(esperado, a.ColsAceptadas);
        }

        // §7 "Columnas Probables" — el ejemplo de los 4 grandes:
        // 4 partidos a triple (81 apuestas) + una CP donde los 4 ganan pidiendo 3 ó 4
        // aciertos (1 o ningún fallo) reduce de 81 a 9 columnas.
        [Fact]
        public void CP_CuatroGrandes_De81A9_ComoElManual()
        {
            var a = new Analizador();
            for (int i = 0; i < 14; i++) a.SetPronostico(i, i < 10 ? "1" : "1,X,2"); // 10 fijos + 4 triples

            var grupo = a.GruposPartidos[0];
            var fcp = (FiltroColProbables)grupo.GetFiltro(Filtro.ColProbables.ToString());

            var cp = new ColumnaProbable();
            var pron = new string[14];
            for (int i = 0; i < 14; i++) pron[i] = i < 10 ? "" : "1"; // solo los 4 grandes cuentan; ganan ("1")
            cp.Pronosticos = pron;
            cp.SetNoAciertos("3,4");                  // 3 ó 4 aciertos
            cp.SetNoAciertosSeguidos(TodosLosValores); // aciertos seguidos: acepta cualquiera
            cp.SetNoFallosSeguidos(TodosLosValores);   // fallos seguidos: acepta cualquiera
            fcp.ColProbables.Add(cp);
            fcp.IsActive = true;

            a.AnalizaCombinacion(false);

            Assert.Equal(9, a.ColsAceptadas);
        }

        // §4 "Flujo de trabajo" — sin condiciones, el universo de un boleto es 3^t·2^d.
        // 6 triples + 6 dobles + 2 fijos = 3^6·2^6 = 46 656 (ejemplo del manual §2).
        [Fact]
        public void Combinacion_6Triples6Dobles_Genera46656_ComoElManual()
        {
            var a = new Analizador();
            for (int i = 0; i < 14; i++)
                a.SetPronostico(i, i < 6 ? "1,X,2" : i < 12 ? "1,X" : "1");

            a.AnalizaCombinacion(false);

            Assert.Equal(46656, a.ColsAceptadas);
        }
    }
}
