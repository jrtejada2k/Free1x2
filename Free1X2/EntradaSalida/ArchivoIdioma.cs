using System.Collections.Generic;
using System.Text;

using System.IO;

namespace Free1X2.EntradaSalida
{
    public class ArchivoIdioma
    {
        public Dictionary<string, string> ObtenerIdioma(string idioma)
        {
            Dictionary<string, string> diccionario = new Dictionary<string, string>();
            string path = System.AppContext.BaseDirectory.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar)+"/Idioma/";
            DirectoryInfo dInfo = new DirectoryInfo(System.AppContext.BaseDirectory.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar) + "/Idioma/");
            FileInfo[] fInfo = dInfo.GetFiles("*.lang");
            if (fInfo.Length > 0)
            {
                for (int i = 0; i < fInfo.Length; i++)
                {
                    if (Path.GetFileNameWithoutExtension(fInfo[i].ToString()) == idioma)
                    {
                        try
                        {
                            StreamReader sr = new StreamReader(path + fInfo[i], Encoding.Default);
                            while (sr.Peek() != -1)
                            {
                                string[] linea = sr.ReadLine().Split('#');
                                if (!diccionario.ContainsKey(linea[0]))
                                {
                                    diccionario.Add(linea[0], linea[1]);
                                }
                            }
                            sr.Close();
                            sr.Dispose();
                        }
                        catch
                        {

                        }
                    }
                }
            }
            return diccionario;
        }
        public string[] ObtenerListaDeIdiomasDisponibles()
        {
            string[] lista = null;
            DirectoryInfo dInfo = new DirectoryInfo(System.AppContext.BaseDirectory.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar) + "/Idioma/");
            FileInfo[] fInfo = dInfo.GetFiles("*.lang");
            if (fInfo.Length > 0)
            {
                lista = new string[fInfo.Length];
                for (int i = 0; i < fInfo.Length; i++)
                {
                    lista[i] = Path.GetFileNameWithoutExtension(fInfo[i].ToString());
                }
            }
            return lista;
        }
    }
}
