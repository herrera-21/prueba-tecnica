using System.Web.Mvc;
using System.Web.Security;
using GestionClientes.Data;
using GestionClientes.Models;

namespace GestionClientes.Controllers
{
    public class AccountController : Controller
    {
        // muestra el formulario de login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // revisa el usuario en la base de datos
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel modelo, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            if (!UsuarioDao.ValidarLogin(modelo.NombreUsuario, modelo.Password))
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos");
                return View(modelo);
            }

            FormsAuthentication.SetAuthCookie(modelo.NombreUsuario, false);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Clientes");
        }

        // cierra la sesión del usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}
