// ARCHIVO TEMPORAL — eliminar al completar Fase 1 aprobada.
// Propósito: verificar visualmente que el tema se aplica correctamente a todos los tipos de control.

using System;
using System.Drawing;
using System.Windows.Forms;
using Free1X2.UI.Modern.Theming;

namespace Free1X2.UI.Modern
{
    public class ThemeTestForm : ModernFormBase
    {
        public ThemeTestForm()
        {
            Text        = "Theme Test — Fase 1 verificación visual";
            Size        = new Size(900, 680);
            MinimumSize = new Size(900, 680);
            StartPosition = FormStartPosition.CenterScreen;

            BuildUI();
        }

        private void BuildUI()
        {
            // ----------------------------------------------------------------
            // MenuStrip
            // ----------------------------------------------------------------
            var menu = new MenuStrip();
            var mArchivo  = new ToolStripMenuItem("Archivo");
            var mEdicion  = new ToolStripMenuItem("Edición");
            var mVer      = new ToolStripMenuItem("Ver");
            mArchivo.DropDownItems.Add("Nuevo");
            mArchivo.DropDownItems.Add("Abrir");
            mArchivo.DropDownItems.Add(new ToolStripSeparator());
            mArchivo.DropDownItems.Add("Salir");
            menu.Items.AddRange(new ToolStripItem[] { mArchivo, mEdicion, mVer });
            menu.Text = "menuStrip";
            Controls.Add(menu);
            MainMenuStrip = menu;

            // ----------------------------------------------------------------
            // ToolStrip
            // ----------------------------------------------------------------
            var toolbar = new ToolStrip();
            toolbar.Items.Add(new ToolStripButton("Nuevo"));
            toolbar.Items.Add(new ToolStripButton("Abrir"));
            toolbar.Items.Add(new ToolStripSeparator());
            toolbar.Items.Add(new ToolStripButton("Guardar"));
            toolbar.Items.Add(new ToolStripButton("Imprimir"));
            Controls.Add(toolbar);

            // ----------------------------------------------------------------
            // Main panel (below toolbars)
            // ----------------------------------------------------------------
            var main = new Panel
            {
                Dock    = DockStyle.Fill,
                Padding = new Padding(12),
            };
            Controls.Add(main);

            // ----------------------------------------------------------------
            // Left column
            // ----------------------------------------------------------------
            var left = new Panel
            {
                Width   = 280,
                Dock    = DockStyle.Left,
                Padding = new Padding(0, 0, 8, 0),
            };
            main.Controls.Add(left);

            AddGroup(left, "Entrada de datos", 0, BuildInputSection);
            AddGroup(left, "Selección", 180, BuildSelectionSection);
            AddGroup(left, "Botones", 310, BuildButtonSection);

            // ----------------------------------------------------------------
            // Right column — TabControl + DataGridView
            // ----------------------------------------------------------------
            var right = new Panel { Dock = DockStyle.Fill };
            main.Controls.Add(right);

            var tabs = new TabControl { Dock = DockStyle.Fill };
            var tabGrid  = new TabPage("DataGridView");
            var tabList  = new TabPage("ListView");
            var tabMixed = new TabPage("Controles mixtos");

            tabGrid.Controls.Add(BuildDataGridView());
            tabList.Controls.Add(BuildListView());
            tabMixed.Controls.Add(BuildMixedPanel());

            tabs.TabPages.AddRange(new[] { tabGrid, tabList, tabMixed });
            right.Controls.Add(tabs);

            // ----------------------------------------------------------------
            // StatusStrip
            // ----------------------------------------------------------------
            var status = new StatusStrip();
            status.Items.Add(new ToolStripStatusLabel("Listo"));
            status.Items.Add(new ToolStripStatusLabel { Spring = true });
            status.Items.Add(new ToolStripStatusLabel("v0.77.2"));
            Controls.Add(status);
        }

        // -- Helpers ---------------------------------------------------------

        private static void AddGroup(Panel parent, string title, int top, Action<GroupBox> populate)
        {
            var gb = new GroupBox
            {
                Text    = title,
                Left    = 0,
                Top     = top,
                Width   = parent.Width - parent.Padding.Horizontal,
                Height  = 120,
                Anchor  = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Padding = new Padding(8),
            };
            parent.Controls.Add(gb);
            populate(gb);
        }

