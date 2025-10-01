// created on 19/03/2005 at 11:40
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
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.using System;
using System;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using Free1X2.EntradaSalida;

namespace Free1X2.Utils
{
	/// <summary>
	/// Summary description for LAE.
	/// </summary>
	public class LAE
	{
		private ArrayList Jornadas;
		private string _ColumnaPremiada="";
		private int[] _Acertantes = new int[5];
		private readonly double _Recaudacion;
		private readonly double _PrecioApuesta;
        private readonly string moneda = "";
		private readonly double[] _PorcentajePremioCategoria = new double[5];
		private double[] _Premios = new double[5];
		private string _Temporada;
		private string _Jornada;
		public LAE(string Temporada, string numJornada, string columnaPremiada, double recaudacion)
		{
			_Temporada=Temporada;
			_Jornada=numJornada.PadLeft (2,'0');
			_ColumnaPremiada=columnaPremiada;


			AConfiguracion ac = new AConfiguracion(Application.StartupPath);
			ac.ObtenValoresLAE(ref _PrecioApuesta, ref _PorcentajePremioCategoria[0], ref _Recaudacion, ref moneda);
			_PorcentajePremioCategoria[1]=10;
			_PorcentajePremioCategoria[2]=10;
			_PorcentajePremioCategoria[3]=10;
			_PorcentajePremioCategoria[4]=10;
			if(recaudacion>0) _Recaudacion =recaudacion;

		}
		public string ColumnaPremiada
		{
			get{ return _ColumnaPremiada; }
			set{_ColumnaPremiada = value;}
		}
		public string Temporada
		{
			get{ return _Temporada; }
			set{_Temporada = value;}
		}
		public string Jornada
		{
			get{ return _Jornada; }
			set{_Jornada = value;}
		}
		public double[] Premios
		{
			get{ return _Premios; }
			set{_Premios = value;}
		}
		public void GrabarJornada(int[] acertantes)
		{
			//---Guardar datos del L.A.E. de la Jornada ------

			Jornadas =new ArrayList();
			
			string[] ValorsJornada;
			bool JornadaYaExiste =false;
			_Acertantes = acertantes;
			CalculaPremios();

            IArchivoColumnas comBaseCols;
			string NombreFicheroJornadas=Application.StartupPath + "/Jornadas/InfoJornadasLAE.txt";
			comBaseCols = new ArchivoColumnasTexto(NombreFicheroJornadas);
			StringBuilder linea= new StringBuilder ("");
			while( comBaseCols.SiguienteColumna() )
			{
				linea.Remove (0,linea.Length );
				linea.Append (comBaseCols.LeeColumnaSinComas());
				ValorsJornada=  linea.ToString ().Split ((char) 9);
				
				if(ValorsJornada[1]==_Temporada  && ValorsJornada[2]==_Jornada) 
				{
					JornadaYaExiste=true;
					linea.Remove (0,linea.Length );
					linea.Append ( MontaLinea());
				}
				Jornadas.Add (linea.ToString());
			}
			if(!JornadaYaExiste)
			{
				linea.Remove (0,linea.Length );
				linea.Append ( MontaLinea());
				Jornadas.Add (linea.ToString());
			}
			comBaseCols.Cerrar();
            IArchivoColumnas comCols = new ArchivoColumnasTexto(NombreFicheroJornadas);
			foreach(string str in Jornadas)
			{
				comCols.GuardarColsComa(str);
			}
			comCols.Cerrar();	
		}
		private string MontaLinea()
		{
			StringBuilder linea= new StringBuilder (_ColumnaPremiada);
			char sep=(char)9;
			double Recaudacion =_Recaudacion/_PrecioApuesta;

			linea.Append( sep);
			linea.Append( _Temporada);
			linea.Append( sep);
			linea.Append( _Jornada);
			linea.Append( sep);
			linea.Append( Recaudacion.ToString ().Replace (".",","));
			
			for (int i=0;i<5;i++)
			{
				linea.Append( sep);
				linea.Append( _Premios[i].ToString ().Replace (".",","));
			}

			return linea.ToString ();
		}
		private void CalculaPremios()
		{
			for (int i=0;i<5;i++)
			{
				_Premios [i]=_Recaudacion / _PrecioApuesta  * _PorcentajePremioCategoria [i]/100/_Acertantes [i];
				_Premios [i]=Math.Round (_Premios [i]+0.005,2);
			}
		}

	}
}
