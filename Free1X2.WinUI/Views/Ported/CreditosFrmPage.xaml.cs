// Free1X2 · WinUI 3 — WIN3
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
            // Contactos de terceros: emails OFUSCADOS (anti-scrape) y mostrados como
            // texto plano no clicable. No se genera ningún mailto: con la dirección real.
            new("Luis Fernández", "lfernandezrodriguez [at] gmail [dot] com"),
            new("Morrison", null),
            new("Toni Moreno", "toni [at] moreno-csa [dot] com"),
            new("Joan Duatis", "duatis [at] coac [dot] net"),
            new("Indeciso", "jjchild [at] lycos [dot] co [dot] uk"),
            new("JVallespin", "jvallespin [at] gmail [dot] com"),
            new("Xfsf", "segura33 [at] ono [dot] com"),
            new("ArDoC", "ardock [at] gmail [dot] com"),
            // Propietario del repo: mailto funcional e intencional.
            CreditoEntrada.Mailto("Juan Tejada", "elrevisor2k@gmail.com"),
        };

        public CreditosFrmPage()
        {
            InitializeComponent();
        }

        private async void OnWebsiteClick(object sender, RoutedEventArgs e)
        {
            // Equivale a lnkEquipoFree_LinkClicked: abre el sitio del proyecto.
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://clubprogol.com"));
        }
    }

    /// <summary>
    /// Entrada de la lista de créditos.
    ///
    /// Un colaborador puede mostrar su contacto de dos formas:
    /// - Texto plano NO clicable: para los emails de terceros, que se guardan ya
    ///   OFUSCADOS (formato "user [at] domain [dot] tld") para que no sean rastreables
    ///   en el repositorio público. No se genera ningún <see cref="Uri"/> mailto:.
    /// - Enlace funcional (mailto:): reservado para el propietario del repo, creado
    ///   con la fábrica <see cref="Mailto"/>.
    /// </summary>
    public sealed class CreditoEntrada
    {
        /// <summary>
        /// Contacto mostrado como texto plano (no clicable). Para terceros: el email
        /// ya ofuscado. Use null si el colaborador no expone contacto.
        /// </summary>
        public CreditoEntrada(string nombre, string? contacto)
        {
            Nombre = nombre;
            Contacto = contacto ?? string.Empty;
            HasContacto = !string.IsNullOrEmpty(contacto);
            Uri = null;
        }

        private CreditoEntrada(string nombre, string contacto, Uri uri)
        {
            Nombre = nombre;
            Contacto = contacto;
            HasContacto = true;
            Uri = uri;
        }

        /// <summary>
        /// Crea una entrada con enlace mailto: funcional y clicable.
        /// Reservado para contactos que se publican intencionadamente en claro.
        /// </summary>
        public static CreditoEntrada Mailto(string nombre, string email) =>
            new(nombre, email, new Uri($"mailto:{email}"));

        public string Nombre { get; }

        public string Contacto { get; }

        public bool HasContacto { get; }

        /// <summary>Uri de navegación del HyperlinkButton (mailto:). Null si el contacto no es clicable.</summary>
        public Uri? Uri { get; }

        /// <summary>true si el contacto es un enlace funcional clicable.</summary>
        public bool EsClicable => Uri is not null;

        /// <summary>Visibilidad del enlace clicable (mailto:) en la plantilla.</summary>
        public Visibility VisibilidadEnlace =>
            EsClicable ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>Visibilidad del contacto en texto plano (no clicable) en la plantilla.</summary>
        public Visibility VisibilidadTexto =>
            (HasContacto && !EsClicable) ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>Etiqueta accesible para lectores de pantalla.</summary>
        public string ContactoAccesible =>
            HasContacto ? $"Contactar con {Nombre}" : Nombre;
    }
}
