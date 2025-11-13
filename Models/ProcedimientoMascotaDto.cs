using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Veterinaria.Web.Models
{
    public sealed class ProcedimientoMascotaDto
    {
        public Guid Id { get; set; }
        public Guid MascotaId { get; set; }

        public Guid? ClienteId { get; set; }
        public Guid? EmpleadoId { get; set; }            // GUID del veterinario (legacy)

        // ← Para compatibilidad: la API de citas nos da el nombre del servicio
        //    y su Id. Los usamos para la grilla (Index) y futuros detalles/edición.
        public int ServicioId { get; set; }             // NUEVO
        public string? Servicio { get; set; }            // NUEVO (nombre del servicio)

        // La API serializa el enum como texto; aquí lo tratamos como string por simplicidad
        public string? Tipo { get; set; }                // (se mantiene por compatibilidad)

        public DateTime Fecha { get; set; }
        public string? Notas { get; set; }

        // La API envía camelCase ("nombreMascota"). Con PropertyNameCaseInsensitive se mapea igual.
        public string? NombreMascota { get; set; }

        // Por si alguna versión vieja devolviera "mascotaNombre"
        public string? MascotaNombre { get; set; }

        // Info del veterinario que ya devuelve /api/citas
        public Guid? VeterinarioId { get; set; }       // NUEVO (si lo necesitas en detalles/edición)
        public string? Veterinario { get; set; }       // NUEVO (nombre del veterinario)

        public decimal IvaPorcentaje { get; set; }

        public string Estado { get; set; } = "Agendado";

        // Precio (legacy). Lo dejamos nullable para no chocar con Citas.
        public decimal? Precio { get; set; }             // CAMBIO: ahora nullable

        // Precio cobrado real que viene de /api/citas
        public decimal? PrecioCobrado { get; set; }      // NUEVO (ya lo estabas usando)
    }

    // Estos DTOs se mantienen por compatibilidad con vistas/acciones que
    // aún los usen. No afectan el listado Index (que viene de /api/citas).

    public sealed class ProcedimientoMascotaCreateDto
    {
        [Required]
        public Guid MascotaId { get; set; }

        public Guid? ClienteId { get; set; }

        // Si en algún punto quieres mandar explícito el veterinario:
        public Guid? EmpleadoId { get; set; }

        // En tu API "Tipo" llega como texto; lo dejamos string.
        public string? Tipo { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        public string? Notas { get; set; }

        public decimal IvaPorcentaje { get; set; }

        // Valor por defecto razonable; ajusta si tu API impone otro
        public string Estado { get; set; } = "Agendado";

        // Legacy: lo dejamos opcional para no chocar con el flujo vía /api/citas
        public decimal? Precio { get; set; }             // CAMBIO: antes [Required]
    }

    public sealed class ProcedimientoMascotaUpdateDto
    {
        [Required]
        public Guid MascotaId { get; set; }

        public Guid? ClienteId { get; set; }
        public Guid? EmpleadoId { get; set; }

        public string? Tipo { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        public string? Notas { get; set; }

        public decimal IvaPorcentaje { get; set; }

        public string Estado { get; set; } = "Agendado";

        // Lo dejamos nullable para permitir editar otros campos sin forzar precio
        public decimal? PrecioCobrado { get; set; }      // CAMBIO: ya no [Required]
    }
}
