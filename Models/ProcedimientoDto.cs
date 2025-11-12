using System.Text.Json.Serialization;

namespace Veterinaria.Web.Models
{


    public sealed class ProcedimientoDto
    {
        public Guid Id { get; set; }
        public Guid MascotaId { get; set; }
        public Guid? ClienteId { get; set; }
        public Guid? EmpleadoId { get; set; }

        // La API serialize el enum como texto; aquí lo tratamos como string para simplicidad
        public string? Tipo { get; set; }

        public DateTime Fecha { get; set; }
        public string? Notas { get; set; }

        // La API envía camelCase ("nombreMascota"). Con PropertyNameCaseInsensitive se mapea igual.
        public string? NombreMascota { get; set; }

        // Por si alguna versión vieja devolviera "mascotaNombre", lo conservamos (no usado en la vista)
        public string? MascotaNombre { get; set; }

        public decimal IvaPorcentaje { get; set; }

        public string Estado { get; set; } = "Agendado";
        public decimal Precio { get; set; }
    }

}
