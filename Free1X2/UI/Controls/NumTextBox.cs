using System;
using System.Windows.Forms;

namespace Free1X2.UI.Controls
{
	/// <summary>
	/// Descripción breve de NumTextBox.
	/// </summary>
	public class NumTextBox : TextBox
	{
		/// <summary> 
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public NumTextBox()
		{
			InitializeComponent();
		}

		/// <summary> 
		/// Limpiar los recursos que se estén utilizando.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Método necesario para admitir el Diseñador. No se puede modificar 
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// NumTextBox
			// 
			this.Text = "0";
			this.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.Leave += new System.EventHandler(this.NumTextBox_Leave);

		}
		#endregion

		private void NumTextBox_Leave(object sender, EventArgs e)
		{
			string numeros="0123456789-";
		    string texto=Text;
			// Establece el punto decimal y lo cambia
			string punto = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
			numeros+=punto;
			char[] arrayNumeros=numeros.ToCharArray();
			if(punto==".")
			{
				texto.Replace(",", ".");
			}
			if(punto==",")
			{
				texto.Replace(".", ",");
			}

			// Comprueba la longitud de la cadena
			if(texto.Length==0)
			{
				texto="0";
			}
			for(int i=0;i<texto.Length;i++)
			{
			    char caracter = texto.ToCharArray()[i];
			    if(Array.IndexOf(arrayNumeros,caracter)<0)
				{
					MessageBox.Show("Se han detectado caracteres incorrectos. El campo debe ser numérico.","",MessageBoxButtons.OK,MessageBoxIcon.Warning);
					Select(i,1);
					Focus();
					return;
				}
			}
		    Text=texto;
		}
	}

}
