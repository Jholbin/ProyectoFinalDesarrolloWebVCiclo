namespace ProyectoFinalDesarrolloWeb.Models
{
    public class Pedido
    {
            public int PedidoID { get; set; }
            public string NombreUsuario { get; set; }
            public int LibroID { get; set; }
            public string Titulo { get; set; }            
            public int Cantidad { get; set; }
            public decimal Precio { get; set; }

            
    }
}
