using System;

namespace GestionClientes.Models
{
    public class Bitacora
    {
        public long Id { get; set; }
        public string Accion { get; set; }
        public int ClienteId { get; set; }
        public string NombreUsuario { get; set; }
        public DateTime FechaHora { get; set; }
        public string Detalle { get; set; }
    }
}
