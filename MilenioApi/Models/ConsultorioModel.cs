using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class ConsultorioModel
    {
        public string list_Especialidad { get; set; }
        public Guid Id_Entidad { get; set; }
        public Guid Id_Consultorio { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
        [Required]
        public string token { get; set; }
    }
}
