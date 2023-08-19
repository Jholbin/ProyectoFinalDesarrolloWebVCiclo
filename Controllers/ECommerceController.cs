using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Session;
using Newtonsoft.Json;
using ProyectoFinalDesarrolloWeb.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace ProyectoFinalDesarrolloWeb.Controllers
{
    [Authorize (Policy ="Cliente")]
    public class ECommerceController : Controller
    {

        private readonly IConfiguration _config;

        public ECommerceController(IConfiguration Iconfig){
            _config = Iconfig;
        }

        IEnumerable<Libro> libros()
        {
            List<Libro> lista = new List<Libro>();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("exec splibros", cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Libro(){

                        LibroID = dr.GetInt32(0),
                        Titulo = dr.GetString(1),
                        Autor= dr.GetString(2),
                        Genero = dr.GetString(3),
                        Precio= dr.GetDecimal(4),
                        Stock = dr.GetInt32(5)
                    });
                }
            }
            return lista;
        }

        Libro Buscar(int id = 0)
        {
            Libro reg = libros().Where(p => p.LibroID == id).FirstOrDefault();
            if(reg == null)
                reg = new Libro();

            return reg;
        }

        public async Task<IActionResult> Portal()
        {
            if (HttpContext.Session.GetString("Canasta") == null)
                HttpContext.Session.SetString("Canasta", JsonConvert.SerializeObject(new List<Item>()));

            var idUsuarioClaim = User.FindFirst("idusuario")?.Value;
            if (!string.IsNullOrEmpty(idUsuarioClaim) && int.TryParse(idUsuarioClaim, out int idUsuario))
            {
                ViewBag.IdUsuario = idUsuario;
            }


            return View(await Task.Run(() => libros()));
        }

        public async Task<IActionResult> Agregar(int id = 0)
        {
            return View(await Task.Run(() => Buscar(id)));
        }


        [HttpPost] public ActionResult Agregar(int id, int stock)
        {
            Libro reg = Buscar(id);
            if(stock > reg.Stock)
            {
                ViewBag.mensaje = string.Format("El producto solo dispone de {0} unidades", reg.Stock);
                return View(reg);
            }

            Item it = new Item();
            it.LibroID = id;
            it.Titulo = reg.Titulo;
            it.Genero = reg.Genero;
            it.Precio = reg.Precio;
            it.Stock = stock;
            it.nombreusuario = User.Identity.Name;

            string connectionString = _config["ConnectionStrings:cn"];

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("INSERT INTO Pedido (NombreUsuario, LibroID, Titulo, Cantidad, Precio) VALUES (@NombreUsuario, @LibroID, @Titulo, @Cantidad, @Precio)", connection))
                {
                    command.Parameters.AddWithValue("@NombreUsuario", User.Identity.Name);
                    command.Parameters.AddWithValue("@LibroID", id);
                    command.Parameters.AddWithValue("@Titulo", reg.Titulo);
                    command.Parameters.AddWithValue("@Cantidad", stock);
                    command.Parameters.AddWithValue("@Precio", reg.Precio*stock);

                    command.ExecuteNonQuery();
                }
            }

            List<Item> carrito = JsonConvert.DeserializeObject<List<Item>>(HttpContext.Session.GetString("Canasta"));
            carrito.Add(it);
            HttpContext.Session.SetString("Canasta", JsonConvert.SerializeObject(carrito));
            ViewBag.mensaje = "Libro Agregado";

            return View(reg);

        }

        public IActionResult Canasta()
        {
            if (HttpContext.Session.GetString("Canasta") == null) return RedirectToAction("Portal");

            IEnumerable<Item> carrito =
                JsonConvert.DeserializeObject<List<Item>>(HttpContext.Session.GetString("Canasta"));

            return View(carrito);
        }

        public IActionResult Delete(int id)
        {
            List<Item> carrito =
                JsonConvert.DeserializeObject<List<Item>>(HttpContext.Session.GetString("Canasta"));
            Item reg = carrito.Where(it => it.LibroID == id).First();
            carrito.Remove(reg);
            HttpContext.Session.SetString("Canasta", JsonConvert.SerializeObject(carrito));
            return RedirectToAction("Canasta");

        }

        public IActionResult CrearPedido()
        {
            string userName = User.Identity.Name; 


            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
