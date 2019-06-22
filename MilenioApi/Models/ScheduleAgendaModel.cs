using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace MilenioApi.Models
{
    public class ScheduleAgendaModel : Basic
    {
        private string idday = string.Empty;
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
        [Display(Name = "idday")]
        [JsonProperty("idday")]
        public string Dia
        {
            get
            {
                //return day;
                switch (idday)
                {
                    case "Monday":
                        return "Lunes";
                    case "Tuesday":
                        return "Martes";
                    case "Wednesday":
                        return "Miercoles";
                    case "Thursday":
                        return "Jueves";
                    case "Friday":
                        return "Viernes";
                    case "Saturday":
                        return "Sábado";
                    case "Sunday":
                        return "Domingo";
                    default:
                        return idday;
                }
            }
            set
            {
                idday = value;
            }
        }

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

        [Display(Name = "strfromHour")]
        [JsonProperty("strfromHour")]
        public string strfromHour
        {
            get
            {
                return Hora_Desde.ToString("HH:mm:ss");
            }
            set
            {

            }
        }
        [Display(Name = "strtoHour")]
        [JsonProperty("strtoHour")]
        public string strtoHour
        {
            get
            {
                return Hora_Hasta.ToString("HH:mm:ss");
            }
            set
            {

            }
        }
        [Display(Name = "office")]
        [JsonProperty("office")]
        public string Consultorio { get; set; }

        [Display(Name = "cantidad")]
        [JsonProperty("cantidad")]
        public int cantidad { get; set; }
    }
}