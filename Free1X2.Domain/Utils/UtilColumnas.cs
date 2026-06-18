// Free1X2 · WinUI 3 — WIN3
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Luis Fernandez - luifer@onetel.net.uk
// Copyright (C) 2004 Joan Duatis - duatis@coac.net
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

using System.Text;

namespace Free1X2.Utils
{
	public class UtilColumnas
	{
        public static string ConvLongToStr(long L)
        {
            StringBuilder res = new StringBuilder();
            while (L != 0)
            {
                res.Append("2X1"[(int)(L & 7) >> 1]);
                L >>= 3;
            }
            return res.ToString();
        }
            ///<summary>
            ///Devuelve el signo del <paramref name="partido"/> en una columna <paramref name="L"/> expresada como un long
            ///</summary>
            ///<param name="L"> Columna 1X2 expresada como un long </param>
            ///<param name="partido"> Partido que se va a buscar </param>

        public static string ObtenerSigno(long L, int partido)
        {

            //Se nos pasa el número del partido, por lo que
            //posicion = partido-1;
            //Obtenemos el primer signo antes de mover los bits, por lo
            //que por ejemplo, hay que mover una vez para ver el partido2,
            //Dos veces para ver el partido 3, etc...

            StringBuilder res = new StringBuilder();
            res.Append("2X1"[(int)((L >>= ((partido - 1) * 3)) & 7) >> 1]);

            return res.ToString();
        }

        public static int ObtenerSignoInt(long L, int partido)
        {

            //Se nos pasa el número del partido, por lo que
            //posicion = partido-1;
            //Obtenemos el primer signo antes de mover los bits, por lo
            //que por ejemplo, hay que mover una vez para ver el partido2,
            //Dos veces para ver el partido 3, etc...

            return (int)((L >>= ((partido - 1) * 3)) & 7);
        }

        public static long ConvStrToLong(string[] Pbase)
        {
            long L = 0;

            for (int i = Pbase.Length - 1; i > -1; i--)
            {
                int b = 0;
                for (int j = 0; j < Pbase[i].Length; j += 2)
                {
                    b |= 1 << "2X1".IndexOf(Pbase[i][j]);
                }
                L = (L << 3) | (uint)b;
            }
            return L;
        }

        public static byte ConvPartidoStrToByte(string partido)
        {
            byte partidoByte;

            switch (partido)
            { 
                case "1":
                    partidoByte = 4;
                    break;
                case "X":
                    partidoByte = 2;
                    break;
                case "2":
                    partidoByte = 1;
                    break;
                case "1X":
                case "1,X":
                    partidoByte = 6;
                    break;
                case "12":
                case "1,2":
                    partidoByte = 5;
                    break;
                case "X2":
                case "X,2":
                    partidoByte = 3;
                    break;
                case "1X2":
                case "1,X,2":
                    partidoByte = 7;
                    break;
                default:
                    partidoByte = 0;
                    break;

            }

            return partidoByte;        
        }

