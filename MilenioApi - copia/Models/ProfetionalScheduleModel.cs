using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class ProfetionalScheduleModel : Basic
    {
        [Display(Name = "idschedule")]
        [JsonProperty("idschedule")]
        public Guid Id_Agenda_Profesional { get; set; }

        [Required]

        [Display(Name = "fromdate")]
        [JsonProperty("fromdate")]
        [DataType(DataType.Date)]
        public DateTime Fecha_Desde { get; set; }

        [Required]
        [Display(Name = "todate")]
        [JsonProperty("todate")]
        [DataType(DataType.Date)]
        public DateTime Fecha_Hasta { get; set; }

        [Required]
        [Display(Name = "idspeciality")]
        [JsonProperty("idspeciality")]
        public Guid Id_Especialidad { get; set; }

        [Required]
        [Display(Name = "idprofetional")]
        [JsonProperty("idprofetional")]
        public Guid Id_Profesional { get; set; }

        
        [Display(Name = "identity")]
        [JsonProperty("identity")]
        public Guid Id_Entidad { get; set; }

      
        [Display(Name = "status")]
        [JsonProperty("status")]
        public bool Estado { get; set; }

        public bool between { get; set; }
    }
}