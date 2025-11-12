using Microsoft.AspNetCore.Mvc;
using Veterinaria.Web.Models;
using Veterinaria.Web.Models.Reportes;
using Veterinaria.Web.Services;



namespace Veterinaria.Web.Controllers
{


    public class ReportesController : Controller
    {
        private readonly ClientesApiClient _clientesApi;
        private readonly MascotasApiClient _mascotasApi;
        private readonly ProcedimientoMascotasApiClient _atencionesApi;

        public ReportesController(
            ClientesApiClient clientesApi,
            MascotasApiClient mascotasApi,
            ProcedimientoMascotasApiClient atencionesApi)
        {
            _clientesApi = clientesApi;
            _mascotasApi = mascotasApi;
            _atencionesApi = atencionesApi;
        }

        // GET: /Reportes/VacunacionAnual
        public async Task<IActionResult> VacunacionAnual()
        {
            // Fecha base
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var (ini, fin) = RangoSemanaQueViene(hoy);
            ViewBag.RangoTexto = $"{ini:yyyy-MM-dd} a {fin:yyyy-MM-dd}";
            ViewBag.Hoy = hoy;

            // Traemos clientes y mascotas
            var clientes = (await _clientesApi.GetAllAsync()).ToList();
            var mascotas = (await _mascotasApi.GetAllAsync(null)).ToList();

            // Para lookup rápido
            var clientesById = clientes.ToDictionary(c => c.Id, c => c);

            var filas = new List<ReporteVacunacionProximaItem>();

           

            // Ordenamos por fecha próxima (y luego por cliente)
            var modelo = filas
                .OrderBy(f => f.FechaProximaVacunacion)
                .ThenBy(f => f.Apellidos)
                .ThenBy(f => f.Nombre)
                .ToList();

            return View(modelo);
        }

        // Lunes a domingo de la "semana que viene" (basado en hoy)
        private static (DateOnly Inicio, DateOnly Fin) RangoSemanaQueViene(DateOnly hoy)
        {
            // DayOfWeek: Sunday=0 ... Saturday=6
            var dow = (int)hoy.ToDateTime(TimeOnly.MinValue).DayOfWeek;
            var daysUntilNextMonday = ((int)DayOfWeek.Monday - dow + 7) % 7;
            if (daysUntilNextMonday == 0) daysUntilNextMonday = 7; // si hoy es lunes, toma el lunes siguiente
            var inicio = hoy.AddDays(daysUntilNextMonday);
            var fin = inicio.AddDays(6);
            return (inicio, fin);
        }
    }

}
