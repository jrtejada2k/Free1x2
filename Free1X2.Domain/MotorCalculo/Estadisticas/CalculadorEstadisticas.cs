using System.Collections.Generic;
using System.IO;

using Free1X2.EntradaSalida;

namespace Free1X2.MotorCalculo.Estadisticas
{
    public class CalculadorEstadisticas
    {
        private float ObtenerEstadisticasFiltro(IFiltro filtro, string archivoGanadoras)
        {
            IArchivoColumnas arComb = new ArchivoColumnasTexto(archivoGanadoras);
            int total = 0;
            int validas = 0;
            bool nohayCols = false;
            while(arComb.SiguienteColumna())
            {
                long columna = Utils.UtilColumnas.ConvStrToLong(arComb.LeeColumnaSinComas());
                total++;
                if (filtro.Analizar(columna))
                {
                    validas++;
                }
            }
            if (total == 0)
            {
                nohayCols = true;
            }
            if (nohayCols)
            {
                return 0;
            }
            float cumplimiento = validas / (float)total;
            return cumplimiento * 100;
        }
        public List<Estadistica> EstadisticasFiltro(IFiltro filtro, string directorio)
        {
            DirectoryInfo dirInf = new DirectoryInfo(directorio);
            FileInfo[] fileInf = dirInf.GetFiles("*.txt");
            List<Estadistica> arrayEstadisticas = new List<Estadistica>();
            for (int j = 0; j < fileInf.Length; j++)
            {
                string archivo = directorio + fileInf[j];
                Estadistica es = new Estadistica();
                es.Archivo = fileInf[j].ToString();
                es.Cumplimiento = ObtenerEstadisticasFiltro(filtro, archivo);
                arrayEstadisticas.Add(es);
            }

            return arrayEstadisticas;
        }

    }
}
