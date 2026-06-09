using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Free1X2.UI.Modern.Icons
{
    /// <summary>
    /// Renders Windows 11 Segoe Fluent Icons (with Segoe MDL2 Assets fallback for Win10)
    /// as Bitmaps usable by WinForms ToolStripButton/Button.Image.
    /// Glyphs are addressed by Unicode codepoint constants in <see cref="Glyph"/>.
    /// </summary>
    public static class SegoeIcons
    {
        private const string PrimaryFamily  = "Segoe Fluent Icons";
        private const string FallbackFamily = "Segoe MDL2 Assets";

        private static readonly Lazy<string> ResolvedFamily = new Lazy<string>(ResolveFamily);

        private static string ResolveFamily()
        {
            using (var fc = new InstalledFontCollection())
            {
                foreach (var f in fc.Families)
                {
                    if (string.Equals(f.Name, PrimaryFamily, StringComparison.OrdinalIgnoreCase))
                        return PrimaryFamily;
                }
                foreach (var f in fc.Families)
                {
                    if (string.Equals(f.Name, FallbackFamily, StringComparison.OrdinalIgnoreCase))
                        return FallbackFamily;
                }
            }
            return null;
        }

        public static bool IsAvailable => ResolvedFamily.Value != null;

        /// <summary>
        /// Render a glyph as a Bitmap of size×size pixels, drawn in <paramref name="color"/>.
        /// Returns null if no Fluent/MDL2 font is installed (fallback to legacy icons).
        /// </summary>
        public static Bitmap Render(string glyph, int size, Color color)
        {
            if (string.IsNullOrEmpty(glyph)) return null;
            string family = ResolvedFamily.Value;
            if (family == null) return null;

            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode      = SmoothingMode.AntiAlias;
                g.TextRenderingHint  = TextRenderingHint.AntiAliasGridFit;
                g.InterpolationMode  = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode    = PixelOffsetMode.HighQuality;

                float emSize = size * 0.72f;
                using (var font = new Font(family, emSize, FontStyle.Regular, GraphicsUnit.Pixel))
                using (var brush = new SolidBrush(color))
                using (var fmt = new StringFormat
                {
                    Alignment     = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                {
                    g.DrawString(glyph, font, brush, new RectangleF(0, 0, size, size), fmt);
                }
            }
            return bmp;
        }

        /// <summary>
        /// Subset of Segoe Fluent / MDL2 glyphs used by Free1X2.
        /// Codepoints reference Microsoft's Segoe MDL2 Assets table; missing glyphs
        /// in older fonts simply render as a tofu box without crashing.
        /// </summary>
        public static class Glyph
        {
            // ===== Files / archive =====
            public const string NewFile          = ""; // Page
            public const string OpenFile         = ""; // OpenFile
            public const string Save             = "";
            public const string SaveAs           = "";
            public const string Delete           = "";
            public const string CloudDownload    = "";
            public const string Folder           = "";
            public const string FolderOpen       = "";
            public const string Document         = "";
            public const string Print            = "";

            // ===== App / system =====
            public const string PowerButton      = "";
            public const string Settings         = "";
            public const string DeveloperTools   = "";
            public const string Help             = "";
            public const string Info             = "";
            public const string Refresh          = "";
            public const string Tools            = "";

            // ===== People =====
            public const string People           = "";
            public const string Contact          = "";
            public const string Contact2         = "";
            public const string ContactInfo      = "";
            public const string AddFriend        = "";

            // ===== Math / operations =====
            public const string Calculator       = "";
            public const string Sigma            = ""; // (LineChart fallback)
            public const string Sync             = "";
            public const string Switch           = "";
            public const string Crop             = "";
            public const string Rotate           = "";

            // ===== Navigation =====
            public const string Back             = "";
            public const string Forward          = "";
            public const string First            = "";
            public const string Last             = "";
            public const string Up               = "";
            public const string Down             = "";

            // ===== Editing =====
            public const string Cut              = "";
            public const string Copy             = "";
            public const string Paste            = "";
            public const string Add              = "";
            public const string Remove           = "";
            public const string Edit             = "";

            // ===== Combination toolbar =====
            public const string View             = "";
            public const string Search           = "";
            public const string DocumentApproval = "";
            public const string BulletedList     = "";
            public const string Equalizer        = "";
            public const string BarChart4        = "";
            public const string BarChartVertical = "";
            public const string LineChart        = "";
            public const string Error            = "";
            public const string BackToWindow     = "";
            public const string Important        = "";

            // ===== Filtros / Condiciones =====
            public const string Filter           = "";
            public const string FilterChecked    = ""; // CheckMark
            public const string Star             = "";
            public const string Brush            = "";
            public const string Pause            = "";
            public const string Tablet           = "";
            public const string Permissions      = "";
            public const string MirrorImage      = ""; // Reflect
            public const string Branch           = ""; // Branch (may degrade)
            public const string Trophy           = ""; // Crown
            public const string Bank             = "";
            public const string ColumnLeftTwo    = "";
            public const string ColumnRightTwo   = "";
            public const string ViewAll          = "";
            public const string Sort             = "";
            public const string Diff             = ""; // Compare
        }
    }
}
