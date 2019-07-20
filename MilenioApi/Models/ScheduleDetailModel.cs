using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace MilenioApi.Models
{
    public class ScheduleDetailModel : Basic
    {
        [Display(Name = "idprofetionalschedule")]
        [JsonProperty("idprofetionalschedule")]
        public Guid Id_Agenda_Profesional { get; set; }

        [Display(Name = "idscheduledetail")]
        [JsonProperty("idscheduledetail")]
        public Guid Id_Detalle_Agenda { get; set; }
        [Required]
        [Display(Name = "idscheduleagenda")]
        [JsonProperty("idscheduleagenda")]
        public Guid Id_Horario_Agenda { get; set; }

        [Display(Name = "idspeciality")]
        [JsonProperty("idspeciality")]
        public Guid Id_Speciality { get; set; }

        [Display(Name = "fromhour")]
        [JsonProperty("fromhour")]
        public DateTime Hora_Desde { get; set; }

        [Display(Name = "tohour")]
        [JsonProperty("tohour")]
        public DateTime Hora_Hasta { get; set; }

        [Display(Name = "datefrom")]
        [JsonProperty("datefrom")]
        public DateTime Fecha_Desde { get; set; }

        [Display(Name = "date")]
        [JsonProperty("date")]
        public DateTime Fecha { get; set; }

        [Display(Name = "dateto")]
        [JsonProperty("dateto")]
        public DateTime Fecha_Hasta { get; set; }

        [Display(Name = "taken")]
        [JsonProperty("taken")]
        public bool Tomada { get; set; }


        [Display(Name = "idoffice")]
        [JsonProperty("idoffice")]
        public Guid Id_Consultorio { get; set; }

        [Display(Name = "idProfAlternate")]
        [JsonProperty("idProfAlternate")]
        public Guid id_Suplente { get; set; }

        [Display(Name = "nameoffice")]
        [JsonProperty("nameoffice")]
        public string Name_Consultorio { get; set; }

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


    }
}