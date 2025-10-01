// Free1X2 - Enhanced Control Compatibility Layer for .NET Migration
// This file provides comprehensive compatibility aliases for legacy Windows Forms controls
// to ease the migration from .NET Framework 2.0 to .NET 8

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.Threading.Tasks;

// ===== WEB SERVICE COMPATIBILITY STUBS =====
namespace Free1X2
{
    // Web service compatibility stubs for missing references
    public class Free1X2WService
    {
        public string Url { get; set; }
        public ICredentials Credentials { get; set; }
        public int Timeout { get; set; } = 30000;
        
        public virtual Task<object> GetDataAsync() => Task.FromResult<object>(null);
        public virtual object GetData() => null;
        public virtual NotificacionFree1x2[] ObtenerNotificaciones() => new NotificacionFree1x2[0];
        public virtual NotificacionFree1x2[] ObtenerNotificaciones(DateTime fecha) => new NotificacionFree1x2[0];
        public virtual NotificacionFree1x2[] ObtenerNotificaciones(DateTime fecha, string extra) => new NotificacionFree1x2[0];
        public virtual string UltimaVersionFree1X2() => "1.0.0";
        public virtual JornadaActual ObtenerJornadaActual() => new JornadaActual();
        public virtual string ObtenerBoleto(int jornada, string temporada) => string.Empty;
    }
    
    public class SVC_Actualizador : Free1X2WService
    {
        public virtual bool CheckForUpdates() => false;
        public virtual string GetLatestVersion() => "1.0.0";
        public virtual byte[] DownloadUpdate() => new byte[0];
    }
    
    public class NotificacionFree1x2
    {
        public int IdNotificacion { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public string Remitente { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public DateTime FechaCaducidad { get; set; } = DateTime.Now.AddDays(30);
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public bool Leida { get; set; } = false;
        public bool Borrada { get; set; } = false;
    }    public class JornadaActual
    {
        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = "Activa";
    public bool EsValida => Numero > 0;
        
    public string P1 { get; set; }
    public string P2 { get; set; }
    public string P3 { get; set; }
    public string P4 { get; set; }
    public string P5 { get; set; }
    public string P6 { get; set; }
    public string P7 { get; set; }
    public string P8 { get; set; }
    public string P9 { get; set; }
    public string P10 { get; set; }
    public string P11 { get; set; }
    public string P12 { get; set; }
    public string P13 { get; set; }
    public string P14 { get; set; }
    public string P15 { get; set; }
    }
    
    // CRC32 compatibility stub for ICSharpCode.SharpZipLib
    public class Crc32
    {
        private uint _value = 0xFFFFFFFF;
        
        public void Reset() => _value = 0xFFFFFFFF;
        public void Update(byte[] buffer) => Update(buffer, 0, buffer.Length);
        public void Update(byte[] buffer, int offset, int count)
        {
            // Simple CRC32 implementation stub
            for (int i = offset; i < offset + count; i++)
            {
                _value = (_value >> 8) ^ CrcTable[(_value ^ buffer[i]) & 0xFF];
            }
        }
        
        public uint Value => _value ^ 0xFFFFFFFF;
        
        private static readonly uint[] CrcTable = new uint[256];
        
        static Crc32()
        {
            // Initialize CRC table
            for (uint i = 0; i < 256; i++)
            {
                uint crc = i;
                for (int j = 8; j > 0; j--)
                {
                    if ((crc & 1) == 1)
                        crc = (crc >> 1) ^ 0xEDB88320;
                    else
                        crc >>= 1;
                }
                CrcTable[i] = crc;
            }
        }
    }
}

namespace System.Windows.Forms
{
    // ===== DATAGRID COMPATIBILITY =====
    
    // Legacy DataGrid compatibility - comprehensive wrapper around DataGridView
    public class DataGrid : DataGridView
    {
        private readonly DataGridTableStyleCollection _tableStyles;
        private readonly List<DataGridColumnStyle> _columnStyles;
        private object _dataSource;
        private string _dataMember;
        
