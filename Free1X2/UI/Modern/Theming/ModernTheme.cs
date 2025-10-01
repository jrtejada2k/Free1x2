using System;
using System.Drawing;
using System.Windows.Forms;

namespace Free1X2.UI.Modern.Theming
{
    /// <summary>
    /// Modern theme system for Free1X2 application
    /// Provides consistent visual styling across all forms and controls
    /// </summary>
    public static class ModernTheme
    {
        #region Color Scheme

        public static class Colors
        {
            // Primary colors
            public static readonly Color Primary = Color.FromArgb(0, 120, 215);           // Windows Blue
            public static readonly Color PrimaryDark = Color.FromArgb(0, 99, 177);        // Darker blue
            public static readonly Color PrimaryLight = Color.FromArgb(153, 209, 255);    // Light blue
            
            // Background colors
            public static readonly Color Background = SystemColors.Control;
            public static readonly Color BackgroundAlt = Color.FromArgb(250, 250, 250);
            public static readonly Color Surface = SystemColors.Window;
            public static readonly Color SurfaceAlt = Color.FromArgb(248, 249, 250);
            
            // Text colors
            public static readonly Color Text = SystemColors.ControlText;
            public static readonly Color TextSecondary = Color.FromArgb(108, 117, 125);
            public static readonly Color TextOnPrimary = Color.White;
            
            // Border colors
            public static readonly Color Border = Color.FromArgb(222, 226, 230);
            public static readonly Color BorderFocus = Primary;
            
            // Status colors
            public static readonly Color Success = Color.FromArgb(25, 135, 84);
            public static readonly Color Warning = Color.FromArgb(255, 193, 7);
            public static readonly Color Error = Color.FromArgb(220, 53, 69);
            public static readonly Color Info = Color.FromArgb(13, 202, 240);
            
            // Grid colors
            public static readonly Color GridHeader = Color.FromArgb(248, 249, 250);
            public static readonly Color GridAlternate = Color.FromArgb(248, 249, 250);
            public static readonly Color GridSelection = Color.FromArgb(0, 120, 215, 50);
        }

        #endregion

        #region Fonts

        public static class Fonts
        {
            public static readonly Font Default = SystemFonts.DefaultFont;
            public static readonly Font Small = new Font(SystemFonts.DefaultFont.FontFamily, 8.25f);
            public static readonly Font Large = new Font(SystemFonts.DefaultFont.FontFamily, 10f);
            public static readonly Font Header = new Font(SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
            public static readonly Font Title = new Font(SystemFonts.DefaultFont.FontFamily, 14f, FontStyle.Bold);
            public static readonly Font Caption = new Font(SystemFonts.DefaultFont.FontFamily, 8.25f, FontStyle.Regular);
            public static readonly Font Bold = new Font(SystemFonts.DefaultFont.FontFamily, SystemFonts.DefaultFont.Size, FontStyle.Bold);
        }

        #endregion

        #region Sizing

        public static class Sizes
        {
            // Spacing
            public const int SpacingXSmall = 4;
            public const int SpacingSmall = 8;
            public const int SpacingMedium = 12;
            public const int SpacingLarge = 16;
            public const int SpacingXLarge = 24;
            
            // Control heights
            public const int ControlHeight = 23;
            public const int ButtonHeight = 30;
            public const int ToolbarHeight = 35;
            
            // Border radius
            public const int BorderRadius = 4;
            public const int BorderRadiusLarge = 8;
            
            // Minimum sizes
            public static readonly Size MinimumFormSize = new Size(400, 300);
            public static readonly Size DefaultButtonSize = new Size(100, ButtonHeight);
        }

        #endregion

        #region Application Methods

        /// <summary>
        /// Apply modern theme to a form
        /// </summary>
        public static void ApplyToForm(Form form)
        {
            if (form == null) return;

            form.BackColor = Colors.Background;
            form.ForeColor = Colors.Text;
            form.Font = Fonts.Default;
            
            // Apply to all child controls
            ApplyToControlsRecursive(form.Controls);
        }

