using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class OfficeModel : Basic
    {
        public string list_Especialidad { get; set; }

        [Display(Name = "idoffice")]
        [JsonProperty("idoffice")]
        public Guid Id_Consultorio { get; set; }

        [Display(Name = "name")]
        [JsonProperty("name")]
        [Required]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "description")]
        [JsonProperty("description")]
        public string Descripcion { get; set; }

        [Required]
        [Display(Name = "status")]
        [JsonProperty("status")]
        public bool Estado { get; set; }
    }
}
