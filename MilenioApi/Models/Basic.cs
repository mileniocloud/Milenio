using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class Basic
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public messagetype messagetype { get; set; }
        public string message { get; set; }
        public string token { get; set; }

    }
    public enum messagetype
    {
        error,
        warning,
        suscess
    }
}