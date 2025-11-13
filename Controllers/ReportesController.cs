using Microsoft.AspNetCore.Mvc;
using Veterinaria.Web.Models;
using Veterinaria.Web.Models.Reportes;
using Veterinaria.Web.Services;



namespace Veterinaria.Web.Controllers
{



    [Route("Reportes")]

    
    public sealed class ReportesController : Controller
        {
            private readonly ReportesApiClient _reportesApi;
            public ReportesController(ReportesApiClient reportesApi) => _reportesApi = reportesApi;

            [HttpGet("VacunacionAnual")]
            public async Task<IActionResult> VacunacionAnual(CancellationToken ct)
            {
                var rows = await _reportesApi.GetVacunacionProximaSemanaAsync(ct);
                return View(rows); // <- pásale el IEnumerable a la vista
            }

    }

}
