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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms ;

namespace Free1X2.Utils
{
	/// <summary>
	/// Summary description for Grafico.
	/// </summary>
	public class Grafico
	{
		// Valores del mundo real
		private double _Xmin;
		private double _Ymin;
		private double _Xmax;
		private double _Ymax;
		

		//valores del dispositivo fisico
		private int _Width;
		private int _Height;
		private Point[] _Puntos;

		//valores para convertir valores reales en fisicos
		private double _EscalaX;
		private double _EscalaY;

		private PictureBox _objeto;
		private Bitmap B;

		public Grafico(Point[] Puntos, PictureBox obj)
		{
			_objeto = obj;

			B = new Bitmap(_objeto.Width, _objeto.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			if(_objeto.Image != null) B = (Bitmap) _objeto.Image ;

			ObtenerMaximosyMinimos (Puntos);
			EscalarPuntos(Puntos);

		}

		public double MinEjeX
		{
			get{ return _Xmin;}
			set{_Xmin = value;}
		}
		public double MaxEjeX
		{
			get{ return _Xmax;}
			set{_Xmax = value;}
		}
		public double MinEjeY
		{
			get{ return _Ymin;}
			set{_Ymin = value;}
		}
		public double MaxEjeY
		{
			get{ return _Ymax;}
			set{_Ymax = value;}
		}
		public double EscalaX
		{
			get{ return _EscalaX;}
			set{_EscalaX = value;}
		}
		public double EscalaY
		{
			get{ return _EscalaY;}
			set{_EscalaY = value;}
		}
		public int XFisica( double Xreal)
		{
			return (int) (Xreal/_EscalaX);
		}
		public int YFisica( double Yreal)
		{
			return (int) (Yreal/_EscalaY);
		}
		public void DibujarEjes()
		{
			Graphics Lienzo = Graphics.FromImage(B);
			//Eje horizontal
			Pen Lapiz = new Pen(Color.Purple, 3);
			Lapiz.EndCap = LineCap.ArrowAnchor;
			Lienzo.DrawLine(Lapiz, (new Point((int)((Math.Abs (_Xmin)*_EscalaX)), _objeto.Height)),new Point((int)((Math.Abs(_Xmin)*_EscalaX)) , 0) );
			//Eje vertical
			Lienzo.DrawLine(Lapiz, new Point(0, (int)((Math.Abs (_Ymax)*_EscalaY)-1)), new Point(_objeto.Width, (int)((Math.Abs (_Ymax)*_EscalaY)-1)));
			_objeto.Image = B;
			Lienzo.Dispose(); //Al final cerramos el objeto Graphics

		}
		public void DibujaGrid(int intervalo)
		{
			Graphics Lienzo = Graphics.FromImage(B);
			Lienzo.DrawPath(new Pen(Color.Silver,1 ), Grid(intervalo));
			_objeto.Image =B;
			Lienzo.Dispose();
		}

		private GraphicsPath Grid(int Intervalo)
		{

			int i;
			//Necesitamos saber el resto obtenido de dividir la anchura o la altura
			//por la distancia entre cada línea recogida en el parámetro Intervalo
			//Para ello el operador mod nos viene que ni pintado.
			//Dividimos el resultado por Dos para repartir el margen 
			//a los Dos extremos de la cuadrícula
			int MargenXInicial  = 0; //(papel.Width % Intervalo) / 2;
			int MargenYInicial  = _objeto.Height % Intervalo;
			GraphicsPath Trayecto = new GraphicsPath();

			//Comenzando por el margen inicial, vamos dibujando líneas
			//guardando entre ellas la distancia que nos indica la variable Intervalo
			//Líneas verticales

			for (i = MargenXInicial; i<(_objeto.Width - MargenXInicial);i+=Intervalo)
			{
				Trayecto.AddLine(new Point(i, 0), new Point(i, _objeto.Height));
				Trayecto.StartFigure();
			}
			//Líneas horizontales
			for (i = MargenYInicial;i<(_objeto.Height - MargenYInicial);i+=Intervalo)
			{
				Trayecto.AddLine(new Point(0, i), new Point(_objeto.Width, i));
				Trayecto.StartFigure();
			}
			return Trayecto;
		}
		public void DibujaCurva(Pen pen)
		{
			Graphics Lienzo = Graphics.FromImage(B);
			Lienzo.DrawCurve (pen, _Puntos, (float) 0.1);
			_objeto.Image =B;
			Lienzo.Dispose ();
		}
		private void ObtenerMaximosyMinimos (Point[] Puntos)
		{
			// encontramos los valores maximos y minimos
			Point x =Puntos[0] ;
			_Xmin=x.X;
			_Xmax=x.X;
			_Ymin=x.Y;
			_Ymax=x.Y;

			foreach (Point Punto in Puntos)
			{
				_Xmin=Math.Min (_Xmin,Punto.X );
				_Xmax=Math.Max (_Xmax,Punto.X );
				_Ymin=Math.Min (_Ymin,Punto.Y );
				_Ymax=Math.Max (_Ymax,Punto.Y );
			}
		}
		private void EscalarPuntos(Point[] Puntos)
		{
			_Width = _objeto.Width;
			_Height = _objeto.Height;
			_EscalaX= _Width/(_Xmax-_Xmin);
			_EscalaY= _Height/(_Ymax-_Ymin);
			_Puntos = new Point[Puntos.GetUpperBound(0)+1];
			int i=0;
			foreach (Point Punto in Puntos)
			{
				_Puntos.SetValue(new Point((int)(_EscalaX*Punto.X) , (int) (_EscalaY*(_Ymax-Punto.Y))), i);
				i++;
			}
		}
	}
}
