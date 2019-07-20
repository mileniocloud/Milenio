using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class MenuModel
    {
        public string name { get; set; }
        public string url { get; set; }
        public string icon { get; set; }
        public string estilo { get; set; }
        public string padre { get; set; }
        public int posicion { get; set; }

        public List<MenuModel> child = new List<MenuModel>();
    }
}