using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Port de WinForms "CreditosFrm" (categoría Ayuda).
    /// Pantalla de créditos / "Acerca de" del equipo Free1X2: muestra el título del
    /// proyecto, la lista de colaboradores con su enlace de contacto (email o web) y
    /// una nota de agradecimiento a los usuarios.
    ///
    /// Esta pantalla es puramente informativa: no recibe parámetros de entrada ni
    /// ejecuta lógica de dominio, por lo que NO necesita ViewModel.
    ///
    /// TODO (dominio): si en el futuro se quiere mostrar versión/build dinámicos
    /// (p.ej. "0.77.2 Rarotonga"), obtenerlos del servicio de aplicación legacy
    /// correspondiente cuando exista en Free1X2.Domain (no implementado aún).
    /// </summary>
    public sealed partial class CreditosFrmPage : Page
    {
        /// <summary>Colaboradores mostrados en la tarjeta de créditos.</summary>
        public IReadOnlyList<CreditoEntrada> Creditos { get; } = new List<CreditoEntrada>
        {
            new("Luis Fernández", "lfernandezrodriguez@gmail.com", isEmail: true),
            new("Morrison", null, isEmail: false),
            new("Toni Moreno", "toni@moreno-csa.com", isEmail: true),
            new("Joan Duatis", "duatis@coac.net", isEmail: true),
            new("Indeciso", "jjchild@lycos.co.uk", isEmail: true),
            new("JVallespin", "jvallespin@gmail.com", isEmail: true),
            new("Xfsf", "segura33@ono.com", isEmail: true),
            new("ArDoC", "ardock@gmail.com", isEmail: true),
            new("Juan Tejada", "elrevisor2k@gmail.com", isEmail: true),
        };

        public CreditosFrmPage()
        {
            InitializeComponent();
        }

        private async void OnWebsiteClick(object sender, RoutedEventArgs e)
        {
            // Equivale a lnkEquipoFree_LinkClicked: abre el sitio del proyecto.
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.free1x2.com"));
        }
    }

    /// <summary>
    /// Entrada de la lista de créditos. Cada colaborador puede tener un contacto
    /// por email (mailto:) o ninguno (texto plano).
    /// </summary>
    public sealed class CreditoEntrada
    {
        public CreditoEntrada(string nombre, string? contacto, bool isEmail)
        {
            Nombre = nombre;
            Contacto = contacto ?? string.Empty;
            HasContacto = !string.IsNullOrEmpty(contacto);
            Uri = HasContacto
                ? new Uri(isEmail ? $"mailto:{contacto}" : contacto!)
                : null;
        }

        public string Nombre { get; }

        public string Contacto { get; }

        public bool HasContacto { get; }

        /// <summary>Uri de navegación del HyperlinkButton (mailto: o http). Null si no hay contacto.</summary>
        public Uri? Uri { get; }

        /// <summary>Etiqueta accesible para lectores de pantalla.</summary>
        public string ContactoAccesible =>
            HasContacto ? $"Contactar con {Nombre}" : Nombre;
    }
}
