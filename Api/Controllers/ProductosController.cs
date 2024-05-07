using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Api.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [EnableCors("ReglasCors")]
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly string cadenaSQL;

        public ProductosController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaConexion");
        }

        [HttpGet]
        [Route("obtener")]
        public IActionResult Obtener() { 
        List<Productos> Lista = new List<Productos>();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("ObtenerProductos", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = cmd.ExecuteReader()) {
                        while (reader.Read())//Recorrer el resultado SQL
                        {
                            Lista.Add(new Productos()//Agregar el resultado y incluirlo al modelo
                            {
                                IdProducto = Convert.ToInt32(reader["IdProducto"]),
                                CodigoBara = reader["CodigoBara"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Marca = reader["Marca"].ToString(),
                                Categoria = reader["Categoria"].ToString(),
                                Precios = Convert.ToDecimal(reader["Precios"])
                            });
                        }

                        foreach (var producto in Lista)
                        {
                            producto.Estado = producto.Precios >= 1000 ? "Alto Precio" : "Bajo Precio";
                        }
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new {mensaje="Exito", data = Lista});//Respuesta con un status de 200 respuesta bien elaborada y la informacion correcta
            }catch (Exception Error) {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = Error.Message, data = 0 });
            }
        }


        [HttpGet]
        [Route("obtener/{idproducto:int}")]
        public IActionResult ObtenerId(int idproducto)
        {
            List<Productos> lista = new List<Productos>();
            Productos productos = new Productos();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("ObtenerProductos", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())//Recorrer el resultado SQL
                        {
                            lista.Add(new Productos()//Agregar el resultado y incluirlo al modelo
                            {
                                IdProducto = Convert.ToInt32(reader["IdProducto"]),
                                CodigoBara = reader["CodigoBara"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Marca = reader["Marca"].ToString(),
                                Categoria = reader["Categoria"].ToString(),
                                Precios = Convert.ToDecimal(reader["Precios"])
                            });
                        }

                        foreach (var producto in lista)
                        {
                            producto.Estado = producto.Precios >= 1000 ? "Alto Precio" : "Bajo Precio";
                        }

                        productos = lista.Where(item => item.IdProducto == idproducto).FirstOrDefault();
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Exito", data = productos });//Respuesta con un status de 200 respuesta bien elaborada y la informacion correcta
            }
            catch (Exception Error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = Error.Message, data = 0 });
            }
        }

        [HttpPost]
        [Route("Insertar")]
        public IActionResult Insertar([FromBody] Productos productos)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("InsertarProductos", conexion);
                    cmd.Parameters.AddWithValue("CodigoBara", productos.CodigoBara);
                    cmd.Parameters.AddWithValue("Nombre", productos.Nombre);
                    cmd.Parameters.AddWithValue("Marca", productos.Marca);
                    cmd.Parameters.AddWithValue("Categoria", productos.Categoria);
                    cmd.Parameters.AddWithValue("Precios", productos.Precios);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();//Ejecuta la consulta
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Exito"});//Respuesta con un status de 200 respuesta sactifactoria
            }
            catch (Exception Error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = Error.Message});
            }
        }

        [HttpPut]
        [Route("Actualizar")]
        public IActionResult Actualizar([FromBody] Productos productos)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("ActualizarProductos", conexion);
                    cmd.Parameters.AddWithValue("IdProducto", productos.IdProducto == 0 ? DBNull.Value : productos.IdProducto);
                    cmd.Parameters.AddWithValue("CodigoBara", productos.CodigoBara is null ? DBNull.Value: productos.CodigoBara);
                    cmd.Parameters.AddWithValue("Nombre", productos.Nombre is null ? DBNull.Value: productos.Nombre);
                    cmd.Parameters.AddWithValue("Marca", productos.Marca is null ? DBNull.Value: productos.Marca);
                    cmd.Parameters.AddWithValue("Categoria", productos.Categoria);
                    cmd.Parameters.AddWithValue("Precios", productos.Precios == 0 ? DBNull.Value: productos.Precios);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();//Ejecuta la consulta
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Exito" });//Respuesta con un status de 200 respuesta sactifactoria
            }
            catch (Exception Error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = Error.Message });
            }
        }

        [HttpDelete]
        [Route("Eliminar/{IdProducto:int}")]
        public IActionResult Eliminar(int IdProducto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("EliminarProductos", conexion);
                    cmd.Parameters.AddWithValue("IdProducto", IdProducto);
                   cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();//Ejecuta la consulta
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Exito" });//Respuesta con un status de 200 respuesta sactifactoria
            }
            catch (Exception Error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = Error.Message });
            }
        }
    }
}
