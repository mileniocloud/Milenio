using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class LoginModel: Basic
    {
        public Guid Id_User { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public List<ComboModel> Roles = new List<ComboModel>();
        public List<ComboModel> Entidades = new List<ComboModel>();
    }
}