using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;



namespace Veterinaria.Web.Models
{



    public class ClienteDto
    {
        // No enviar Id en Create
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid Id { get; set; }

        [JsonPropertyName("cedula")]
        public string Cedula { get; set; } = "";

        // La API espera nombre + apellidos en POST/PUT
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = "";

        [JsonPropertyName("apellidos")]
        public string Apellidos { get; set; } = "";

        [JsonPropertyName("email")]
        public string Email { get; set; } = "";

        [JsonPropertyName("telefono")]
        public string Telefono { get; set; } = "";

        [JsonPropertyName("canalPreferido")]
        public string CanalPreferido { get; set; } = "Telefono";

        // En la API se llama "direccion"
        [JsonPropertyName("direccion")]
        public string Direccion { get; set; } = "";

        [JsonPropertyName("activo")]
        public bool Activo { get; set; } = true;

        // No enviar si es null
        [JsonPropertyName("fechaRegistro")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? FechaRegistro { get; set; }

        //  La API en GET envía "nombreCompleto". Lo mapeamos SOLO para lectura.
        // Hacemos que el getter devuelva null (para no serializarlo) y el setter
        // divida el valor en Nombre y Apellidos.
        [JsonPropertyName("nombreCompleto")]
        public string? NombreCompleto
        {
            get => null; 
            set
            {
                if (string.IsNullOrWhiteSpace(value)) return;
                // Partimos en 2: primer "token" = Nombre, resto = Apellidos
                var trimmed = value.Trim();
                var idx = trimmed.IndexOf(' ');
                if (idx > 0)
                {
                    Nombre = trimmed[..idx].Trim();
                    Apellidos = trimmed[(idx + 1)..].Trim();
                }
                else
                {
                    Nombre = trimmed;
                    Apellidos = "";
                }
            }
        }
    }
}



