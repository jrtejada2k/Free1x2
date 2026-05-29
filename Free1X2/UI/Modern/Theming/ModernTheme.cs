using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Free1X2.UI.Modern.Theming
{
    public static class ModernTheme
    {
        #region Colors

        public static class Colors
        {
            // Slate / Indigo palette — modern, higher contrast than Windows-11 grey.
            // Backgrounds use cool slate (Tailwind slate-50/100), accent is indigo-600.
            // Backgrounds
            public static readonly Color Background    = Color.FromArgb(245, 247, 250); // #F5F7FA slate-50
            public static readonly Color BackgroundAlt = Color.FromArgb(232, 237, 244); // #E8EDF4 slate-100
            public static readonly Color Surface       = Color.White;
            public static readonly Color SurfaceAlt    = Color.FromArgb(248, 250, 252);

            // Accent - Indigo 600
            public static readonly Color Primary       = Color.FromArgb( 79,  70, 229); // #4F46E5
            public static readonly Color PrimaryHover  = Color.FromArgb( 67,  56, 202); // #4338CA indigo-700
            public static readonly Color PrimaryPress  = Color.FromArgb( 55,  48, 163); // #3730A3 indigo-800
            public static readonly Color PrimaryLight  = Color.FromArgb(224, 231, 255); // #E0E7FF indigo-100

            // Text - slate scale, very high contrast
            public static readonly Color Text          = Color.FromArgb( 15,  23,  42); // #0F172A slate-900
            public static readonly Color TextSecondary = Color.FromArgb( 51,  65,  85); // #334155 slate-700 (was 97,97,97 — too light)
            public static readonly Color TextDisabled  = Color.FromArgb(148, 163, 184); // #94A3B8 slate-400
            public static readonly Color TextOnPrimary = Color.White;

            // Borders
            public static readonly Color Border        = Color.FromArgb(203, 213, 225); // #CBD5E1 slate-300
            public static readonly Color BorderFocus   = Color.FromArgb( 79,  70, 229);

            // Grids
            public static readonly Color GridHeader    = Color.FromArgb(241, 245, 249); // slate-100
            public static readonly Color GridAlternate = Color.FromArgb(248, 250, 252); // slate-50
            public static readonly Color GridSelection = Color.FromArgb(224, 231, 255); // indigo-100

            // Status — keep semantic colors but tuned to new palette
            public static readonly Color Success  = Color.FromArgb( 22, 163,  74); // #16A34A green-600
            public static readonly Color Warning  = Color.FromArgb(217, 119,   6); // #D97706 amber-600
            public static readonly Color Error    = Color.FromArgb(220,  38,  38); // #DC2626 red-600
            public static readonly Color Info     = Color.FromArgb( 79,  70, 229);
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

            // The form's default action button becomes the accent/primary button —
            // gives every dialog a clear visual hierarchy without per-form work.
            if (form.AcceptButton is Button accept && !(accept.Tag is string at && at == "no-theme"))
                MakePrimary(accept);
        }

        /// <summary>
        /// Promote a button to the accent (primary) style: filled indigo, white text.
        /// Safe to call from forms that want an explicit primary action.
        /// </summary>
        public static void MakePrimary(Button b)
        {
            b.BackColor = Colors.Primary;
            b.ForeColor = Colors.TextOnPrimary;
            b.FlatAppearance.BorderColor       = Colors.Primary;
            b.FlatAppearance.MouseOverBackColor = Colors.PrimaryHover;
            b.FlatAppearance.MouseDownBackColor = Colors.PrimaryPress;
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
                case TreeView        tv:  StyleTreeView(tv);   break;
                case PictureBox      pb:  StylePictureBox(pb); break;
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
            // We paint our own rounded border, so disable the built-in square one.
            b.FlatAppearance.BorderSize          = 0;
            b.FlatAppearance.BorderColor         = Colors.Border;
            b.FlatAppearance.MouseOverBackColor  = Colors.BackgroundAlt;
            b.FlatAppearance.MouseDownBackColor  = Colors.PrimaryLight;
            b.Cursor = Cursors.Hand;
            RoundButton.Attach(b);
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

        /// <summary>
        /// True if the color is one of the legacy decorative backgrounds the modern
        /// theme replaces (Bisque/Khaki/Wheat/... and SystemColors.Control).
        /// </summary>
        private static bool IsLegacyBackground(Color bg)
        {
            return bg == Color.Bisque       || bg == Color.NavajoWhite ||
                   bg == Color.Khaki        || bg == SystemColors.Control ||
                   bg == Color.DarkSalmon   || bg == Color.LightSalmon  ||
                   bg == Color.LemonChiffon || bg == Color.AntiqueWhite ||
                   bg == Color.Ivory        || bg == Color.Wheat        ||
                   bg == Color.OldLace      || bg == Color.PapayaWhip   ||
                   bg == Color.Moccasin     || bg == Color.PeachPuff;
        }

        private static void StyleUserControl(UserControl uc)
        {
            // Replace legacy background colors; preserve intentional Surface/white controls
            if (IsLegacyBackground(uc.BackColor))
                uc.BackColor = Colors.Surface;
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
            // Use full-strength text + bold so titles stay legible even when
            // the GroupBox is disabled (Windows lightens the ForeColor further).
            gb.ForeColor = Colors.Text;
            gb.Font      = Fonts.Bold;
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
            // Remap any legacy decorative background (was: only SystemColors.Control,
            // which left Bisque/Khaki/etc. panels as stale islands of old color).
            if (IsLegacyBackground(p.BackColor))
                p.BackColor = Colors.Background;
            p.ForeColor = Colors.Text;
        }

        private static void StyleTreeView(TreeView tv)
        {
            tv.BackColor   = Colors.Surface;
            tv.ForeColor   = Colors.Text;
            tv.BorderStyle = BorderStyle.FixedSingle;
            tv.LineColor   = Colors.Border;
        }

        private static void StylePictureBox(PictureBox pb)
        {
            // Only neutralize legacy filler backgrounds; never touch boxes that
            // intentionally carry an image or a deliberate color.
            if (IsLegacyBackground(pb.BackColor))
                pb.BackColor = Colors.Surface;
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
    // Rounded-corner rendering for flat Buttons.
    // Clips the button to a rounded region and paints a smooth 1px border using
    // the button's own FlatAppearance.BorderColor — so SetBotonEstado (which only
    // swaps BackColor/BorderColor) keeps working and just renders rounded now.
    // ---------------------------------------------------------------------------
    internal static class RoundButton
    {
        private const int Radius = 6;

        // Tracks buttons already wired up so re-theming never double-subscribes.
        private static readonly ConditionalWeakTable<Button, object> Wired =
            new ConditionalWeakTable<Button, object>();

        public static void Attach(Button b)
        {
            if (Wired.TryGetValue(b, out _)) { UpdateRegion(b); return; }
            Wired.Add(b, new object());

            UpdateRegion(b);
            b.SizeChanged += (s, e) => UpdateRegion((Button)s);
            b.Paint       += OnPaint;
        }

        private static int EffectiveRadius(Button b)
        {
            int r = Radius;
            r = Math.Min(r, b.Height / 2);
            r = Math.Min(r, b.Width  / 2);
            return Math.Max(0, r);
        }

        private static void UpdateRegion(Button b)
        {
            if (b.Width <= 0 || b.Height <= 0) return;
            int r = EffectiveRadius(b);
            if (r <= 0) { b.Region = null; return; }
            using (var path = RoundedRect(new Rectangle(0, 0, b.Width, b.Height), r))
                b.Region = new Region(path);
        }

        private static void OnPaint(object sender, PaintEventArgs e)
        {
            var b = (Button)sender;
            int r = EffectiveRadius(b);
            if (r <= 0) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            // Inset by 1px so the full stroke stays inside the clipped region.
            var rect = new Rectangle(0, 0, b.Width - 1, b.Height - 1);
            using (var path = RoundedRect(rect, r))
            using (var pen  = new Pen(b.FlatAppearance.BorderColor))
                e.Graphics.DrawPath(pen, path);
        }

        internal static GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            if (d <= 0) { path.AddRectangle(rect); return path; }
            path.AddArc(rect.X,             rect.Y,              d, d, 180, 90);
            path.AddArc(rect.Right - d,     rect.Y,              d, d, 270, 90);
            path.AddArc(rect.Right - d,     rect.Bottom - d,     d, d,   0, 90);
            path.AddArc(rect.X,             rect.Bottom - d,     d, d,  90, 90);
            path.CloseFigure();
            return path;
        }
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
