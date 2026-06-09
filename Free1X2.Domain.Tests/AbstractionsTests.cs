using System;
using Free1X2.Abstractions;
using Free1X2.Utils;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Caracteriza los hooks de UI desacoplados (<see cref="UiPump"/>,
    /// <see cref="UserDialogs"/>, <see cref="AnalisisUi"/>). Garantizan que el
    /// dominio puede correr headless (tests/WinUI): por defecto son no-op y la
    /// capa de UI puede reasignarlos sin que el dominio se rompa.
    ///
    /// NOTA: estos hooks son estaticos/globales. Los tests restauran el valor
    /// original en un finally para no contaminar otros tests (xUnit no garantiza
    /// orden ni aislamiento de estado estatico entre clases).
    /// </summary>
    public class AbstractionsTests
    {
        [Fact]
        public void UiPump_PorDefecto_EsNoOpYNoLanza()
        {
            // El default no debe lanzar al invocarse.
            UiPump.Pump();
        }

        [Fact]
        public void UiPump_SePuedeReasignar_YSeInvoca()
        {
            var original = UiPump.Pump;
            try
            {
                int veces = 0;
                UiPump.Pump = () => veces++;

                UiPump.Pump();
                UiPump.Pump();

                Assert.Equal(2, veces);
            }
            finally
            {
                UiPump.Pump = original;
            }
        }

        [Fact]
        public void UserDialogs_PorDefecto_NoLanzan()
        {
            UserDialogs.ShowError("error de prueba");
            UserDialogs.ShowInfo("info de prueba");
        }

        [Fact]
        public void UserDialogs_SePuedenReasignar_YRecibenElMensaje()
        {
            var origError = UserDialogs.ShowError;
            var origInfo = UserDialogs.ShowInfo;
            try
            {
                string capturadoError = null;
                string capturadoInfo = null;
                UserDialogs.ShowError = m => capturadoError = m;
                UserDialogs.ShowInfo = m => capturadoInfo = m;

                UserDialogs.ShowError("boom");
                UserDialogs.ShowInfo("ok");

                Assert.Equal("boom", capturadoError);
                Assert.Equal("ok", capturadoInfo);
            }
            finally
            {
                UserDialogs.ShowError = origError;
                UserDialogs.ShowInfo = origInfo;
            }
        }

        [Fact]
        public void AnalisisUi_PorDefecto_EsNoOpYNoLanza()
        {
            AnalisisUi.MostrarVisor(new object(), new object());
        }

        [Fact]
        public void AnalisisUi_SePuedeReasignar_YRecibeLosArgumentos()
        {
            var original = AnalisisUi.MostrarVisor;
            try
            {
                object a = null, b = null;
                AnalisisUi.MostrarVisor = (x, y) => { a = x; b = y; };

                var arg1 = new object();
                var arg2 = new object();
                AnalisisUi.MostrarVisor(arg1, arg2);

                Assert.Same(arg1, a);
                Assert.Same(arg2, b);
            }
            finally
            {
                AnalisisUi.MostrarVisor = original;
            }
        }
    }

    /// <summary>
    /// Caracterización de <see cref="Combinacion"/>, un simple DTO (Temporada,
    /// Jornada, Path) usado para localizar ficheros de quiniela.
    /// </summary>
    public class CombinacionTests
    {
        [Fact]
        public void Constructor_AsignaTemporadaJornadaYPath()
        {
            var c = new Combinacion("2024-2025", "07", @"C:\quinielas\j07.txt");

            Assert.Equal("2024-2025", c.Temporada);
            Assert.Equal("07", c.Jornada);
            Assert.Equal(@"C:\quinielas\j07.txt", c.Path);
        }

        [Fact]
        public void Propiedades_SonModificables()
        {
            var c = new Combinacion("a", "b", "c")
            {
                Temporada = "2025-2026",
                Jornada = "14",
                Path = "ruta/nueva"
            };

            Assert.Equal("2025-2026", c.Temporada);
            Assert.Equal("14", c.Jornada);
            Assert.Equal("ruta/nueva", c.Path);
        }
    }
}
