using System.ComponentModel.DataAnnotations;
namespace ProyectoFinalDesarrolloWeb.Models
{
    public class Libro
    {
        public int LibroID { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public int idGenero { get; set; }
        public string Genero { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }

    }
}
