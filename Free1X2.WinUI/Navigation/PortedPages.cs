using System;
using System.Collections.Generic;
using Free1X2.WinUI.Views.Ported;

namespace Free1X2.WinUI.Navigation;

/// <summary>Una pantalla portada desde WinForms, para la navegación data-driven.</summary>
public record PortedPage(string Title, string Glyph, Type PageType, string Category);

/// <summary>
/// Registro de pantallas portadas a WinUI 3. Cada ola de migración añade entradas aquí
/// y el NavigationView se puebla solo — sin tocar el XAML del shell.
/// </summary>
public static class PortedPagesRegistry
{
    public static readonly IReadOnlyList<PortedPage> All = new[]
    {
        // ===== Ola 1 =====
        new PortedPage("Configuración",       "", typeof(ConfiguracionFrmPage),         "Ajustes"),
        new PortedPage("Configurar análisis", "", typeof(ConfiguracionAnalisisFrmPage), "Ajustes"),
        new PortedPage("Añadir Pleno al 15",  "", typeof(AgregaP15FrmPage),             "Combinación"),
        new PortedPage("Cambiar puntuación",  "", typeof(CambioPuntosFrmPage),          "Utilidades"),
        new PortedPage("Filtro: Distancias",  "", typeof(DistanciasFrmPage),            "Filtros"),
        new PortedPage("Filtro: Dibujos",     "", typeof(DibujosFrmPage),               "Filtros"),
        new PortedPage("Filtro: Formatos",    "", typeof(FormatosFrmPage),              "Filtros"),
        new PortedPage("Filtro: Contactos",   "", typeof(ContactosFrmPage),             "Filtros"),
        new PortedPage("Acerca de",           "", typeof(AcercaDeFrmPage),              "Ayuda"),
        new PortedPage("Créditos",            "", typeof(CreditosFrmPage),              "Ayuda"),
    };
}
