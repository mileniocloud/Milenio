using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class LoginModel
    {
        public Guid idusuario { get; set; }
        public Guid identidad { get; set; }

        [Display(Name ="Login") ]
        [Required]
        public string user { get; set; }

        [Required]
        public string password { get; set; }
        [EmailAddress]
        public string email { get; set; }
        public string token { get; set; }
        public List<ComboModel> Roles = new List<ComboModel>();
        public List<ComboModel> Entidades = new List<ComboModel>();
    }
}