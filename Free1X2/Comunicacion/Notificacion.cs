using System;
using System.Collections.Generic;
using System.Text;

namespace Free1X2.Comunicacion
{
    public class Notificacion
    {
        private string contenido, titulo, remitente;
        private DateTime fechaCaducidad, fechaCreacion;
        private int idNotificacion;
        private bool leida = false;
        private bool borrada = false;
        public Notificacion()
        {

        }

        public string Contenido
        {
            get { return contenido; }
            set { contenido = value; }
        }
        public string Titulo
        {
            get { return titulo; }
            set { titulo = value; }
        }
        public string Remitente
        {
            get { return remitente; }
            set { remitente = value; }
        }
        public DateTime FechaCaducidad
        {
            get { return fechaCaducidad; }
            set { fechaCaducidad = value; }
        }
        public DateTime FechaCreacion
        {
            get { return fechaCreacion; }
            set { fechaCreacion = value; }
        }
        public int IdNotificacion
        {
            get { return idNotificacion; }
            set { idNotificacion = value; }
        }
        public bool Leida
        {
            get { return leida; }
            set { leida = value; }
        }
        public bool Borrada
        {
            get { return borrada; }
            set { borrada = value; }
        }
    }
}
