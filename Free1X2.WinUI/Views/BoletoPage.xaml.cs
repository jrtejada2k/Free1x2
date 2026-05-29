using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;

namespace Free1X2.WinUI.Views;

public sealed partial class BoletoPage : Page
{
    private const int NumPartidos = 14;

    public BoletoPage()
    {
        this.InitializeComponent();
        ConstruirPartidos();
    }

    // Construye 14 filas de partido, cada una con tres ToggleButton 1 / X / 2.
    // (Slice de prueba del patrón; en la migración real esto será un control con MVVM
    //  alimentado por la lógica de dominio existente.)
    private void ConstruirPartidos()
    {
        for (int i = 1; i <= NumPartidos; i++)
        {
            var row = new Grid { ColumnSpacing = 6, Padding = new Thickness(0, 2, 0, 2) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var num = new TextBlock
            {
                Text = i.ToString("00"),
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = (Brush)Application.Current.Resources["AppTextSecondaryBrush"]
            };
            Grid.SetColumn(num, 0);
            row.Children.Add(num);

            AddSigno(row, 1, "1");
            AddSigno(row, 2, "X");
            AddSigno(row, 3, "2");

            PartidosPanel.Children.Add(row);
        }
        ActualizarResumen();
    }

    private void AddSigno(Grid row, int col, string signo)
    {
        var tb = new ToggleButton
        {
            Content = signo,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            CornerRadius = new CornerRadius(6),
            MinWidth = 0
        };
        tb.Checked   += (_, _) => ActualizarResumen();
        tb.Unchecked += (_, _) => ActualizarResumen();
        Grid.SetColumn(tb, col);
        row.Children.Add(tb);
    }

    private void ActualizarResumen()
    {
        int fijos = 0, dobles = 0, triples = 0;
        foreach (var child in PartidosPanel.Children)
        {
            if (child is not Grid g) continue;
            int marcados = 0;
            foreach (var c in g.Children)
                if (c is ToggleButton t && t.IsChecked == true) marcados++;
            switch (marcados)
            {
                case 1: fijos++;   break;
                case 2: dobles++;  break;
                case 3: triples++; break;
            }
        }
        ResumenTexto.Text = $"Fijos: {fijos} · Dobles: {dobles} · Triples: {triples}";
    }
}
