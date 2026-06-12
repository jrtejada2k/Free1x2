// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2007 Morrison - morrison.ne@gmail.com
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.


using System;
using System.Collections.Generic;

namespace Free1X2.Utils
{
    public class UtilidadesEntradasValores
    {
        public static List<int> ObtenerArrayAciertos(string aciertos)
        {
            List<int> arrayA = new List<int>();
            string[] aciertosTemp = aciertos.Split(',');
            for (int i = 0; i < aciertosTemp.Length; i++)
            {
                int a = Convert.ToInt32(aciertosTemp[i]);
                arrayA.Add(a);
            }
            return arrayA;
        }
        ///<summary>
        ///Obtiene una lista de indices a partir de
        ///una cadena de <paramref name="entrada"/>
        ///</summary>
        ///<param name="entrada">Cadena de entrada</param>
        public static List<int> ObtenerListaFromTxt(string entrada)
        {
            List<int> indices = new List<int>();
            try
            {
                if (entrada.IndexOf(',') != -1)
                {
                    //Hay comas
                    string[] partes = entrada.Split(',');
                    //para cada parte, ver si hay guiones
                    for (int i = 0; i < partes.Length; i++)
                    {
                        if (partes[i].IndexOf('-') != -1)
                        {
                            string[] partes2 = partes[i].Split('-');
                            int min = Convert.ToInt32(partes2[0]);
                            int max = Convert.ToInt32(partes2[1]);

                            for (int j = min; j <= max; j++)
                            {
                                if (!indices.Contains(j))
                                {
                                    indices.Add(j - 1);
                                }
                            }
                        }
                        else
                        {
                            int valor = Convert.ToInt32(partes[i]);
                            if (!indices.Contains(valor))
                            {
                                indices.Add(valor - 1);
                            }
                        }
                    }
                }
                else
                {
                    if (entrada.IndexOf('-') != -1)
                    {
                        string[] partes2 = entrada.Split('-');
                        int min = Convert.ToInt32(partes2[0]);
                        int max = Convert.ToInt32(partes2[1]);

                        for (int j = min; j <= max; j++)
                        {
                            if (!indices.Contains(j))
                            {
                                indices.Add(j - 1);
                            }
                        }
                    }
                    else
                    {
                        if (!indices.Contains(Convert.ToInt32(entrada)))
                        {
                            indices.Add(Convert.ToInt32(entrada) - 1);
                        }
                    }
                }
            }
            catch
            {
                indices.Clear();
            }
            return indices;
        }

        ///<summary>
        ///Obtiene una lista de valores a partir de
        ///una cadena de <paramref name="entrada"/>
        ///</summary>
        ///<param name="entrada">Cadena de entrada</param>
        public static List<int> ObtenerListaFromTxtAciertos(string entrada)
        {
            List<int> indices = new List<int>();
            try
            {
                if (entrada.IndexOf(',') != -1)
                {
                    //Hay comas
                    string[] partes = entrada.Split(',');
                    //para cada parte, ver si hay guiones
                    for (int i = 0; i < partes.Length; i++)
                    {
                        if (partes[i].IndexOf('-') != -1)
                        {
                            string[] partes2 = partes[i].Split('-');
                            int min = Convert.ToInt32(partes2[0]);
                            int max = Convert.ToInt32(partes2[1]);

                            for (int j = min; j <= max; j++)
                            {
                                if (!indices.Contains(j))
                                {
                                    indices.Add(j);
                                }
                            }
                        }
                        else
                        {
                            int valor = Convert.ToInt32(partes[i]);
                            if (!indices.Contains(valor))
                            {
                                indices.Add(valor);
                            }
                        }
                    }
                }
                else
                {
                    if (entrada.IndexOf('-') != -1)
                    {
                        string[] partes2 = entrada.Split('-');
                        int min = Convert.ToInt32(partes2[0]);
                        int max = Convert.ToInt32(partes2[1]);

                        for (int j = min; j <= max; j++)
                        {
                            if (!indices.Contains(j))
                            {
                                indices.Add(j);
                            }
                        }
                    }
                    else
                    {
                        if (!indices.Contains(Convert.ToInt32(entrada)))
                        {
                            indices.Add(Convert.ToInt32(entrada));
                        }
                    }
                }
            }
            catch
            {
                indices.Clear();
            }
            return indices;
        }

