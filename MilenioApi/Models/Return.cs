using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class Return
    {
        public string Codigo { get; set; }
        public string Response_Code { get; set; }
        public string Message { get; set; }
        public Guid? id { get; set; }
        public string  custom { get; set; }
        public string token { get; set; }
        public int count { get; set; }

        public List<object> listresponse = new List<object>();
        public object objectresponse = new object();
    }
}