using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using Veterinaria.Web.Models;

namespace Veterinaria.Web.Services
{
    public class ProcedimientoMascotasApiClient
    {
        private readonly HttpClient _http;
        public ProcedimientoMascotasApiClient(HttpClient http) => _http = http;

        // DTO que enviamos al API
        public sealed class ProcedimientoMascotaCreateDto
        {
            public Guid MascotaId { get; set; }
            public Guid? ClienteId { get; set; }
            public Guid? EmpleadoId { get; set; }
            public string Tipo { get; set; } = "Consulta"; // nombre del enum
            public DateTime Fecha { get; set; }
            public string? Notas { get; set; }
            public decimal? Precio { get; set; }           // null => API calcula
            public decimal IvaPorcentaje { get; set; } = 13m;
            public string Estado { get; set; } = "Agendado";
        }

        public async Task<bool> CreateAsync(
            ProcedimientoMascotaCreateDto dto,
            int? servicioId = null,
            string? codigo = null,
            decimal? pesoKg = null,
            CancellationToken ct = default)
        {
            var url = "/api/ProcedimientoMascotas";
            var qs = new Dictionary<string, string>();

            if (servicioId is not null) qs["servicioId"] = servicioId.Value.ToString();
            if (!string.IsNullOrWhiteSpace(codigo)) qs["codigo"] = codigo!;
            if (pesoKg is not null) qs["peso"] = (pesoKg ?? 0m).ToString(CultureInfo.InvariantCulture);

            if (qs.Count > 0) url = QueryHelpers.AddQueryString(url, qs);

            using var resp = await _http.PostAsJsonAsync(url, dto, ct);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync(ct);
                throw new HttpRequestException($"POST {url} => {(int)resp.StatusCode}. {body}");
            }
            return true;
        }

        public sealed class CitaCreateDto
        {
            public Guid MascotaId { get; set; }
            public int ServicioId { get; set; }         // si no lo tienes, manda 0
            public Guid VeterinarioId { get; set; }      // = EmpleadoId (GUID)
            public DateTime FechaHora { get; set; }
            public string? Estado { get; set; }
            public string? Notas { get; set; }
        }

        // POST /api/Citas con el formato requerido
        public async Task<Guid> CreateCitaAsync(CitaCreateDto dto, CancellationToken ct = default)
        {
            const string url = "api/citas";
            using var resp = await _http.PostAsJsonAsync(url, dto, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"POST {url} => {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");

            // Si la API devuelve el objeto creado, intenta leer el Id (opcional)
            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("id", out var idProp) &&
                    Guid.TryParse(idProp.GetString(), out var id))
                    return id;
            }
            catch { /* no pasa nada si no viene Id */ }

            return Guid.Empty;
        }

    }
}
