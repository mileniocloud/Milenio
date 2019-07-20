using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace MilenioApi.Models
{
    public class AppointmentModel : Basic
    {
       
        [Display(Name = "typequery")]
        [JsonProperty("typequery")]
        public string Tipo_Cita { get; set; }
       
        [Display(Name = "idpatient")]
        [JsonProperty("idpatient")]
        public Guid Id_Paciente { get; set; }

        [Display(Name = "idprofetional")]
        [JsonProperty("idprofetional")]
        public Guid Id_Profetional { get; set; }

        [Required]
        [Display(Name = "identity")]
        [JsonProperty("identity")]
        public Guid Id_Entidad { get; set; }

        [Required]
        [Display(Name = "specialities")]
        [JsonProperty("specialities")]
        public Guid Id_Especialidad { get; set; }

        [Display(Name = "idcup")]
        [JsonProperty("idcup")]
        public Guid Id_Cup { get; set; }
       
        [Display(Name = "idscheduledatail")]
        [JsonProperty("idscheduledatail")]
        public Guid Id_Detalle_Agenda { get; set; }

        [Display(Name = "idappointment")]
        [JsonProperty("idappointment")]
        public Guid Id_Cita { get; set; }

        [Display(Name = "autorization")]
        [JsonProperty("autorization")]
        public string Cod_Aprobacion { get; set; }

        [Display(Name = "confirmation")]
        [JsonProperty("confirmation")]
        public bool Confirmada { get; set; }
        public DateTime Fecha_Create { get; set; }

        [Display(Name = "specialitycode")]
        [JsonProperty("specialitycode")]
        public string Codigo_Especilidad { get; set; }

        [Display(Name = "cupcode")]
        [JsonProperty("cupcode")]
        public string Codigo_Cup { get; set; }

        [Display(Name = "month")]
        [JsonProperty("month")]
        public int Mes { get; set; }

        public Guid id { get; set; }

        [Display(Name = "typeappointment")]
        [JsonProperty("typeappointment")]
        public bool tipo_consulta { get; set; }

        [Display(Name = "typedocument")]
        [JsonProperty("typedocument")]
        public string Id_Tipo_Identificacion { get; set; }

        [Display(Name = "document")]
        [JsonProperty("document")]
        public string Numero_Identificacion { get; set; }
    }
}