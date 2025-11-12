using System.Text.Json;
using System.Text.Json.Serialization;
using Veterinaria.Web.Models;

public class ProcedimientoMascotasApiClient
{
    private readonly HttpClient _http;
    private const string BasePath = "/api/ProcedimientoMascotas"; 

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public ProcedimientoMascotasApiClient(HttpClient http) => _http = http;

    public async Task<IEnumerable<ProcedimientoDto>> GetAllAsync(Guid? mascotaId)
    {
        var url = mascotaId is null ? BasePath : $"{BasePath}?mascotaId={mascotaId:D}";
        using var resp = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            throw new HttpRequestException($"GET {url} => {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");
        }

        await using var stream = await resp.Content.ReadAsStreamAsync();
        var data = await JsonSerializer.DeserializeAsync<List<ProcedimientoDto>>(stream, JsonOptions);
        return data ?? new List<ProcedimientoDto>();
    }

    
}
