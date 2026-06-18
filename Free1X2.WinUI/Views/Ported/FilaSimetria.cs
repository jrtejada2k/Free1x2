using CommunityToolkit.Mvvm.ComponentModel;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Entrada editable de una simetría (equivale a un CtrlSimetria con su LblNum + TxtSimetria).
///
/// Se define como clase pública de nivel superior (no anidada) para que el
/// compilador XAML pueda referenciarla con <c>x:DataType="vm:FilaSimetria"</c>
/// dentro del DataTemplate; el syntax de clase anidada (<c>ViewModel+FilaSimetria</c>)
/// genera código .g.cs inválido.
/// </summary>
public partial class FilaSimetria : ObservableObject
{
    public FilaSimetria(int numero) => Numero = numero;

    // Número de orden mostrado (LblNum del CtrlSimetria legacy).
    public int Numero { get; }

    // Etiqueta string para el TextBlock (regla anti-crash 2: no bindear int directo a Text).
    public string NumeroTexto => Numero.ToString();

    // Texto de la simetría: partidos separados por comas/guiones (TxtSimetria legacy).
    [ObservableProperty]
    private string _partidos = string.Empty;
}
