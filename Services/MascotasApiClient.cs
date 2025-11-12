using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Veterinaria.Web.Models;

namespace Veterinaria.Web.Services
{
    public class MascotasApiClient
    {
        private readonly HttpClient _http;

        private static readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public MascotasApiClient(HttpClient http) => _http = http;

        
        public async Task<IEnumerable<MascotaDto>> GetAllAsync(Guid? clientId = null)
        {
            var url = clientId is null ? "/api/mascotas" : $"/api/mascotas?clientId={clientId:D}";
            using var resp = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET {url} => {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");

            return JsonSerializer.Deserialize<IEnumerable<MascotaDto>>(body, _json) ?? Enumerable.Empty<MascotaDto>();
        }

        
        public Task<IEnumerable<MascotaDto>> GetByClienteAsync(Guid clientId) => GetAllAsync(clientId);

        public async Task<MascotaDto?> GetByIdAsync(Guid id)
        {
            var url = $"/api/mascotas/{id:D}";
            using var resp = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET {url} => {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");

            return JsonSerializer.Deserialize<MascotaDto>(body, _json);
        }

        public async Task<bool> CreateAsync(MascotaDto dto)
        {
            const string url = "/api/mascotas";
            using var resp = await _http.PostAsJsonAsync(url, dto);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"POST {url} => {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");

            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, MascotaDto dto)
        {
            var url = $"/api/mascotas/{id:D}";
            using var resp = await _http.PutAsJsonAsync(url, dto);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"PUT {url} => {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var url = $"/api/mascotas/{id:D}";
            using var resp = await _http.DeleteAsync(url);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"DELETE {url} => {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");

            return true;
        }
    }
}
