using System.IO;
using System.Linq;
using Free1X2.MotorCalculo;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// #16 (decisión del dueño, 4-nov-2026): al Generar, el If-Then se suma a las condiciones (AND), como ya hacía
    /// la ruta de analizar columnas. El original lo asignaba y una columna que no cumplía las condiciones podía
    /// entrar por cumplir el If-Then.
    /// </summary>
    public class IfThenAndTests
    {
        [Fact]
        public void Generar_ConIfThen_SoloDejaPasarLoQueYaPasaSinEl()
        {
            var sinIfThen = Generar(Abrir(conIfThen: false), "free1x2_q16_sin.txt", out int aceptadasSin);
            var conIfThen = Generar(Abrir(conIfThen: true), "free1x2_q16_con.txt", out int aceptadasCon);

            Assert.True(aceptadasCon <= aceptadasSin);
            Assert.All(conIfThen, c => Assert.Contains(c, sinIfThen));
        }

        private static Analizador Abrir(bool conIfThen)
        {
            var arch = new Free1X2.EntradaSalida.ArchivoCombinacion();
            arch.AbrirArchivoCombinacion(FixturePaths.Ruta("completa.comb"));
            var a = new Analizador();
            arch.CargaControladorGrupos(a.CtrlGrupos);
            a.IfThen = conIfThen ? arch.CargaIfThen() : null;
            for (int i = 0; i < 14; i++) a.SetPronostico(i, i < 8 ? "1,X,2" : "1");
            return a;
        }

        private static string[] Generar(Analizador a, string nombre, out int aceptadas)
        {
            string ruta = Path.Combine(Path.GetTempPath(), nombre);
            File.Delete(ruta);
            a.AnalizaCombinacion(ruta);
            aceptadas = a.ColsAceptadas;
            if (!File.Exists(ruta)) return new string[0];
            var lineas = File.ReadAllLines(ruta).Where(l => l.Trim().Length > 0).Select(l => l.Trim()).ToArray();
            File.Delete(ruta);
            return lineas;
        }
    }
}
