using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MilenioApi.Models
{
    public class PatientModel: Basic
    {
        [Display(Name = "idpatient")]
        [JsonProperty("idpatient")]
        public Guid Id_Paciente { get; set; }

        [Display(Name = "typedocument")]
        [JsonProperty("typedocument")]
        public string Id_Tipo_Identificacion { get; set; }

        [Display(Name = "document")]
        [JsonProperty("document")]
        public string Numero_Identificacion { get; set; }

        [Display(Name = "names")]
        [JsonProperty("names")]
        public string Nombres { get; set; }

        [Display(Name = "lastnames")]
        [JsonProperty("lastnames")]
        public string Apellidos { get; set; }

        [Display(Name = "birthday")]
        [JsonProperty("birthday")]
        public DateTime Fecha_Nacimiento { get; set; }

        [Display(Name = "cell")]
        [JsonProperty("cell")]
        public string Celular { get; set; }

        [Display(Name = "phone")]
        [JsonProperty("phone")]
        public string Telefono { get; set; }

        [Display(Name = "email")]
        [JsonProperty("email")]
        public string Email { get; set; }

        [Display(Name = "address")]
        [JsonProperty("address")]
        public string Direccion { get; set; }

        [Display(Name = "confirm")]
        [JsonProperty("confirm")]
        public bool Confirmado { get; set; }
        
        [Display(Name = "idspeciality")]
        [JsonProperty("idspeciality")]
        public Guid Id_Especialidad { get; set; }

        [Display(Name = "specialityname")]
        [JsonProperty("specialityname")]
        public Guid specialityname { get; set; }

        [Display(Name = "cups")]
        [JsonProperty("cups")]
        public string Codigo_Cup { get; set; }

        [Display(Name = "cupsname")]
        [JsonProperty("cupsname")]
        public string cupsname { get; set; }

        [Display(Name = "entityname")]
        [JsonProperty("entityname")]
        public string entityname { get; set; }

        [Display(Name = "typequery")]
        [JsonProperty("typequery")]
        public string Tipo_Cita { get; set; }

        [Display(Name = "autorization")]
        [JsonProperty("autorization")]
        public string Cod_Aprobacion { get; set; }

    }
}