        public DataGrid() : base()
        {
            // Basic compatibility settings to mimic old DataGrid behavior
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToResizeRows = false;
            this.RowHeadersVisible = false;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.MultiSelect = false;
            this.ReadOnly = true;
            this.BorderStyle = BorderStyle.Fixed3D;
            
            _tableStyles = new DataGridTableStyleCollection();
            _columnStyles = new List<DataGridColumnStyle>();
        }

        // Legacy DataGrid Properties
        public bool AllowSorting { get; set; } = true;
        public bool CaptionVisible { get; set; } = true;
        public string CaptionText { get; set; } = "";
        public Color CaptionBackColor { get; set; } = SystemColors.ActiveCaption;
        public Color CaptionForeColor { get; set; } = SystemColors.ActiveCaptionText;
        public Font CaptionFont { get; set; } = SystemFonts.CaptionFont;
        public Color HeaderForeColor { get; set; } = SystemColors.ControlText;
        public Color HeaderBackColor { get; set; } = SystemColors.Control;
        public Font HeaderFont { get; set; } = SystemFonts.DefaultFont;
        public bool FlatMode { get; set; } = false;
        public Color GridLineColor { get; set; } = SystemColors.Control;
        public Color LinkColor { get; set; } = Color.Blue;
        public Color ParentRowsBackColor { get; set; } = SystemColors.Control;
        public Color ParentRowsForeColor { get; set; } = SystemColors.WindowText;
        public int PreferredColumnWidth { get; set; } = 75;
        public int RowHeaderWidth { get; set; } = 35;
        public Color SelectionBackColor { get; set; } = SystemColors.Highlight;
        public Color SelectionForeColor { get; set; } = SystemColors.HighlightText;
        public Color AlternatingBackColor { get; set; } = SystemColors.Window;

        // Legacy collection properties
        public DataGridTableStyleCollection TableStyles => _tableStyles;

        // Legacy methods
        public void SetDataBinding(object dataSource, string dataMember)
        {
            _dataSource = dataSource;
            _dataMember = dataMember;
            
            if (dataSource is DataSet ds && !string.IsNullOrEmpty(dataMember))
            {
                this.DataSource = ds.Tables[dataMember];
            }
            else
            {
                this.DataSource = dataSource;
            }
        }

        public bool IsSelected(int row)
        {
            if (row >= 0 && row < this.Rows.Count)
            {
                return this.Rows[row].Selected;
            }
            return false;
        }

        public void Select(int row)
        {
            if (row >= 0 && row < this.Rows.Count)
            {
                this.ClearSelection();
                this.Rows[row].Selected = true;
                this.CurrentCell = this.Rows[row].Cells[0];
            }
        }
        
        // Legacy CurrentCell support using DataGridCell
        private DataGridCell _currentDataGridCell = new DataGridCell(0, 0);
        public new DataGridCell CurrentCell
        {
            get => _currentDataGridCell;
            set
            {
                _currentDataGridCell = value;
                if (value != null && value.RowNumber >= 0 && value.RowNumber < this.Rows.Count &&
                    value.ColumnNumber >= 0 && value.ColumnNumber < this.Columns.Count)
                {
                    base.CurrentCell = this.Rows[value.RowNumber].Cells[value.ColumnNumber];
                }
            }
        }

        public void UnSelect(int row)
        {
            if (row >= 0 && row < this.Rows.Count)
            {
                this.Rows[row].Selected = false;
            }
        }

        public Rectangle GetCellBounds(int row, int col)
        {
            if (row >= 0 && row < this.Rows.Count && col >= 0 && col < this.Columns.Count)
            {
                return this.GetCellDisplayRectangle(col, row, false);
            }
            return Rectangle.Empty;
        }

