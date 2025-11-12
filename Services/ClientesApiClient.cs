using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using Veterinaria.Web.Models;
using System.Text.Json;







namespace Veterinaria.Web.Services
{


    public class ClientesApiClient
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };


        public ClientesApiClient(HttpClient http) => _http = http;


        public async Task<IEnumerable<ClienteDto>> GetAllAsync()
        {
            const string url = "/api/Clientes";
            using var resp = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET {url} => {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");

            return JsonSerializer.Deserialize<List<ClienteDto>>(body, _json) ?? new();
        }



        public async Task<ClienteDto?> GetByIdAsync(Guid id)
        {
            var url = $"api/clientes/{id:D}";
            var resp = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"GET {url} => {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");

            var jsonOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<ClienteDto>(body, jsonOpts);
        }



        public async Task<bool> CreateAsync(ClienteDto cliente)
        {
            var resp = await _http.PostAsJsonAsync("api/clientes", cliente);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                throw new HttpRequestException($"POST api/clientes => {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");
            }
            return true;
        }


        public async Task<bool> UpdateAsync(Guid id, ClienteDto cliente)
        {
            var resp = await _http.PutAsJsonAsync($"api/clientes/{id:D}", cliente);
            return resp.IsSuccessStatusCode;
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            var resp = await _http.DeleteAsync($"api/clientes/{id:D}");
            return resp.IsSuccessStatusCode;
        }

    }



}
