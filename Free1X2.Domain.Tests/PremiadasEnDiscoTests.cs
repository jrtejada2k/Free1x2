using System.Collections;
using System.Collections.Generic;
using System.IO;
using Free1X2.Escrutinio;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// #5: las premiadas de Escrutinios en un fichero temporal, por páginas de 1.000. Mismo caso de
    /// páginas que la app Mac: 2.187 premiadas dan 3 páginas (1.000, 1.000 y 187), iguales y en el
    /// mismo orden que la lista en memoria.
    /// </summary>
    public class PremiadasEnDiscoTests
    {
        [Fact]
        public void TresPaginasIgualesYEnElMismoOrden()
        {
            var lista = new ArrayList();
            for (int i = 0; i < 2187; i++)
            {
                lista.Add(new ColumnasPremiadas
                {
                    // Una de cada tres con tabulador dentro de la columna.
                    Columna = "1X21X21X21X2" + (i % 3 == 0 ? "\t1" : "X"),
                    Fichero = i < 1500 ? "a.txt" : "b.txt",
                    Jornada = i / 729 + 1,
                    Premio = i % 15,
                    NoColumna = i + 1,
                    NoBoleto = i / 8 + 1,
                });
            }

            PremiadasEnDisco p;
            using (var escritor = new EscritorPremiadas())
            {
                escritor.Anadir(lista.GetRange(0, 700));
                escritor.Anadir(lista.GetRange(700, 1487));
                p = escritor.Cerrar();
            }
            try
            {
                Assert.Equal(2187, p.Total);
                Assert.Equal(3, p.Paginas);
                var leidas = new List<ColumnasPremiadas>();
                int[] tamanos = { 1000, 1000, 187 };
                for (int k = 0; k < 3; k++)
                {
                    var pagina = p.Pagina(k);
                    Assert.Equal(tamanos[k], pagina.Count);
                    leidas.AddRange(pagina);
                }
                for (int i = 0; i < lista.Count; i++)
                {
                    var a = (ColumnasPremiadas)lista[i];
                    var b = leidas[i];
                    Assert.Equal(a.Columna, b.Columna);
                    Assert.Equal(a.Fichero, b.Fichero);
                    Assert.Equal(a.Jornada, b.Jornada);
                    Assert.Equal(a.Premio, b.Premio);
                    Assert.Equal(a.NoColumna, b.NoColumna);
                    Assert.Equal(a.NoBoleto, b.NoBoleto);
                }
                Assert.Equal(2187, new List<string>(p.Columnas()).Count);
            }
            finally
            {
                p.Borrar();
            }
            Assert.False(File.Exists(p.Ruta));
        }
    }
}