        public new HitTestInfo HitTest(int x, int y)
        {
            var modernInfo = base.HitTest(x, y);
            
            // Convert modern hit test to legacy format
            int legacyTypeInt = 0;
            switch (modernInfo.Type)
            {
                case DataGridViewHitTestType.Cell:
                    legacyTypeInt = HitTestType.Cell;
                    break;
                case DataGridViewHitTestType.ColumnHeader:
                    legacyTypeInt = HitTestType.ColumnHeader;
                    break;
                case DataGridViewHitTestType.RowHeader:
                    legacyTypeInt = HitTestType.RowHeader;
                    break;
            }
            
            return new HitTestInfo(modernInfo.RowIndex, modernInfo.ColumnIndex, legacyTypeInt);
        }

        // DataGrid cell indexer compatibility
        public new object this[int row, int col]
        {
            get
            {
                if (row >= 0 && row < this.Rows.Count && col >= 0 && col < this.Columns.Count)
                {
                    return this.Rows[row].Cells[col].Value;
                }
                return null;
            }
            set
            {
                if (row >= 0 && row < this.Rows.Count && col >= 0 && col < this.Columns.Count)
                {
                    this.Rows[row].Cells[col].Value = value;
                }
            }
        }
        
        // Legacy HitTest compatibility
        public HitTestInfo HitTest(Point point)
        {
            var modernInfo = base.HitTest(point.X, point.Y);
            
            // Convert modern hit test to legacy format
            int legacyTypeInt = 0;
            switch (modernInfo.Type)
            {
                case DataGridViewHitTestType.Cell:
                    legacyTypeInt = HitTestType.Cell;
                    break;
                case DataGridViewHitTestType.ColumnHeader:
                    legacyTypeInt = HitTestType.ColumnHeader;
                    break;
                case DataGridViewHitTestType.RowHeader:
                    legacyTypeInt = HitTestType.RowHeader;
                    break;
            }
            
            return new HitTestInfo(modernInfo.RowIndex, modernInfo.ColumnIndex, legacyTypeInt);
        }
        
        // Nested HitTestType enum matching .NET Framework 2.0 DataGrid pattern
        public static class HitTestType
        {
            public const int None = 0;
            public const int Cell = 1;
            public const int ColumnHeader = 2;
            public const int RowHeader = 3;
            public const int ColumnResize = 4;
            public const int RowResize = 5;
            public const int Caption = 6;
            public const int ParentRows = 7;
        }

        // Nested HitTestInfo class matching .NET Framework 2.0 DataGrid pattern
        public class HitTestInfo
        {
            private readonly int _row;
            private readonly int _column;
            private readonly int _type;
            
            public HitTestInfo(int row, int column, int type)
            {
                _row = row;
                _column = column;
                _type = type;
            }
            
            public HitTestInfo() : this(-1, -1, 0) { }
            
            public int Row => _row;
            public int Column => _column;
            public int Type => _type;
            
            public static bool operator ==(HitTestInfo left, int right)
            {
                return left?.Type == right;
            }
            
            public static bool operator !=(HitTestInfo left, int right)
            {
                return !(left == right);
            }
            
            public override bool Equals(object obj)
            {
                if (obj is int type) return Type == type;
                if (obj is HitTestInfo info) return Type == info.Type && Row == info.Row && Column == info.Column;
                return false;
            }
            
            public override int GetHashCode() => HashCode.Combine(Type, Row, Column);
        }
    }
    
    // DataGrid Cell compatibility
    public class DataGridCell
    {
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        
        public DataGridCell(int row, int column)
        {
            RowNumber = row;
            ColumnNumber = column;
        }
        
        // Implicit conversion from DataGridViewCell
        public static implicit operator DataGridCell(DataGridViewCell cell)
        {
            return new DataGridCell(cell?.RowIndex ?? -1, cell?.ColumnIndex ?? -1);
        }
    }

    // Legacy DataGrid style classes
    public class DataGridTableStyleCollection : ICollection<DataGridTableStyle>
    {
        private readonly List<DataGridTableStyle> _items = new List<DataGridTableStyle>();