        public static string ConvPartidoByteToStr(byte partido)
        {
           string partidoStr;

            switch (partido)
            {
                case 4:
                    partidoStr = "1";
                    break;
                case 2:
                    partidoStr = "X";
                    break;
                case 1:
                    partidoStr = "2";
                    break;
                case 6:
                    partidoStr = "1X";
                    break;
                case 5:
                    partidoStr = "12";
                    break;
                case 3:
                    partidoStr = "X2";
                    break;
                case 7:
                    partidoStr = "1X2";
                    break;
                default:
                    partidoStr = "";
                    break;
            }

            return partidoStr;
        }
        public static long ConvStrToLong(string s, string partidosDeMenos)
        {
            if (partidosDeMenos == "")
            {
                return ConvStrToLong(s, VariablesGlobales.NumeroPartidos - s.Length);
            }
            s += partidosDeMenos;
            long res = 0;
            for (int i = s.Length - 1; i > -1; i--)
            {
                res = (res <<= 3) ^ (1 << "2X1".IndexOf(s[i]));
            }
            return res;
        }
        public static long ConvStrToLong(string s, int partidosDeMenos)
        {
            string[] signos = new string[] { "1", "11", "111", "1111", "11111", "111111", "1111111", "11111111", "111111111", "1111111111", "11111111111", "111111111111", "1111111111111", "11111111111111", "111111111111111" };
            s += signos[partidosDeMenos - 1];
            long res = 0;
            for (int i = s.Length - 1; i > -1; i--)
            {
                res = (res <<= 3) ^ (1 << "2X1".IndexOf(s[i]));
            }
            return res;
        }
        public static long ConvStrToLong(string s)
        {
            long res = 0;
            for (int i = s.Length - 1; i > -1; i--)
            {
                res = (res <<= 3) ^ (1 << "2X1".IndexOf(s[i]));
            }
            return res;
        }
        public static long ConvStrToLong(string s, bool[] partidosMarcados)
        {
            long res = 0;
            for (int i = s.Length - 1; i > -1; i--)
            {
                if (partidosMarcados[i])
                {
                    res = (res <<= 3) ^ (1 << "2X1".IndexOf(s[i]));
                }
                else
                {
                    res = (res <<= 3) | 0;
                }
            }
            return res;
        }

        public static int ConvStrToInt32(string s)
        {
            //Este método se usa para comprimir con un método experimental
            int res = 0;
            int longitud = 0;
            string temp = "";
            string colTemp = "";
            for (int i = 0; i <= s.Length; i++)
            {
                if (colTemp.Length < s.Length)
                {
                    if (temp.Length < 2)
                    {
                        temp += s[i];
                        colTemp += s[i];
                    }
                    else
                    {
                        if (longitud * 2 < s.Length)
                        {
                            i--;
                        }
                        switch (temp)
                        {
                            case "11":
                                res = (res <<= 4) ^ 1;
                                break;
                            case "1X":
                                res = (res <<= 4) ^ 2;
                                break;
                            case "12":
                                res = (res <<= 4) ^ 3;
                                break;
                            case "X1":
                                res = (res <<= 4) ^ 4;
                                break;
                            case "XX":
                                res = (res <<= 4) ^ 5;
                                break;
                            case "X2":
                                res = (res <<= 4) ^ 6;
                                break;
                            case "21":
                                res = (res <<= 4) ^ 7;
                                break;
                            case "2X":
                                res = (res <<= 4) ^ 8;
                                break;
                            case "22":
                                res = (res <<= 4) ^ 9;
                                break;

                        }
                        temp = "";
                        longitud++;
                    }
                }
            }
            //¿Ha quedado algún signo suelto?
            if (temp.Length == 1)
            {
                switch (temp)
                {
                    case "1":
                        res = (res <<= 4) ^ 10;
                        break;
                    case "X":
                        res = (res <<= 4) ^ 11;
                        break;
                    case "2":
                        res = (res <<= 4) ^ 12;
                        break;
                }
            }
            return res;
        }

        public static int ContarBitsA1 (long b)
        {            
           //contamos el numero de bits en el long que estan puestos a 1

           int count = 0;

           while (b!=0)
           {
              b = b & (b-1);
              count++;
           }

           return count;
        }

		public int Compara( string ColumnaBase, string Columna)
		{
			//string columnas con formato 1x12x111111111		
		   
			int noSignosDistintos = 0;
			
			char[] arrayColumnaBase = ColumnaBase.ToCharArray();
			char[] arrayColumna = Columna.ToCharArray();			
			
			for(int i = 0; i < arrayColumnaBase.Length; i++ )
			{
				if( !arrayColumnaBase[i].Equals( arrayColumna[i] ) )
				{
					noSignosDistintos++;
				}			
			}		
		
			return noSignosDistintos;
		}	
		