        /// <summary>
        /// Apply modern theme to a control collection recursively
        /// </summary>
        private static void ApplyToControlsRecursive(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                ApplyToControl(control);
                
                if (control.HasChildren)
                {
                    ApplyToControlsRecursive(control.Controls);
                }
            }
        }

        /// <summary>
        /// Apply modern theme to a specific control
        /// </summary>
        public static void ApplyToControl(Control control)
        {
            if (control == null) return;

            // Set default font
            if (control.Font.Equals(SystemFonts.DefaultFont) || control.Font.FontFamily.Name == "Microsoft Sans Serif")
            {
                control.Font = Fonts.Default;
            }

            switch (control)
            {
                case Button button:
                    ApplyToButton(button);
                    break;
                
                case TextBox textBox:
                    ApplyToTextBox(textBox);
                    break;
                
                case ComboBox comboBox:
                    ApplyToComboBox(comboBox);
                    break;
                
                case DataGridView dataGrid:
                    ApplyToDataGridView(dataGrid);
                    break;
                
                case GroupBox groupBox:
                    ApplyToGroupBox(groupBox);
                    break;
                
                case TabControl tabControl:
                    ApplyToTabControl(tabControl);
                    break;
                
                case StatusStrip statusStrip:
                    ApplyToStatusStrip(statusStrip);
                    break;
                
                case MenuStrip menuStrip:
                    ApplyToMenuStrip(menuStrip);
                    break;
                
                case ToolStrip toolStrip:
                    ApplyToToolStrip(toolStrip);
                    break;
                
                case Panel panel:
                    ApplyToPanel(panel);
                    break;
                
                default:
                    // For other control types, apply basic styling
                    break;
            }
        }

        #endregion

        #region Control-Specific Styling

        private static void ApplyToButton(Button button)
        {
            button.FlatStyle = FlatStyle.System;
            button.BackColor = Colors.Primary;
            button.ForeColor = Colors.TextOnPrimary;
            button.Font = Fonts.Default;
            button.UseVisualStyleBackColor = true;
            
            if (button.Size == Size.Empty || button.Size.Width < Sizes.DefaultButtonSize.Width)
            {
                button.Size = Sizes.DefaultButtonSize;
            }
        }

        private static void ApplyToTextBox(TextBox textBox)
        {
            textBox.BorderStyle = BorderStyle.Fixed3D;
            textBox.BackColor = Colors.Surface;
            textBox.ForeColor = Colors.Text;
            textBox.Font = Fonts.Default;
        }

        private static void ApplyToComboBox(ComboBox comboBox)
        {
            comboBox.FlatStyle = FlatStyle.Standard;
            comboBox.BackColor = Colors.Surface;
            comboBox.ForeColor = Colors.Text;
            comboBox.Font = Fonts.Default;
        }

        private static void ApplyToDataGridView(DataGridView dataGrid)
        {
            // Background colors
            dataGrid.BackgroundColor = Colors.Surface;
            dataGrid.GridColor = Colors.Border;
            
            // Default cell style
            dataGrid.DefaultCellStyle.BackColor = Colors.Surface;
            dataGrid.DefaultCellStyle.ForeColor = Colors.Text;
            dataGrid.DefaultCellStyle.SelectionBackColor = Colors.Primary;
            dataGrid.DefaultCellStyle.SelectionForeColor = Colors.TextOnPrimary;
            dataGrid.DefaultCellStyle.Font = Fonts.Default;
            
            // Column header style
            dataGrid.ColumnHeadersDefaultCellStyle.BackColor = Colors.GridHeader;
            dataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Colors.Text;
            dataGrid.ColumnHeadersDefaultCellStyle.Font = Fonts.Bold;
            dataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            
            // Alternating row style
            dataGrid.AlternatingRowsDefaultCellStyle.BackColor = Colors.GridAlternate;
            dataGrid.AlternatingRowsDefaultCellStyle.ForeColor = Colors.Text;
            
            // General settings
            dataGrid.EnableHeadersVisualStyles = false;
            dataGrid.BorderStyle = BorderStyle.Fixed3D;
            dataGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        }

        private static void ApplyToGroupBox(GroupBox groupBox)
        {
            groupBox.ForeColor = Colors.Text;
            groupBox.Font = Fonts.Default;
        }

