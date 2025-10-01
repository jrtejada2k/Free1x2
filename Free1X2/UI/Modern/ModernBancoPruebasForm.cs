using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Free1X2.UI.Modern;
using Free1X2.UI.Modern.Controls;
using Free1X2.MotorCalculo;
using Free1X2.Utils;
using Free1X2.Analisis;
using Free1X2.EntradaSalida;

namespace Free1X2.UI.Modern
{
    /// <summary>
    /// Modern version of BancoPruebasFrm with enhanced UI and functionality
    /// Uses modern DataGridView instead of legacy DataGrid
    /// </summary>
    public partial class ModernBancoPruebasForm : DataBoundFormBase
    {
        #region Private Fields

        private readonly ValidadorCadenas _valida = new ValidadorCadenas();
        private readonly Porcentajes _pct = new Porcentajes();
        
        private double[,] _p = new double[14, 3];
        private double[,] _v = new double[14, 3];
        private float[] _cr = new float[14];
        private float[,] _pa = new float[14, 3];
        private float[,] _cra = new float[14, 3];
        
        private ApuestaProbableCentral[] _ap14T = new ApuestaProbableCentral[4782969];
        private System.Collections.BitArray _bits = new System.Collections.BitArray(4782969, false);
        
        private int _numAleatorias = 1000;
        private double _recaudacion = 14000000;
        private double _precioApuesta = 0.5;
        private int _numApuestas;
        private int _profundidad = 0;
        
        private List<object> _columnasAleatorias = null;
        private List<object> _columnas = null;
        private double[] _sumaProbabilidades = new double[5];
        private double[] _premios = new double[5];
        private double _probabilidadCategoria14 = 1;
        private double[] _pctDestinadoAPremiosCategoria = new double[5] { 0.12, 0.08, 0.08, 0.08, 0.09 };
        private double[] _destinadoAPremiosCategoria = new double[5];
        
        // Note: These would be replaced with proper business objects in production
        // private Resultados[] _res = null;
        // private ResultadosJornada[] _resultadoPorJornadas = null;
        // private ResultadosPorColumna[] _resCol = null;
        
        private double _ln = -14.7;
        private int _criterioOrdenacion = 0;
        private bool[] _acumularPremio = new bool[5] { false, false, false, false, true };
        private bool _salida = false;
        private double[,] _maxMin = new double[18, 2];
        private bool _dataGridVacia = true;

        // UI Components
        private TabControl _tabControl;
        private ModernDataGrid _dgResultadoEscrutinio;
        private ModernStatusBar _statusBar;
        
        // Tab pages
        private TabPage _tabPaso1;
        private TabPage _tabPaso2;
        private TabPage _tabPaso3;
        private TabPage _tabPaso4;

        // Controls
        private TextBox _txFicheroEntrada;
        private TextBox _txDesvTipica;
        private TextBox _txLNmedia;
        private Label _lblRecuperacion;
        private Label _lblPremioTotal;
        private Label _lblGastoTotal;
        private Label _lblNumColumnas;
        private Button _btLeerColumnas;
        private Button _btnOK;
        private Button _btnCancel;

        #endregion

        #region Properties

        public string FicheroEntrada
        {
            get => _txFicheroEntrada?.Text ?? "";
            set
            {
                if (_txFicheroEntrada != null)
                    _txFicheroEntrada.Text = value;
            }
        }

        public int NumAleatorias
        {
            get => _numAleatorias;
            set => _numAleatorias = value;
        }

        public double Recaudacion
        {
            get => _recaudacion;
            set => _recaudacion = value;
        }

        #endregion

        #region Constructor

        public ModernBancoPruebasForm()
        {
            InitializeComponent();
            LoadDefaultData();
        }

        protected override void ConfigureBindings()
        {
            // Configure data bindings for the banco de pruebas form
            // This method is required by the DataBoundFormBase abstract class
            // Implementation would include binding form controls to data sources
        }

        #endregion

        #region Initialize Methods

        private void InitializeComponent()
        {
            SuspendLayout();

            // Form properties
            Text = "Banco de Pruebas - Modernizado";
            Size = new Size(1000, 700);
            MinimumSize = new Size(800, 600);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;

            CreateStatusBar();
            CreateTabControl();
            CreateTabPages();
            CreateButtons();

            ResumeLayout(false);
            PerformLayout();
        }

