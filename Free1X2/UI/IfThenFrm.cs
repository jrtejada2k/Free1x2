using System;
using System.Windows.Forms;
using Free1X2.MotorCalculo;
using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	/// <summary>
	/// Descripción breve de IfThenFrm.
	/// </summary>
	public class IfThenFrm : Form
	{
		private Panel panel1;
		private Controls.MenuCondiciones menuCondiciones1;
		private TabControl tabControl1;
		private TabPage tabPage1;
		private TabPage tabPage2;
		private TextBox txtCondsLista;
		private TextBox txtConds;
		private Button btnAdd;
		private GroupBox groupBox2;
		private ComboBox comboCE_Then;
		private ComboBox comboCG_Then;
		private Label label6;
		private Label label7;
		private Label label8;
		private GroupBox groupBox1;
		private ComboBox comboCE_If;
		private ComboBox comboCG_If;
		private Label label5;
		private Label label4;
		private Label label3;
		private Label label2;
		private Label label1;
		private ListView listaCondiciones;
		private ColumnHeader columnHeader1;
		private ColumnHeader columnHeader2;
		private TextBox valor_Then;
		private TextBox valor_If;
		private NumericUpDown upDown_If;
		private NumericUpDown upDown_Then;
		private ListBox cmbGrupo_If;
		private ListBox cmbGrupo_Then;
		private Label label9;
		private Label label10;
		private ColumnHeader columnHeader3;
		private ColumnHeader columnHeader4;
		private Label label11;
		private Label label12;
		private ListView listaGrupos;
		private Button btnAddGrupo;
		private TextBox txtGruposLista;
		private TextBox txtGrupos;
		private Analizador analizador;
		private Button btnBorrarGrupo;
		private Button btnBorrar;
		private MainForm parentFrm;
		private CheckBox chkNoCond_If;
		private CheckBox chkNoCond_Then;
		private CheckBox chkNoGrupo_If;
		private CheckBox chkNoGrupo_Then;
        protected FormulariosHelper formHelper = new FormulariosHelper();

		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components;

		public IfThenFrm(MainForm frm)
		{
			InitializeComponent();
			parentFrm=frm;
			analizador=parentFrm.analizador;
			compruebaPegar();
			// Si hay datos previos, se cargan
			cargarDatos(analizador.IfThen);

            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IfThenFrm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuCondiciones1 = new Free1X2.UI.Controls.MenuCondiciones();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnBorrar = new System.Windows.Forms.Button();
            this.txtCondsLista = new System.Windows.Forms.TextBox();
            this.txtConds = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkNoCond_Then = new System.Windows.Forms.CheckBox();
            this.upDown_Then = new System.Windows.Forms.NumericUpDown();
            this.valor_Then = new System.Windows.Forms.TextBox();
            this.comboCE_Then = new System.Windows.Forms.ComboBox();
            this.comboCG_Then = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkNoCond_If = new System.Windows.Forms.CheckBox();
            this.upDown_If = new System.Windows.Forms.NumericUpDown();
            this.valor_If = new System.Windows.Forms.TextBox();
            this.comboCE_If = new System.Windows.Forms.ComboBox();
            this.comboCG_If = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listaCondiciones = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.chkNoGrupo_Then = new System.Windows.Forms.CheckBox();
            this.chkNoGrupo_If = new System.Windows.Forms.CheckBox();
            this.btnBorrarGrupo = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtGruposLista = new System.Windows.Forms.TextBox();
            this.txtGrupos = new System.Windows.Forms.TextBox();
            this.btnAddGrupo = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.listaGrupos = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.cmbGrupo_Then = new System.Windows.Forms.ListBox();
            this.cmbGrupo_If = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDown_Then)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDown_If)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.menuCondiciones1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 517);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(616, 48);
            this.panel1.TabIndex = 48;
            // 
            // menuCondiciones1
            // 
            this.menuCondiciones1.Alineacion = Free1X2.alignment.Horizontal;
            this.menuCondiciones1.AutoSize = true;
            this.menuCondiciones1.BackColor = System.Drawing.Color.Bisque;
            this.menuCondiciones1.BotonAbrir = true;
            this.menuCondiciones1.BotonAbrirEnabled = true;
            this.menuCondiciones1.BotonBorrar = true;
            this.menuCondiciones1.BotonBorrarEnabled = true;
            this.menuCondiciones1.BotonCancelar = true;
            this.menuCondiciones1.BotonCancelarEnabled = true;
            this.menuCondiciones1.BotonCopiar = true;
            this.menuCondiciones1.BotonCopiarEnabled = true;
            this.menuCondiciones1.BotonEstadisticas = true;
            this.menuCondiciones1.BotonEstadisticasEnabled = false;
            this.menuCondiciones1.BotonGuardar = true;
            this.menuCondiciones1.BotonGuardarEnabled = true;
            this.menuCondiciones1.BotonOk = true;
            this.menuCondiciones1.BotonOkEnabled = true;
            this.menuCondiciones1.BotonPegar = true;
            this.menuCondiciones1.BotonPegarEnabled = false;
            this.menuCondiciones1.Location = new System.Drawing.Point(280, 8);
            this.menuCondiciones1.Name = "menuCondiciones1";
            this.menuCondiciones1.NumBotones = 8;
            this.menuCondiciones1.Size = new System.Drawing.Size(306, 36);
            this.menuCondiciones1.TabIndex = 48;
            this.menuCondiciones1.BOk += new System.EventHandler(this.menuCondiciones1_BOk);
            this.menuCondiciones1.BGuardar += new System.EventHandler(this.menuCondiciones1_BGuardar);
            this.menuCondiciones1.BAbrir += new System.EventHandler(this.menuCondiciones1_BAbrir);
            this.menuCondiciones1.BPegar += new System.EventHandler(this.menuCondiciones1_BPegar);
            this.menuCondiciones1.BBorrar += new System.EventHandler(this.menuCondiciones1_BBorrar);
            this.menuCondiciones1.BCancelar += new System.EventHandler(this.menuCondiciones1_BCancelar);
            this.menuCondiciones1.BCopiar += new System.EventHandler(this.menuCondiciones1_BCopiar);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.HotTrack = true;
            this.tabControl1.ItemSize = new System.Drawing.Size(128, 20);
            this.tabControl1.Location = new System.Drawing.Point(8, 16);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(592, 495);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 49;
            this.tabControl1.TabStop = false;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Bisque;
            this.tabPage1.Controls.Add(this.btnBorrar);
            this.tabPage1.Controls.Add(this.txtCondsLista);
            this.tabPage1.Controls.Add(this.txtConds);
            this.tabPage1.Controls.Add(this.btnAdd);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.listaCondiciones);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(584, 467);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Condiciones sencillas";
            // 
            // btnBorrar
            // 
            this.btnBorrar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnBorrar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBorrar.Image = ((System.Drawing.Image)(resources.GetObject("btnBorrar.Image")));
            this.btnBorrar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBorrar.Location = new System.Drawing.Point(320, 184);
            this.btnBorrar.Name = "btnBorrar";
            this.btnBorrar.Size = new System.Drawing.Size(122, 32);
            this.btnBorrar.TabIndex = 66;
            this.btnBorrar.Text = "Borrar relación";
            this.btnBorrar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBorrar.UseVisualStyleBackColor = false;
            this.btnBorrar.Click += new System.EventHandler(this.btnBorrar_Click);
            // 
            // txtCondsLista
            // 
            this.txtCondsLista.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCondsLista.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCondsLista.Location = new System.Drawing.Point(168, 432);
            this.txtCondsLista.Name = "txtCondsLista";
            this.txtCondsLista.ReadOnly = true;
            this.txtCondsLista.Size = new System.Drawing.Size(32, 21);
            this.txtCondsLista.TabIndex = 54;
            this.txtCondsLista.Text = "0";
            this.txtCondsLista.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtConds
            // 
            this.txtConds.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtConds.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConds.Location = new System.Drawing.Point(476, 432);
            this.txtConds.Name = "txtConds";
            this.txtConds.Size = new System.Drawing.Size(32, 21);
            this.txtConds.TabIndex = 53;
            this.txtConds.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(160, 184);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(106, 32);
            this.btnAdd.TabIndex = 52;
            this.btnAdd.Text = "Añadir";
            this.btnAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkNoCond_Then);
            this.groupBox2.Controls.Add(this.upDown_Then);
            this.groupBox2.Controls.Add(this.valor_Then);
            this.groupBox2.Controls.Add(this.comboCE_Then);
            this.groupBox2.Controls.Add(this.comboCG_Then);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox2.Location = new System.Drawing.Point(48, 99);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(480, 73);
            this.groupBox2.TabIndex = 51;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " Entonces ... ";
            // 
            // chkNoCond_Then
            // 
            this.chkNoCond_Then.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNoCond_Then.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNoCond_Then.Location = new System.Drawing.Point(410, 40);
            this.chkNoCond_Then.Name = "chkNoCond_Then";
            this.chkNoCond_Then.Size = new System.Drawing.Size(64, 20);
            this.chkNoCond_Then.TabIndex = 8;
            this.chkNoCond_Then.Text = "Negado";
            // 
            // upDown_Then
            // 
            this.upDown_Then.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.upDown_Then.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.upDown_Then.Location = new System.Drawing.Point(360, 40);
            this.upDown_Then.Maximum = new decimal(new int[] {
            14,
            0,
            0,
            0});
            this.upDown_Then.Name = "upDown_Then";
            this.upDown_Then.Size = new System.Drawing.Size(40, 20);
            this.upDown_Then.TabIndex = 7;
            this.upDown_Then.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.upDown_Then.ValueChanged += new System.EventHandler(this.upDown_Then_ValueChanged);
            this.upDown_Then.Leave += new System.EventHandler(this.upDown_Then_ValueChanged);
            // 
            // valor_Then
            // 
            this.valor_Then.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.valor_Then.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.valor_Then.Location = new System.Drawing.Point(360, 64);
            this.valor_Then.Name = "valor_Then";
            this.valor_Then.Size = new System.Drawing.Size(48, 20);
            this.valor_Then.TabIndex = 5;
            this.valor_Then.Text = "0";
            this.valor_Then.Visible = false;
            // 
            // comboCE_Then
            // 
            this.comboCE_Then.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboCE_Then.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.comboCE_Then.Location = new System.Drawing.Point(144, 40);
            this.comboCE_Then.Name = "comboCE_Then";
            this.comboCE_Then.Size = new System.Drawing.Size(208, 21);
            this.comboCE_Then.TabIndex = 4;
            this.comboCE_Then.Visible = false;
            // 
            // comboCG_Then
            // 
            this.comboCG_Then.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboCG_Then.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.comboCG_Then.Items.AddRange(new object[] {
            "Cantidad de signos",
            "Dibujos",
            "Signos Seguidos",
            "Interrupciones",
            "Formatos",
            "Pesos numéricos",
            "Distancias",
            "Contactos"});
            this.comboCG_Then.Location = new System.Drawing.Point(16, 40);
            this.comboCG_Then.Name = "comboCG_Then";
            this.comboCG_Then.Size = new System.Drawing.Size(120, 21);
            this.comboCG_Then.TabIndex = 3;
            this.comboCG_Then.SelectedIndexChanged += new System.EventHandler(this.cambioCombo);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(360, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "Valor:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(144, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "Condición específica:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(16, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(105, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "Condición genérica:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkNoCond_If);
            this.groupBox1.Controls.Add(this.upDown_If);
            this.groupBox1.Controls.Add(this.valor_If);
            this.groupBox1.Controls.Add(this.comboCE_If);
            this.groupBox1.Controls.Add(this.comboCG_If);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox1.Location = new System.Drawing.Point(48, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(480, 79);
            this.groupBox1.TabIndex = 50;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Si ... ";
            // 
            // chkNoCond_If
            // 
            this.chkNoCond_If.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNoCond_If.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNoCond_If.Location = new System.Drawing.Point(408, 40);
            this.chkNoCond_If.Name = "chkNoCond_If";
            this.chkNoCond_If.Size = new System.Drawing.Size(64, 21);
            this.chkNoCond_If.TabIndex = 7;
            this.chkNoCond_If.Text = "Negado";
            // 
            // upDown_If
            // 
            this.upDown_If.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.upDown_If.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.upDown_If.Location = new System.Drawing.Point(360, 40);
            this.upDown_If.Maximum = new decimal(new int[] {
            14,
            0,
            0,
            0});
            this.upDown_If.Name = "upDown_If";
            this.upDown_If.Size = new System.Drawing.Size(40, 20);
            this.upDown_If.TabIndex = 6;
            this.upDown_If.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.upDown_If.ValueChanged += new System.EventHandler(this.upDown_If_ValueChanged);
            this.upDown_If.Leave += new System.EventHandler(this.upDown_If_ValueChanged);
            // 
            // valor_If
            // 
            this.valor_If.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.valor_If.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.valor_If.Location = new System.Drawing.Point(360, 64);
            this.valor_If.Name = "valor_If";
            this.valor_If.Size = new System.Drawing.Size(48, 20);
            this.valor_If.TabIndex = 5;
            this.valor_If.Text = "0";
            this.valor_If.Visible = false;
            // 
            // comboCE_If
            // 
            this.comboCE_If.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboCE_If.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.comboCE_If.Location = new System.Drawing.Point(144, 40);
            this.comboCE_If.Name = "comboCE_If";
            this.comboCE_If.Size = new System.Drawing.Size(208, 21);
            this.comboCE_If.TabIndex = 4;
            this.comboCE_If.Visible = false;
            // 
            // comboCG_If
            // 
            this.comboCG_If.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboCG_If.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.comboCG_If.Items.AddRange(new object[] {
            "Cantidad de signos",
            "Dibujos",
            "Signos Seguidos",
            "Interrupciones",
            "Formatos",
            "Pesos numéricos",
            "Distancias",
            "Contactos"});
            this.comboCG_If.Location = new System.Drawing.Point(16, 40);
            this.comboCG_If.Name = "comboCG_If";
            this.comboCG_If.Size = new System.Drawing.Size(120, 21);
            this.comboCG_If.TabIndex = 3;
            this.comboCG_If.SelectedIndexChanged += new System.EventHandler(this.cambioCombo);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(360, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "Valor:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(144, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "Condición específica:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(16, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Condición genérica:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(272, 432);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(204, 13);
            this.label2.TabIndex = 49;
            this.label2.Text = "Condiciones que deben cumplirse:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(40, 432);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 13);
            this.label1.TabIndex = 48;
            this.label1.Text = "Condiciones en lista:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listaCondiciones
            // 
            this.listaCondiciones.CheckBoxes = true;
            this.listaCondiciones.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listaCondiciones.FullRowSelect = true;
            this.listaCondiciones.GridLines = true;
            this.listaCondiciones.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listaCondiciones.Location = new System.Drawing.Point(24, 224);
            this.listaCondiciones.MultiSelect = false;
            this.listaCondiciones.Name = "listaCondiciones";
            this.listaCondiciones.Size = new System.Drawing.Size(536, 200);
            this.listaCondiciones.TabIndex = 47;
            this.listaCondiciones.UseCompatibleStateImageBehavior = false;
            this.listaCondiciones.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Si se produce ...";
            this.columnHeader1.Width = 266;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "... debe cumplirse que ...";
            this.columnHeader2.Width = 266;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Bisque;
            this.tabPage2.Controls.Add(this.chkNoGrupo_Then);
            this.tabPage2.Controls.Add(this.chkNoGrupo_If);
            this.tabPage2.Controls.Add(this.btnBorrarGrupo);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.txtGruposLista);
            this.tabPage2.Controls.Add(this.txtGrupos);
            this.tabPage2.Controls.Add(this.btnAddGrupo);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.listaGrupos);
            this.tabPage2.Controls.Add(this.cmbGrupo_Then);
            this.tabPage2.Controls.Add(this.cmbGrupo_If);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(584, 467);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Grupos";
            // 
            // chkNoGrupo_Then
            // 
            this.chkNoGrupo_Then.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNoGrupo_Then.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNoGrupo_Then.Location = new System.Drawing.Point(168, 240);
            this.chkNoGrupo_Then.Name = "chkNoGrupo_Then";
            this.chkNoGrupo_Then.Size = new System.Drawing.Size(72, 16);
            this.chkNoGrupo_Then.TabIndex = 67;
            this.chkNoGrupo_Then.Text = "Negado";
            // 
            // chkNoGrupo_If
            // 
            this.chkNoGrupo_If.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNoGrupo_If.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNoGrupo_If.Location = new System.Drawing.Point(16, 240);
            this.chkNoGrupo_If.Name = "chkNoGrupo_If";
            this.chkNoGrupo_If.Size = new System.Drawing.Size(72, 16);
            this.chkNoGrupo_If.TabIndex = 66;
            this.chkNoGrupo_If.Text = "Negado";
            // 
            // btnBorrarGrupo
            // 
            this.btnBorrarGrupo.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnBorrarGrupo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBorrarGrupo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBorrarGrupo.Image = ((System.Drawing.Image)(resources.GetObject("btnBorrarGrupo.Image")));
            this.btnBorrarGrupo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBorrarGrupo.Location = new System.Drawing.Point(168, 272);
            this.btnBorrarGrupo.Name = "btnBorrarGrupo";
            this.btnBorrarGrupo.Size = new System.Drawing.Size(128, 32);
            this.btnBorrarGrupo.TabIndex = 65;
            this.btnBorrarGrupo.Text = "Borrar relación";
            this.btnBorrarGrupo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBorrarGrupo.UseVisualStyleBackColor = false;
            this.btnBorrarGrupo.Click += new System.EventHandler(this.btnBorrarGrupo_Click);
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.SystemColors.Control;
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(160, 32);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(136, 16);
            this.label12.TabIndex = 64;
            this.label12.Text = "... entonces grupo ...";
            this.label12.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.SystemColors.Control;
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(8, 32);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(136, 16);
            this.label11.TabIndex = 63;
            this.label11.Text = "Si grupo ...";
            this.label11.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // txtGruposLista
            // 
            this.txtGruposLista.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGruposLista.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGruposLista.Location = new System.Drawing.Point(530, 246);
            this.txtGruposLista.Name = "txtGruposLista";
            this.txtGruposLista.ReadOnly = true;
            this.txtGruposLista.Size = new System.Drawing.Size(32, 21);
            this.txtGruposLista.TabIndex = 61;
            this.txtGruposLista.Text = "0";
            this.txtGruposLista.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtGrupos
            // 
            this.txtGrupos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGrupos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGrupos.Location = new System.Drawing.Point(530, 278);
            this.txtGrupos.Name = "txtGrupos";
            this.txtGrupos.Size = new System.Drawing.Size(32, 21);
            this.txtGrupos.TabIndex = 60;
            this.txtGrupos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnAddGrupo
            // 
            this.btnAddGrupo.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnAddGrupo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAddGrupo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddGrupo.Image = ((System.Drawing.Image)(resources.GetObject("btnAddGrupo.Image")));
            this.btnAddGrupo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddGrupo.Location = new System.Drawing.Point(44, 272);
            this.btnAddGrupo.Name = "btnAddGrupo";
            this.btnAddGrupo.Size = new System.Drawing.Size(100, 32);
            this.btnAddGrupo.TabIndex = 59;
            this.btnAddGrupo.Text = "Añadir";
            this.btnAddGrupo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddGrupo.UseVisualStyleBackColor = false;
            this.btnAddGrupo.Click += new System.EventHandler(this.btnAddGrupo_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(320, 280);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(204, 13);
            this.label9.TabIndex = 58;
            this.label9.Text = "Condiciones que deben cumplirse:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(320, 248);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(126, 13);
            this.label10.TabIndex = 57;
            this.label10.Text = "Condiciones en lista:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listaGrupos
            // 
            this.listaGrupos.CheckBoxes = true;
            this.listaGrupos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.listaGrupos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listaGrupos.FullRowSelect = true;
            this.listaGrupos.GridLines = true;
            this.listaGrupos.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listaGrupos.Location = new System.Drawing.Point(320, 32);
            this.listaGrupos.MultiSelect = false;
            this.listaGrupos.Name = "listaGrupos";
            this.listaGrupos.Size = new System.Drawing.Size(256, 200);
            this.listaGrupos.TabIndex = 56;
            this.listaGrupos.UseCompatibleStateImageBehavior = false;
            this.listaGrupos.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Si se produce ...";
            this.columnHeader3.Width = 126;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "... debe cumplirse que ...";
            this.columnHeader4.Width = 126;
            // 
            // cmbGrupo_Then
            // 
            this.cmbGrupo_Then.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbGrupo_Then.Location = new System.Drawing.Point(160, 48);
            this.cmbGrupo_Then.Name = "cmbGrupo_Then";
            this.cmbGrupo_Then.ScrollAlwaysVisible = true;
            this.cmbGrupo_Then.Size = new System.Drawing.Size(136, 186);
            this.cmbGrupo_Then.TabIndex = 8;
            // 
            // cmbGrupo_If
            // 
            this.cmbGrupo_If.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbGrupo_If.Location = new System.Drawing.Point(8, 48);
            this.cmbGrupo_If.Name = "cmbGrupo_If";
            this.cmbGrupo_If.ScrollAlwaysVisible = true;
            this.cmbGrupo_If.Size = new System.Drawing.Size(136, 186);
            this.cmbGrupo_If.TabIndex = 7;
            // 
            // IfThenFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(616, 565);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IfThenFrm";
            this.Text = "Condiciones relacionadas (if-then)";
            this.Load += new System.EventHandler(this.IfThenFrm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDown_Then)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDown_If)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion


		private void cargarDatos(ControladorIfThen ifThen)
		{
			if(ifThen==null) return;
			int i;
			string txt;
		    ListViewItem li;
			for(i=0;i<ifThen.ControlesCondiciones.Count;i++)
			{
				CondicionIfThen condicion = (CondicionIfThen)ifThen.ControlesCondiciones[i];
				txt=condicion.CondIf;
				if(txt.IndexOf("(NO) ")>=0) condicion.NoIf=true; else condicion.NoIf=false;
				li=new ListViewItem(txt);
				txt=condicion.CondThen;
				if(txt.IndexOf("(NO) ")>=0) condicion.NoThen=true; else condicion.NoThen=false;
				li.SubItems.Add(txt);
				listaCondiciones.Items.Add(li);
			}
			if(ifThen.ControlesCondiciones.Count>0) txtConds.Text=ifThen.RangoAciertoCondiciones;
			txtCondsLista.Text=listaCondiciones.Items.Count.ToString();
			for(i=0;i<ifThen.ControlesGrupos.Count;i++)
			{
				GrupoIfThen grupo = (GrupoIfThen)ifThen.ControlesGrupos[i];
				// Si no existen los grupos en el analizador NO se añade a la lista.
				if(grupo.NumGrupoIf>=analizador.GruposPartidos.Count || grupo.NumGrupoThen>=analizador.GruposPartidos.Count)
				{
					MessageBox.Show("Uno de los grupos del fichero no existe en la combinación actual. Se quita de la lista.","Atención",MessageBoxButtons.OK,MessageBoxIcon.Error);
					continue;
				}
				grupo.GrupoIf=analizador.GruposPartidos[grupo.NumGrupoIf];
				txt=grupo.NumGrupoIf+" - "+grupo.GrupoIf.NombreGrupo;
				if(grupo.NoIf) txt="(NO) "+txt;
				li=new ListViewItem(txt);
				grupo.GrupoThen=analizador.GruposPartidos[grupo.NumGrupoThen];
				txt=grupo.NumGrupoThen+" - "+grupo.GrupoThen.NombreGrupo;
				if(grupo.NoThen) txt="(NO) "+txt;
				li.SubItems.Add(txt);
				listaGrupos.Items.Add(li);
			}
			if(ifThen.ControlesGrupos.Count>0) txtGrupos.Text=ifThen.RangoAciertoGrupos;
			txtGruposLista.Text=listaGrupos.Items.Count.ToString();
		}

		private void cambioCombo(object sender, EventArgs e)
		{
		    ComboBox comboDestino;
			TextBox cajaTexto;
			NumericUpDown upDown;
			ComboBox comboOrigen = (ComboBox) sender;
			if(comboOrigen.Name.IndexOf("_If")>=0)
			{
				comboDestino=comboCE_If;
				cajaTexto=valor_If;
				upDown=upDown_If;
			}
			else
			{
				comboDestino=comboCE_Then;
				cajaTexto=valor_Then;
				upDown=upDown_Then;
			}
			cajaTexto.Text=upDown.Value.ToString();
			cajaTexto.Visible=false;
			upDown.Visible=true;
			string txt = comboOrigen.SelectedItem.ToString();
			// Rellena el combo destino.
			comboDestino.Visible=true;
			comboDestino.Items.Clear();
			comboDestino.Text="";
			switch(txt)
			{
				case "Cantidad de signos":
					comboDestino.Items.Add("Cantidad de Variantes");
					comboDestino.Items.Add("Cantidad de X");
					comboDestino.Items.Add("Cantidad de 2");
					upDown.Minimum=0;
					upDown.Maximum=VariablesGlobales.NumeroPartidos;
					break;
				case "Dibujos":
					for(int i=0;i<=VariablesGlobales.NumeroPartidos;i++)
					{
						for(int j=0;j<=VariablesGlobales.NumeroPartidos;j++)
						{
							if((i+j)<=VariablesGlobales.NumeroPartidos)
								comboDestino.Items.Add("Dibujo "+i+"+"+j);
						}
					}
					upDown.Visible=false;
					break;
				case "Signos Seguidos":
					comboDestino.Items.Add("Signos Seguidos de Variantes");
					comboDestino.Items.Add("Signos Seguidos de 1");
					comboDestino.Items.Add("Signos Seguidos de X");
					comboDestino.Items.Add("Signos Seguidos de 2");
					upDown.Minimum=0;
					upDown.Maximum=VariablesGlobales.NumeroPartidos;
					break;
				case "Interrupciones":
					comboDestino.Items.Add("Interrupciones Globales");
					comboDestino.Items.Add("Interrupciones de Variantes");
					comboDestino.Items.Add("Interrupciones de 1");
					comboDestino.Items.Add("Interrupciones de X");
					comboDestino.Items.Add("Interrupciones de 2");
					comboDestino.Items.Add("Interrupciones Seguidas Globales");
					comboDestino.Items.Add("Interrupciones Seguidas de Variantes");
					comboDestino.Items.Add("Interrupciones Seguidas de 1");
					comboDestino.Items.Add("Interrupciones Seguidas de X");
					comboDestino.Items.Add("Interrupciones Seguidas de 2");
					upDown.Minimum=0;
					upDown.Maximum=VariablesGlobales.NumeroPartidos-1;
					break;
				case "Pesos numéricos":
					comboDestino.Items.Add("Peso numérico Global");
					comboDestino.Items.Add("Peso numérico de Variantes");
					comboDestino.Items.Add("Peso numérico de 1");
					comboDestino.Items.Add("Peso numérico de X");
					comboDestino.Items.Add("Peso numérico de 2");
					upDown.Minimum=0;
					upDown.Maximum=9;
					break;
				case "Distancias":
					comboDestino.Items.Add("Distancia de Variantes");
					comboDestino.Items.Add("Distancia de 1");
					comboDestino.Items.Add("Distancia de X");
					comboDestino.Items.Add("Distancia de 2");
					upDown.Minimum=0;
					upDown.Maximum=VariablesGlobales.NumeroPartidos-1;
					break;
				case "Contactos":
					comboDestino.Items.Add("Contactos de 1X");
					comboDestino.Items.Add("Contactos de 12");
					comboDestino.Items.Add("Contactos de X2");
					comboDestino.Items.Add("Contactos de 11");
					comboDestino.Items.Add("Contactos de XX");
					comboDestino.Items.Add("Contactos de 22");
					comboDestino.Items.Add("Contactos de 1V");
					comboDestino.Items.Add("Contactos de XV");
					comboDestino.Items.Add("Contactos de 2V");
					comboDestino.Items.Add("Contactos de VV");
					upDown.Minimum=0;
					upDown.Maximum=VariablesGlobales.NumeroPartidos-1;
					break;
			}
			if(cajaTexto.Visible) cajaTexto.Location=upDown.Location;
		}

		private void menuCondiciones1_BCancelar(object sender, EventArgs e)
		{
			Close();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
		    // CONDICIONES IF
			// Comprueba que haya valores
			string txt;
			if(comboCG_If.SelectedIndex<0) return;
			if(comboCE_If.Visible)
			{
				if(comboCE_If.SelectedIndex<0) return;
				txt=comboCE_If.SelectedItem.ToString();
				if(comboCE_If.SelectedItem.ToString().IndexOf("Dibujo")<0) txt+=": ";
			}
			else
			{
				txt=comboCG_If.SelectedItem+": ";
				if(valor_If.Text.Length==0) return;
			}
			if(upDown_If.Visible || valor_If.Visible) txt+=valor_If.Text;
			// CONDICIONES THEN
			// Comprueba que haya valores
			string txt2;
			if(comboCG_Then.SelectedIndex<0) return;
			if(comboCE_Then.Visible)
			{
				if(comboCE_Then.SelectedIndex<0) return;
				txt2=comboCE_Then.SelectedItem.ToString();
				if(comboCE_Then.SelectedItem.ToString().IndexOf("Dibujo")<0) txt2+=": ";
			}
			else
			{
				txt2=comboCG_Then.SelectedItem+": ";
				if(valor_Then.Text.Length==0) return;
			}
			if(upDown_Then.Visible || valor_Then.Visible) txt2+=valor_Then.Text;
			// Comprueba que no usemos la misma condición
			if(txt==txt2)
			{
				MessageBox.Show("Las condiciones deben ser diferentes");
				return;
			}
		    if(txt.IndexOf("Dibujo")>=0 && txt2.IndexOf("Dibujo")>=0)
		    {
		        MessageBox.Show("Las condiciones deben ser diferentes");
		        return;
		    }
		    // Añade las negaciones si las hay
			if(chkNoCond_If.Checked) txt="(NO) "+txt;
			if(chkNoCond_Then.Checked) txt2="(NO) "+txt2;
			// Se añade la condición al ListView
			ListViewItem item = new ListViewItem(txt);
			item.SubItems.Add(txt2);
			listaCondiciones.Items.Add(item);
			txtCondsLista.Text=listaCondiciones.Items.Count.ToString();
			resetConds();
		}

		private void btnBorrar_Click(object sender, EventArgs e)
		{
			for(int i=listaCondiciones.Items.Count-1;i>=0;i--)
			{
				if(listaCondiciones.Items[i].Checked) listaCondiciones.Items[i].Remove();
			}
			txtCondsLista.Text=listaCondiciones.Items.Count.ToString();
		}

		private void upDown_If_ValueChanged(object sender, EventArgs e)
		{
			valor_If.Text=upDown_If.Value.ToString();
		}

		private void upDown_Then_ValueChanged(object sender, EventArgs e)
		{
			valor_Then.Text=upDown_Then.Value.ToString();
		}

		private void reset()
		{
			resetConds();
			resetGrupos();
		}

		private void resetConds()
		{
			comboCE_If.Visible=false;
			comboCE_If.Items.Clear();
			valor_If.Visible=false;
			upDown_If.Visible=false;
			comboCG_If.Items.Clear();
			llenarCombo(comboCG_If);
			comboCG_If.Text="";
			chkNoCond_If.Checked=false;
			Application.DoEvents();

			comboCE_Then.Visible=false;
			comboCE_Then.Items.Clear();
			valor_Then.Visible=false;
			upDown_Then.Visible=false;
			comboCG_Then.Items.Clear();
			comboCG_Then.Text="";
			llenarCombo(comboCG_Then);
			chkNoCond_Then.Checked=false;
			Application.DoEvents();
		}

		private void llenarCombo(ComboBox combo)
		{
			combo.Items.Add("Cantidad de signos");
			combo.Items.Add("Dibujos");
			combo.Items.Add("Signos Seguidos");
			combo.Items.Add("Interrupciones");
			combo.Items.Add("Pesos numéricos");
			combo.Items.Add("Distancias");
			combo.Items.Add("Contactos");
		}

		private void resetGrupos()
		{
			cmbGrupo_If.Items.Clear();
			cmbGrupo_Then.Items.Clear();
			chkNoGrupo_If.Checked=false;
			chkNoGrupo_Then.Checked=false;
			llenarGrupos();
		}

		private void llenarGrupos()
		{
			for(int i=1;i<analizador.GruposPartidos.Count;i++)
			{
				cmbGrupo_If.Items.Add(i+" - "+analizador.GruposPartidos[i].NombreGrupo);
				cmbGrupo_Then.Items.Add(i+" - "+analizador.GruposPartidos[i].NombreGrupo);
			}
			if(analizador.GruposPartidos.Count==1)
				tabPage2.Dispose();
		}

		private void IfThenFrm_Load(object sender, EventArgs e)
		{
			reset();
		}

		private void btnAddGrupo_Click(object sender, EventArgs e)
		{
			string txt="", txt2="";
		    // Comprueba que haya valores seleccionados
			if(cmbGrupo_If.SelectedIndex<0) return;
			if(cmbGrupo_Then.SelectedIndex<0) return;
			if(cmbGrupo_If.SelectedIndex==cmbGrupo_Then.SelectedIndex)
			{
				MessageBox.Show("Los grupos deben ser diferentes");
				return;
			}
			// Se añade la condición al ListView
			if(chkNoGrupo_If.Checked) txt="(NO) ";
			if(chkNoGrupo_Then.Checked) txt2="(NO) ";
			txt+=cmbGrupo_If.SelectedItem.ToString();
			txt2+=cmbGrupo_Then.SelectedItem.ToString();
			ListViewItem item = new ListViewItem(txt);
			item.SubItems.Add(txt2);
			listaGrupos.Items.Add(item);
			txtGruposLista.Text=listaGrupos.Items.Count.ToString();
			cmbGrupo_If.ClearSelected();
			cmbGrupo_Then.ClearSelected();
			// Limpia los checkboxes de negación
			chkNoGrupo_If.Checked=false;
			chkNoGrupo_Then.Checked=false;
		}

		private void btnBorrarGrupo_Click(object sender, EventArgs e)
		{
			for(int i=listaGrupos.Items.Count-1;i>=0;i--)
			{
				if(listaGrupos.Items[i].Checked) listaGrupos.Items[i].Remove();
			}
		}

		private ControladorIfThen guardarCondicion()
		{
			if(txtConds.Text.Length==0 && Convert.ToInt16(txtCondsLista.Text)>0)
			{
				MessageBox.Show("Debe indicarse la cantidad de condiciones que se deben cumplir.");
				return null;
			}
			if(txtGrupos.Text.Length==0 && Convert.ToInt16(txtGruposLista.Text)>0)
			{
				MessageBox.Show("Debe indicarse la cantidad de grupos que se deben cumplir.");
				return null;
			}
			string txt;
		    ControladorIfThen ifThen=new ControladorIfThen();
			CondicionIfThen cond;
		    Grupo grupoThen;
			ListViewItem li;
			// Lee la lista de condiciones, selecciona el filtro y añade al controlador.
			for(int i=0;i<listaCondiciones.Items.Count;i++)
			{
				li=listaCondiciones.Items[i];
				txt=li.Text;
				cond=new CondicionIfThen();
				cond.CondIf=txt;
				txt=li.SubItems[1].Text;
				cond.CondThen=txt;
				ifThen.AddCondiciones(cond);
			}
			// Añade el controlador de condiciones
			if(listaCondiciones.Items.Count>0) ifThen.RangoAciertoCondiciones=txtConds.Text;

			// Lee la lista de grupos y los añade al controlador.
			for(int i=0;i<listaGrupos.Items.Count;i++)
			{
				GrupoIfThen gr = new GrupoIfThen();
				li=listaGrupos.Items[i];
				txt=li.Text;
				if(txt.IndexOf("(NO)")==0)
				{
					gr.NoIf=true;
					txt=txt.Substring(5);
				}
				int posicion = txt.IndexOf(" ");
				int numGrupo = Convert.ToInt16(txt.Substring(0,posicion));
				gr.NumGrupoIf=numGrupo;
			    Grupo grupoIf = analizador.GruposPartidos[numGrupo];
				txt=li.SubItems[1].Text;
				if(txt.IndexOf("(NO)")==0)
				{
					gr.NoThen=true;
					txt=txt.Substring(5);
				}
				posicion=txt.IndexOf(" ");
				numGrupo=Convert.ToInt16(txt.Substring(0,posicion));
				gr.NumGrupoThen=numGrupo;
			    grupoThen=analizador.GruposPartidos[numGrupo];
				gr.GrupoIf=grupoIf;
				gr.GrupoThen=grupoThen;
				ifThen.AddGrupos(gr);
			}
			// Añade el controlador de grupos
			if(listaGrupos.Items.Count>0) ifThen.RangoAciertoGrupos=txtGrupos.Text;
			// Devuelve el controlador
			return ifThen;
		}

		private void limpiar()
		{
			reset();
			listaCondiciones.Items.Clear();
			txtCondsLista.Text=listaCondiciones.Items.Count.ToString();
			txtConds.Text="";
			listaGrupos.Items.Clear();
			txtGruposLista.Text=listaGrupos.Items.Count.ToString();
			txtGrupos.Text="";
		}

		private void menuCondiciones1_BOk(object sender, EventArgs e)
		{
			ControladorIfThen ifThen=guardarCondicion();
			if(ifThen==null) return;
			// Añade el controlador de relaciones al analizador y cierra la ventana.
			parentFrm.analizador.IfThen=ifThen;
			Close();
		}

		private void menuCondiciones1_BBorrar(object sender, EventArgs e)
		{
			ControladorIfThen ifThen=guardarCondicion();
			if(ifThen.EsVacio==false)
			{
				if(MessageBox.Show("¿Borrar los datos del filtro?","Borrar condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
					return;
			}
		    limpiar();
		}

		private void menuCondiciones1_BAbrir(object sender, EventArgs e)
		{
			ControladorIfThen ifThen=guardarCondicion();
			if(ifThen.EsVacio==false)
			{
				if(MessageBox.Show("Ya hay datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
					return;
			}
			OpenFileDialog abreCombDialog = new OpenFileDialog();
			abreCombDialog.InitialDirectory = "Condiciones\\" ;
			abreCombDialog.Filter = "Condiciones relacionadas(*.if)|*.if|Condiciones relacionadas(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
			if(abreCombDialog.ShowDialog() == DialogResult.OK)
				abrir(abreCombDialog.FileName);
		}

		private void menuCondiciones1_BGuardar(object sender, EventArgs e)
		{
			ControladorIfThen ifThen=guardarCondicion();
			if(ifThen.EsVacio) return;
			SaveFileDialog saveDialog = new SaveFileDialog();
			saveDialog.InitialDirectory = "Condiciones\\" ;
			saveDialog.Filter = "Condiciones relacionadas(*.if)|*.if|Condiciones relacionadas(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
			if(saveDialog.ShowDialog() == DialogResult.OK)
				guardar(saveDialog.FileName, ifThen);
		}

		private void abrir(string nombreArchivo)
		{
			limpiar();
			//leer combinacion desde archivo
			ArchivoCondiciones archComb = new ArchivoCondiciones();
			if(archComb.AbrirArchivoCombinacion( nombreArchivo ))
			{
				ControladorIfThen ifThen=archComb.LeeIfThen();
				cargarDatos(ifThen);
			}
		}

		private void guardar(string nombreArchivo, ControladorIfThen ifThen)
		{
			ArchivoCondiciones archComb = new ArchivoCondiciones();
			archComb.NombreArchivo=nombreArchivo;
			archComb.GuardaArchivo(ifThen);
		}

		private void menuCondiciones1_BCopiar(object sender, EventArgs e)
		{
			ControladorIfThen ifThen=guardarCondicion();
			if(ifThen.EsVacio) return;
			// Crea un fichero temporal
			string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.if";
			guardar(nombreFichero, ifThen);
			menuCondiciones1.BotonPegarEnabled=true;
		}

		private void menuCondiciones1_BPegar(object sender, EventArgs e)
		{
			ControladorIfThen ifThen=guardarCondicion();
			if(ifThen.EsVacio==false)
			{
				if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
					return;
			}
			string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.if";
			abrir(nombreFichero);
		}

		private void compruebaPegar()
		{
			// Comprueba si el botón pegar es habilitable
            if (formHelper.ExisteFicheroTemporal("tmp.if"))
				menuCondiciones1.BotonPegarEnabled=true;
			else
				menuCondiciones1.BotonPegarEnabled=false;
		}

	}
}
