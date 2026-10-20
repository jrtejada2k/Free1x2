using Free1X2.MotorCalculo;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// #15: al borrar un grupo, <see cref="ControladorGrupos.QuitarGrupo"/> lo quita de los controles (por sus números,
    /// también en rangos), quita el control que se queda vacío y renumera los conjuntos. Caso del issue, igual que la
    /// prueba eliminarGrupoDeLosControles de la Mac.
    /// </summary>
    public class QuitarGrupoTests
    {
        private static ControlGrupos Control(string grupos, string fallos)
        {
            var c = new ControlGrupos();
            c.PonerGruposControlados(grupos);
            c.PonerFallosPermitidos(fallos);
            return c;
        }

        private static ControlConjuntos Conjunto(string controles, string fallos)
        {
            var c = new ControlConjuntos();
            c.PonerCtrlGruposControlados(controles);
            c.PonerFallosPermitidos(fallos);
            return c;
        }

        [Fact]
        public void BorrarElGrupo2_QuitaSuControlYRenumera()
        {
            var cg = new ControladorGrupos();
            cg.ControlesGrupos[0].PonerGruposControlados("0,1"); // el control 0 (libres) lo crea el constructor
            cg.ControlesGrupos.Add(Control("2", "0,1"));
            cg.ControlesGrupos.Add(Control("2-4", "0,1"));
            cg.ControlesGrupos.Add(Control("3-4", "1"));
            cg.ControlesConjuntos = new System.Collections.Generic.List<ControlConjuntos>
            {
                Conjunto("0", ""), Conjunto("1,3", "0,1"), Conjunto("1", "0"),
            };

            cg.QuitarGrupo(2);

            Assert.Equal(3, cg.ControlesGrupos.Count);
            Assert.Equal("0,1", cg.ControlesGrupos[0].ObtenGruposControlados()); // sin el grupo ni otros detrás: igual
            Assert.Equal(new[] { 2, 3 }, cg.ControlesGrupos[1].GruposControlados);
            Assert.Equal(new[] { 2, 3 }, cg.ControlesGrupos[2].GruposControlados);
            Assert.Equal("0,1", cg.ControlesGrupos[1].ObtenFallosPermitidos()); // los fallos no cambian
            Assert.Equal("1", cg.ControlesGrupos[2].ObtenFallosPermitidos());

            Assert.Equal(2, cg.ControlesConjuntos.Count);
            Assert.Equal("0", cg.ControlesConjuntos[0].ObtenCtrlGruposControladosStr());
            Assert.Equal(new[] { 2 }, cg.ControlesConjuntos[1].ObtenCtrolGruposConjunto());
            Assert.Equal("0,1", cg.ControlesConjuntos[1].ObtenFallosPermitidosStr());
        }

        [Fact]
        public void BorrarUnGrupoSinControles_NoToca()
        {
            var cg = new ControladorGrupos();
            cg.ControlesGrupos[0].PonerGruposControlados("0,1"); // el control 0 (libres) lo crea el constructor
            cg.ControlesGrupos.Add(Control("2-3", "0"));
            cg.QuitarGrupo(4);
            Assert.Equal("0,1", cg.ControlesGrupos[0].ObtenGruposControlados());
            Assert.Equal("2-3", cg.ControlesGrupos[1].ObtenGruposControlados());
        }
    }
}
