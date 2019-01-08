using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class Basic
    {      
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public messagetype messagetype { get; set; }      

        public string Codigo { get; set; }
        public string Message { get; set; }
        public Guid? id { get; set; }
        public string custom { get; set; }
        public string token { get; set; }

    }
    public enum messagetype
    {
        error,
        warning,
        suscess
    }
}