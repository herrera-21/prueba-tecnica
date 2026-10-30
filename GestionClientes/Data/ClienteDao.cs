using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GestionClientes.Models;

namespace GestionClientes.Data
{
    public static class ClienteDao
    {
        // lista los clientes y permite buscar por nombre, apellido o documento
        public static List<Cliente> Listar(string buscar)
        {
            List<Cliente> lista = new List<Cliente>();

            const string sql = @"SELECT Id, Nombres, Apellidos, Documento, Correo, Telefono, Direccion, FechaCreacion
                                 FROM dbo.Clientes
                                 WHERE (@Buscar = ''
                                        OR Nombres LIKE '%' + @Buscar + '%'
                                        OR Apellidos LIKE '%' + @Buscar + '%'
                                        OR Documento LIKE '%' + @Buscar + '%')
                                 ORDER BY Apellidos, Nombres";

            using (SqlConnection conexion = Conexion.Abrir())
            using (SqlCommand comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.Add("@Buscar", SqlDbType.NVarChar, 100).Value = (buscar ?? "").Trim();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(Leer(lector));
                    }
                }
            }

            return lista;
        }

        // trae un cliente por id, si no existe devuelve null
        public static Cliente ObtenerPorId(int id)
        {
            const string sql = @"SELECT Id, Nombres, Apellidos, Documento, Correo, Telefono, Direccion, FechaCreacion
                                 FROM dbo.Clientes
                                 WHERE Id = @Id";

            using (SqlConnection conexion = Conexion.Abrir())
            using (SqlCommand comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (!lector.Read())
                    {
                        return null;
                    }

                    return Leer(lector);
                }
            }
        }

        // guarda el cliente y su registro en la bitacora en una sola transaccion
        public static int Insertar(Cliente cliente, string usuario)
        {
            Limpiar(cliente);

            const string sqlCliente = @"INSERT INTO dbo.Clientes (Nombres, Apellidos, Documento, Correo, Telefono, Direccion)
                                        OUTPUT INSERTED.Id
                                        VALUES (@Nombres, @Apellidos, @Documento, @Correo, @Telefono, @Direccion)";

            using (SqlConnection conexion = Conexion.Abrir())
            using (SqlTransaction transaccion = conexion.BeginTransaction())
            {
                try
                {
                    int id;

                    using (SqlCommand comando = new SqlCommand(sqlCliente, conexion, transaccion))
                    {
                        AgregarParametros(comando, cliente);
                        id = (int)comando.ExecuteScalar();
                    }

                    RegistrarBitacora(conexion, transaccion, "AGREGAR", id, usuario, Detalle("Cliente agregado", cliente));

                    transaccion.Commit();
                    return id;
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        // actualiza el cliente y deja el registro en la bitacora
        public static void Actualizar(Cliente cliente, string usuario)
        {
            Limpiar(cliente);

            const string sqlCliente = @"UPDATE dbo.Clientes
                                        SET Nombres = @Nombres,
                                            Apellidos = @Apellidos,
                                            Documento = @Documento,
                                            Correo = @Correo,
                                            Telefono = @Telefono,
                                            Direccion = @Direccion
                                        WHERE Id = @Id";

            using (SqlConnection conexion = Conexion.Abrir())
            using (SqlTransaction transaccion = conexion.BeginTransaction())
            {
                try
                {
                    int filas;

                    using (SqlCommand comando = new SqlCommand(sqlCliente, conexion, transaccion))
                    {
                        AgregarParametros(comando, cliente);
                        comando.Parameters.Add("@Id", SqlDbType.Int).Value = cliente.Id;
                        filas = comando.ExecuteNonQuery();
                    }

                    if (filas == 0)
                    {
                        throw new InvalidOperationException("El cliente ya no existe.");
                    }

                    RegistrarBitacora(conexion, transaccion, "EDITAR", cliente.Id, usuario, Detalle("Cliente actualizado", cliente));

                    transaccion.Commit();
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        // elimina el cliente pero deja el registro en la bitacora
        public static void Eliminar(int id, string usuario)
        {
            const string sqlBuscar = @"SELECT Nombres, Apellidos, Documento
                                       FROM dbo.Clientes
                                       WHERE Id = @Id";

            const string sqlCliente = @"DELETE FROM dbo.Clientes WHERE Id = @Id";

            using (SqlConnection conexion = Conexion.Abrir())
            using (SqlTransaction transaccion = conexion.BeginTransaction())
            {
                try
                {
                    Cliente cliente = null;

                    using (SqlCommand comando = new SqlCommand(sqlBuscar, conexion, transaccion))
                    {
                        comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                cliente = new Cliente
                                {
                                    Nombres = lector["Nombres"].ToString(),
                                    Apellidos = lector["Apellidos"].ToString(),
                                    Documento = lector["Documento"].ToString()
                                };
                            }
                        }
                    }

                    if (cliente == null)
                    {
                        throw new InvalidOperationException("El cliente ya no existe.");
                    }

                    using (SqlCommand comando = new SqlCommand(sqlCliente, conexion, transaccion))
                    {
                        comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                        comando.ExecuteNonQuery();
                    }

                    RegistrarBitacora(conexion, transaccion, "ELIMINAR", id, usuario, Detalle("Cliente eliminado", cliente));

                    transaccion.Commit();
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        // inserta el registro de bitacora dentro de la misma transaccion
        private static void RegistrarBitacora(SqlConnection conexion, SqlTransaction transaccion,
                                              string accion, int clienteId, string usuario, string detalle)
        {
            const string sql = @"INSERT INTO dbo.Bitacora (Accion, ClienteId, NombreUsuario, Detalle)
                                 VALUES (@Accion, @ClienteId, @NombreUsuario, @Detalle)";

            using (SqlCommand comando = new SqlCommand(sql, conexion, transaccion))
            {
                comando.Parameters.Add("@Accion", SqlDbType.NVarChar, 20).Value = accion;
                comando.Parameters.Add("@ClienteId", SqlDbType.Int).Value = clienteId;
                comando.Parameters.Add("@NombreUsuario", SqlDbType.NVarChar, 50).Value = usuario;
                comando.Parameters.Add("@Detalle", SqlDbType.NVarChar, 500).Value = detalle;
                comando.ExecuteNonQuery();
            }
        }

        private static Cliente Leer(SqlDataReader lector)
        {
            return new Cliente
            {
                Id = (int)lector["Id"],
                Nombres = lector["Nombres"].ToString(),
                Apellidos = lector["Apellidos"].ToString(),
                Documento = lector["Documento"].ToString(),
                Correo = lector["Correo"] == DBNull.Value ? null : lector["Correo"].ToString(),
                Telefono = lector["Telefono"] == DBNull.Value ? null : lector["Telefono"].ToString(),
                Direccion = lector["Direccion"] == DBNull.Value ? null : lector["Direccion"].ToString(),
                FechaCreacion = (DateTime)lector["FechaCreacion"]
            };
        }

        private static void AgregarParametros(SqlCommand comando, Cliente cliente)
        {
            comando.Parameters.Add("@Nombres", SqlDbType.NVarChar, 100).Value = cliente.Nombres;
            comando.Parameters.Add("@Apellidos", SqlDbType.NVarChar, 100).Value = cliente.Apellidos;
            comando.Parameters.Add("@Documento", SqlDbType.NVarChar, 20).Value = cliente.Documento;
            comando.Parameters.Add("@Correo", SqlDbType.NVarChar, 100).Value = (object)cliente.Correo ?? DBNull.Value;
            comando.Parameters.Add("@Telefono", SqlDbType.NVarChar, 20).Value = (object)cliente.Telefono ?? DBNull.Value;
            comando.Parameters.Add("@Direccion", SqlDbType.NVarChar, 200).Value = (object)cliente.Direccion ?? DBNull.Value;
        }

        // quita espacios y deja en null los campos opcionales vacios
        private static void Limpiar(Cliente cliente)
        {
            cliente.Nombres = (cliente.Nombres ?? "").Trim();
            cliente.Apellidos = (cliente.Apellidos ?? "").Trim();
            cliente.Documento = (cliente.Documento ?? "").Trim();
            cliente.Correo = TextoOpcional(cliente.Correo);
            cliente.Telefono = TextoOpcional(cliente.Telefono);
            cliente.Direccion = TextoOpcional(cliente.Direccion);
        }

        private static string TextoOpcional(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return null;
            }

            return valor.Trim();
        }

        private static string Detalle(string texto, Cliente cliente)
        {
            return texto + ": " + cliente.Nombres + " " + cliente.Apellidos + " (" + cliente.Documento + ")";
        }
    }
}
