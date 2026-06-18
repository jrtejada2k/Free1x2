// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Free1X2.WinUI.Controls.Analisis
{
    /// <summary>
    /// Rejilla nativa que muestra una <see cref="MatrizAnalisis"/> (cabecera de índices +
    /// filas de conceptos). Sustituye al volcado de texto plano previo del visor de análisis
    /// por la rejilla alineada equivalente a los UserControls CtrlAnalisis* del WinForms.
    /// </summary>
    public sealed partial class MatrizAnalisisView : UserControl
    {
        public MatrizAnalisisView()
        {
            this.InitializeComponent();
        }

        /// <summary>Matriz a representar. Al asignarla se vuelca en la rejilla.</summary>
        public MatrizAnalisis? Matriz
        {
            get => (MatrizAnalisis?)GetValue(MatrizProperty);
            set => SetValue(MatrizProperty, value);
        }

        public static readonly DependencyProperty MatrizProperty =
            DependencyProperty.Register(
                nameof(Matriz),
                typeof(MatrizAnalisis),
                typeof(MatrizAnalisisView),
                new PropertyMetadata(null, OnMatrizChanged));

        private static void OnMatrizChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MatrizAnalisisView view)
            {
                view.FilasHost.ItemsSource = (e.NewValue as MatrizAnalisis)?.Filas;
            }
        }

        /// <summary>
        /// Fondo de cada celda según sea cabecera (índice) o dato. Reemplaza el
        /// Color.NavajoWhite de las casillas de índice del WinForms por un token del tema.
        /// </summary>
        public static Brush FondoCelda(bool esCabecera)
        {
            string clave = esCabecera ? "AppSurfaceAltBrush" : "AppSurfaceBrush";
            if (Application.Current.Resources.TryGetValue(clave, out var recurso) && recurso is Brush brush)
            {
                return brush;
            }
            return new SolidColorBrush(Microsoft.UI.Colors.Transparent);
        }
    }
}
