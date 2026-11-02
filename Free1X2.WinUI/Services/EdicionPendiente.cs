// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Services;

/// <summary>
/// Conserva el ViewModel de una pantalla de condición mientras se visita una pantalla auxiliar
/// (Estadísticas, Saca formatos, Analizador, editor de figuras…) para recuperarlo al volver.
///
/// En el original WinForms esas pantallas se abrían con <c>ShowDialog()</c> encima de la condición,
/// que seguía viva con lo editado y no aceptado (p. ej. <c>FormatosFrm.cs:891</c>,
/// <c>Formatos123Frm.cs:931</c>, <c>ContactosFrm.cs:771</c>, <c>ValoracionFrm.cs:1116</c>). En WinUI
/// la <see cref="Page"/> se crea de nuevo en cada navegación (sin <c>NavigationCacheMode</c>), así que
/// sin esto la condición volvía vacía y «Aceptar» la borraba.
///
/// Uso: la página llama a <see cref="Guardar"/> en <c>OnNavigatedFrom</c> y a <see cref="Recuperar{T}"/>
/// en <c>OnNavigatedTo</c>. Solo se guarda al ir hacia delante (New) y solo se devuelve al volver (Back);
/// cualquier otra navegación lo descarta, de modo que abrir la condición desde Inicio carga su grupo.
/// </summary>
public static class EdicionPendiente
{
    private static readonly Dictionary<Type, object> Pendientes = new();

    /// <summary>Al salir de la página: guarda su ViewModel si se va hacia delante; si no, lo descarta.</summary>
    public static void Guardar(Page pagina, object viewModel, NavigationEventArgs e)
    {
        if (e.NavigationMode == NavigationMode.New)
            Pendientes[pagina.GetType()] = viewModel;
        else
            Pendientes.Remove(pagina.GetType());
    }

    /// <summary>
    /// Al entrar en la página: devuelve el ViewModel guardado si se vuelve atrás (Back); null en otro caso.
    /// Siempre lo retira.
    /// </summary>
    public static T? Recuperar<T>(Page pagina, NavigationEventArgs e) where T : class
    {
        Pendientes.Remove(pagina.GetType(), out var vm);
        return e.NavigationMode == NavigationMode.Back ? vm as T : null;
    }
}
