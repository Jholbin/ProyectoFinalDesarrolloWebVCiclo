using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using ProyectoFinalDesarrolloWeb.Models;

namespace ProyectoFinalDesarrolloWeb.Controllers
{
    [Authorize(Policy ="Admin")]
    public class LibroController : Controller
    {
        public readonly IConfiguration _config;

        public LibroController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Libro()
        {
            ViewBag.GenerosDeLibros = ObtenerGenerosdeLibros();
            return View();
        }

        IEnumerable<Genero> ObtenerGenerosdeLibros()
        {
            List<Genero> generos = new List<Genero>();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("listarGenero", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    generos.Add(new Genero()
                    {
                        idGenero = dr.GetInt32(0),
                        desGenero = dr.GetString(1),
                    });
                }
                return generos;
            }


        }

        public IActionResult Registro(Libro objlibro)
        {
            string mensaje = "";

            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("MergeLibro", cn);
                    cn.Open();

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Titulo", objlibro.Titulo);
                    cmd.Parameters.AddWithValue("@Autor", objlibro.Autor);
                    cmd.Parameters.AddWithValue("@idGenero", objlibro.idGenero);
                    cmd.Parameters.AddWithValue("@Precio", objlibro.Precio);
                    cmd.Parameters.AddWithValue("@Stock", objlibro.Stock);

                    int c = cmd.ExecuteNonQuery();
                    mensaje = $"registro insertado{c}en Libro";


                }
                catch(Exception ex)
                {
                    mensaje = ex.Message;
                }
            }
            ViewBag.mensaje = mensaje;
            return RedirectToAction("ListadoLibros", "Libro");
        }

        IEnumerable<Libro> libros()
        {
            List<Libro> libros = new List<Libro>();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("listarlibros", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    libros.Add(new Libro()
                    {
                        LibroID = dr.GetInt32(0),
                        Titulo = dr.GetString(1),
                        Autor = dr.GetString(2),
                        Genero = dr.GetString(3),
                        Precio = dr.GetDecimal(4),
                        Stock = dr.GetInt32(5)
                    });
                }
            }
            return libros;
        }

        //[Authorize (Roles="admin")]
        public async Task<IActionResult> listadoLibros(int p)
        {
            int nr = 5;
            int tr = libros().Count();
            int paginas = nr % tr > 0 ? tr / nr + 1 : tr / nr;

            ViewBag.paginas = paginas;

            return View(await Task.Run(() => libros().Skip(p * nr).Take(nr)));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.GenerosDeLibros = ObtenerGenerosdeLibros();
            Libro lib = new Libro();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("buscarLibro @LibroID", cn);
                cmd.Parameters.AddWithValue("@LibroID", id);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        lib.LibroID = Convert.ToInt32(dr["LibroID"]);
                        lib.Titulo = (String)dr["Titulo"];
                        lib.Autor = (String)dr["Autor"];
                        lib.idGenero = (int)dr["idGenero"];
                        lib.Precio = (decimal)dr["Precio"];
                        lib.Stock = (int)dr["Stock"];
                    }
                }
            }
            return View(lib);
        }

        [HttpPost, ActionName("Edit")]

        public IActionResult Edit_Post(Libro objDisco)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("MergeLibro", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cn.Open();
                    cmd.Parameters.AddWithValue("@LibroID", objDisco.LibroID);
                    cmd.Parameters.AddWithValue("@Titulo", objDisco.Titulo);
                    cmd.Parameters.AddWithValue("@Autor", objDisco.Autor);
                    cmd.Parameters.AddWithValue("@idGenero", objDisco.idGenero);
                    cmd.Parameters.AddWithValue("@Precio", objDisco.Precio);
                    cmd.Parameters.AddWithValue("@Stock", objDisco.Stock);

                    int c = cmd.ExecuteNonQuery();
                    mensaje = $"registro actualizado {c}en Libros";

                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }

            }
            ViewBag.mensaje = mensaje;
            return RedirectToAction("ListadoLibros");
        }

        [HttpGet, ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("eliminarLibro", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cn.Open();
                    cmd.Parameters.AddWithValue("@LibroID", id);

                    int c = cmd.ExecuteNonQuery();
                    mensaje = $"registro eliminado{c} de Libros";
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }
            }
            ViewBag.mensaje = mensaje;
            return RedirectToAction("ListadoLibros");

        }


    }
}
