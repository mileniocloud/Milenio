using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class Return
    {
        public string name { get; set; }
        public string codigo { get; set; }
        public string response_Code { get; set; }
        public string message { get; set; }
        public Guid? id { get; set; }
        public string  custom { get; set; }
        public string token { get; set; }
        public int count { get; set; }

        public List<object> listresponse = new List<object>();
        public object objectresponse = new object();
    }
}