using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoFinalDesarrolloWeb.Models;

namespace ProyectoFinalDesarrolloWeb.Controllers
{

    public class UserController : Controller
    {
        public readonly IConfiguration _config;
        public UserController(IConfiguration IConfig)
        {
            _config = IConfig;

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Users()
        {
            ViewBag.TiposDeSexo = ObtenerTiposdeSexo();
            return View();
        }

        IEnumerable<Sexo> ObtenerTiposdeSexo()
        {
            List<Sexo> tipoS = new List<Sexo>();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("listarTipoSexo", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    tipoS.Add(new Sexo()
                    {
                        idSexo = dr.GetInt32(0),
                        desSexo = dr.GetString(1),
                    });
                }
                return tipoS;
            }
        }

        public IActionResult Registro(User objUser)
        {
            string mensaje = "";

            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn);
                    cn.Open();

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idTipo", 2);
                    cmd.Parameters.AddWithValue("@usuario", objUser.usuario);
                    cmd.Parameters.AddWithValue("@clave", objUser.clave);
                    cmd.Parameters.AddWithValue("@correo", objUser.correo);
                    cmd.Parameters.AddWithValue("@idsexo", objUser.idSexo);
                    cmd.Parameters.AddWithValue("@Direccion", objUser.direccion);

                    int c = cmd.ExecuteNonQuery();
                    mensaje = $"registro insertado{c}en User";
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }
            }
            ViewBag.mensaje = mensaje;
            return RedirectToAction("Index", "Autenticacion");
        }

        [HttpGet]
        public IActionResult EditarUsuario(int id)
        {
            ViewBag.TiposDeSexo = ObtenerTiposdeSexo();
            User use = new User();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("buscarUsuario @idusuario", cn);
                cmd.Parameters.AddWithValue("@idusuario", id);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        use.idusuario = Convert.ToInt32(dr["idusuario"]);
                        use.idTipo = (int)dr["idTipo"];
                        use.usuario = (String)dr["usuario"];
                        use.clave = (String)dr["clave"];
                        use.correo = (String)dr["correo"];
                        use.direccion = (String)dr["direccion"];
                        use.idSexo = (int)dr["idSexo"];
                    }
                }
            }
            return View(use);
        }

        [HttpPost, ActionName("EditarUsuario")]

        public IActionResult Edit_Post(User objUser)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("ActualizarUsuario", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cn.Open();
                    cmd.Parameters.AddWithValue("@idusuario", objUser.idusuario);
                    cmd.Parameters.AddWithValue("@idTipo", 2);
                    cmd.Parameters.AddWithValue("@usuario", objUser.usuario);
                    cmd.Parameters.AddWithValue("@clave", objUser.clave);
                    cmd.Parameters.AddWithValue("@correo", objUser.correo);
                    cmd.Parameters.AddWithValue("@Direccion", objUser.direccion);
                    cmd.Parameters.AddWithValue("@idSexo", objUser.idSexo);

                    int c = cmd.ExecuteNonQuery();
                    mensaje = $"registro actualizado {c}en Usuarios";

                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }

            }
            ViewBag.mensaje = mensaje;
            return RedirectToAction("Portal","ECommerce");
        }



    }
}
