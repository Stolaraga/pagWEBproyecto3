using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Veterinaria.Web.Models;
using Veterinaria.Web.Services;

namespace Veterinaria.Web.Controllers
{


    public class EmpleadosController : Controller
    {
        private readonly EmpleadosApiClient _api;

        // Catálogo simple de roles (ajústalo a tus reglas)
        private static readonly string[] _roles = new[] { "Veterinario", "Asistente", "Administracion" , "Groomer" , "Otro" };

        public EmpleadosController(EmpleadosApiClient api) => _api = api;

        private void CargarRoles() => ViewBag.Roles = new SelectList(_roles);

        // GET: /Empleados
        public async Task<IActionResult> Index()
        {
            var lista = await _api.GetAllAsync();
            return View(lista);
        }

        // GET: /Empleados/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var e = await _api.GetByIdAsync(id);
            if (e is null) return NotFound();
            return View(e);
        }

        // GET: /Empleados/Create
        public IActionResult Create()
        {
            CargarRoles();
            return View(new EmpleadoDto { Activo = true, Rol = "Veterinario" });
        }

        // POST: /Empleados/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmpleadoDto model)
        {
            CargarRoles();
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _api.CreateAsync(model);
                TempData["Ok"] = "Empleado creado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: /Empleados/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var e = await _api.GetByIdAsync(id);
            if (e is null) return NotFound();
            CargarRoles();
            return View(e);
        }

        // POST: /Empleados/Edit/{id}
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EmpleadoDto model)
        {
            if (id != model.Id) return BadRequest();
            CargarRoles();
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _api.UpdateAsync(id, model);
                TempData["Ok"] = "Empleado actualizado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: /Empleados/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var e = await _api.GetByIdAsync(id);
            if (e is null) return NotFound();
            return View(e);
        }

        // POST: /Empleados/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var e = await _api.GetByIdAsync(id);
            if (e is null) return NotFound();

            try
            {
                await _api.DeleteAsync(id);
                TempData["Ok"] = "Empleado eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Delete", e);
            }
        }
    }


}
