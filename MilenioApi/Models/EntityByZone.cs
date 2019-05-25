using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class EntityByZone
    {
        public string group { get; set; }

        public List<BasicList> entities = new List<BasicList>();
    }
}