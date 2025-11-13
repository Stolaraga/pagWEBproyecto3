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

            
            var rows = citas.Select(c => new Veterinaria.Web.Models.ProcedimientoMascotaDto
            {
                Id = c.Id,
                MascotaId = c.MascotaId,
                Tipo = c.Servicio,
                Fecha = c.FechaHora,
                Estado = string.IsNullOrWhiteSpace(c.Estado) ? "Pendiente" : c.Estado!,
                NombreMascota = c.NombreMascota
            });

            return View(rows);
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
                Notas = vm.Notas
            }, ct);



            TempData["Ok"] = "Procedimiento creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