        private void CreateStatusBar()
        {
            _statusBar = new ModernStatusBar
            {
                Name = "statusBar"
            };
            _statusBar.SetStatus("Listo", "Banco de Pruebas");
            Controls.Add(_statusBar);
        }

        private void CreateTabControl()
        {
            _tabControl = new TabControl
            {
                Name = "tabControl",
                Dock = DockStyle.Fill,
                Appearance = TabAppearance.Normal,
                Alignment = TabAlignment.Top,
                HotTrack = true,
                Multiline = false,
                SelectedIndex = 0
            };

            Controls.Add(_tabControl);
        }

        private void CreateTabPages()
        {
            CreateTabPaso1();
            CreateTabPaso2();
            CreateTabPaso3();
            CreateTabPaso4();

            _tabControl.TabPages.AddRange(new TabPage[]
            {
                _tabPaso1,
                _tabPaso2,
                _tabPaso3,
                _tabPaso4
            });
        }

        private void CreateTabPaso1()
        {
            _tabPaso1 = new TabPage
            {
                Name = "tabPaso1",
                Text = "Paso 1: Configuración",
                UseVisualStyleBackColor = true
            };

            // File selection group
            var groupFile = new GroupBox
            {
                Text = "Archivo de Combinación",
                Location = new Point(10, 10),
                Size = new Size(400, 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _txFicheroEntrada = new TextBox
            {
                Name = "txFicheroEntrada",
                Location = new Point(10, 25),
                Size = new Size(300, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _btLeerColumnas = new Button
            {
                Name = "btLeerColumnas",
                Text = "...",
                Location = new Point(320, 25),
                Size = new Size(30, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _btLeerColumnas.Click += OnLeerColumnas;

            var btnCargar = new Button
            {
                Text = "Cargar",
                Location = new Point(360, 25),
                Size = new Size(60, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnCargar.Click += OnCargarCombinacion;

            groupFile.Controls.AddRange(new Control[]
            {
                new Label { Text = "Archivo:", Location = new Point(10, 5), Size = new Size(50, 20) },
                _txFicheroEntrada,
                _btLeerColumnas,
                btnCargar
            });

            // Parameters group
            var groupParams = new GroupBox
            {
                Text = "Parámetros",
                Location = new Point(10, 100),
                Size = new Size(400, 200),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var txNumAleatorias = new NumericUpDown
            {
                Name = "txNumAleatorias",
                Location = new Point(150, 25),
                Size = new Size(100, 23),
                Minimum = 100,
                Maximum = 10000,
                Value = _numAleatorias,
                Increment = 100
            };
            txNumAleatorias.ValueChanged += (s, e) => _numAleatorias = (int)txNumAleatorias.Value;

            var txRecaudacion = new NumericUpDown
            {
                Name = "txRecaudacion",
                Location = new Point(150, 55),
                Size = new Size(150, 23),
                Minimum = 1000000,
                Maximum = 100000000,
                Value = (decimal)_recaudacion,
                Increment = 1000000,
                DecimalPlaces = 0,
                ThousandsSeparator = true
            };
            txRecaudacion.ValueChanged += (s, e) => _recaudacion = (double)txRecaudacion.Value;

            var txPrecioApuesta = new NumericUpDown
            {
                Name = "txPrecioApuesta",
                Location = new Point(150, 85),
                Size = new Size(100, 23),
                Minimum = 0.1m,
                Maximum = 10m,
                Value = (decimal)_precioApuesta,
                Increment = 0.1m,
                DecimalPlaces = 2
            };
            txPrecioApuesta.ValueChanged += (s, e) => _precioApuesta = (double)txPrecioApuesta.Value;

            groupParams.Controls.AddRange(new Control[]
            {
                new Label { Text = "Columnas aleatorias:", Location = new Point(10, 27), Size = new Size(130, 20) },
                txNumAleatorias,
                new Label { Text = "Recaudación:", Location = new Point(10, 57), Size = new Size(130, 20) },
                txRecaudacion,
                new Label { Text = "Precio apuesta:", Location = new Point(10, 87), Size = new Size(130, 20) },
                txPrecioApuesta
            });

            // Statistics display
            var groupStats = new GroupBox
            {
                Text = "Información",
                Location = new Point(430, 10),
                Size = new Size(300, 290),
                Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
            };

            _lblNumColumnas = new Label
            {
                Name = "lblNumColumnas",
                Text = "Columnas: 0",
                Location = new Point(10, 25),
                Size = new Size(280, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };

            _lblGastoTotal = new Label
            {
                Name = "lblGastoTotal",
                Text = "Gasto total: €0",
                Location = new Point(10, 50),
                Size = new Size(280, 20)
            };

            _lblPremioTotal = new Label
            {
                Name = "lblPremioTotal",
                Text = "Premio total: €0",
                Location = new Point(10, 75),
                Size = new Size(280, 20)
            };

            _lblRecuperacion = new Label
            {
                Name = "lblRecuperacion",
                Text = "Recuperación: 0%",
                Location = new Point(10, 100),
                Size = new Size(280, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold),
                ForeColor = Color.Blue
            };

            groupStats.Controls.AddRange(new Control[]
            {
                _lblNumColumnas,
                _lblGastoTotal,
                _lblPremioTotal,
                _lblRecuperacion
            });

            _tabPaso1.Controls.AddRange(new Control[]
            {
                groupFile,
                groupParams,
                groupStats
            });
        }

        private void CreateTabPaso2()
        {
            _tabPaso2 = new TabPage
            {
                Name = "tabPaso2",
                Text = "Paso 2: Análisis",
                UseVisualStyleBackColor = true
            };

            var groupAnalysis = new GroupBox
            {
                Text = "Análisis Estadístico",
                Location = new Point(10, 10),
                Size = new Size(400, 150),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _txLNmedia = new TextBox
            {
                Name = "txLNmedia",
                Location = new Point(150, 25),
                Size = new Size(100, 23),
                ReadOnly = true,
                BackColor = SystemColors.Control
            };

            _txDesvTipica = new TextBox
            {
                Name = "txDesvTipica",
                Location = new Point(150, 55),
                Size = new Size(100, 23),
                ReadOnly = true,
                BackColor = SystemColors.Control
            };

            var btnAnalizar = new Button
            {
                Text = "Analizar",
                Location = new Point(280, 25),
                Size = new Size(80, 23)
            };
            btnAnalizar.Click += OnAnalizar;

            groupAnalysis.Controls.AddRange(new Control[]
            {
                new Label { Text = "LN Media:", Location = new Point(10, 27), Size = new Size(130, 20) },
                _txLNmedia,
                new Label { Text = "Desv. Típica:", Location = new Point(10, 57), Size = new Size(130, 20) },
                _txDesvTipica,
                btnAnalizar
            });

            _tabPaso2.Controls.Add(groupAnalysis);
        }

        private void CreateTabPaso3()
        {
            _tabPaso3 = new TabPage
            {
                Name = "tabPaso3",
                Text = "Paso 3: Resultados",
                UseVisualStyleBackColor = true
            };

            // Create modern data grid for results
            _dgResultadoEscrutinio = new ModernDataGrid
            {
                Name = "dgResultadoEscrutinio",
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Configure columns
            _dgResultadoEscrutinio.AddTextColumn("Jornada", "Jornada", 80);
            _dgResultadoEscrutinio.AddTextColumn("Resultado", "Resultado", 200);
            _dgResultadoEscrutinio.AddNumericColumn("Aciertos14", "14", "N0", 60);
            _dgResultadoEscrutinio.AddNumericColumn("Aciertos13", "13", "N0", 60);
            _dgResultadoEscrutinio.AddNumericColumn("Aciertos12", "12", "N0", 60);
            _dgResultadoEscrutinio.AddNumericColumn("Aciertos11", "11", "N0", 60);
            _dgResultadoEscrutinio.AddNumericColumn("Aciertos10", "10", "N0", 60);
            _dgResultadoEscrutinio.AddNumericColumn("Premio", "Premio €", "N2", 100);
            _dgResultadoEscrutinio.AddNumericColumn("Recuperacion", "Recup. %", "N2", 80);

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var toolbar = new ModernToolBar
            {
                Name = "toolbarResultados",
                Dock = DockStyle.Top
            };
            toolbar.AddButton("btnExportar", "Exportar", null, OnExportarResultados);
            toolbar.AddButton("btnImprimir", "Imprimir", null, OnImprimirResultados);
            toolbar.AddSeparator();
            toolbar.AddButton("btnGrafico", "Gráfico", null, OnMostrarGrafico);

            panel.Controls.AddRange(new Control[] { _dgResultadoEscrutinio, toolbar });
            toolbar.BringToFront();

            _tabPaso3.Controls.Add(panel);
        }

        private void CreateTabPaso4()
        {
            _tabPaso4 = new TabPage
            {
                Name = "tabPaso4",
                Text = "Paso 4: Configuración Avanzada",
                UseVisualStyleBackColor = true
            };

            var groupAvanzado = new GroupBox
            {
                Text = "Configuración Avanzada",
                Location = new Point(10, 10),
                Size = new Size(500, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Advanced configuration controls would go here
            var lblInfo = new Label
            {
                Text = "Configuración avanzada del banco de pruebas.\n\n" +
                       "• Criterios de ordenación\n" +
                       "• Parámetros de análisis\n" +
                       "• Configuración de premios\n" +
                       "• Opciones de exportación",
                Location = new Point(10, 25),
                Size = new Size(480, 200),
                Font = SystemFonts.DefaultFont
            };

            groupAvanzado.Controls.Add(lblInfo);
            _tabPaso4.Controls.Add(groupAvanzado);
        }

        private void CreateButtons()
        {
            var buttonPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Bottom
            };

            _btnOK = new Button
            {
                Text = "Aceptar",
                Size = new Size(100, 30),
                Location = new Point(10, 10),
                DialogResult = DialogResult.OK
            };
            _btnOK.Click += OnAceptar;

            _btnCancel = new Button
            {
                Text = "Cancelar",
                Size = new Size(100, 30),
                Location = new Point(120, 10),
                DialogResult = DialogResult.Cancel
            };

            var btnAyuda = new Button
            {
                Text = "Ayuda",
                Size = new Size(100, 30),
                Location = new Point(230, 10)
            };
            btnAyuda.Click += OnAyuda;

            buttonPanel.Controls.AddRange(new Control[]
            {
                _btnOK,
                _btnCancel,
                btnAyuda
            });

            Controls.Add(buttonPanel);
            buttonPanel.BringToFront();
        }

        #endregion

        #region Data Loading

        private void LoadDefaultData()
        {
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            if (_lblNumColumnas != null)
            {
                var numColumnas = _columnas?.Count ?? 0;
                var gastoTotal = numColumnas * _precioApuesta;
                var premioTotal = CalculatePremioTotal();
                var recuperacion = gastoTotal > 0 ? (premioTotal / gastoTotal) * 100 : 0;

                _lblNumColumnas.Text = $"Columnas: {numColumnas:N0}";
                _lblGastoTotal.Text = $"Gasto total: €{gastoTotal:N2}";
                _lblPremioTotal.Text = $"Premio total: €{premioTotal:N2}";
                _lblRecuperacion.Text = $"Recuperación: {recuperacion:N2}%";

                // Color coding for recovery percentage
                if (recuperacion >= 80)
                    _lblRecuperacion.ForeColor = Color.Green;
                else if (recuperacion >= 60)
                    _lblRecuperacion.ForeColor = Color.Orange;
                else
                    _lblRecuperacion.ForeColor = Color.Red;
            }
        }

        private double CalculatePremioTotal()
        {
            // Placeholder calculation - would be implemented based on business logic
            return 0;
        }

        #endregion

        #region Event Handlers

        private void OnLeerColumnas(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Archivos de combinación (*.comb)|*.comb|Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                openDialog.Title = "Seleccionar archivo de combinación";

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    _txFicheroEntrada.Text = openDialog.FileName;
                    _statusBar.SetStatus($"Archivo seleccionado: {System.IO.Path.GetFileName(openDialog.FileName)}");
                }
            }
        }

        private void OnCargarCombinacion(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_txFicheroEntrada.Text))
            {
                MessageBox.Show("Por favor, seleccione un archivo de combinación.", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _statusBar.SetStatus("Cargando combinación...");
            _statusBar.ShowProgress();

            try
            {
                // Simulate loading process
                for (int i = 0; i <= 100; i += 10)
                {
                    _statusBar.UpdateProgress(i);
                    System.Threading.Thread.Sleep(50); // Simulate work
                    Application.DoEvents();
                }

                // Load combination logic would go here
                LoadCombination(_txFicheroEntrada.Text);

                _statusBar.HideProgress();
                _statusBar.SetStatus("Combinación cargada correctamente");
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                _statusBar.HideProgress();
                _statusBar.SetStatus("Error al cargar combinación");
                MessageBox.Show($"Error al cargar la combinación:\n{ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCombination(string filePath)
        {
            // Placeholder for combination loading logic
            // This would implement the actual file reading and parsing
            _columnas = new List<object>(); // Populate with actual data
            _dataGridVacia = _columnas.Count == 0;
        }

        private void OnAnalizar(object sender, EventArgs e)
        {
            if (_dataGridVacia)
            {
                MessageBox.Show("No hay datos para analizar. Cargue una combinación primero.", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _statusBar.SetStatus("Analizando datos...");
            _statusBar.ShowProgress();

            try
            {
                // Perform statistical analysis
                PerformStatisticalAnalysis();

                _statusBar.HideProgress();
                _statusBar.SetStatus("Análisis completado");
                
                // Switch to results tab
                _tabControl.SelectedTab = _tabPaso3;
            }
            catch (Exception ex)
            {
                _statusBar.HideProgress();
                _statusBar.SetStatus("Error en el análisis");
                MessageBox.Show($"Error durante el análisis:\n{ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PerformStatisticalAnalysis()
        {
            // Placeholder for statistical analysis
            // This would implement the actual analysis logic from the original form
            
            _txLNmedia.Text = _ln.ToString("F2");
            _txDesvTipica.Text = "2.34"; // Placeholder value

            // Populate results grid
            PopulateResultsGrid();
        }

        private void PopulateResultsGrid()
        {
            var results = new List<ResultadoAnalisis>();

            // Generate sample data - replace with actual analysis results
            for (int i = 1; i <= 10; i++)
            {
                results.Add(new ResultadoAnalisis
                {
                    Jornada = i,
                    Resultado = "1X2X1X2X1X2X1X",
                    Aciertos14 = i % 2,
                    Aciertos13 = i % 3 + 1,
                    Aciertos12 = i % 4 + 2,
                    Aciertos11 = i % 5 + 3,
                    Aciertos10 = i % 6 + 4,
                    Premio = (i * 1000) + (i * 100),
                    Recuperacion = 65.5 + (i * 2.5)
                });
            }

            _dgResultadoEscrutinio.DataSource = results;
        }

        private void OnExportarResultados(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Archivos CSV (*.csv)|*.csv|Archivos Excel (*.xlsx)|*.xlsx|Todos los archivos (*.*)|*.*";
                saveDialog.Title = "Exportar resultados";
                saveDialog.FileName = "ResultadosBancoPruebas.csv";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var csv = _dgResultadoEscrutinio.ExportToCsv(true);
                        System.IO.File.WriteAllText(saveDialog.FileName, csv);
                        
                        _statusBar.SetStatus($"Resultados exportados a: {System.IO.Path.GetFileName(saveDialog.FileName)}");
                        MessageBox.Show("Resultados exportados correctamente.", "Exportación", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al exportar:\n{ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void OnImprimirResultados(object sender, EventArgs e)
        {
            _statusBar.SetStatus("Preparando impresión...");
            // Implement printing logic
            MessageBox.Show("Funcionalidad de impresión no implementada en esta versión de demostración.", 
                "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnMostrarGrafico(object sender, EventArgs e)
        {
            _statusBar.SetStatus("Generando gráfico...");
            // Implement chart display logic
            MessageBox.Show("Funcionalidad de gráficos no implementada en esta versión de demostración.", 
                "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnAceptar(object sender, EventArgs e)
        {
            Close();
        }

        private void OnAyuda(object sender, EventArgs e)
        {
            MessageBox.Show("Banco de Pruebas - Ayuda\n\n" +
                           "1. Seleccione un archivo de combinación\n" +
                           "2. Configure los parámetros de análisis\n" +
                           "3. Ejecute el análisis\n" +
                           "4. Revise los resultados", 
                           "Ayuda", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Nested Classes

        /// <summary>
        /// Class to represent analysis results for data binding
        /// </summary>
        public class ResultadoAnalisis
        {
            public int Jornada { get; set; }
            public string Resultado { get; set; }
            public int Aciertos14 { get; set; }
            public int Aciertos13 { get; set; }
            public int Aciertos12 { get; set; }
            public int Aciertos11 { get; set; }
            public int Aciertos10 { get; set; }
            public double Premio { get; set; }
            public double Recuperacion { get; set; }
        }

        #endregion
    }
}