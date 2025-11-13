using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Veterinaria.Web.Models
{
    public sealed class ProcedimientoMascotaEditVm
    {
        public Guid Id { get; set; }

        [Required]
        public Guid MascotaId { get; set; }

        [Required]
        public int? ServicioId { get; set; }

        [Required]
        public Guid? EmpleadoId { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        [Required, RegularExpression("Pendiente|Completada|Cancelada")]
        public string Estado { get; set; } = "Pendiente";

        public string? Notas { get; set; }

        public decimal? PrecioCobrado { get; set; }

        // Para el <select> de veterinarios
        public List<SelectListItem> Veterinarios { get; set; } = new();
    }
}
