using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace MilenioApi.Models
{
    public class Response
    {
        public HttpStatusCode status { get; set; }
        public string response_code { get; set; }
        public string message { get; set; }
        public int cantidad { get; set; }
        public int pagina { get; set; }
        public string rute { get; set; }

        public object data = new object();
        public List<object> error = new List<object>();
    }
}