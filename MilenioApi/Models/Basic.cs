using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Web;

namespace MilenioApi.Models
{
    public class Basic
    {
        public string id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        [Display(Name = "page")]
        [JsonProperty("page")]
        public int Pagina { get; set; } = 1;

        [Display(Name = "pagesize")]
        [JsonProperty("pagesize")]
        public int Cant_Registros { get; set; } = int.Parse(ConfigurationManager.AppSettings["pagesize"]);

        public string _token { get; set; }

        public string token
        {
            get
            {
                var httpRequest = HttpContext.Current.Request;
                var token = httpRequest.RequestContext.HttpContext.Items;

                if (((System.Net.Http.HttpRequestMessage)(token["MS_HttpRequestMessage"])).Headers.Authorization != null)
                    if (((System.Net.Http.HttpRequestMessage)(token["MS_HttpRequestMessage"])).Headers.Authorization.Parameter != null)
                        _token = ((System.Net.Http.HttpRequestMessage)(token["MS_HttpRequestMessage"])).Headers.Authorization.Parameter.ToString();

                return _token;
            }
            set
            {

            }
        }

    }
}