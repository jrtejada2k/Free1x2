using System;
using System.Windows.Forms ;
using System.Drawing ;
using System.Drawing.Drawing2D ;
using System.ComponentModel ;

namespace Free1X2.UI.Controls
{
	/// <summary>
	/// Summary description for Vertical_Label.
	/// </summary>
	public class Vertical_Label: System.Windows.Forms.Label 
	{
		public Vertical_Label()
		{

		}
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g=e.Graphics ;

			StringFormat stringFormat=new StringFormat ();
			stringFormat.Alignment =StringAlignment.Center ;
			stringFormat.Trimming =StringTrimming.None ;
			stringFormat.FormatFlags =StringFormatFlags.DirectionVertical ;
 
			Brush textBrush=new SolidBrush (this.ForeColor );
			Matrix storeState=g.Transform ;
			g.RotateTransform (180f);
			g.TranslateTransform (-ClientRectangle.Width ,-ClientRectangle.Height );
 
			g.DrawString (this.Text,this.Font ,textBrush,ClientRectangle ,stringFormat );
			g.Transform =storeState;

		}
	}
}
