using MilenioApi.Action;
using MilenioApi.Models;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Seguridad")]
    public class SeguridadController : ApiController
    {
        [AllowAnonymous]
        [Route("Login")]
        public oPersonaModel Login()
        {
            aSeguridad s = new aSeguridad();
            return s.Login(HttpContext.Current.Request);
        }

        [AllowAnonymous]
        [Route("LoginEntidad")]
        public oPersonaModel LoginEntidad()
        {
            aSeguridad s = new aSeguridad();
            return s.LoginEntidad(HttpContext.Current.Request);
        }
    }
}