        public void Add(DataGridTableStyle item) => _items.Add(item);
        public void AddRange(DataGridTableStyle[] items) => _items.AddRange(items);
        public void Clear() => _items.Clear();
        public bool Contains(DataGridTableStyle item) => _items.Contains(item);
        public void CopyTo(DataGridTableStyle[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
        public IEnumerator<DataGridTableStyle> GetEnumerator() => _items.GetEnumerator();
        public bool Remove(DataGridTableStyle item) => _items.Remove(item);
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
        public int Count => _items.Count;
        public bool IsReadOnly => false;
    }

    public class DataGridTableStyle
    {
        public string MappingName { get; set; } = "";
        public bool AllowSorting { get; set; } = true;
        public bool ColumnHeadersVisible { get; set; } = true;
        public Color AlternatingBackColor { get; set; } = SystemColors.Window;
        public Color BackColor { get; set; } = SystemColors.Window;
        public Color ForeColor { get; set; } = SystemColors.WindowText;
        public Color GridLineColor { get; set; } = SystemColors.Control;
        public Color HeaderBackColor { get; set; } = SystemColors.Control;
        public Color HeaderForeColor { get; set; } = SystemColors.ControlText;
        public Color SelectionBackColor { get; set; } = SystemColors.Highlight;
        public Color SelectionForeColor { get; set; } = SystemColors.HighlightText;
        public DataGrid DataGrid { get; set; }
        
        public DataGridColumnStyleCollection GridColumnStyles { get; } = new DataGridColumnStyleCollection();
    }

    public class DataGridColumnStyleCollection : ICollection<DataGridColumnStyle>
    {
        private readonly List<DataGridColumnStyle> _items = new List<DataGridColumnStyle>();

        public void Add(DataGridColumnStyle item) => _items.Add(item);
        public void AddRange(DataGridColumnStyle[] items) => _items.AddRange(items);
        public void Clear() => _items.Clear();
        public bool Contains(DataGridColumnStyle item) => _items.Contains(item);
        public void CopyTo(DataGridColumnStyle[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
        public IEnumerator<DataGridColumnStyle> GetEnumerator() => _items.GetEnumerator();
        public bool Remove(DataGridColumnStyle item) => _items.Remove(item);
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
        public int Count => _items.Count;
        public bool IsReadOnly => false;
    }

    public abstract class DataGridColumnStyle
    {
        public string MappingName { get; set; } = "";
        public string HeaderText { get; set; } = "";
        public int Width { get; set; } = 100;
        public bool ReadOnly { get; set; } = false;
        public string NullText { get; set; } = "";
    }

    public class DataGridTextBoxColumn : DataGridColumnStyle
    {
        public HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;
        public string Format { get; set; } = "";
        public IFormatProvider FormatInfo { get; set; } = null;
    }

    public class DataGridBoolColumn : DataGridColumnStyle
    {
        public HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Center;
        public object TrueValue { get; set; } = true;
        public object FalseValue { get; set; } = false;
        public object NullValue { get; set; } = DBNull.Value;
    }

    // ===== STATUSBAR COMPATIBILITY =====
    
    public class StatusBar : StatusStrip
    {
        private readonly StatusBarPanelCollection _panels;
        
        public StatusBar() : base()
        {
            _panels = new StatusBarPanelCollection();
        }

        public bool ShowPanels { get; set; } = true;
        public StatusBarPanelCollection Panels => _panels;
    }

    public class StatusBarPanelCollection : ICollection<StatusBarPanel>
    {
        private readonly List<StatusBarPanel> _items = new List<StatusBarPanel>();

        public void Add(StatusBarPanel item) => _items.Add(item);
        public void AddRange(StatusBarPanel[] items) => _items.AddRange(items);
        public void Clear() => _items.Clear();
        public bool Contains(StatusBarPanel item) => _items.Contains(item);
        public void CopyTo(StatusBarPanel[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
        public IEnumerator<StatusBarPanel> GetEnumerator() => _items.GetEnumerator();
        public bool Remove(StatusBarPanel item) => _items.Remove(item);
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
        public int Count => _items.Count;
        public bool IsReadOnly => false;

        public StatusBarPanel this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }
    }

    public class StatusBarPanel
    {
        public string Name { get; set; } = "";
        public string Text { get; set; } = "";
        public string ToolTipText { get; set; } = "";
        public int Width { get; set; } = 100;
        public Icon Icon { get; set; }
        public StatusBarPanelAutoSize AutoSize { get; set; } = StatusBarPanelAutoSize.None;
        public StatusBarPanelBorderStyle BorderStyle { get; set; } = StatusBarPanelBorderStyle.Sunken;
        public StatusBarPanelStyle Style { get; set; } = StatusBarPanelStyle.Text;
    }

    public enum StatusBarPanelAutoSize
    {
        None,
        Spring,
        Contents
    }

    public enum StatusBarPanelBorderStyle
    {
        None,
        Sunken,
        Raised
    }

    public enum StatusBarPanelStyle
    {
        Text,
        OwnerDraw
    }

    // ===== TOOLBAR COMPATIBILITY =====

    public class ToolBar : ToolStrip
    {
        private readonly ToolBarButtonCollection _buttons;
        
        public ToolBar() : base()
        {
            _buttons = new ToolBarButtonCollection();
        }

        public ToolBarAppearance Appearance { get; set; } = ToolBarAppearance.Normal;
        public Size ButtonSize { get; set; } = new Size(24, 22);
        public bool Divider { get; set; } = true;
        public bool DropDownArrows { get; set; } = false;
        public bool ShowToolTips { get; set; } = false;
        public ToolBarTextAlign TextAlign { get; set; } = ToolBarTextAlign.Underneath;
        public ToolBarButtonCollection Buttons => _buttons;

        public event ToolBarButtonClickEventHandler ButtonClick;

        protected virtual void OnButtonClick(ToolBarButtonClickEventArgs e)
        {
            ButtonClick?.Invoke(this, e);
        }
    }

    public class ToolBarButtonCollection : ICollection<ToolBarButton>
    {
        private readonly List<ToolBarButton> _items = new List<ToolBarButton>();

        public void Add(ToolBarButton item) => _items.Add(item);
        public void AddRange(ToolBarButton[] items) => _items.AddRange(items);
        public void Clear() => _items.Clear();
        public bool Contains(ToolBarButton item) => _items.Contains(item);
        public void CopyTo(ToolBarButton[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
        public IEnumerator<ToolBarButton> GetEnumerator() => _items.GetEnumerator();
        public bool Remove(ToolBarButton item) => _items.Remove(item);
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
        public int Count => _items.Count;
        public bool IsReadOnly => false;

        public ToolBarButton this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public int IndexOf(ToolBarButton button) => _items.IndexOf(button);
    }

    public class ToolBarButton
    {
        public string Name { get; set; } = "";
        public string Text { get; set; } = "";
        public string ToolTipText { get; set; } = "";
        public int ImageIndex { get; set; } = -1;
        public ToolBarButtonStyle Style { get; set; } = ToolBarButtonStyle.PushButton;
        public bool Enabled { get; set; } = true;
        public bool Visible { get; set; } = true;
        public object Tag { get; set; }
        public Rectangle Rectangle { get; set; } = Rectangle.Empty;
    }

    public enum ToolBarAppearance
    {
        Normal,
        Flat
    }

    public enum ToolBarTextAlign
    {
        Underneath,
        Right
    }

    public enum ToolBarButtonStyle
    {
        PushButton,
        ToggleButton,
        Separator,
        DropDownButton
    }

    public delegate void ToolBarButtonClickEventHandler(object sender, ToolBarButtonClickEventArgs e);

    public class ToolBarButtonClickEventArgs : EventArgs
    {
        public ToolBarButton Button { get; }
        
        public ToolBarButtonClickEventArgs(ToolBarButton button)
        {
            Button = button;
        }
    }

    // ===== CONTEXT MENU COMPATIBILITY =====

    public class ContextMenu : ContextMenuStrip
    {
        private readonly MenuItemCollection _menuItems;
        
        public ContextMenu() : base()
        {
            _menuItems = new MenuItemCollection();
        }
        
        public MenuItemCollection MenuItems => _menuItems;
    }

    // ===== MAINMENU COMPATIBILITY =====

    public class MainMenu : MenuStrip
    {
        private readonly MenuItemCollection _menuItems;
        
        public MainMenu() : base()
        {
            _menuItems = new MenuItemCollection();
        }

        public MainMenu(MenuItem[] items) : base()
        {
            _menuItems = new MenuItemCollection();
            if (items != null)
                _menuItems.AddRange(items);
        }
        
        public MenuItemCollection MenuItems => _menuItems;
    }

    public class MenuItemCollection : ICollection<MenuItem>
    {
        private readonly List<MenuItem> _items = new List<MenuItem>();

        public void Add(MenuItem item) => _items.Add(item);
        public void AddRange(MenuItem[] items) => _items.AddRange(items);
        public void Clear() => _items.Clear();
        public bool Contains(MenuItem item) => _items.Contains(item);
        public void CopyTo(MenuItem[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
        public IEnumerator<MenuItem> GetEnumerator() => _items.GetEnumerator();
        public bool Remove(MenuItem item) => _items.Remove(item);
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
        public int Count => _items.Count;
        public bool IsReadOnly => false;

        public MenuItem this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }
    }

    public class MenuItem : ToolStripMenuItem
    {
        private readonly MenuItemCollection _menuItems;
        
        public MenuItem() : base()
        {
            _menuItems = new MenuItemCollection();
        }

        public MenuItem(string text) : base(text)
        {
            _menuItems = new MenuItemCollection();
        }

        public bool DefaultItem { get; set; } = false;
        public bool RadioCheck { get; set; } = false;
        public int Index { get; set; } = -1;
        public MenuItemCollection MenuItems => _menuItems;
    }
    
    // ===== FORM COMPATIBILITY EXTENSIONS =====
    
    public static class FormExtensions
    {
        private static readonly Dictionary<Form, MainMenu> _formMenus = new Dictionary<Form, MainMenu>();
        
        public static MainMenu GetMenu(this Form form)
        {
            _formMenus.TryGetValue(form, out var menu);
            return menu;
        }
        
        public static void SetMenu(this Form form, MainMenu menu)
        {
            if (_formMenus.ContainsKey(form))
                _formMenus[form] = menu;
            else
                _formMenus.Add(form, menu);
                
            // Set the actual MenuStrip
            form.MainMenuStrip = menu;
        }
        
        // Legacy Menu property support through reflection
        public static MainMenu Menu(this Form form)
        {
            return GetMenu(form);
        }
    }
    
    // ===== WEB SERVICE COMPATIBILITY LAYER =====
    
    public class Free1X2WService
    {
        public NotificacionFree1x2[] ObtenerNotificaciones(DateTime fecha)
        {
            // Stub implementation for offline mode
            return new NotificacionFree1x2[0];
        }
        
        public JornadaActual ObtenerJornadaActual()
        {
            // Stub implementation
            return new JornadaActual { Jornada = 1, Temporada = "2024-25" };
        }
        
        public string ObtenerBoleto(int jornada, string temporada)
        {
            // Stub implementation for offline mode
            return string.Empty;
        }
        
        public void AlmacenarInformeError(string info, string objetoCausante, string comentarios, string version, string user, string email)
        {
            // Stub implementation for offline mode - no actual storage
        }
    }
    
    public class NotificacionFree1x2
    {
        public string ID { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Tipo { get; set; } = string.Empty;
        public bool Leida { get; set; } = false;
    }
    
    public class JornadaActual
    {
        public int Jornada { get; set; } = 1;
        public string Temporada { get; set; } = "2024-25";
    }
    
    // SVC_Actualizador namespace for compatibility
    namespace SVC_Actualizador
    {
        public class Free1X2WServiceSoapClient
        {
            public enum EndpointConfiguration
            {
                Free1X2WServiceSoap
            }
            
            public NotificacionFree1x2[] ObtenerNotificaciones(DateTime fecha)
            {
                return new NotificacionFree1x2[0];
            }
            
            public NotificacionFree1x2[] ObtenerNotificaciones(DateTime fecha, string extra)
            {
                return new NotificacionFree1x2[0];
            }
        }
    }
}