using System.Collections.Generic;
using System.Web.Mvc;
using GestionClientes.Data;
using GestionClientes.Models;

namespace GestionClientes.Controllers
{
    [Authorize]
    public class BitacoraController : Controller
    {
        // solo lectura, la bitacora se llena desde el mantenimiento de clientes
        public ActionResult Index(string buscar)
        {
            List<Bitacora> movimientos = BitacoraDao.Listar(buscar);
            ViewBag.Buscar = buscar;
            return View(movimientos);
        }
    }
}
