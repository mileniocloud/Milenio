using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MilenioApi.Models
{
    public class LoginModel: Basic
    {
        public Guid Id_Usuario { get; set; }
        [Display(Name = "entity")]
        [JsonProperty("entity")]
        public Guid Id_Entidad { get; set; }

        [Display(Name = "user")]
        [JsonProperty("user")]
        [Required]
        public string User { get; set; }        
        [Required]
        public string Password { get; set; }

        [Display(Name = "changetoken")]
        [JsonProperty("changetoken")]
        public string changetoken { get; set; }

        public string OldPassword { get; set; }
        [EmailAddress]
        public string Email { get; set; }
       
        public List<ComboModel> Roles = new List<ComboModel>();
        public List<ComboModel> Entidades = new List<ComboModel>();
    }
}