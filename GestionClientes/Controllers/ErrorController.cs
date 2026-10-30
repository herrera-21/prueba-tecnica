using System.Web.Mvc;

namespace GestionClientes.Controllers
{
    public class ErrorController : Controller
    {
        // pagina amigable para cuando ocurre un error en produccion
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
    }
}
