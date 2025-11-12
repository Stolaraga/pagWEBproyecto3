
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Veterinaria.Web.Models;
using Veterinaria.Web.Services;




namespace Veterinaria.Web.Controllers
{

    public class MascotasController : Controller
    {
        private readonly MascotasApiClient _apiMascotas;
        private readonly ClientesApiClient _apiClientes;

        // Catálogos simples (podés moverlos a config si querés)
        private static readonly string[] _sexos = new[] { "Macho", "Hembra" };
        private static readonly string[] _especies = new[] { "Perro", "Gato", "Ave","Reptil", "Roedor" , "Otro" };

        public MascotasController(MascotasApiClient apiMascotas, ClientesApiClient apiClientes)
        {
            _apiMascotas = apiMascotas;
            _apiClientes = apiClientes;
        }

        private async Task CargarCombosAsync(bool incluirClientes = false)
        {
            ViewBag.Sexos = _sexos;
            ViewBag.Especies = _especies;

            if (incluirClientes)
            {
                var clientes = await _apiClientes.GetAllAsync();
                var items = clientes
                    .Select(c => new
                    {
                        c.Id,
                        Nombre = $"{c.Cedula} - {c.Nombre} {c.Apellidos}"
                    })
                    .ToList();

                ViewBag.Clientes = new SelectList(items, "Id", "Nombre");
            }
        }

        // GET: /Mascotas?clienteId={guid}
        public async Task<IActionResult> Index(Guid? clienteId)
        {
            ViewBag.ClienteId = clienteId;
            var lista = await _apiMascotas.GetAllAsync(clienteId);
            return View(lista);
        }

        // GET: /Mascotas/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var m = await _apiMascotas.GetByIdAsync(id);
            if (m is null) return NotFound();
            return View(m);
        }

        // GET: /Mascotas/Create?clienteId={guid}
        public async Task<IActionResult> Create(Guid? clienteId)
        {
            // Si viene clienteId, no necesitamos el selector de clientes (solo mostramos readonly)
            var incluirSelectorClientes = !(clienteId.HasValue && clienteId.Value != Guid.Empty);
            await CargarCombosAsync(incluirSelectorClientes);

            var modelo = new MascotaDto
            {
                Activo = true
            };

            if (clienteId is Guid c && c != Guid.Empty)
            {
                modelo.ClienteId = c;
                ViewBag.TieneClientePrefijado = true;
            }
            else
            {
                ViewBag.TieneClientePrefijado = false;
            }

            return View(modelo);
        }

        // POST: /Mascotas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MascotaDto model)
        {
            // En Create necesitamos combos y, si ClienteId no viene prefijado, el selector de clientes
            var incluirSelectorClientes = model.ClienteId == Guid.Empty;
            await CargarCombosAsync(incluirSelectorClientes);

            if (model.ClienteId == Guid.Empty)
                ModelState.AddModelError(nameof(model.ClienteId), "Debe seleccionar un cliente.");

            if (!ModelState.IsValid) return View(model);

            try
            {
                await _apiMascotas.CreateAsync(model);
                TempData["Ok"] = "Mascota creada.";
                return RedirectToAction(nameof(Index), new { clienteId = model.ClienteId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: /Mascotas/Edit/{id}

        public async Task<IActionResult> Edit(Guid id)
        {
            var m = await _apiMascotas.GetByIdAsync(id);
            if (m is null) return NotFound();

            // ⬇️ Cargar combos + selector de clientes
            await CargarCombosAsync(incluirClientes: true);
            return View(m);
        }

        // POST: /Mascotas/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, MascotaDto model)
        {
            if (id != model.Id) return BadRequest();

            // ⬇️ Volver a cargar combos + selector de clientes si hubo error de validación
            await CargarCombosAsync(incluirClientes: true);

            if (model.ClienteId == Guid.Empty)
                ModelState.AddModelError(nameof(model.ClienteId), "Debe seleccionar un cliente.");

            if (!ModelState.IsValid) return View(model);

            try
            {
                await _apiMascotas.UpdateAsync(id, model);
                TempData["Ok"] = "Mascota actualizada.";
                return RedirectToAction(nameof(Index), new { clienteId = model.ClienteId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }


        // GET: /Mascotas/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var m = await _apiMascotas.GetByIdAsync(id);
            if (m is null) return NotFound();
            return View(m);
        }

        // POST: /Mascotas/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var m = await _apiMascotas.GetByIdAsync(id);
            if (m is null) return NotFound();

            try
            {
                await _apiMascotas.DeleteAsync(id);
                TempData["Ok"] = "Mascota eliminada.";
                return RedirectToAction(nameof(Index), new { clienteId = m.ClienteId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Delete", m);
            }
        }
    }





}

