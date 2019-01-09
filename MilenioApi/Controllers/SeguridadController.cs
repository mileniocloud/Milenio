using MilenioApi.Action;
using MilenioApi.Models;
using System.Web;
using System.Web.Http;
using System.Net.Http;

using Newtonsoft.Json;
using System.Net;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Seguridad")]
    public class SeguridadController : ApiController
    {
        aUtilities ut = new aUtilities();
        
        [AllowAnonymous]
        [Route("Login")]
        public HttpResponseMessage Login()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.Login(HttpContext.Current.Request));
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("LoginEntidad")]
        public HttpResponseMessage LoginEntidad()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.LoginEntidad(HttpContext.Current.Request));
        }

        [AllowAnonymous]
        [Route("CambioEntidad")]
        public HttpResponseMessage CambioEntidad()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.CambioEntidad(HttpContext.Current.Request));
        }
    }
}
