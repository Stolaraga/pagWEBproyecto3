using Microsoft.AspNetCore.Mvc;
using Veterinaria.Web.Services;
using Veterinaria.Web.Models;
using System;
using System.Threading.Tasks;

namespace Veterinaria.Web.Controllers
{
    public class ProcedimientosController : Controller
    {
        private readonly ProcedimientoMascotasApiClient _procedimientosApi;
        private readonly ClientesApiClient _clientesApi;
        private readonly MascotasApiClient _mascotasApi;

        public ProcedimientosController(
            ProcedimientoMascotasApiClient procedimientosApi,
            ClientesApiClient clientesApi,
            MascotasApiClient mascotasApi)
        {
            _procedimientosApi = procedimientosApi;
            _clientesApi = clientesApi;
            _mascotasApi = mascotasApi;
        }

        // Vistas
        [HttpGet]
        public async Task<IActionResult> Index(Guid? mascotaId)
        {
            var lista = await _procedimientosApi.GetAllAsync(mascotaId);
            return View(lista); // Views/Procedimientos/Index.cshtml
        }

        [HttpGet]
        public IActionResult Create() => View(); // Views/Procedimientos/Create.cshtml

        // ====== ENDPOINTS JSON (mismo host MVC) ======

        [HttpGet]
        public async Task<IActionResult> ClientesJson()
            => Ok(await _clientesApi.GetAllAsync());

        [HttpGet]
        public async Task<IActionResult> MascotasPorCliente(Guid clienteId)
            => Ok(await _mascotasApi.GetByClienteAsync(clienteId));

        [HttpGet]
        public async Task<IActionResult> MascotaDetalle(Guid id)
        {
            var item = await _mascotasApi.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }
    }
}
