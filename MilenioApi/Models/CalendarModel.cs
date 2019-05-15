using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class CalendarModel
    {
        public string idagenda { get; set; }
        public DateTime fecha { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string color { get; set; }
        public string draggable { get; set; }
        public string resizable { get; set; }

        public List<ComboModel> profetional = new List<ComboModel>();
    }
}