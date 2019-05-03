using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace MilenioApi.Models
{
    public class ScheduleDetailModel : Basic
    {
        [Display(Name = "idscheduledetail")]
        [JsonProperty("idscheduledetail")]
        public Guid Id_Detalle_Agenda { get; set; }
        [Required]
        [Display(Name = "idscheduleagenda")]
        [JsonProperty("idscheduleagenda")]
        public Guid Id_Horario_Agenda { get; set; }
        
        [Display(Name = "fromhour")]
        [JsonProperty("fromhour")]
        public DateTime Hora_Desde { get; set; }
        
        [Display(Name = "tohour")]
        [JsonProperty("tohour")]
        public DateTime Hora_Hasta { get; set; }
        
        [Display(Name = "date")]
        [JsonProperty("date")]
        public DateTime Fecha { get; set; }
        [Display(Name = "idoffice")]
        [JsonProperty("idoffice")]
        public Guid Id_Consultorio { get; set; }
    }
}