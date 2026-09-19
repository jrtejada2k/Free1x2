// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// <see cref="ObservableCollection{T}"/> con reemplazo en bloque (C-13).
///
/// Problema que resuelve: el patrón <c>X.Clear(); foreach (…) X.Add(…);</c> sobre una colección
/// enlazada a un <c>ListView</c> emite UN <c>CollectionChanged</c> POR ÍTEM. Con cientos o miles
/// de filas (columnas premiadas, escrutinios, combinaciones) eso son miles de notificaciones y
/// otros tantos ciclos de layout del ListView para un único resultado.
///
/// <see cref="ReemplazarTodo"/> hace el mismo reemplazo emitiendo un ÚNICO <c>Reset</c>. El
/// contenido final y su ORDEN son idénticos a los del bucle que sustituye: se escribe sobre la
/// lista interna (<see cref="Collection{T}.Items"/>, que no notifica) en el mismo orden de
/// enumeración de la fuente, y solo al terminar se notifica.
/// </summary>
public sealed class ColeccionUi<T> : ObservableCollection<T>
{
    public ColeccionUi() { }

    public ColeccionUi(IEnumerable<T> origen) : base(origen) { }

    /// <summary>
    /// Vacía la colección y la rellena con <paramref name="elementos"/>, en ese mismo orden,
    /// notificando una sola vez (<c>Reset</c>) en lugar de una vez por elemento.
    /// Debe invocarse desde el hilo de UI, igual que el <c>Clear()/Add()</c> al que sustituye.
    /// </summary>
    public void ReemplazarTodo(IEnumerable<T> elementos)
    {
        CheckReentrancy();

        Items.Clear();
        if (elementos is not null)
        {
            foreach (var elemento in elementos)
            {
                Items.Add(elemento);
            }
        }

        // Un solo golpe de notificaciones: Count, indexador y Reset (lo mismo que emite Clear(),
        // así que cualquier oyente correcto de ObservableCollection lo maneja igual).
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}
