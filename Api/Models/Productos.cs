using System.Runtime.CompilerServices;

namespace Api.Models
{
    public class Productos
    {
        public int IdProducto { get; set; }
        public string CodigoBara { get; set; }
        public string Nombre { get; set; }
        public string Marca { get; set;}
        public string Categoria { get; set; }
        public decimal Precios { get; set;}
        public string Estado { get; set;}
    }
}
