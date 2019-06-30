using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MilenioApi.Models
{
    public class SpecialtyCupModel:Basic
    {
        [Display(Name = "idspecialty")]
        [JsonProperty("idspecialty")]
        public Guid Id_Especialidad { get; set; }

        [Display(Name = "idcup")]
        [JsonProperty("idcup")]
        public List<ComboListModel> cups = new List<ComboListModel>();
        

        [Display(Name = "status")]
        [JsonProperty("status")]
        public Guid Estado { get; set; }
    }
}