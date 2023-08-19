namespace ProyectoFinalDesarrolloWeb.Models
{
    public class Item
    {
        public int LibroID { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string Genero { get; set; }
        public decimal Precio { get; set; }
        public decimal monto { get { return Precio * Stock; } }
        public int Stock { get; set; }
        public string nombreusuario { get; set; }
    }
}
