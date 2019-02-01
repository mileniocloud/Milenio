using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class UsuarioModel : Basic
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public bool CambiarClave { get; set; }
    }
}