using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using GestionClientes.Data;
using GestionClientes.Models;

namespace GestionClientes.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        // listado de clientes con buscador
        public ActionResult Index(string buscar)
        {
            List<Cliente> clientes = ClienteDao.Listar(buscar);
            ViewBag.Buscar = buscar;
            return View(clientes);
        }

        [HttpGet]
        public ActionResult Crear()
        {
            return View(new Cliente());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return View(cliente);
            }

            try
            {
                ClienteDao.Insertar(cliente, User.Identity.Name);
                TempData["Mensaje"] = "Cliente agregado correctamente.";
                return RedirectToAction("Index");
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                ModelState.AddModelError("Documento", "Ya existe un cliente con ese documento.");
                return View(cliente);
            }
        }

        [HttpGet]
        public ActionResult Editar(int id)
        {
            Cliente cliente = ClienteDao.ObtenerPorId(id);

            if (cliente == null)
            {
                return HttpNotFound();
            }

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return View(cliente);
            }

            try
            {
                ClienteDao.Actualizar(cliente, User.Identity.Name);
                TempData["Mensaje"] = "Cliente actualizado correctamente.";
                return RedirectToAction("Index");
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                ModelState.AddModelError("Documento", "Ya existe otro cliente con ese documento.");
                return View(cliente);
            }
            catch (System.InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // se elimina solo por POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id)
        {
            try
            {
                ClienteDao.Eliminar(id, User.Identity.Name);
                TempData["Mensaje"] = "Cliente eliminado correctamente.";
            }
            catch (System.InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
