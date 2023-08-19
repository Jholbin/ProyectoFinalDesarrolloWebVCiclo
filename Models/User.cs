using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalDesarrolloWeb.Models
{
    public class User
    {
        public int idusuario { get; set; }
        public int idTipo { get; set; }
        public string desTipo { get; set; }
        public string correo { get; set; }
        public int idSexo { get; set; }
        public string desSexo { get; set; }
        public string direccion { get; set; }
        public string usuario { get; set; }
        public string clave { get; set; }
    }
}