		public int Compara( string ColumnaBase, string Columna, int Maximo)
		{
			// Si las diferencias superal al máximo indicado, deja de comparar y sale.
			//string columnas con formato 1x12x111111111
			int noSignosDistintos = 0;
			char[] arrayColumnaBase = ColumnaBase.ToCharArray();
			char[] arrayColumna = Columna.ToCharArray();			
			
			for(int i = 0; i < arrayColumnaBase.Length; i++ )
			{
				if( !arrayColumnaBase[i].Equals( arrayColumna[i] ) )
				{
					noSignosDistintos++;
					if(noSignosDistintos>Maximo) return noSignosDistintos;
				}			
			}
			return noSignosDistintos;
		}	
		
		public int ObtenNoCoincidencias( string ColumnaBase, string Columna )
		{
		
			//string columnas con formato 1x12x111111111		
		
			int noCoincidencias = 0;
			
			char[] arrayColumnaBase = ColumnaBase.ToCharArray();
			char[] arrayColumna = Columna.ToCharArray();			
			
			for(int i = 0; i < arrayColumnaBase.Length; i++ )
			{
				if( arrayColumnaBase[i].Equals( arrayColumna[i] ) )
				{
					noCoincidencias++;
				}			
			}		
		
			return noCoincidencias;
	
		}

        public static long ObtenerLongFromTxtApuestaMultiple(string[] partidos)
        {
            long columnaLong = 0;
            for (int i = partidos.Length - 1; i >= 0; i--)
            {
                columnaLong <<= 3;
                columnaLong |= ConvPartidoStrToByte(partidos[i]);
            }
            return columnaLong;
        }
        public static string ObtenerStringFromLongApuestaMultiple(long columna)
        {
            string pronosticos = "";
            while (columna != 0)
            {
                pronosticos += ConvPartidoByteToStr((byte)(columna & 7)) + ",";
                columna >>= 3;
            }
            return pronosticos.Substring(0,pronosticos.Length - 1);
        }

        public static long PonerPartidoATriple(bool[] partidos, long col)
        {
            long columna = 0;
            for (int i = partidos.Length - 1; i > 0; i--)
            {
                if (!partidos[i])
                {
                    columna = columna | 7;
                }

                columna <<= 3;
            }
            return (columna | col);
        }

        public static long CambiarSignoPartido(long L, int partido, byte signo, int partidos)
        {
            long columna = 0;
            long B = L;

            for (int i = partidos; i > 0; i--)
            {
                columna <<= 3;

                if (i == partido)
                {
                    columna |= signo;
                }
                else
                {
                    columna |= (byte)ObtenerSignoInt(L, i);
                }

                B >>= 3;
            }

            return columna;
        }

        public static long IgnorarPartidos(bool[] partidos, long col)
        {
            long mascaraTemp = 0;
            
            for (int i = partidos.Length - 1; i > 0; i--)
            {               
                if (partidos[i])
                {
                    byte partido = (byte)ObtenerSignoInt(col, i);
                    mascaraTemp <<= 3;
                    mascaraTemp |= partido;
                    
                }
            }
            return mascaraTemp;
        }
        public static long ObtenerMascaraParcial(bool[] partidos)
        {
            long mascaraTemp = 0;
            for (int i = partidos.Length - 1; i > 0; i--)
            {
                mascaraTemp <<= 3;
                if (!partidos[i])
                {
                    mascaraTemp |= 0;
                }
                else
                {
                    mascaraTemp |= 7;
                }
            }
            return mascaraTemp;
        }

        public static long ConvStrToLongDosBits(string columna)
        {
            string s = columna.ToUpper();
            long res = 0;
            for (int i = s.Length - 1; i > -1; i--)
            {
                res <<= 3;
                switch (s[i])
                {
                    case '1':
                        res ^= 2;
                        break;
                    case 'X':
                        res ^= 3;
                        break;
                    case '2':
                        res ^= 1;
                        break;
                    case '-':
                        res ^= 0;
                        break;
                }
            }
            return res;
        }
    }
}
