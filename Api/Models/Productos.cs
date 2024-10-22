namespace Api.Models
{
    public class Productos
    {
        public int IdProducto { get; set; }
        public string CodigoBara { get; set; }
        public string Nombre { get; set; }
        public string Marca { get; set; }
        public string Categoria { get; set; }
        public decimal Precios { get; set; }
        public string Estado { get; set; }
        public Boolean IdEstadoRegistro { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int IdUsuarioCreacion { get; set; }
        public string DireccionEquipoCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int IdUsuarioModificacion { get; set; }
        public string DireccionEquipoModificacion { get; set; }
    }
}
