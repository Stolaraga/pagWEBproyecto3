using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Veterinaria.Web.Models;
using Veterinaria.Web.Services;



namespace Veterinaria.Web.Controllers
{



    public class ClientesController : Controller
    {
        private readonly ClientesApiClient _api;

        public ClientesController(ClientesApiClient api) => _api = api;

        // GET: /Clientes
        public async Task<IActionResult> Index()
        {
            var lista = await _api.GetAllAsync();
            return View(lista);
        }

        // GET: /Clientes/Create
        public IActionResult Create() => View();

        // POST: /Clientes/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteDto model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _api.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"Error al crear: {ex.Message}");
                return View(model);
            }
        }


        // GET: /Clientes/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var cli = await _api.GetByIdAsync(id);
            if (cli is null) return NotFound();
            return View(cli);
        }

        // GET: /Clientes/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var cli = await _api.GetByIdAsync(id);
            if (cli is null) return NotFound();
            return View(cli);
        }

        // POST: /Clientes/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ClienteDto model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var ok = await _api.UpdateAsync(id, model);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "No se pudo actualizar el cliente.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Clientes/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var cli = await _api.GetByIdAsync(id);
            if (cli is null) return NotFound();
            return View(cli);
        }

        // POST: /Clientes/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var ok = await _api.DeleteAsync(id);
            if (!ok)
            {
                var cli = await _api.GetByIdAsync(id);
                if (cli is null) return NotFound();
                ModelState.AddModelError(string.Empty, "No se pudo eliminar el cliente.");
                return View("Delete", cli);
            }

            return RedirectToAction(nameof(Index));
        }
    }




}
