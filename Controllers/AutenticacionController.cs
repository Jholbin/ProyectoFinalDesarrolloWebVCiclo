using ProyectoFinalDesarrolloWeb.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace ProyectoFinalDesarrolloWeb.Controllers
{
    public class AutenticacionController : Controller
    {
        private readonly IConfiguration _IConfig;

        public AutenticacionController(IConfiguration iConfig)
        {
            _IConfig = iConfig;

        }

        [HttpGet]
        public IActionResult Index()
        {
            User usu = new User();
            return View(usu);
        }

        [HttpPost]
        public async Task<IActionResult> Index(User reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_IConfig["ConnectionStrings:cn"]))
            {
                if (string.IsNullOrEmpty(reg.usuario) || string.IsNullOrEmpty(reg.clave))
                {
                    ModelState.AddModelError("", "Ingresar los datos solicitados");
                }
                else
                {
                    try
                    {
                        cn.Open();
                        SqlCommand cmd = new SqlCommand("sp_seguridad_usuario", cn);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@usuario", reg.usuario);
                        cmd.Parameters.AddWithValue("@contra", reg.clave);
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.Read())
                        {
                            string userType = dr["desTipo"].ToString();
                            int idUsuario = Convert.ToInt32(dr["idusuario"]);

                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, reg.usuario),
                                new Claim(ClaimTypes.Role, userType),
                                new Claim("desTipo", userType ),
                                new Claim("idusuario", idUsuario.ToString())

                            };

                            ViewBag.mensaje = "reg.usuario";
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                            //redireccionamos
                            if (userType == "admin")
                            {
                                return RedirectToAction("ListadoLibros", "Libro");
                            }
                            else if (userType == "cliente")
                            {
                                return RedirectToAction("Portal", "ECommerce");
                            }
                            else
                            {
                                
                                ModelState.AddModelError("", "Tipo de usuario no válido");
                            }

                        }
                        else
                        {
                            ModelState.AddModelError("", "Datos no Validos");
                        }
                    }
                    catch (Exception ex)
                    {

                        mensaje = ex.Message;

                    }
                }
            }
            ViewBag.mensaje = mensaje;
            return View();
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            //redireccionamos 
            return RedirectToAction("Index", "Autenticacion");

        }

        public async Task<IActionResult> Mensaje()
        {
            return View("Mensaje");

        }
    }
}