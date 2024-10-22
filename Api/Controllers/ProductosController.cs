using Api.Filter;
using Api.Models;
using Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [EnableCors("ReglasCors")]
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductosRepository _productoRepository;

        public ProductosController(IProductosRepository productosRepository)
        {
            _productoRepository = productosRepository;
        }

        [HttpGet]
        [ComprobarAutorizacion(IdPermiso = "OBTENER_PRODUCTO")]
        [Route("obtener")]
        public async Task<IActionResult> Obtener()
        {
            try
            {
                var lista = await _productoRepository.Obtener();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Éxito", data = lista });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, data = 0 });
            }
        }


        [HttpGet]
        [Route("obtener/{idproducto:int}")]
        public async Task<IActionResult> ObtenerId(int idproducto)
        {
            try
            {
                var producto =  await _productoRepository.ObtenerPorId(idproducto);
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Éxito", data = producto });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, data = 0 });
            }
        }

        [HttpPost]
        [Route("Insertar")]
        public  IActionResult  Insertar([FromBody] Productos productos)
        {
            try
            {
                _productoRepository.Insertar(productos);
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Éxito" });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }

        [HttpPut]
        [Route("Actualizar")]
        public IActionResult Actualizar([FromBody] Productos productos)
        {
            try
            {
                if (productos == null)
                {
                    return BadRequest(new { mensaje = "Datos de producto inválidos" });
                }
                _productoRepository.Actualizar(productos);
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Producto actualizado exitosamente" });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error al actualizar el producto: " + error.Message });
            }
        }

        [HttpDelete]
        [Route("Eliminar/{IdProducto:int}")]
        public IActionResult Eliminar(int IdProducto)
        {
            try
            {
                _productoRepository.Eliminar(IdProducto);
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Éxito" });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
    }
}
