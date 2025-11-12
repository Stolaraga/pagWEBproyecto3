using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Veterinaria.Web.Models
{
    public sealed class ProcedimientoMascotaCreateVm
    {
        [Display(Name = "Cliente")]
        [Required(ErrorMessage = "Seleccione un cliente")]
        public Guid? ClienteId { get; set; }

        [Display(Name = "Mascota")]
        [Required(ErrorMessage = "Seleccione una mascota")]
        public Guid? MascotaId { get; set; }

        public List<SelectListItem> Clientes { get; set; } = new();
        public List<SelectListItem> Mascotas { get; set; } = new();
    }
}
