using System.Configuration;
using System.Data.SqlClient;

namespace GestionClientes.Data
{
    public static class Conexion
    {
        // lee la cadena de conexion del Web.config y abre la conexion
        public static SqlConnection Abrir()
        {
            string cadena = ConfigurationManager.ConnectionStrings["GestionClientesDb"].ConnectionString;
            SqlConnection conexion = new SqlConnection(cadena);
            conexion.Open();
            return conexion;
        }
    }
}