        public static string ObtenerTextoFromListAciertos(List<int> lista)
        {
            string s = "";
            for (int i = 0; i < lista.Count; i++)
            {
                s += lista[i].ToString();
                if (i < lista.Count - 1)
                {
                    s += ",";
                }
            }
            return s;
        }

        public static string ObtenerTextoFromBool(bool[] lista)
        {
            string s = "";
            for (int i = 0; i < lista.Length; i++)
            {
                if (lista[i])
                {
                    s += i.ToString();
                    s += ",";
                    
                }               
            }
            if (s.Length > 0)
            {
                return s.Substring(0, s.Length - 1);
            }
            return "";
        }
        ///<summary>
        ///Obtiene una Figura expresada como long a partir de
        ///una cadena de <paramref name="entrada"/>
        ///</summary>
        ///<param name="texto">Figura expresada como texto (x-y-z...)</param>
        public static long ObtenerLongFiguraFromText(string texto)
        {
            long figuraTemp = 0;
            string[] valores = texto.Split('-');
            for (int i = 0; i < valores.Length; i++)
            {
                figuraTemp <<= 4;
                figuraTemp |= (uint)Convert.ToInt32(valores[i]);
            }
            
            return figuraTemp;
        }

        ///<summary>
        ///Obtiene una Figura expresada como texto (x-y-z...) a partir de
        ///un long de <paramref name="entrada"/>
        ///</summary>
        ///<param name="fig">Figura expresada como long</param>
        public static string ObtenerTextoFiguraFromLong(long fig)
        {
            string cadena = "";
            while (fig != 0)
            {
                string valor = Convert.ToString(fig & 15);
                cadena = "-" + valor + cadena;

                fig >>= 4;
            }
            if (cadena.Length > 0)
            {
                return cadena.Substring(1);
            }
            return "0";
        }

        public static bool[] ObtenerBoolArrayFromTxt(string txt, int longitudArray)
        {
            bool[] valores = null;
            if (txt.Length > 0)
            {
                List<int> lista = ObtenerListaFromTxtAciertos(txt);
                lista.Sort(); //Esto lo hacemos para obtener el mayor acierto

                if (longitudArray > lista[lista.Count - 1])
                {
                    valores = new bool[longitudArray + 1];
                }
                else
                {
                    valores = new bool[lista[lista.Count - 1] + 1];
                }

                for (int i = 0; i < valores.Length; i++)
                {
                    if (lista.Contains(i))
                    {
                        valores[i] = true;
                    }
                }
            }
            return valores;
        }
        public static bool[] ObtenerBoolArrayFromTxt(string txt)
        {
            bool[] valores = null;
            if (txt.Length > 0)
            {
                List<int> lista = ObtenerListaFromTxtAciertos(txt);
                lista.Sort(); //Esto lo hacemos para obtener el mayor acierto

                    valores = new bool[VariablesGlobales.NumeroPartidos + 1];
               

                for (int i = 0; i < valores.Length; i++)
                {
                    if (lista.Contains(i))
                    {
                        valores[i] = true;
                    }
                }
            }
            return valores;
        }

        public static bool ValidarCamposDeTexto(string[] entradas, int max)
        {
            int enBlanco = 0;
            for (int i = 0; i < entradas.Length; i++)
            {
                if (entradas[i] == "")
                {
                    enBlanco++;
                }
            }

            if (enBlanco > max)
            {
                return false;
            }
            return true;
        }

