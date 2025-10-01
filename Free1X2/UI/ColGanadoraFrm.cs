using System.Windows.Forms;
using Free1X2.MotorCalculo;
using Free1X2.EntradaSalida;
using Free1X2.Utils;

namespace Free1X2.UI
{
	/// <summary>
	/// Descripción breve de ColGanadoraFrm.
	/// </summary>
	public class ColGanadoraFrm : Form
	{
		private Label labCG;
		public TextBox txtCG;
        private Button btnComienzo;
		private RadioButton rbCombiPantalla;
		private RadioButton rbAbrirCombi;
		private int numPartidos=14;
		private string nombreComb;
		private Analizador analizadorBase;
		private string[] listaPronosticos;
		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ColGanadoraFrm(int nPartidos, string nombreCombi, Analizador analizador, string[] listaPronosticos)
		{
			numPartidos=nPartidos;
			nombreComb=nombreCombi;
			analizadorBase=analizador;
			this.listaPronosticos=listaPronosticos;

			InitializeComponent();

            FormulariosHelper fHelper = new FormulariosHelper();
            fHelper.Traducir(this);
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

		#region Código generado por el Diseñador de Windows Forms
		/// <summary>
		/// Método necesario para admitir el Diseñador. No se puede modificar
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColGanadoraFrm));
            this.labCG = new System.Windows.Forms.Label();
            this.txtCG = new System.Windows.Forms.TextBox();
            this.btnComienzo = new System.Windows.Forms.Button();
            this.rbAbrirCombi = new System.Windows.Forms.RadioButton();
            this.rbCombiPantalla = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // labCG
            // 
            this.labCG.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labCG.Location = new System.Drawing.Point(6, 11);
            this.labCG.Name = "labCG";
            this.labCG.Size = new System.Drawing.Size(131, 16);
            this.labCG.TabIndex = 10;
            this.labCG.Text = "Columna Ganadora";
            this.labCG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtCG
            // 
            this.txtCG.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCG.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCG.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCG.Location = new System.Drawing.Point(143, 9);
            this.txtCG.MaxLength = 16;
            this.txtCG.Name = "txtCG";
            this.txtCG.Size = new System.Drawing.Size(124, 21);
            this.txtCG.TabIndex = 0;
            // 
            // btnComienzo
            // 
            this.btnComienzo.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnComienzo.FlatAppearance.BorderSize = 0;
            this.btnComienzo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnComienzo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnComienzo.Image = ((System.Drawing.Image)(resources.GetObject("btnComienzo.Image")));
            this.btnComienzo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnComienzo.Location = new System.Drawing.Point(86, 126);
            this.btnComienzo.Name = "btnComienzo";
            this.btnComienzo.Size = new System.Drawing.Size(100, 29);
            this.btnComienzo.TabIndex = 1;
            this.btnComienzo.Text = "Analizar";
            this.btnComienzo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnComienzo.UseVisualStyleBackColor = false;
            this.btnComienzo.Click += new System.EventHandler(this.btnComienzo_Click);
            // 
            // rbAbrirCombi
            // 
            this.rbAbrirCombi.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbAbrirCombi.Location = new System.Drawing.Point(28, 73);
            this.rbAbrirCombi.Name = "rbAbrirCombi";
            this.rbAbrirCombi.Size = new System.Drawing.Size(216, 16);
            this.rbAbrirCombi.TabIndex = 13;
            this.rbAbrirCombi.Text = "Abrir combinación para analizar ";
            // 
            // rbCombiPantalla
            // 
            this.rbCombiPantalla.Checked = true;
            this.rbCombiPantalla.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbCombiPantalla.Location = new System.Drawing.Point(28, 49);
            this.rbCombiPantalla.Name = "rbCombiPantalla";
            this.rbCombiPantalla.Size = new System.Drawing.Size(216, 16);
            this.rbCombiPantalla.TabIndex = 12;
            this.rbCombiPantalla.TabStop = true;
            this.rbCombiPantalla.Text = "Analizar combinación en pantalla";
            // 
            // ColGanadoraFrm
            // 
            this.AcceptButton = this.btnComienzo;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(273, 165);
            this.Controls.Add(this.rbAbrirCombi);
            this.Controls.Add(this.rbCombiPantalla);
            this.Controls.Add(this.labCG);
            this.Controls.Add(this.txtCG);
            this.Controls.Add(this.btnComienzo);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColGanadoraFrm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Análisis de fallos en combinación";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion



		private void btnComienzo_Click(object sender, System.EventArgs e)
		{
			string cg=txtCG.Text;
            if (cg.Length != numPartidos)
            {
                MessageBox.Show("La columna ganadora debe tener " + VariablesGlobales.NumeroPartidos + " signos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

			string archivo;
			Analizador analizador;
			string[] miListaPronosticos;
			// Comprueba si queremos analizar la combinación en pantalla o abrimos una
			if(rbCombiPantalla.Checked)
			{
				// Combi en pantalla
				archivo=nombreComb;
				analizador=analizadorBase;
				miListaPronosticos=listaPronosticos;
			}
			else
			{
				// Abrimos una combinación
				OpenFileDialog abreCombDialog = new OpenFileDialog();
				abreCombDialog.InitialDirectory = "Combinaciones\\" ;
                abreCombDialog.Filter = "Todas las combinaciones(*.comb, *.xml)|*.comb; *.xml|Combinaciones(*.comb)|*.comb|Combinaciones(*.xml)|*.xml|Todos los archivos (*.*)|*.*";

				if(abreCombDialog.ShowDialog() == DialogResult.OK)
				{
					archivo = abreCombDialog.FileName;
		    	
					//leer combinacion desde archivo
					ArchivoCombinacion archComb = new ArchivoCombinacion();
					archComb.AbrirArchivoCombinacion( archivo );
					analizador = new Analizador();
					archComb.CargaControladorGrupos( analizador.CtrlGrupos );
					miListaPronosticos=archComb.LeePronosticos();
					string tmp;
					for(int i=0;i<miListaPronosticos.Length;i++)
					{
						tmp=miListaPronosticos[i];
						miListaPronosticos[i]=tmp.Replace(",","");
					}
				}
				else
					return;
			}
			Analisis.AnalisisCombinacion a=new Analisis.AnalisisCombinacion();
			a.AnalizarCombinacion(archivo, UtilColumnas.ConvStrToLong(cg), analizador, miListaPronosticos);
		}
	}
}
