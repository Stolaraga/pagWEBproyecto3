using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Veterinaria.Web.Models;
using Veterinaria.Web.Services;

namespace Veterinaria.Web.Controllers
{




    [Route("Procedimientos")]
    public sealed class ProcedimientoMascotasController : Controller
    {
        private readonly ClientesApiClient _clientesApi;
        private readonly ProcedimientoMascotasApiClient _procApi;
        private readonly EmpleadosApiClient _empleadosApi;

        public ProcedimientoMascotasController(
            ClientesApiClient clientesApi,
            EmpleadosApiClient empleadosApi,
            ProcedimientoMascotasApiClient procApi)
        {
            _clientesApi = clientesApi;
            _empleadosApi = empleadosApi;
            _procApi = procApi;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(Guid? mascotaId, string? estado, CancellationToken ct)
        {
            
            var citas = await _procApi.GetCitasAsync(mascotaId, estado, null, null, ct);

            return View(citas);
        }

        [HttpGet("Details/{id:guid}")]
        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            var cita = await _procApi.GetCitaAsync(id, ct);
            if (cita is null) return NotFound();
            return View(cita);
        }

        [HttpGet("Edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
        {
            var cita = await _procApi.GetCitaAsync(id, ct);
            if (cita is null) return NotFound();

            ViewBag.Estados = new[]
            {
        new SelectListItem("Pendiente",  "Pendiente",  cita.Estado == "Pendiente"),
        new SelectListItem("Completada", "Completada", cita.Estado == "Completada"),
        new SelectListItem("Cancelada",  "Cancelada",  cita.Estado == "Cancelada"),
    };

            // Usamos directamente el DTO de lectura de Citas del ApiClient
            return View(cita);
        }

        [HttpPost("Edit/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    Guid id,
    [FromForm] string Estado,
    [FromForm] string? Notas,
    [FromForm] DateTime? FechaHora,   // ← NUEVO: opcional
    [FromForm] int? ServicioId,       // ← NUEVO: opcional
    CancellationToken ct)
        {
            // Normaliza a los 3 valores permitidos por tu CHECK en SQL
            static string NormEstado(string? e) => (e ?? "").Trim().ToLowerInvariant() switch
            {
                "pendiente" => "Pendiente",
                "completada" => "Completada",
                "cancelada" => "Cancelada",
                _ => "Pendiente"
            };

            var dto = new Veterinaria.Web.Services.ProcedimientoMascotasApiClient.CitaUpdateDto
            {
                Estado = NormEstado(Estado),
                Notas = Notas
                
            };

            // Si el usuario ingresó una fecha, la enviamos; si quedó vacía, NO la tocamos (se conserva en BD)
            if (FechaHora.HasValue)
            {
            
            
                dto.FechaHora = FechaHora.Value; // mantener simple si tu API guarda hora local
            }

            
            if (ServicioId.HasValue && ServicioId.Value > 0)
            {
                dto.ServicioId = ServicioId.Value;
            }

            await _procApi.UpdateCitaAsync(id, dto, ct);

            TempData["Ok"] = "Cita actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }





        [HttpGet("Create")]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var clientes = await _clientesApi.GetAllAsync();

            static string AliasCedula(ClienteDto c)
                => string.IsNullOrWhiteSpace(c.Cedula) ? c.Id.ToString("N")[..8] : c.Cedula;

            static string NombreYApe(ClienteDto c)
            {
                var n = (c.Nombre ?? "").Trim();
                var a = (c.Apellidos ?? "").Trim();
                var nombre = (n + " " + a).Trim();
                return string.IsNullOrWhiteSpace(nombre) ? "(Sin nombre)" : nombre;
            }

            var vm = new ProcedimientoMascotaCreateVm
            {
                Clientes = clientes
                    .OrderBy(c => (c.Apellidos ?? "").Trim())
                    .ThenBy(c => (c.Nombre ?? "").Trim())
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = $"{AliasCedula(c)} — {NombreYApe(c)}"
                    }).ToList(),

                Veterinarios = await CargarVeterinariosAsync(ct)      // ← AQUÍ
            };

            return View(vm);
        }

        [HttpPost("Delete/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            try
            {
                await _procApi.DeleteCitaAsync(id, ct);
                TempData["Ok"] = "Cita eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Err"] = $"No se pudo eliminar la cita: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }




        private async Task<List<SelectListItem>> CargarVeterinariosAsync(CancellationToken ct)


        {
            var empleados = await _empleadosApi.GetAllAsync();

            static string? GetProp(object obj, params string[] names)
            {
                var t = obj.GetType();
                foreach (var n in names)
                {
                    var p = t.GetProperty(n);
                    if (p is null) continue;
                    var v = p.GetValue(obj) as string;
                    if (!string.IsNullOrWhiteSpace(v)) return v.Trim();
                }
                return null;
            }

            return empleados
                .Select(e =>
                {
                    // intenta varias convenciones: Nombre/Apellidos, Nombres/Apellidos, NombreCompleto, etc.
                    var nombre = GetProp(e, "Nombre", "Nombres", "FirstName", "PrimerNombre");
                    var apellidos = GetProp(e, "Apellidos", "Apellido", "LastName", "ApellidosCompletos");
                    var display = $"{nombre} {apellidos}".Trim();

                    if (string.IsNullOrWhiteSpace(display))
                        display = GetProp(e, "NombreCompleto", "FullName")
                                  ?? GetProp(e, "Email", "Correo")
                                  ?? GetProp(e, "Cedula", "Documento")
                                  ?? e.Id.ToString("N")[..8]; // fallback visible

                    return new SelectListItem
                    {
                        Value = e.Id.ToString(),   // GUID que pide tu API
                        Text = display
                    };
                })
                .OrderBy(x => x.Text)
                .ToList();
        }





        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
                ProcedimientoMascotaCreateVm vm,
                [FromForm] string? Estado,                  
                CancellationToken ct)
        {

            static string NormEstado(string? e) => (e ?? "").Trim().ToLowerInvariant() switch
            {
                "pendiente" => "Pendiente",
                "completada" => "Completada",
                "cancelada" => "Cancelada",
                _ => "Pendiente"
            };
            var estado = NormEstado(Estado);


            if (!ModelState.IsValid)
            {
                var clientes = await _clientesApi.GetAllAsync();
                vm.Clientes = clientes
                    .OrderBy(c => (c.Apellidos ?? "").Trim())
                    .ThenBy(c => (c.Nombre ?? "").Trim())
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = $"{(string.IsNullOrWhiteSpace(c.Cedula) ? c.Id.ToString("N")[..8] : c.Cedula)} — {(c.Nombre + " " + c.Apellidos).Trim()}"
                    }).ToList();

                vm.Veterinarios = await CargarVeterinariosAsync(ct);  // ← AQUÍ
                vm.Mascotas ??= new();
                return View(vm);
            }


            await _procApi.CreateCitaAsync(new ProcedimientoMascotasApiClient.CitaCreateDto
            {
                MascotaId = vm.MascotaId!.Value,
                ServicioId = vm.ServicioId ?? 0,     
                VeterinarioId = vm.EmpleadoId!.Value,
                FechaHora = vm.Fecha,
                Estado = estado,                 
                Notas = vm.Notas,
                PrecioCobrado = null,
                PesoKg = vm.PesoKg


            }, ct);


         



            TempData["Ok"] = "Procedimiento creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
