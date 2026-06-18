// Free1X2 · WinUI 3 — WIN3
// created on 20/07/2004 at 18:12
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Joan Duatis - duatis@coac.net
// 
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
	/// <summary>
	/// Summary description for ConvertidorDeBases.
	/// </summary>
	public class ConvertidorDeBases
	{
        private readonly int[] PotDe3 = new int[] { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323, 4782969, 14348907, 43046721 };
        private readonly int[] DosPotDe3 = new int[] { 2, 6, 18, 54, 162, 486, 1458, 4374, 13122, 39366, 118098, 354294, 1062882, 3188646, 9565938, 28697814, 86093442 };

        private readonly char[] SignoQuini = new char[] { '1', 'X', '2', ' '};
	    readonly byte _NumPartidos;

		public ConvertidorDeBases()
		{
            _NumPartidos=(byte)VariablesGlobales.NumeroPartidos;
		}
        public ConvertidorDeBases(byte pNumPartidos)
        {
            _NumPartidos = pNumPartidos;
        }
        
		public int ConvColumnaANumero (string columna)
		{
			int b=0;

            for (byte i = 0; i < columna.Length; i++) 
			{ 
				if (columna[i]=='1') {} 
				else if ( columna[i]=='2' ) b+=DosPotDe3[i];
				else  b+=PotDe3[i]; 
			} 
			return b;
		}
        public int ConvColumnaANumero(string columna, int numPartidos)
        {
            int b = 0;

            for (byte i = 0; i < numPartidos; i++)
            {
                if (columna[i] == '1') { }
                else if (columna[i] == '2') b += DosPotDe3[i];
                else b += PotDe3[i];
            }
            return b;
        }

        public int ConvLongANumero(long L)
        {
            int b = 0;
            int partido = 0;
            while (L != 0)
            {
                byte bt = (byte)(L & 7);
                if (bt == 4) //El 1
                {
                    
                }
                else if (bt == 1) //El 2
                {
                    b += DosPotDe3[partido];
                }
                else if (bt == 2)// La X
                {
                    b += PotDe3[partido];
                }
                else
                {
                    b += 7;
                }
                L >>= 3;
                partido++;
            }
            return b;
        }
        public string ConvNumAColumna(int Numero)
        {
            StringBuilder Columna = new StringBuilder();
            do
            {
                Columna.Append (SignoQuini[Numero % 3]);
                Numero /= 3;
            } while (Numero != 0);
            return Columna.Append('1', _NumPartidos - Columna.Length).ToString();
            
        }

		public string ConvNumAColumna ( long Numero )
		{
			StringBuilder Columna = new StringBuilder ();
			do
			{
				Columna.Append ( SignoQuini[Numero % 3] );
				Numero /= 3;
			} while (Numero != 0);
			return Columna.Append ( '1', _NumPartidos - Columna.Length ).ToString ();
		}
        public int ObtenTamañoBitArray(int longitud)
        {
            return PotDe3[longitud];
        }
        public int ObtenNumeroPartidosColBin(int longitud)
        {
            int numPar;
            switch (longitud)
            {
                case 1:
                    numPar = 0;
                    break;
                case 3:
                    numPar = 1;
                    break;
                case 9:
                    numPar = 2;
                    break;
                case 27:
                    numPar = 3;
                    break;
                case 81:
                    numPar = 4;
                    break;
                case 243:
                    numPar = 5;
                    break;
                case 729:
                    numPar = 6;
                    break;
                case 2187:
                    numPar = 7;
                    break;
                case 6561:
                    numPar = 8;
                    break;
                case 19683:
                    numPar = 9;
                    break;
                case 59049:
                    numPar = 10;
                    break;
                case 177147:
                    numPar = 11;
                    break;
                case 531441:
                    numPar = 12;
                    break;
                case 1594323:
                    numPar = 13;
                    break;
                case 4782969:
                    numPar = 14;
                    break;
                case 14348907:
                    numPar = 15;
                    break;
                case 43046721:
                    numPar = 16;
                    break;
                default:
                    numPar = 14;
                    break;
            }
            return numPar;
        }
	}
}

