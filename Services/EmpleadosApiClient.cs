using System.Net.Http.Json;
using System.Text.Json;
using Veterinaria.Web.Models;



namespace Veterinaria.Web.Services
{


    public class EmpleadosApiClient
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public EmpleadosApiClient(HttpClient http) => _http = http;

        public async Task<IEnumerable<EmpleadoDto>> GetAllAsync()
        {
            var url = "api/empleados";
            var resp = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET {url} => {(int)resp.StatusCode}. {body}");
            return JsonSerializer.Deserialize<IEnumerable<EmpleadoDto>>(body, _json) ?? Enumerable.Empty<EmpleadoDto>();
        }

        public async Task<EmpleadoDto?> GetByIdAsync(Guid id)
        {
            var url = $"api/empleados/{id:D}";
            var resp = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET {url} => {(int)resp.StatusCode}. {body}");
            return JsonSerializer.Deserialize<EmpleadoDto>(body, _json);
        }

        public async Task<bool> CreateAsync(EmpleadoDto dto)
        {
            var resp = await _http.PostAsJsonAsync("api/empleados", dto);
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"POST api/empleados => {(int)resp.StatusCode}. {await resp.Content.ReadAsStringAsync()}");
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, EmpleadoDto dto)
        {
            var resp = await _http.PutAsJsonAsync($"api/empleados/{id:D}", dto);
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"PUT api/empleados/{id:D} => {(int)resp.StatusCode}. {await resp.Content.ReadAsStringAsync()}");
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var resp = await _http.DeleteAsync($"api/empleados/{id:D}");
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"DELETE api/empleados/{id:D} => {(int)resp.StatusCode}. {await resp.Content.ReadAsStringAsync()}");
            return true;
        }
    }




}
