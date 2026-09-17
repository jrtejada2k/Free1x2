// Free1X2 · WinUI 3 — WIN3
using System.Collections.Generic;

namespace Free1X2.WinUI.Services;

/// <summary>
/// Opción de país para los selectores de la integración online (texto visible + código "es"/"mx",
/// el mismo que espera <see cref="QuinielaOnlineService"/>).
/// <c>ToString()</c> devuelve el nombre porque los ComboBox de las páginas muestran el elemento
/// directamente, sin DataTemplate.
/// </summary>
public sealed record OpcionPais(string Nombre, string Codigo)
{
    public override string ToString() => Nombre;
}

/// <summary>
/// Catálogo compartido de países de la integración online. Antes (C-10) el record y la lista
/// estaban DUPLICADOS en <c>DescargaBoletoFrmViewModel</c> y <c>GestorEquiposFrmViewModel</c>:
/// añadir un país obligaba a tocar los dos y podían divergir.
/// </summary>
public static class PaisesOnline
{
    /// <summary>Países disponibles (España / México). Diseño aprobado; orden: España primero.</summary>
    public static IReadOnlyList<OpcionPais> Todos { get; } = new List<OpcionPais>
    {
        new OpcionPais("España", "es"),
        new OpcionPais("México", "mx"),
    };

    /// <summary>País por defecto: España, la quiniela "clásica" de la app.</summary>
    public static OpcionPais PorDefecto => Todos[0];

    /// <summary>
    /// Devuelve la opción cuyo código coincide con <paramref name="codigo"/> (ignorando may/min),
    /// o <see cref="PorDefecto"/> si no hay coincidencia.
    /// </summary>
    public static OpcionPais PorCodigo(string? codigo)
    {
        if (!string.IsNullOrWhiteSpace(codigo))
        {
            foreach (var op in Todos)
            {
                if (string.Equals(op.Codigo, codigo, System.StringComparison.OrdinalIgnoreCase))
                    return op;
            }
        }
        return PorDefecto;
    }
}
