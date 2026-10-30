using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GestionClientes.Models;

namespace GestionClientes.Data
{
    public static class BitacoraDao
    {
        // trae los ultimos 200 movimientos, se puede filtrar por cliente, usuario o accion
        public static List<Bitacora> Listar(string buscar)
        {
            List<Bitacora> lista = new List<Bitacora>();

            const string sql = @"SELECT TOP (200) Id, Accion, ClienteId, NombreUsuario, FechaHora, Detalle
                                 FROM dbo.Bitacora
                                 WHERE (@Buscar = ''
                                        OR CAST(ClienteId AS varchar(20)) = @Buscar
                                        OR NombreUsuario LIKE '%' + @Buscar + '%'
                                        OR Accion LIKE '%' + @Buscar + '%')
                                 ORDER BY FechaHora DESC, Id DESC";

            using (SqlConnection conexion = Conexion.Abrir())
            using (SqlCommand comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.Add("@Buscar", SqlDbType.NVarChar, 100).Value = (buscar ?? "").Trim();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(new Bitacora
                        {
                            Id = (long)lector["Id"],
                            Accion = lector["Accion"].ToString(),
                            ClienteId = (int)lector["ClienteId"],
                            NombreUsuario = lector["NombreUsuario"].ToString(),
                            FechaHora = (DateTime)lector["FechaHora"],
                            Detalle = lector["Detalle"] == DBNull.Value ? null : lector["Detalle"].ToString()
                        });
                    }
                }
            }

            return lista;
        }
    }
}
