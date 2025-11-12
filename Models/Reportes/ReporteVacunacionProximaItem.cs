using System;


namespace Veterinaria.Web.Models.Reportes
{


    public class ReporteVacunacionProximaItem
    {
        // Cliente
        public Guid ClienteId { get; set; }
        public string Cedula { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Apellidos { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string Email { get; set; } = "";

        // Mascota
        public Guid MascotaId { get; set; }
        public string MascotaNombre { get; set; } = "";
        public string Especie { get; set; } = "";

        // Vacunación
        public DateOnly? FechaUltimaVacunacion { get; set; }
        public DateOnly FechaProximaVacunacion { get; set; }
        public int DiasRestantes { get; set; }
    }


}
