using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace MilenioApi.Models
{
    public class ScheduleAgendaModel : Basic
    {       
        [Display(Name = "idscheduleagenda")]
        [JsonProperty("idscheduleagenda")]
        public Guid Id_Horario_Agenda { get; set; }

        [Required]
        [Display(Name = "idprofetionalschedule")]
        [JsonProperty("idprofetionalschedule")]
        public Guid Id_Agenda_Profesional { get; set; }

        [Required]
        [Display(Name = "fromhour")]
        [JsonProperty("fromhour")]
        public DateTime Hora_Desde { get; set; }

        [Required]
        [Display(Name = "tohour")]
        [JsonProperty("tohour")]
        public DateTime Hora_Hasta { get; set; }

        [Required]
        [Display(Name = "day")]
        [JsonProperty("day")]
        public string Dia { get; set; }

        [Required]
        [Display(Name = "duration")]
        [JsonProperty("duration")]
        public int Duracion { get; set; }

        [Required]
        [Display(Name = "idoffice")]
        [JsonProperty("idoffice")]
        public Guid Id_Consultorio { get; set; }

        [Display(Name = "status")]
        [JsonProperty("status")]
        public bool Estado { get; set; }
    }
}