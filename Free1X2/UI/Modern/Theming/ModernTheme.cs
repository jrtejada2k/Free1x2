using System;
using System.Drawing;
using System.Windows.Forms;

namespace Free1X2.UI.Modern.Theming
{
    public static class ModernTheme
    {
        #region Colors

        public static class Colors
        {
            // Backgrounds
            public static readonly Color Background    = Color.FromArgb(243, 243, 243); // #F3F3F3 Windows 11
            public static readonly Color BackgroundAlt = Color.FromArgb(238, 238, 238);
            public static readonly Color Surface       = Color.White;
            public static readonly Color SurfaceAlt    = Color.FromArgb(249, 249, 249);

            // Accent - Windows 11 blue
            public static readonly Color Primary       = Color.FromArgb(0, 120, 212);
            public static readonly Color PrimaryHover  = Color.FromArgb(0, 102, 180);
            public static readonly Color PrimaryPress  = Color.FromArgb(0, 84, 153);
            public static readonly Color PrimaryLight  = Color.FromArgb(204, 228, 255);

            // Text
            public static readonly Color Text          = Color.FromArgb(26,  26,  26);
            public static readonly Color TextSecondary = Color.FromArgb(97,  97,  97);
            public static readonly Color TextDisabled  = Color.FromArgb(161, 161, 161);
            public static readonly Color TextOnPrimary = Color.White;

            // Borders
            public static readonly Color Border        = Color.FromArgb(213, 213, 213);
            public static readonly Color BorderFocus   = Color.FromArgb(0, 120, 212);

            // Grids
            public static readonly Color GridHeader    = Color.FromArgb(248, 249, 250);
            public static readonly Color GridAlternate = Color.FromArgb(248, 249, 250);
            public static readonly Color GridSelection = Color.FromArgb(204, 228, 255); // solid, alpha breaks DataGridView

            // Status
            public static readonly Color Success  = Color.FromArgb(16,  124, 16);
            public static readonly Color Warning  = Color.FromArgb(193, 100, 0);
            public static readonly Color Error    = Color.FromArgb(196, 43,  28);
            public static readonly Color Info     = Color.FromArgb(0,   120, 212);
        }

        #endregion

        #region Fonts

        public static class Fonts
        {
            private static Font Create(float size, FontStyle style = FontStyle.Regular)
            {
                try   { return new Font("Segoe UI", size, style); }
                catch { return new Font(SystemFonts.DefaultFont.FontFamily, size, style); }
            }

            public static readonly Font Default  = Create(9f);
            public static readonly Font Small    = Create(8f);
            public static readonly Font Large    = Create(10f);
            public static readonly Font Bold     = Create(9f,  FontStyle.Bold);
            public static readonly Font Header   = Create(11f, FontStyle.Bold);
            public static readonly Font Title    = Create(14f, FontStyle.Bold);
            public static readonly Font Caption  = Create(8f);
        }

        #endregion

        #region Sizes

        public static class Sizes
        {
            public const int SpacingXSmall  = 4;
            public const int SpacingSmall   = 8;
            public const int SpacingMedium  = 12;
            public const int SpacingLarge   = 16;
            public const int SpacingXLarge  = 24;

            public const int ControlHeight  = 28;
            public const int ButtonHeight   = 32;
            public const int ToolbarHeight  = 40;
            public const int GridRowHeight  = 24;
            public const int BorderRadius   = 4;

            public static readonly Size MinimumFormSize    = new Size(400, 300);
            public static readonly Size DefaultButtonSize  = new Size(100, ButtonHeight);
        }

        #endregion

        #region Apply — public entry points

        public static void ApplyToForm(Form form)
        {
            if (form == null) return;
            form.BackColor = Colors.Background;
            form.ForeColor = Colors.Text;
            form.Font      = Fonts.Default;
            ApplyRecursive(form.Controls);
        }

        public static void ApplyToControl(Control control)
        {
            if (control == null) return;
            AdaptFont(control);
            DispatchControl(control);
            if (control.HasChildren) ApplyRecursive(control.Controls);
        }

        #endregion

        #region Apply — dispatch

        private static void ApplyRecursive(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
                ApplyToControl(c);
        }

