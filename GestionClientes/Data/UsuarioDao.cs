using System.Data;
using System.Data.SqlClient;
using GestionClientes.Models;
using GestionClientes.Seguridad;

namespace GestionClientes.Data
{
    public static class UsuarioDao
    {
        // busca un usuario por su nombre, si no existe devuelve null
        public static Usuario BuscarPorNombre(string nombreUsuario)
        {
            const string sql = @"SELECT Id, NombreUsuario, PasswordHash, PasswordSalt, Activo
                                 FROM dbo.Usuarios
                                 WHERE NombreUsuario = @NombreUsuario";

            using (SqlConnection conexion = Conexion.Abrir())
            using (SqlCommand comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.Add("@NombreUsuario", SqlDbType.NVarChar, 50).Value = nombreUsuario;

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (!lector.Read())
                    {
                        return null;
                    }

                    return new Usuario
                    {
                        Id = (int)lector["Id"],
                        NombreUsuario = lector["NombreUsuario"].ToString(),
                        PasswordHash = lector["PasswordHash"].ToString(),
                        PasswordSalt = lector["PasswordSalt"].ToString(),
                        Activo = (bool)lector["Activo"]
                    };
                }
            }
        }

        // devuelve true solo si el usuario existe, esta activo y la contraseña coincide
        public static bool ValidarLogin(string nombreUsuario, string password)
        {
            Usuario usuario = BuscarPorNombre(nombreUsuario);

            if (usuario == null || !usuario.Activo)
            {
                return false;
            }

            return Hash.Verificar(password, usuario.PasswordSalt, usuario.PasswordHash);
        }
    }
}
