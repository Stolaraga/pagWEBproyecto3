using System.Net.Http.Json;
using System.Text.Json;
using Veterinaria.Web.Models;

namespace Veterinaria.Web.Services
{
    public class ProcedimientoMascotasApiClient
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public ProcedimientoMascotasApiClient(HttpClient http) => _http = http;

        /// <summary>
        /// Lista procedimientos. Puede filtrar por mascotaId. Agrega un parámetro nocache para evitar caching.
        /// </summary>
        public async Task<IEnumerable<ProcedimientoMascotaDto>> GetAllAsync(Guid? mascotaId = null, bool noCache = true)
        {
            var url = "/api/ProcedimientoMascotas";
            var qs = new List<string>();

            if (mascotaId.HasValue && mascotaId.Value != Guid.Empty)
                qs.Add($"mascotaId={mascotaId.Value:D}");

            if (noCache)
                qs.Add($"nocache={DateTimeOffset.UtcNow.Ticks}");

            if (qs.Count > 0)
                url += "?" + string.Join("&", qs);

            using var resp = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET {url} => {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");

            return JsonSerializer.Deserialize<List<ProcedimientoMascotaDto>>(body, _json) ?? new();
        }

        /// <summary>
        /// Obtiene un procedimiento por Id.
        /// </summary>
        public async Task<ProcedimientoMascotaDto?> GetByIdAsync(Guid id)
        {
            var url = $"api/ProcedimientoMascotas/{id:D}";
            using var resp = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET {url} => {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");

            return JsonSerializer.Deserialize<ProcedimientoMascotaDto>(body, _json);
        }

        /// <summary>
        /// Crea un procedimiento.
        /// </summary>
        public async Task<bool> CreateAsync(ProcedimientoMascotaCreateDto dto, CancellationToken ct = default)
        {
            var url = "api/ProcedimientoMascotas";
            using var resp = await _http.PostAsJsonAsync(url, dto, ct);

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync(ct);
                throw new HttpRequestException($"POST {url} => {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");
            }
            return true;
        }

        /// <summary>
        /// Actualiza un procedimiento.
        /// </summary>
        public async Task<bool> UpdateAsync(Guid id, ProcedimientoMascotaUpdateDto dto, CancellationToken ct = default)
        {
            var url = $"api/ProcedimientoMascotas/{id:D}";
            using var resp = await _http.PutAsJsonAsync(url, dto, ct);

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync(ct);
                throw new HttpRequestException($"PUT {url} => {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");
            }
            return true;
        }

        /// <summary>
        /// Elimina un procedimiento.
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var url = $"api/ProcedimientoMascotas/{id:D}";
            using var resp = await _http.DeleteAsync(url, ct);

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync(ct);
                throw new HttpRequestException($"DELETE {url} => {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");
            }
            return true;
        }
    }
}