        private static void ApplyToTabControl(TabControl tabControl)
        {
            tabControl.Font = Fonts.Default;
        }

        private static void ApplyToStatusStrip(StatusStrip statusStrip)
        {
            statusStrip.BackColor = Colors.Background;
            statusStrip.ForeColor = Colors.Text;
            statusStrip.Font = Fonts.Default;
        }

        private static void ApplyToToolStrip(ToolStrip toolStrip)
        {
            toolStrip.BackColor = Colors.Background;
            toolStrip.ForeColor = Colors.Text;
            toolStrip.Font = Fonts.Default;
            toolStrip.RenderMode = ToolStripRenderMode.System;
        }

        private static void ApplyToMenuStrip(MenuStrip menuStrip)
        {
            menuStrip.BackColor = Colors.Background;
            menuStrip.ForeColor = Colors.Text;
            menuStrip.Font = Fonts.Default;
            menuStrip.RenderMode = ToolStripRenderMode.System;
        }

        private static void ApplyToPanel(Panel panel)
        {
            if (panel.BackColor == SystemColors.Control)
            {
                panel.BackColor = Colors.Background;
            }
            panel.ForeColor = Colors.Text;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Get status color based on value
        /// </summary>
        public static Color GetStatusColor(double value, double threshold1 = 80, double threshold2 = 60)
        {
            if (value >= threshold1) return Colors.Success;
            if (value >= threshold2) return Colors.Warning;
            return Colors.Error;
        }

        /// <summary>
        /// Create a modern gradient brush for backgrounds
        /// </summary>
        public static System.Drawing.Drawing2D.LinearGradientBrush CreateGradientBrush(Rectangle bounds, Color color1, Color color2)
        {
            return new System.Drawing.Drawing2D.LinearGradientBrush(bounds, color1, color2, 90f);
        }

        /// <summary>
        /// Lighten a color by a percentage
        /// </summary>
        public static Color LightenColor(Color color, float percentage)
        {
            var r = Math.Min(255, color.R + (int)(255 * percentage));
            var g = Math.Min(255, color.G + (int)(255 * percentage));
            var b = Math.Min(255, color.B + (int)(255 * percentage));
            return Color.FromArgb(color.A, r, g, b);
        }

        /// <summary>
        /// Darken a color by a percentage
        /// </summary>
        public static Color DarkenColor(Color color, float percentage)
        {
            var r = Math.Max(0, color.R - (int)(255 * percentage));
            var g = Math.Max(0, color.G - (int)(255 * percentage));
            var b = Math.Max(0, color.B - (int)(255 * percentage));
            return Color.FromArgb(color.A, r, g, b);
        }

        #endregion
    }

    /// <summary>
    /// Modern icon provider for consistent iconography
    /// </summary>
    public static class ModernIcons
    {
        // Standard sizes
        public const int SmallSize = 16;
        public const int MediumSize = 24;
        public const int LargeSize = 32;

        /// <summary>
        /// Get system icon for common operations
        /// </summary>
        public static Icon GetSystemIcon(SystemIconType iconType, int size = MediumSize)
        {
            switch (iconType)
            {
                case SystemIconType.Application:
                    return SystemIcons.Application;
                case SystemIconType.Error:
                    return SystemIcons.Error;
                case SystemIconType.Warning:
                    return SystemIcons.Warning;
                case SystemIconType.Information:
                    return SystemIcons.Information;
                case SystemIconType.Question:
                    return SystemIcons.Question;
                default:
                    return SystemIcons.Application;
            }
        }

        /// <summary>
        /// Create a simple colored bitmap for use as icon
        /// </summary>
        public static Bitmap CreateColoredBitmap(Color color, int size = MediumSize)
        {
            var bitmap = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(color);
                g.DrawRectangle(new Pen(ModernTheme.Colors.Border), 0, 0, size - 1, size - 1);
            }
            return bitmap;
        }
    }

    /// <summary>
    /// System icon types
    /// </summary>
    public enum SystemIconType
    {
        Application,
        Error,
        Warning,
        Information,
        Question
    }
}