        private static void BuildInputSection(GroupBox gb)
        {
            var lblNombre = new Label { Text = "Nombre:", Left = 8, Top = 20, Width = 70 };
            var txNombre  = new TextBox { Left = 82, Top = 17, Width = 150, Text = "Ejemplo" };
            var lblNum    = new Label { Text = "Número:", Left = 8, Top = 50, Width = 70 };
            var nud       = new NumericUpDown { Left = 82, Top = 47, Width = 100, Value = 42, Minimum = 0, Maximum = 999 };
            var lblCombo  = new Label { Text = "Opción:", Left = 8, Top = 80, Width = 70 };
            var combo     = new ComboBox { Left = 82, Top = 77, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            combo.Items.AddRange(new object[] { "Opción 1", "Opción 2", "Opción 3" });
            combo.SelectedIndex = 0;
            gb.Controls.AddRange(new Control[] { lblNombre, txNombre, lblNum, nud, lblCombo, combo });
        }

        private static void BuildSelectionSection(GroupBox gb)
        {
            var ck1 = new CheckBox { Text = "Activar filtro A",  Left = 8, Top = 20, Width = 200, Checked = true };
            var ck2 = new CheckBox { Text = "Activar filtro B",  Left = 8, Top = 44 };
            var rb1 = new RadioButton { Text = "Modo rápido",    Left = 8, Top = 68, Width = 120, Checked = true };
            var rb2 = new RadioButton { Text = "Modo completo",  Left = 130, Top = 68 };
            gb.Controls.AddRange(new Control[] { ck1, ck2, rb1, rb2 });
        }

        private static void BuildButtonSection(GroupBox gb)
        {
            var btnOk     = new Button { Text = "Aceptar",    Left = 8,  Top = 20, Width = 90, Height = 30 };
            var btnCancel = new Button { Text = "Cancelar",   Left = 104, Top = 20, Width = 90, Height = 30 };
            var btnApply  = new Button { Text = "Aplicar",    Left = 8,  Top = 56, Width = 90, Height = 30 };
            var btnReset  = new Button { Text = "Restablecer", Left = 104, Top = 56, Width = 90, Height = 30 };

            // Primary button — set via Tag so theme knows
            btnOk.Tag = "primary";
            btnOk.BackColor = ModernTheme.Colors.Primary;
            btnOk.ForeColor = ModernTheme.Colors.TextOnPrimary;
            btnOk.FlatAppearance.BorderColor = ModernTheme.Colors.Primary;

            gb.Controls.AddRange(new Control[] { btnOk, btnCancel, btnApply, btnReset });
        }

        private static Control BuildDataGridView()
        {
            var dg = new DataGridView
            {
                Dock               = DockStyle.Fill,
                AllowUserToAddRows = false,
            };
            dg.Columns.Add("Jornada", "Jornada");
            dg.Columns.Add("Local",   "Local");
            dg.Columns.Add("Visit",   "Visitante");
            dg.Columns.Add("Prono",   "Pronóstico");
            dg.Columns.Add("Resultado", "Resultado");

            var data = new[]
            {
                new[] { "1", "Real Madrid",   "Barcelona",    "1",  "1" },
                new[] { "2", "Atlético",       "Sevilla",      "X",  "2" },
                new[] { "3", "Valencia",       "Betis",        "2",  "X" },
                new[] { "4", "Athletic",       "Villarreal",   "1",  "1" },
                new[] { "5", "Getafe",         "Osasuna",      "X",  "X" },
                new[] { "6", "Celta",          "Rayo",         "2",  "2" },
                new[] { "7", "Mallorca",       "Girona",       "1",  "1" },
            };
            foreach (var row in data)
                dg.Rows.Add(row);

            return dg;
        }

        private static Control BuildListView()
        {
            var lv = new ListView
            {
                Dock  = DockStyle.Fill,
                View  = View.Details,
                FullRowSelect = true,
                GridLines     = true,
            };
            lv.Columns.Add("Equipo",     150);
            lv.Columns.Add("Puntos",      70);
            lv.Columns.Add("PG",          50);
            lv.Columns.Add("PP",          50);
            lv.Columns.Add("GF",          50);

            var items = new[]
            {
                new[] { "Real Madrid",  "70", "22", "4", "68" },
                new[] { "Barcelona",    "65", "20", "5", "61" },
                new[] { "Atlético",     "58", "17", "7", "44" },
                new[] { "Athletic",     "52", "15", "7", "41" },
                new[] { "Villarreal",   "48", "14", "6", "40" },
            };
            foreach (var r in items)
                lv.Items.Add(new ListViewItem(r));

            return lv;
        }

        private static Control BuildMixedPanel()
        {
            var p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };

            var lbl = new Label
            {
                Text     = "Panel de estado del análisis",
                Font     = ModernTheme.Fonts.Header,
                AutoSize = true,
                Left     = 8,
                Top      = 8,
            };

            var rtb = new RichTextBox
            {
                Left   = 8,
                Top    = 40,
                Width  = 350,
                Height = 120,
                Text   = "Resultado del análisis:\r\n\r\n• Columnas generadas: 128\r\n• Filtros aplicados: 6\r\n• Reducción: 87.5%",
            };

            var listBox = new ListBox { Left = 8, Top = 175, Width = 200, Height = 120 };
            listBox.Items.AddRange(new object[] { "Filtro contactos", "Filtro simetrías", "Filtro figuras", "Filtro distancias", "Filtro pesos" });

            p.Controls.AddRange(new Control[] { lbl, rtb, listBox });
            return p;
        }
    }
}
