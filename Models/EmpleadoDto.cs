using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;




namespace Veterinaria.Web.Models
{

    public class EmpleadoDto
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid Id { get; set; }

        [Required, StringLength(60)]
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = "";

        [Required, StringLength(60)]
        [JsonPropertyName("apellidos")]
        public string Apellidos { get; set; } = "";

        [Required, EmailAddress, StringLength(120)]
        [JsonPropertyName("email")]
        public string Email { get; set; } = "";

        [Required, StringLength(25)]
        [JsonPropertyName("telefono")]
        public string Telefono { get; set; } = "";

        [Required, StringLength(40)]
        [JsonPropertyName("rol")]
        public string Rol { get; set; } = "Veterinario";

        // La API usa DateOnly (solo fecha)
        [DataType(DataType.Date)]
        [JsonPropertyName("fechaIngreso")]
        public DateOnly? FechaIngreso { get; set; }

        [JsonPropertyName("activo")]
        public bool Activo { get; set; } = true;

        // La API manda "nombreCompleto" en GET; lo partimos en Nombre + Apellidos
        [JsonPropertyName("nombreCompleto")]
        public string? NombreCompleto
        {
            get => null;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) return;
                var t = value.Trim();
                var idx = t.IndexOf(' ');
                if (idx > 0)
                {
                    Nombre = t[..idx].Trim();
                    Apellidos = t[(idx + 1)..].Trim();
                }
                else
                {
                    Nombre = t; Apellidos = "";
                }
            }
        }
    }


}
