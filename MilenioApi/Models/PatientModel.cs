using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MilenioApi.Models
{
    public class PatientModel: Basic
    {
        [Display(Name = "idpaciente")]
        [JsonProperty("idpaciente")]
        public Guid Id_Paciente { get; set; }

        [Display(Name = "tipoidentificacion")]
        [JsonProperty("tipoidentificacion")]
        public Guid Id_Tipo_Identificacion { get; set; }

        [Display(Name = "identificacion")]
        [JsonProperty("identificacion")]
        public string Numero_Identificacion { get; set; }

        [Display(Name = "nombres")]
        [JsonProperty("nombres")]
        public string Nombres { get; set; }

        [Display(Name = "apellidos")]
        [JsonProperty("apellidos")]
        public string Apellidos { get; set; }

        [Display(Name = "fechanacimiento")]
        [JsonProperty("fechanacimiento")]
        public DateTime Fecha_Nacimiento { get; set; }

        [Display(Name = "celular")]
        [JsonProperty("celular")]
        public string Celular { get; set; }

        [Display(Name = "telefono")]
        [JsonProperty("telefono")]
        public string Telefono { get; set; }

        [Display(Name = "email")]
        [JsonProperty("email")]
        public string Email { get; set; }

        [Display(Name = "direccion")]
        [JsonProperty("direccion")]
        public string Direccion { get; set; }

        [Display(Name = "confirmado")]
        [JsonProperty("confirmado")]
        public bool Confirmado { get; set; }

    }
}