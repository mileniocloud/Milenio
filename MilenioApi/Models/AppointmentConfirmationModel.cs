using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class AppointmentConfirmationModel
    {
        [Display(Name = "patientname")]
        [JsonProperty("patientname")]
        public string Nombre_paciente { get; set; }

        [Display(Name = "idappointment")]
        [JsonProperty("idappointment")]
        public Guid Id_Appointment { get; set; }

        [Display(Name = "appointmentdate")]
        [JsonProperty("appointmentdate")]
        public DateTime Fecha_Cita { get; set; }

        [Display(Name = "appointmentfromdate")]
        [JsonProperty("appointmentfromdate")]
        public DateTime Hora_Desde { get; set; }

        [Display(Name = "fromhour")]
        [JsonProperty("fromhour")]
        public string Hora_Desde_String
        {
            get
            {
                return Hora_Desde.ToString("HH:mm");
            }
            set
            {

            }
        }

        [Display(Name = "tohour")]
        [JsonProperty("tohour")]
        public string Hora_Hasta_String
        {
            get
            {
                return Hora_Hasta.ToString("HH:mm");
            }
            set
            {

            }
        }

        [Display(Name = "appointmenttodate")]
        [JsonProperty("appointmenttodate")]
        public DateTime Hora_Hasta { get; set; }

        [Display(Name = "patientphone")]
        [JsonProperty("patientphone")]
        public string  Telefono { get; set; }

        [Display(Name = "email")]
        [JsonProperty("email")]
        public string Correo { get; set; }

        [Display(Name = "idpatient")]
        [JsonProperty("idpatient")]
        public Guid Id_Paciente { get; set; }

        [Display(Name = "mobilephone")]
        [JsonProperty("mobilephone")]
        public string Celular { get; set; }
    }
}