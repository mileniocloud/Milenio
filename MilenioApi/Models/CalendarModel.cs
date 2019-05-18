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

        public color color = new color();
        public string draggable { get; set; }

        public resizable resizable = new resizable();
        public string title { get; set; }

        public List<ComboModel> profetional = new List<ComboModel>();
    }

    public class color
    {
        public string primary { get; set; }
        public string secondary { get; set; }
    }

    public class resizable
    {
        public string beforeStart { get; set; }
        public string afterEnd { get; set; }
    }
}