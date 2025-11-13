using System.Text.Json;

namespace Veterinaria.Web.Services
{
    public sealed class ReportesApiClient
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public ReportesApiClient(HttpClient http) => _http = http;

        public sealed class ReporteVacunaRow
        {
            public Guid ClienteId { get; set; }
            public string? ClienteNombre { get; set; }
            public Guid MascotaId { get; set; }
            public string? MascotaNombre { get; set; }
            public DateTime Proxima { get; set; }  // viene como "2025-11-16T00:00:00"
        }

        public async Task<IEnumerable<ReporteVacunaRow>> GetVacunacionProximaSemanaAsync(CancellationToken ct = default)
        {
            const string url = "https://localhost:7195/api/Reportes/vacunacion-proxima-semana";
            using var resp = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET {url} => {(int)resp.StatusCode}. {body}");

            return JsonSerializer.Deserialize<IEnumerable<ReporteVacunaRow>>(body, _json)
                   ?? Enumerable.Empty<ReporteVacunaRow>();
        }
    }
}