        private static void DispatchControl(Control control)
        {
            if (control.Tag is string tag && tag == "no-theme") return;

            switch (control)
            {
                case Button          b:   StyleButton(b);    break;
                case TextBox         tb:  StyleTextBox(tb);  break;
                case ComboBox        cb:  StyleComboBox(cb); break;
                case DataGridView    dg:  StyleDataGrid(dg); break;
                case UserControl     uc:  StyleUserControl(uc); break;
                case GroupBox        gb:  StyleGroupBox(gb); break;
                case TabControl      tc:  StyleTabControl(tc); break;
                case ListView        lv:  StyleListView(lv); break;
                case ListBox         lb:  StyleListBox(lb);  break;
                case CheckBox        ck:  StyleCheckBox(ck); break;
                case RadioButton     rb:  StyleRadioButton(rb); break;
                case Label           lbl: StyleLabel(lbl);   break;
                case NumericUpDown   nud: StyleNumericUpDown(nud); break;
                case StatusStrip     ss:  StyleStatusStrip(ss);   break;
                case MenuStrip       ms:  StyleMenuStrip(ms);     break;
                case ToolStrip       ts:  StyleToolStrip(ts);     break;
                case Panel           p:   StylePanel(p);     break;
                case SplitContainer  sc:  StyleSplitContainer(sc); break;
                case RichTextBox     rtb: StyleRichTextBox(rtb);   break;
            }
        }

        #endregion

        #region Apply — per-control

        private static void StyleButton(Button b)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.UseVisualStyleBackColor = false;
            b.BackColor = Colors.Surface;
            b.ForeColor = Colors.Text;
            b.FlatAppearance.BorderColor        = Colors.Border;
            b.FlatAppearance.MouseOverBackColor  = Colors.BackgroundAlt;
            b.FlatAppearance.MouseDownBackColor  = Colors.PrimaryLight;
            b.Cursor = Cursors.Hand;
        }

        private static void StyleTextBox(TextBox tb)
        {
            tb.BorderStyle = BorderStyle.FixedSingle;
            tb.BackColor   = Colors.Surface;
            tb.ForeColor   = Colors.Text;
        }

        private static void StyleComboBox(ComboBox cb)
        {
            cb.FlatStyle = FlatStyle.Flat;
            cb.BackColor = Colors.Surface;
            cb.ForeColor = Colors.Text;
        }

        private static void StyleDataGrid(DataGridView dg)
        {
            dg.BackgroundColor = Colors.Surface;
            dg.GridColor       = Colors.Border;
            dg.BorderStyle     = BorderStyle.FixedSingle;
            dg.CellBorderStyle           = DataGridViewCellBorderStyle.SingleHorizontal;
            dg.ColumnHeadersBorderStyle  = DataGridViewHeaderBorderStyle.Single;
            dg.EnableHeadersVisualStyles = false;
            dg.RowHeadersVisible         = false;
            dg.SelectionMode             = DataGridViewSelectionMode.FullRowSelect;

            dg.DefaultCellStyle.BackColor          = Colors.Surface;
            dg.DefaultCellStyle.ForeColor          = Colors.Text;
            dg.DefaultCellStyle.SelectionBackColor = Colors.GridSelection;
            dg.DefaultCellStyle.SelectionForeColor = Colors.Text;
            dg.DefaultCellStyle.Font               = Fonts.Default;

            dg.ColumnHeadersDefaultCellStyle.BackColor = Colors.GridHeader;
            dg.ColumnHeadersDefaultCellStyle.ForeColor = Colors.Text;
            dg.ColumnHeadersDefaultCellStyle.Font      = Fonts.Bold;
            dg.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dg.ColumnHeadersDefaultCellStyle.SelectionBackColor = Colors.GridHeader;
            dg.ColumnHeadersDefaultCellStyle.SelectionForeColor = Colors.Text;

            dg.AlternatingRowsDefaultCellStyle.BackColor          = Colors.GridAlternate;
            dg.AlternatingRowsDefaultCellStyle.ForeColor          = Colors.Text;
            dg.AlternatingRowsDefaultCellStyle.SelectionBackColor = Colors.GridSelection;
            dg.AlternatingRowsDefaultCellStyle.SelectionForeColor = Colors.Text;
        }

        private static void StyleUserControl(UserControl uc)
        {
            // Replace legacy background colors; preserve intentional Surface/white controls
            Color bg = uc.BackColor;
            if (bg == Color.Bisque || bg == Color.NavajoWhite || bg == Color.Khaki ||
                bg == SystemColors.Control || bg == Color.DarkSalmon || bg == Color.LightSalmon)
                uc.BackColor = Colors.Background;
            uc.ForeColor = Colors.Text;
        }

        private static void StyleDataGridLegacy(DataGrid dg)
        {
            dg.BackgroundColor    = Colors.Background;
            dg.BackColor          = Colors.Surface;
            dg.ForeColor          = Colors.Text;
            dg.AlternatingBackColor = Colors.GridAlternate;
            dg.SelectionBackColor = Colors.Primary;
            dg.SelectionForeColor = Color.White;
            dg.HeaderBackColor    = Colors.GridHeader;
            dg.HeaderForeColor    = Colors.Text;
            dg.GridLineColor      = Colors.Border;
            dg.BorderStyle        = BorderStyle.FixedSingle;
            dg.Font               = Fonts.Default;
        }

