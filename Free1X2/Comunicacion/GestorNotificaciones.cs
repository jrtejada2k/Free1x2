using System;
using System.Collections.Generic;
using System.Text;
using Free1X2.EntradaSalida;
// using Free1X2.SVC_Actualizador; // TODO: Replace with modern HTTP client
using System.Windows.Forms;

namespace Free1X2.Comunicacion
{
    public class GestorNotificaciones
    {
        List<Notificacion> notificaciones = new List<Notificacion>();
        SortedList<int, string> estadoComunicaciones;


        private void CargaEstadoNotificaciones()
        {
            estadoComunicaciones = new SortedList<int, string>();
            AConfiguracion aConf = new AConfiguracion(Application.StartupPath);
            aConf.ObtenEstadoNotificaciones(ref estadoComunicaciones);
        }
        public List<Notificacion> ObtenerNotificaciones()
        {
            CargaEstadoNotificaciones();
            Free1X2WService free1X2WService = new Free1X2WService();
            NotificacionFree1x2[] lista = free1X2WService.ObtenerNotificaciones(VariablesGlobales.FechaUltimaComprobacionNotificaciones, string.Empty);

            notificaciones.Clear();

            for (int i = 0; i < lista.Length; i++)
            {
                Notificacion not = new Notificacion();
                not.IdNotificacion = lista[i].IdNotificacion;
                not.Titulo = lista[i].Titulo;
                not.Contenido = lista[i].Contenido;
                not.Remitente = lista[i].Remitente;
                not.Leida = lista[i].Leida;
                not.Borrada = lista[i].Borrada;
                not.FechaCaducidad = lista[i].FechaCaducidad;
                not.FechaCreacion = lista[i].FechaCreacion;

                if (estadoComunicaciones.ContainsKey(not.IdNotificacion))
                {
                    string[] estado = estadoComunicaciones[not.IdNotificacion].Split('#');
                    if (!Convert.ToBoolean(estado[1])) //no ha sido borrada, mostrar
                    {
                        not.Borrada = false;
                        //pero... ¿se ha leido?
                        if (!Convert.ToBoolean(estado[0])) //no ha sido leida
                        {
                            not.Leida = false;
                        }
                        else
                        {
                            not.Leida = true;
                        }
                    }
                    else
                    {
                        not.Leida = true;
                        not.Borrada = true;
                    }
                    notificaciones.Add(not);
                }
                else
                {
                    not.Leida = false;
                    not.Borrada = false;
                    notificaciones.Add(not);
                }
            }
            ActualizaFechaUltimaComprobacionNotificaciones();
            return notificaciones;
        }
        private void ActualizaFechaUltimaComprobacionNotificaciones()
        {
            AConfiguracion aConf = new AConfiguracion(Application.StartupPath);
            aConf.GuardarFechaUltimaComprobacionNotificaciones(DateTime.Now);
        }
        public bool HayNotificacionesSinLeer()
        {
            CargaEstadoNotificaciones();
            Free1X2WService free1X2WService = new Free1X2WService();
            NotificacionFree1x2[] lista = free1X2WService.ObtenerNotificaciones(VariablesGlobales.FechaUltimaComprobacionNotificaciones, string.Empty);

            notificaciones.Clear();

            for (int i = 0; i < lista.Length; i++)
            { 
            if (estadoComunicaciones.ContainsKey(lista[i].IdNotificacion))
                {
                    string[] estado = estadoComunicaciones[lista[i].IdNotificacion].Split('#');
                    if (!Convert.ToBoolean(estado[1])) //no ha sido borrada, mostrar
                    {
                        //no ha sido borrada, pero... ¿se ha leido?
                        if (!Convert.ToBoolean(estado[0])) //no ha sido leida
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

    }
}
