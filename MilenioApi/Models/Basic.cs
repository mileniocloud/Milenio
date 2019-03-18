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
        public string codigo { get; set; }
        public string message { get; set; }
        public Guid? id { get; set; }
        public string custom { get; set; }
        public string token { get; set; }
        public string value { get; set; }
        public bool estado { get; set; }
        public int codigoint { get; set; }
        public string codigostring { get; set; }
        public string response_code { get; set; }
    }
}