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

        public string _token { get; set; }

        public string token
        {
            get
            {
                var httpRequest = HttpContext.Current.Request;
                var token = httpRequest.RequestContext.HttpContext.Items;
               return _token = ((System.Net.Http.HttpRequestMessage)(token["MS_HttpRequestMessage"])).Headers.Authorization.Parameter.ToString();
            }
            set
            {
                
            }
        }

    }
}