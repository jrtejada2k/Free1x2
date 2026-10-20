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

        /// <summary>
        /// Como la prueba f07bEliminarGrupoConSuControl de la Mac: con <c>completa.comb</c>, añadir un grupo con su
        /// propio control y su conjunto y borrarlo (como EliminarGrupo de la pantalla) deja que Generar dé las mismas
        /// columnas que antes. Antes el control seguía apuntando al grupo borrado.
        /// </summary>
        [Fact]
        public void CompletaComb_AnadirYBorrarGrupoConSuControl_GeneraLoMismo()
        {
            // Su If-Then activo decide solo (AnalizaColumna(long) asigna, no hace AND, como el original): no todas pasan.
            string antes = Generar(Abrir(), "free1x2_q15_antes.txt", out _, out var aceptadas);
            Assert.True(aceptadas > 0);

            var a = Abrir();
            var cg = a.CtrlGrupos;
            string libres = cg.ControlesGrupos[0].ObtenGruposControlados();
            int nuevo = a.GruposPartidos.Count;
            a.GruposPartidos.AddGrupo(new Grupo()); // como CambiarGrupo de la pantalla: lo pone en el control 0
            cg.ControlesGrupos[0].PonerGruposControlados(libres);
            cg.AddControlGrupo(Control(nuevo.ToString(), "0"));
            cg.ControlesConjuntos.Add(Conjunto((cg.ControlesGrupos.Count - 1).ToString(), "0"));
            Generar(a, "free1x2_q15_con.txt", out _, out _); // con el grupo nuevo también genera

            a.GruposPartidos.BorrarGrupo(nuevo);
            cg.QuitarGrupo(nuevo);
            string despues = Generar(a, "free1x2_q15_despues.txt", out _, out _);
            Assert.Equal(antes, despues);
        }

        private static Analizador Abrir()
        {
            var arch = new Free1X2.EntradaSalida.ArchivoCombinacion();
            arch.AbrirArchivoCombinacion(FixturePaths.Ruta("completa.comb"));
            var a = new Analizador();
            arch.CargaControladorGrupos(a.CtrlGrupos);
            a.IfThen = arch.CargaIfThen();
            for (int i = 0; i < 14; i++) a.SetPronostico(i, i < 8 ? "1,X,2" : "1");
            return a;
        }

        private static string Generar(Analizador a, string nombre, out int analizadas, out int aceptadas)
        {
            string ruta = System.IO.Path.Combine(System.IO.Path.GetTempPath(), nombre);
            System.IO.File.Delete(ruta);
            a.AnalizaCombinacion(ruta);
            analizadas = a.ColsAnalizadas;
            aceptadas = a.ColsAceptadas;
            string texto = System.IO.File.ReadAllText(ruta);
            System.IO.File.Delete(ruta);
            return texto;
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
