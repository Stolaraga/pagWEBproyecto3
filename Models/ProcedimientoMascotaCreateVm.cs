using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Veterinaria.Web.Services;


namespace Veterinaria.Web.Models
{

    public sealed class ProcedimientoMascotaCreateVm
    {
        [Required, Display(Name = "Cliente")]
        public Guid? ClienteId { get; set; }

        [Required, Display(Name = "Mascota")]
        public Guid? MascotaId { get; set; }

        //  NUEVO: Veterinario (GUID de Empleados.Id)
        [Required, Display(Name = "Veterinario")]
        public Guid? EmpleadoId { get; set; }

        public List<SelectListItem> Clientes { get; set; } = new();
        public List<SelectListItem> Mascotas { get; set; } = new();
        public List<SelectListItem> Veterinarios { get; set; } = new(); // (se llenará por JS)

        // Procedimiento/Servicio existentes...
        public int? ServicioId { get; set; }
        public string? Codigo { get; set; }
        [Range(0, 100)] public decimal IvaPorcentaje { get; set; } = 13m;
        public decimal? Precio { get; set; }
        public decimal? PesoKg { get; set; }
        [Required] public string Tipo { get; set; } = "Consulta";
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string? Notas { get; set; }
    }
}
