using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MilenioApi.Models
{
    public class EntidadModel : Basic
    {
        [Display(Name = "identity")]
        [JsonProperty("identity")]
        public Guid Id_Entidad { get; set; }
        [Required]
        [Display(Name = "nit")]
        [JsonProperty("nit")]
        public string Nit { get; set; }

        [Required]
        [Display(Name = "name")]
        [JsonProperty("name")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "organization")]
        [JsonProperty("organization")]
        public string Organizacion { get; set; }

        [Required]
        [Display(Name = "neighborhood")]
        [JsonProperty("neighborhood")]
        public int Poblado_Id { get; set; }

        [Required]
        [Display(Name = "address")]
        [JsonProperty("address")]
        public string Direccion { get; set; }

        [Required]
        [Display(Name = "email")]
        [JsonProperty("email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "telephone")]
        [JsonProperty("telephone")]
        public string Telefono { get; set; }

        [Required]
        [Display(Name = "entitycode")]
        [JsonProperty("entitycode")]
        public string CodigoEntidad { get; set; }

        [Required]
        [Display(Name = "priorityattention")]
        [JsonProperty("priorityattention")]
        public bool Atencion_Prioritaria { get; set; }

        [Required]
        [Display(Name = "taxpayer")]
        [JsonProperty("taxpayer")]
        public bool Contribuyente { get; set; }

        [Display(Name = "photo")]
        [JsonProperty("photo")]
        public string Foto { get; set; }

        [Required]
        [Display(Name = "opening")]
        [JsonProperty("opening")]
        public string Hora_Desde { get; set; }

        [Required]
        [Display(Name = "closing")]
        [JsonProperty("closing")]
        public string Hora_Hasta { get; set; }

        [Display(Name = "specialities")]
        [JsonProperty("specialities")]

        public List<ComboListModel> specialities = new List<ComboListModel>();
        public List<ComboListModel> Notspecialities = new List<ComboListModel>();
    }
}
