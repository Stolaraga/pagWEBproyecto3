using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;



namespace Veterinaria.Web.Models
{


    public class MascotaDto
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid Id { get; set; }

        [Required]
        [JsonPropertyName("clienteId")]
        public Guid ClienteId { get; set; }

        [Required, StringLength(60)]
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = "";

        [Required, StringLength(30)]
        [JsonPropertyName("especie")]
        public string Especie { get; set; } = "Perro";

        [StringLength(60)]
        [JsonPropertyName("raza")]
        public string? Raza { get; set; }

        [Required, StringLength(15)]
        [JsonPropertyName("sexo")]
        public string Sexo { get; set; } = "Macho";

        // 👇 CLAVE: usar DateOnly? para que el JSON sea solo fecha
        [DataType(DataType.Date)]
        [JsonPropertyName("fechaNacimiento")]
        public DateOnly? FechaNacimiento { get; set; }

        [JsonPropertyName("activo")]
        public bool Activo { get; set; } = true;
    }


}
