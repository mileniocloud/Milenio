using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace MilenioApi.Models
{
    public class AppointmentModel : Basic
    {
        [Display(Name = "idpatient")]
        [JsonProperty("idpatient")]
        public Guid Id_Paciente { get; set; }
        [Display(Name = "identity")]
        [JsonProperty("identity")]
        public Guid Id_Entidad { get; set; }
        [Display(Name = "idspeciality")]
        [JsonProperty("idspeciality")]
        public Guid Id_Especialidad { get; set; }
        [Display(Name = "idcup")]
        [JsonProperty("idcup")]
        public Guid Id_Cup { get; set; }
        [Display(Name = "idscheduledatail")]
        [JsonProperty("idscheduledatail")]
        public Guid Id_Detalle_Agenda { get; set; }
        [Display(Name = "codaprobacion")]
        [JsonProperty("codaprobacion")]
        public string Cod_Aprobacion { get; set; }
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
    }
}