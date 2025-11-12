using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Veterinaria.Web.Models;
using Veterinaria.Web.Services;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Veterinaria.Web.Controllers
{
    [Route("Procedimientos")]
    public sealed class ProcedimientoMascotasController : Controller
    {
        private readonly ClientesApiClient _clientesApi;
        private readonly ProcedimientoMascotasApiClient _procApi;

        public ProcedimientoMascotasController(
            ClientesApiClient clientesApi,
            ProcedimientoMascotasApiClient procApi)
        {
            _clientesApi = clientesApi;
            _procApi = procApi;
        }

        [HttpGet("")]
        public IActionResult Index() => View();

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
                    })
                    .ToList(),
                // Mascotas se llenará por JS según el cliente seleccionado
                Mascotas = new List<SelectListItem>()
            };

            return View(vm);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    ProcedimientoMascotaCreateVm vm,
    [FromForm] string? Tipo,
    [FromForm] decimal? Precio,
    [FromForm] decimal? PesoKg,
    CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                // rehidrata solo Clientes; Mascotas las vuelve a cargar JS al entrar
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

                vm.Clientes = clientes
                    .OrderBy(c => (c.Apellidos ?? "").Trim())
                    .ThenBy(c => (c.Nombre ?? "").Trim())
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = $"{AliasCedula(c)} — {NombreYApe(c)}"
                    })
                    .ToList();

                vm.Mascotas ??= new List<SelectListItem>();
                return View(vm);
            }

            // Usa los valores que vienen del form (calculados en el front)
            var precio = Precio ?? 0M;
            var tipo = string.IsNullOrWhiteSpace(Tipo) ? "Procedimiento" : Tipo;

            await _procApi.CreateAsync(new Veterinaria.Web.Models.ProcedimientoMascotaCreateDto
            {
                MascotaId = vm.MascotaId!.Value,
                ClienteId = vm.ClienteId,
                Fecha = DateTime.Now,
                Precio = precio,
                IvaPorcentaje = 0M,         // ajusta si tu API requiere otro IVA
                Estado = "Agendado",
                Tipo = tipo,       // ← lo envías como texto
                                   // Notas = (opcional) podrías mandar $"Peso: {PesoKg} kg" si quieres registrar
            }, ct);

            TempData["Ok"] = "Procedimiento creado correctamente.";
            return RedirectToAction(nameof(Index));
        }



    }








}

