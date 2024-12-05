using Api.Models;

namespace Api.Repository
{
    public interface IProductosRepository
    {
        Task<List<Productos>> Obtener();  // Método para obtener todos los productos de manera asíncrona
        Task<Productos> ObtenerPorId(int id);  // Método para obtener un producto por su ID de manera asíncrona
        Task Insertar(Productos producto);  // Método para insertar un nuevo producto de manera asíncrona
        Task Actualizar(Productos producto);  // Método para actualizar un producto de manera asíncrona
        Task Eliminar(int id);  // Método para eliminar un producto de manera asíncrona

    }
}
