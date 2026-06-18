using System;
using System.IO;
using System.Linq;
using Free1X2.Online;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Tests del parser PURO del catálogo de equipos online (<see cref="CatalogoEquiposParser"/>)
    /// contra las MUESTRAS REALES versionadas del contrato (docs/ejemplos-api/equipos-*.json),
    /// copiadas a Fixtures/online/ junto al binario de pruebas (CopyToOutputDirectory).
    ///
    /// El backend live sirve una ÚNICA división con id="all" ("Equipos (jornadas recientes)").
    /// El parser admite varias divisiones, así que también se cubre la lista plana de-duplicada.
    /// </summary>
    public class CatalogoEquiposParserTests
    {
        private static string LeerFixture(string nombre) =>
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "online", nombre));

        [Fact]
        public void Es_Muestra_Parsea_DivisionAll_Y_ListaPlanaNoVacia()
        {
            string json = LeerFixture("equipos-es.json");

            CatalogoEquipos catalogo = CatalogoEquiposParser.Parse(json);

            Assert.Equal("ES", catalogo.Pais);

            // El backend actual trae una sola división con id "all".
            Assert.Single(catalogo.Divisiones);
            Assert.Equal("all", catalogo.Divisiones[0].Id);
            Assert.Equal("Equipos (jornadas recientes)", catalogo.Divisiones[0].Nombre);

            // Lista plana no vacía con nombres reales y acentos UTF-8 preservados.
            var plano = catalogo.EquiposPlano();
            Assert.NotEmpty(plano);
            Assert.Contains("Real Madrid", plano);
            Assert.Contains("Alavés", plano);
            Assert.Contains("Almería", plano);
        }

        [Fact]
        public void Mx_Muestra_Parsea_DivisionAll_ConAcentos()
        {
            string json = LeerFixture("equipos-mx.json");

            CatalogoEquipos catalogo = CatalogoEquiposParser.Parse(json);

            Assert.Equal("MX", catalogo.Pais);
            Assert.Single(catalogo.Divisiones);
            Assert.Equal("all", catalogo.Divisiones[0].Id);

            var plano = catalogo.EquiposPlano();
            Assert.NotEmpty(plano);
            // "América" con acento (UTF-8 correcto).
            Assert.Contains("América", plano);
            Assert.Contains("Guadalajara", plano);
        }

        [Fact]
        public void ListaPlana_DeduplicaEntreDivisiones_PreservandoOrden()
        {
            // Catálogo sintético multi-división con un equipo repetido entre divisiones y
            // otro repetido con distinta capitalización/espacios: debe quedar una sola vez.
            string json =
                "{ \"pais\":\"ES\", \"divisiones\":[" +
                "  { \"id\":\"1\", \"nombre\":\"Primera\", \"equipos\":[ \"Real Madrid\", \"Sevilla\" ] }," +
                "  { \"id\":\"2\", \"nombre\":\"Segunda\", \"equipos\":[ \"sevilla \", \"Real Madrid\", \"Eibar\" ] }" +
                "] }";

            CatalogoEquipos catalogo = CatalogoEquiposParser.Parse(json);
            var plano = catalogo.EquiposPlano();

            // 3 equipos distintos, primera aparición preservada.
            Assert.Equal(new[] { "Real Madrid", "Sevilla", "Eibar" }, plano.ToArray());
        }

        [Fact]
        public void JsonMalformado_LanzaFormatException()
        {
            string basura = "{ esto no es json válido ][ ";

            FormatException ex = Assert.Throws<FormatException>(
                () => CatalogoEquiposParser.Parse(basura));
            Assert.Contains("JSON", ex.Message);
        }

        [Fact]
        public void JsonVacio_LanzaFormatException()
        {
            Assert.Throws<FormatException>(() => CatalogoEquiposParser.Parse(""));
        }

        [Fact]
        public void SinDivisiones_LanzaFormatException()
        {
            // Array de divisiones presente pero vacío: viola el contrato (al menos una).
            string json = "{ \"pais\":\"ES\", \"divisiones\":[] }";

            Assert.Throws<FormatException>(() => CatalogoEquiposParser.Parse(json));
        }

        [Fact]
        public void DivisionSinEquipos_LanzaFormatException()
        {
            string json = "{ \"pais\":\"ES\", \"divisiones\":[ { \"id\":\"all\", \"equipos\":[] } ] }";

            Assert.Throws<FormatException>(() => CatalogoEquiposParser.Parse(json));
        }

        [Fact]
        public void EquipoVacio_LanzaFormatException()
        {
            string json =
                "{ \"pais\":\"ES\", \"divisiones\":[ { \"id\":\"all\", \"equipos\":[ \"Real Madrid\", \"\" ] } ] }";

            Assert.Throws<FormatException>(() => CatalogoEquiposParser.Parse(json));
        }
    }
}
