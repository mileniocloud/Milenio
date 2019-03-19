using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class LoginModel
    {
        public Guid Id_Usuario { get; set; }
        public Guid Id_Entidad { get; set; }

        [Display(Name ="Login") ]
        [Required]
        public string User { get; set; }

        [Required]
        public string Password { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string token { get; set; }
        public List<ComboModel> Roles = new List<ComboModel>();
        public List<ComboModel> Entidades = new List<ComboModel>();
    }
}