using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class ProfetionalScheduleModel: Basic
    {
        [Required]
        [Display(Name = "idschedule")]
        [JsonProperty("idschedule")]
        public Guid Id_Agenda_Profesional { get; set; }

        [Required]
        [Display(Name = "fromdate")]
        [JsonProperty("fromdate")]
        public DateTime Fecha_Desde { get; set; }

        [Required]
        [Display(Name = "todate")]
        [JsonProperty("todate")]
        public DateTime Fecha_Hasta { get; set; }

        [Required]
        [Display(Name = "idspeciality")]
        [JsonProperty("idspeciality")]
        public Guid Id_Especialidad { get; set; }

        [Required]
        [Display(Name = "idprofetional")]
        [JsonProperty("idprofetional")]
        public Guid Id_Profesional { get; set; }

        [Required]
        [Display(Name = "identity")]
        [JsonProperty("identity")]
        public Guid Id_Entidad { get; set; }

        [Required]
        [Display(Name = "state")]
        [JsonProperty("state")]
        public bool Estado { get; set; }
    }
}