        public static string ObtenerTodosValores()
        {
            string todosvalores = "";
            for (int i = 0; i <= VariablesGlobales.NumeroPartidos; i++)
            {
                todosvalores += i + ",";
            }
            return todosvalores.Substring(0, todosvalores.Length - 1);
        }

        public static string ObtenerTodosValores(int min, int max)
        {
            string todosvalores = "";
            for (int i = min; i <= max; i++)
            {
                todosvalores += i + ",";
            }
            return todosvalores.Substring(0, todosvalores.Length - 1);
        }
        public static string ObtenerValoresSeparadosPorComas(string cadena)
        {
            string todosvalores = "";
            //Buscar comas
            string[] partes = cadena.Split(',');
            for (int i = 0; i < partes.Length; i++)
            {
                if (partes[i].IndexOf('-') != -1)
                {
                    string[] partes2 = partes[i].Split('-');
                    todosvalores += ObtenerTodosValores(Convert.ToInt32(partes2[0]), Convert.ToInt32(partes2[1]));
                    todosvalores += ",";
                }
                else
                {
                    todosvalores += partes[i];
                    todosvalores += ",";
                }
            }
            return todosvalores.Substring(0, todosvalores.Length - 1);
        }
        public static bool SonTodosNumeros(string aciertos)
        {
            bool esValido = true;
            if (aciertos != "")
            {
                try
                {
                    string valores = ObtenerValoresSeparadosPorComas(aciertos);
                    string[] partes = valores.Split(',');
                    for (int i = 0; i < partes.Length; i++)
                    {
                        Convert.ToInt32(partes[i]);
                    }
                }
                catch
                {
                    esValido = false;
                }
            }
            return esValido;
        }
        public static int[] ObtenerRangos(List<int> valores)
        {
            int[] rangos = new int[2];
            if (valores.Count > 0)
            {
                valores.Sort();
                rangos[0] = valores[0];
                rangos[1] = valores[valores.Count - 1];
            }
            return rangos;
        }
        public static string ObtenerRangosSeparadosPorGuion(string valoresSeparadosPorComas)
        {
            string salida = "";
            string[] valores = valoresSeparadosPorComas.Split(',');
            if (valores.Length > 1)
            {
                salida = valores[0] + "-" + valores[valores.Length - 1];
            }
            return salida;
        }
        public static List<string> ObtenerGruposDeValores(int cantidadElementos, bool pasoFijo)
        {
            List<string> lista = new List<string>();
            for (int i = 1; i <= VariablesGlobales.NumeroPartidos; i++)
            {
                int inicial = i;
                for (int j = i + 1; j <= VariablesGlobales.NumeroPartidos; j++)
                {
                    if (j - i == cantidadElementos - 1)
                    {
                        string cadena = ObtenerRangosSeparadosPorGuion(ObtenerTodosValores(inicial, j));
                        lista.Add(cadena);
                        if (pasoFijo)
                        {
                            i = j;
                        }
                        break;
                    }

                }
            }
            return lista;
        }

        public static string PonerEnMayuscula(string palabra)
        {
            char[] separadores = new char[]{'.',' '};
            string palabraEnMayuscula = "";
            if (palabra.Length > 1)
            {
                //Comprobar si hay más de un nombre que capitalizar
                if (palabra.Split(separadores).Length > 1)
                {
                    string[] partes = palabra.Split(separadores);
                    for (int i = 0; i < partes.Length; i++)
                    {
                        palabraEnMayuscula += PonerEnMayuscula(partes[i]) + " ";
                    }
                }
                else
                {
                    string primeraLetra = palabra[0].ToString().ToUpper();
                    string resto = palabra.Substring(1, palabra.Length - 1);

                    palabraEnMayuscula = primeraLetra + resto;
                }
            }
            else
            {
                return palabra.ToUpper();
            }
            return palabraEnMayuscula;
        }
    }
}
