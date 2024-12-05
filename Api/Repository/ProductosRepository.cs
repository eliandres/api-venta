using Api.Models;
using System.Data;
using System.Data.SqlClient;

namespace Api.Repository
{
    public class ProductosRepository : IProductosRepository
    {
        private readonly string cadenaSQL;

        public ProductosRepository(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaConexion");
        }

        public async Task Actualizar(Productos productos)
        {
            var userId = 1;
            var ipAddress = "::F";

            using (var conexion = new SqlConnection(cadenaSQL))
            {
                await conexion.OpenAsync();
                var cmd = new SqlCommand("ActualizarProductos", conexion);
                cmd.Parameters.AddWithValue("@IdProducto", productos.IdProducto == 0 ? DBNull.Value : productos.IdProducto);
                cmd.Parameters.AddWithValue("@CodigoBara", string.IsNullOrEmpty(productos.CodigoBara) ? DBNull.Value : (object)productos.CodigoBara);
                cmd.Parameters.AddWithValue("@Nombre", string.IsNullOrEmpty(productos.Nombre) ? DBNull.Value : (object)productos.Nombre);
                cmd.Parameters.AddWithValue("@Marca", string.IsNullOrEmpty(productos.Marca) ? DBNull.Value : (object)productos.Marca);
                cmd.Parameters.AddWithValue("@Categoria", productos.Categoria);
                cmd.Parameters.AddWithValue("@Precios", productos.Precios == 0 ? DBNull.Value : (object)productos.Precios);
                cmd.Parameters.AddWithValue("@IdUsuarioCreacion", Convert.ToInt32(userId) == 0 ? DBNull.Value : userId);
                cmd.CommandType = CommandType.StoredProcedure;
                await cmd.ExecuteNonQueryAsync(); // Ejecuta la consulta de manera asíncrona
            }
        }

        public async Task Eliminar(int id)
        {
            using (var conexion = new SqlConnection(cadenaSQL))
            {
                await conexion.OpenAsync();
                var cmd = new SqlCommand("EliminarProductos", conexion);
                cmd.Parameters.AddWithValue("IdProducto", id);
                cmd.CommandType = CommandType.StoredProcedure;
                await cmd.ExecuteNonQueryAsync(); // Ejecución asíncrona
            }
        }

        public async Task Insertar(Productos producto)
        {
            using (var conexion = new SqlConnection(cadenaSQL))
            {
                await conexion.OpenAsync();
                var cmd = new SqlCommand("InsertarProductos", conexion);
                cmd.Parameters.AddWithValue("CodigoBara", producto.CodigoBara);
                cmd.Parameters.AddWithValue("Nombre", producto.Nombre);
                cmd.Parameters.AddWithValue("Marca", producto.Marca);
                cmd.Parameters.AddWithValue("Categoria", producto.Categoria);
                cmd.Parameters.AddWithValue("Precios", producto.Precios);
                cmd.Parameters.AddWithValue("IdEstadoRegistro", 1);
                cmd.Parameters.AddWithValue("IdUsuarioCreacion", 1);
                cmd.Parameters.AddWithValue("DireccionEquipoCreacion", "FFF");
                cmd.CommandType = CommandType.StoredProcedure;
                await cmd.ExecuteNonQueryAsync(); // Ejecución asíncrona
            }
        }

        public async Task<List<Productos>> Obtener()
        {
            List<Productos> lista = new List<Productos>();
            using (var conexion = new SqlConnection(cadenaSQL))
            {
                await conexion.OpenAsync();
                var cmd = new SqlCommand("ObtenerProductos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new Productos()
                        {
                            IdProducto = Convert.ToInt32(reader["IdProducto"]),
                            CodigoBara = reader["CodigoBara"].ToString(),
                            Nombre = reader["Nombre"].ToString(),
                            Marca = reader["Marca"].ToString(),
                            Categoria = reader["Categoria"].ToString(),
                            Precios = Convert.ToDecimal(reader["Precios"])
                        });
                    }
                }
            }

            foreach (var producto in lista)
            {
                producto.Estado = producto.Precios >= 1000 ? "Alto Precio" : "Bajo Precio";
            }

            return lista;
        }

        public async Task<Productos> ObtenerPorId(int id)
        {
            Productos producto = null;
            using (var conexion = new SqlConnection(cadenaSQL))
            {
                await conexion.OpenAsync();
                var cmd = new SqlCommand("ObtenerProductos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (Convert.ToInt32(reader["IdProducto"]) == id)
                        {
                            producto = new Productos()
                            {
                                IdProducto = Convert.ToInt32(reader["IdProducto"]),
                                CodigoBara = reader["CodigoBara"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Marca = reader["Marca"].ToString(),
                                Categoria = reader["Categoria"].ToString(),
                                Precios = Convert.ToDecimal(reader["Precios"])
                            };
                            break;
                        }
                    }
                }
            }

            return producto;
        }
    }
}
