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
            public int ServicioId { get; set; }         
            public Guid VeterinarioId { get; set; }     
            public DateTime FechaHora { get; set; }
            public string? Estado { get; set; }
            public string? Notas { get; set; }

            public decimal? PrecioCobrado { get; set; } 
            public decimal? PesoKg { get; set; }

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

        public async Task<IEnumerable<ProcedimientoMascotaDto>> GetAllAsync(
            Guid? mascotaId = null,
            string? estado = null,
            CancellationToken ct = default)
        {
            var url = "/api/ProcedimientoMascotas";
            var qs = new Dictionary<string, string>();
            if (mascotaId is not null) qs["mascotaId"] = mascotaId.Value.ToString();
            if (!string.IsNullOrWhiteSpace(estado)) qs["estado"] = estado!;
            if (qs.Count > 0) url = QueryHelpers.AddQueryString(url, qs);

            using var resp = await _http.GetAsync(url, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET {url} => {(int)resp.StatusCode}. {body}");

            return JsonSerializer.Deserialize<IEnumerable<ProcedimientoMascotaDto>>(
                       body,
                       new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                   ) ?? Enumerable.Empty<ProcedimientoMascotaDto>();
        }

        public async Task<CitaReadDto?> GetCitaAsync(Guid id, CancellationToken ct = default)
        {
            var url = $"api/citas/{id:D}";
            using var resp = await _http.GetAsync(url, ct);

            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET {url} => {(int)resp.StatusCode}. {body}");

            return System.Text.Json.JsonSerializer.Deserialize<CitaReadDto>(
                body,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }


        public async Task<bool> UpdateCitaAsync(Guid id, CitaUpdateDto dto, CancellationToken ct = default)
        {
            var url = $"api/citas/{id:D}";

            var payload = new
            {
                estado = dto.Estado,
                notas = dto.Notas,
                fechaHora = dto.FechaHora,          // <-- AHORA SÍ SE ENVÍA
                precioCobrado = dto.PrecioCobrado,
                pesoKg = dto.PesoKg
            };

            using var resp = await _http.PutAsJsonAsync(url, payload, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"PUT {url} => {(int)resp.StatusCode}. {body}");
            return true;
        }




        public sealed class CitaReadDto
        {
            public Guid Id { get; set; }
            public Guid MascotaId { get; set; }
            public int ServicioId { get; set; }
            public Guid VeterinarioId { get; set; }
            public DateTime FechaHora { get; set; }
            public string? Estado { get; set; }
            public string? Notas { get; set; }

            // vienen por JOIN en el repo de Citas
            public string? NombreMascota { get; set; }
            public string? Servicio { get; set; }
            public string? Veterinario { get; set; }
            public decimal? PrecioCobrado { get; set; }
            public decimal? PesoKg { get; init; }
        }

        public sealed class CitaUpdateDto
        {
            public Guid? MascotaId { get; set; }
            public int? ServicioId { get; set; }          
            public Guid? VeterinarioId { get; set; }
            public DateTime? FechaHora { get; set; }       
            public string? Notas { get; set; }
            public string? Estado { get; set; }
            public decimal? PrecioCobrado { get; set; }
            public decimal? PesoKg { get; set; }
        }


        public async Task<IEnumerable<CitaReadDto>> GetCitasAsync(
            Guid? mascotaId = null,
            string? estado = null,
            DateTime? desde = null,
            DateTime? hasta = null,
            CancellationToken ct = default)
        {
            var url = "/api/citas";
            var qs = new Dictionary<string, string>();
            if (mascotaId is not null) qs["mascotaId"] = mascotaId.Value.ToString();
            if (!string.IsNullOrWhiteSpace(estado)) qs["estado"] = estado!;
            if (desde is not null) qs["desde"] = ((DateTime)desde!).ToString("o");
            if (hasta is not null) qs["hasta"] = ((DateTime)hasta!).ToString("o");
            if (qs.Count > 0) url = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(url, qs);

            using var resp = await _http.GetAsync(url, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET {url} => {(int)resp.StatusCode}. {body}");

            return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<CitaReadDto>>(
                       body,
                       new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                   ) ?? Enumerable.Empty<CitaReadDto>();
        }

        public async Task<bool> DeleteCitaAsync(Guid id, CancellationToken ct = default)
        {
            var url = $"api/citas/{id:D}";
            using var resp = await _http.DeleteAsync(url, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"DELETE {url} => {(int)resp.StatusCode}. {body}");
            return true;
        }



    }
}
