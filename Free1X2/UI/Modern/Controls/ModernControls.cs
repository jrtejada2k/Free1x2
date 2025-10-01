using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Free1X2.UI.Modern.Controls
{
    /// <summary>
    /// Modern replacement for legacy DataGrid with enhanced functionality
    /// </summary>
    public class ModernDataGrid : DataGridView
    {
        public ModernDataGrid()
        {
            InitializeModernGrid();
            ApplyModernStyling();
        }

        private void InitializeModernGrid()
        {
            // Modern grid defaults
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            AllowUserToOrderColumns = true;
            AllowUserToResizeRows = false;
            
            AutoGenerateColumns = false;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
            BorderStyle = BorderStyle.Fixed3D;
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            
            EnableHeadersVisualStyles = true;
            MultiSelect = false;
            ReadOnly = true;
            RowHeadersVisible = false;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
            // Performance optimizations
            VirtualMode = false; // Can be enabled for large datasets
            DoubleBuffered = true;
        }

        private void ApplyModernStyling()
        {
            // Modern color scheme
            BackgroundColor = SystemColors.Window;
            GridColor = SystemColors.ControlLight;
            
            DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = SystemColors.Window,
                ForeColor = SystemColors.WindowText,
                SelectionBackColor = SystemColors.Highlight,
                SelectionForeColor = SystemColors.HighlightText,
                Font = SystemFonts.DefaultFont,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                WrapMode = DataGridViewTriState.False
            };

            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = SystemColors.Control,
                ForeColor = SystemColors.WindowText,
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            RowHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = SystemColors.Control,
                ForeColor = SystemColors.WindowText,
                Font = SystemFonts.DefaultFont,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = SystemColors.WindowText
            };
        }

        /// <summary>
        /// Enhanced method to set data source with proper binding
        /// </summary>
        /// <param name="dataSource">The data source</param>
        /// <param name="dataMember">The data member (optional)</param>
        public void SetDataBinding(object dataSource, string dataMember = null)
        {
            if (string.IsNullOrEmpty(dataMember))
            {
                DataSource = dataSource;
            }
            else
            {
                var bindingSource = new BindingSource
                {
                    DataSource = dataSource,
                    DataMember = dataMember
                };
                DataSource = bindingSource;
            }
        }

        /// <summary>
        /// Add a text column with modern styling
        /// </summary>
        public DataGridViewTextBoxColumn AddTextColumn(string name, string headerText, int width = -1)
        {
            var column = new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = headerText,
                DataPropertyName = name,
                SortMode = DataGridViewColumnSortMode.Automatic,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            };

            if (width > 0)
            {
                column.Width = width;
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }

            Columns.Add(column);
            return column;
        }

        /// <summary>
        /// Add a numeric column with proper formatting
        /// </summary>
        public DataGridViewTextBoxColumn AddNumericColumn(string name, string headerText, string format = "N2", int width = -1)
        {
            var column = AddTextColumn(name, headerText, width);
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.DefaultCellStyle.Format = format;
            return column;
        }

        /// <summary>
        /// Add a checkbox column
        /// </summary>
        public DataGridViewCheckBoxColumn AddCheckBoxColumn(string name, string headerText, int width = -1)
        {
            var column = new DataGridViewCheckBoxColumn
            {
                Name = name,
                HeaderText = headerText,
                DataPropertyName = name,
                SortMode = DataGridViewColumnSortMode.Automatic,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };

            if (width > 0)
            {
                column.Width = width;
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }

            Columns.Add(column);
            return column;
        }

        /// <summary>
        /// Export grid data to CSV format
        /// </summary>
        public string ExportToCsv(bool includeHeaders = true)
        {
            var csv = new System.Text.StringBuilder();

            if (includeHeaders)
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    csv.Append(Columns[i].HeaderText);
                    if (i < Columns.Count - 1)
                        csv.Append(",");
                }
                csv.AppendLine();
            }

            foreach (DataGridViewRow row in Rows)
            {
                if (!row.IsNewRow)
                {
                    for (int i = 0; i < Columns.Count; i++)
                    {
                        var cellValue = row.Cells[i].Value?.ToString() ?? "";
                        if (cellValue.Contains(",") || cellValue.Contains("\""))
                        {
                            cellValue = "\"" + cellValue.Replace("\"", "\"\"") + "\"";
                        }
                        csv.Append(cellValue);
                        if (i < Columns.Count - 1)
                            csv.Append(",");
                    }
                    csv.AppendLine();
                }
            }

            return csv.ToString();
        }
    }

    /// <summary>
    /// Modern replacement for legacy StatusBar
    /// </summary>
    public class ModernStatusBar : StatusStrip
    {
        private ToolStripStatusLabel _statusLabel;
        private ToolStripProgressBar _progressBar;
        private ToolStripStatusLabel _infoLabel;

        public ModernStatusBar()
        {
            InitializeComponents();
            ApplyModernStyling();
        }

        public ToolStripStatusLabel StatusLabel => _statusLabel;
        public ToolStripProgressBar ProgressBar => _progressBar;
        public ToolStripStatusLabel InfoLabel => _infoLabel;

        private void InitializeComponents()
        {
            _statusLabel = new ToolStripStatusLabel
            {
                Name = "statusLabel",
                Text = "Listo",
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _progressBar = new ToolStripProgressBar
            {
                Name = "progressBar",
                Visible = false,
                Style = ProgressBarStyle.Continuous,
                Size = new Size(150, 16)
            };

            _infoLabel = new ToolStripStatusLabel
            {
                Name = "infoLabel",
                Text = "",
                TextAlign = ContentAlignment.MiddleRight,
                BorderSides = ToolStripStatusLabelBorderSides.Left,
                BorderStyle = Border3DStyle.Etched
            };

            Items.AddRange(new ToolStripItem[] { _statusLabel, _progressBar, _infoLabel });
        }

        private void ApplyModernStyling()
        {
            BackColor = SystemColors.Control;
            ForeColor = SystemColors.ControlText;
            GripStyle = ToolStripGripStyle.Hidden;
            SizingGrip = true;
        }

        /// <summary>
        /// Set status text with optional info
        /// </summary>
        public void SetStatus(string status, string info = null)
        {
            _statusLabel.Text = status ?? "Listo";
            if (info != null)
                _infoLabel.Text = info;
        }

        /// <summary>
        /// Show progress with optional maximum value
        /// </summary>
        public void ShowProgress(int value = 0, int maximum = 100)
        {
            _progressBar.Maximum = maximum;
            _progressBar.Value = Math.Min(value, maximum);
            _progressBar.Visible = true;
        }

        /// <summary>
        /// Hide progress bar
        /// </summary>
        public void HideProgress()
        {
            _progressBar.Visible = false;
        }

        /// <summary>
        /// Update progress value
        /// </summary>
        public void UpdateProgress(int value)
        {
            if (_progressBar.Visible)
            {
                _progressBar.Value = Math.Min(Math.Max(value, 0), _progressBar.Maximum);
            }
        }
    }

    /// <summary>
    /// Modern replacement for legacy ToolBar
    /// </summary>
    public class ModernToolBar : ToolStrip
    {
        public ModernToolBar()
        {
            InitializeToolBar();
            ApplyModernStyling();
        }

        private void InitializeToolBar()
        {
            AutoSize = true;
            GripStyle = ToolStripGripStyle.Hidden;
            LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            RenderMode = ToolStripRenderMode.System;
            Stretch = true;
        }

        private void ApplyModernStyling()
        {
            BackColor = SystemColors.Control;
            ForeColor = SystemColors.ControlText;
        }

        /// <summary>
        /// Add a button with modern styling
        /// </summary>
        public ToolStripButton AddButton(string name, string text, Image image = null, EventHandler clickHandler = null)
        {
            var button = new ToolStripButton
            {
                Name = name,
                Text = text,
                Image = image,
                ImageTransparentColor = Color.Magenta,
                DisplayStyle = image != null ? ToolStripItemDisplayStyle.ImageAndText : ToolStripItemDisplayStyle.Text,
                TextImageRelation = TextImageRelation.ImageBeforeText
            };

            if (clickHandler != null)
                button.Click += clickHandler;

            Items.Add(button);
            return button;
        }

        /// <summary>
        /// Add a separator
        /// </summary>
        public ToolStripSeparator AddSeparator()
        {
            var separator = new ToolStripSeparator();
            Items.Add(separator);
            return separator;
        }

        /// <summary>
        /// Add a dropdown button
        /// </summary>
        public ToolStripDropDownButton AddDropDownButton(string name, string text, Image image = null)
        {
            var dropDown = new ToolStripDropDownButton
            {
                Name = name,
                Text = text,
                Image = image,
                ImageTransparentColor = Color.Magenta,
                DisplayStyle = image != null ? ToolStripItemDisplayStyle.ImageAndText : ToolStripItemDisplayStyle.Text,
                TextImageRelation = TextImageRelation.ImageBeforeText
            };

            Items.Add(dropDown);
            return dropDown;
        }
    }
}