        private static void StyleGroupBox(GroupBox gb)
        {
            gb.ForeColor = Colors.TextSecondary;
        }

        private static void StyleTabControl(TabControl tc)
        {
            tc.Font = Fonts.Default;
        }

        private static void StyleListView(ListView lv)
        {
            lv.BackColor             = Colors.Surface;
            lv.ForeColor             = Colors.Text;
            lv.BorderStyle           = BorderStyle.FixedSingle;
            lv.OwnerDraw             = false;
        }

        private static void StyleListBox(ListBox lb)
        {
            lb.BackColor   = Colors.Surface;
            lb.ForeColor   = Colors.Text;
            lb.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void StyleCheckBox(CheckBox ck)
        {
            ck.ForeColor = Colors.Text;
        }

        private static void StyleRadioButton(RadioButton rb)
        {
            rb.ForeColor = Colors.Text;
        }

        private static void StyleLabel(Label lbl)
        {
            // Don't override if label uses a link-style background
            if (lbl.BackColor != Color.Transparent && lbl.BackColor != Colors.Background)
                lbl.BackColor = Color.Transparent;
            lbl.ForeColor = Colors.Text;
        }

        private static void StyleNumericUpDown(NumericUpDown nud)
        {
            nud.BackColor = Colors.Surface;
            nud.ForeColor = Colors.Text;
            nud.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void StyleStatusStrip(StatusStrip ss)
        {
            ss.BackColor   = Colors.Primary;
            ss.ForeColor   = Colors.TextOnPrimary;
            ss.SizingGrip  = false;
            ss.Renderer    = NeoRenderer;
            foreach (ToolStripItem item in ss.Items)
                item.ForeColor = Colors.TextOnPrimary;
        }

        private static void StyleMenuStrip(MenuStrip ms)
        {
            ms.BackColor = Colors.Background;
            ms.ForeColor = Colors.Text;
            ms.Renderer  = NeoRenderer;
            foreach (ToolStripItem item in ms.Items)
                item.ForeColor = Colors.Text;
        }

        private static void StyleToolStrip(ToolStrip ts)
        {
            ts.BackColor = Colors.Background;
            ts.ForeColor = Colors.Text;
            ts.Renderer  = NeoRenderer;
            ts.GripStyle = ToolStripGripStyle.Hidden;
            foreach (ToolStripItem item in ts.Items)
                item.ForeColor = Colors.Text;
        }

        private static void StylePanel(Panel p)
        {
            if (p.BackColor == SystemColors.Control)
                p.BackColor = Colors.Background;
            p.ForeColor = Colors.Text;
        }

        private static void StyleSplitContainer(SplitContainer sc)
        {
            sc.BackColor    = Colors.Background;
            sc.Panel1.BackColor = Colors.Background;
            sc.Panel2.BackColor = Colors.Background;
        }

        private static void StyleRichTextBox(RichTextBox rtb)
        {
            rtb.BackColor   = Colors.Surface;
            rtb.ForeColor   = Colors.Text;
            rtb.BorderStyle = BorderStyle.FixedSingle;
        }

        #endregion

        #region Font adaptation

        private static void AdaptFont(Control control)
        {
            string family = control.Font.FontFamily.Name;
            if (family == "Segoe UI") return; // already modern

            // Preserve bold/italic but not italic-only (legacy decorative fonts)
            FontStyle style = control.Font.Style;
            // Preserve size if deliberately large (header / title usage)
            float size = control.Font.Size > 9f ? control.Font.Size : 9f;

            try   { control.Font = new Font("Segoe UI", size, style); }
            catch { control.Font = Fonts.Default; }
        }

        #endregion

        #region ToolStrip renderer

        private static readonly NeoToolStripRenderer NeoRenderer = new NeoToolStripRenderer();

        #endregion

        #region Utility

        public static Color GetStatusColor(double value, double high = 80, double mid = 60)
        {
            if (value >= high) return Colors.Success;
            if (value >= mid)  return Colors.Warning;
            return Colors.Error;
        }

        public static Color LightenColor(Color c, float pct)
        {
            return Color.FromArgb(c.A,
                Math.Min(255, c.R + (int)(255 * pct)),
                Math.Min(255, c.G + (int)(255 * pct)),
                Math.Min(255, c.B + (int)(255 * pct)));
        }

        public static Color DarkenColor(Color c, float pct)
        {
            return Color.FromArgb(c.A,
                Math.Max(0, c.R - (int)(255 * pct)),
                Math.Max(0, c.G - (int)(255 * pct)),
                Math.Max(0, c.B - (int)(255 * pct)));
        }

        #endregion
    }

    // ---------------------------------------------------------------------------
    // Flat renderer for MenuStrip / ToolStrip / StatusStrip
    // ---------------------------------------------------------------------------
    public sealed class NeoToolStripRenderer : ToolStripProfessionalRenderer
    {
        public NeoToolStripRenderer() : base(new NeoColorTable()) { }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            Color bg = e.ToolStrip is StatusStrip
                ? ModernTheme.Colors.Primary
                : ModernTheme.Colors.Background;
            using (var b = new SolidBrush(bg))
                e.Graphics.FillRectangle(b, e.AffectedBounds);
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Selected && !e.Item.Pressed) return;
            Color bg = e.Item.Pressed
                ? ModernTheme.Colors.PrimaryLight
                : ModernTheme.Colors.BackgroundAlt;
            using (var b = new SolidBrush(bg))
                e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Selected && !e.Item.Pressed) return;
            Color bg = e.Item.Pressed
                ? ModernTheme.Colors.PrimaryLight
                : ModernTheme.Colors.BackgroundAlt;
            using (var b = new SolidBrush(bg))
            {
                var r = new Rectangle(2, 0, e.Item.Width - 4, e.Item.Height);
                e.Graphics.FillRectangle(b, r);
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            int y = e.Item.Height / 2;
            using (var p = new Pen(ModernTheme.Colors.Border))
                e.Graphics.DrawLine(p, 4, y, e.Item.Width - 4, y);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            // No border on toolbar — clean flat look
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            using (var b = new SolidBrush(ModernTheme.Colors.Surface))
                e.Graphics.FillRectangle(b, e.AffectedBounds);
        }
    }

    public sealed class NeoColorTable : ProfessionalColorTable
    {
        public override Color MenuStripGradientBegin           => ModernTheme.Colors.Background;
        public override Color MenuStripGradientEnd             => ModernTheme.Colors.Background;
        public override Color ToolStripGradientBegin           => ModernTheme.Colors.Background;
        public override Color ToolStripGradientEnd             => ModernTheme.Colors.Background;
        public override Color ToolStripGradientMiddle          => ModernTheme.Colors.Background;
        public override Color ToolStripDropDownBackground      => ModernTheme.Colors.Surface;
        public override Color MenuBorder                       => ModernTheme.Colors.Border;
        public override Color MenuItemBorder                   => Color.Transparent;
        public override Color MenuItemSelected                 => ModernTheme.Colors.BackgroundAlt;
        public override Color MenuItemSelectedGradientBegin    => ModernTheme.Colors.BackgroundAlt;
        public override Color MenuItemSelectedGradientEnd      => ModernTheme.Colors.BackgroundAlt;
        public override Color MenuItemPressedGradientBegin     => ModernTheme.Colors.PrimaryLight;
        public override Color MenuItemPressedGradientEnd       => ModernTheme.Colors.PrimaryLight;
        public override Color ImageMarginGradientBegin         => ModernTheme.Colors.Surface;
        public override Color ImageMarginGradientEnd           => ModernTheme.Colors.Surface;
        public override Color ImageMarginGradientMiddle        => ModernTheme.Colors.Surface;
        public override Color CheckBackground                  => ModernTheme.Colors.PrimaryLight;
        public override Color CheckSelectedBackground          => ModernTheme.Colors.Primary;
        public override Color CheckPressedBackground           => ModernTheme.Colors.PrimaryHover;
        public override Color ButtonSelectedHighlight          => ModernTheme.Colors.BackgroundAlt;
        public override Color ButtonPressedHighlight           => ModernTheme.Colors.PrimaryLight;
        public override Color ButtonCheckedHighlight           => ModernTheme.Colors.PrimaryLight;
        public override Color SeparatorDark                    => ModernTheme.Colors.Border;
        public override Color SeparatorLight                   => ModernTheme.Colors.Surface;
        public override Color ToolStripBorder                  => Color.Transparent;
        public override Color ToolStripContentPanelGradientBegin => ModernTheme.Colors.Background;
        public override Color ToolStripContentPanelGradientEnd   => ModernTheme.Colors.Background;
        public override Color ToolStripPanelGradientBegin        => ModernTheme.Colors.Background;
        public override Color ToolStripPanelGradientEnd          => ModernTheme.Colors.Background;
    }

    // ---------------------------------------------------------------------------
    // Legacy helpers kept for backward compatibility
    // ---------------------------------------------------------------------------
    public static class ModernIcons
    {
        public const int SmallSize  = 16;
        public const int MediumSize = 24;
        public const int LargeSize  = 32;
    }

    public enum SystemIconType { Application, Error, Warning, Information, Question }
}
