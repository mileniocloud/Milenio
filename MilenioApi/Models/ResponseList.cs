using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class ResponseList
    {
        public int cant { get; set; }
        public int pagina { get; set; }
        public List<object> list = new List<object>();
    }
}