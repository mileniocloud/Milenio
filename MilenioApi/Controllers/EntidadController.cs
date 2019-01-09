using MilenioApi.Action;
using MilenioApi.Models;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Entidad")]
    public class EntidadController : ApiController
    {
        aUtilities ut = new aUtilities();

        [AllowAnonymous]
        [Route("CreateEntidad")]
        public HttpResponseMessage CreateEntidad()
        {
            aEntidad e = new aEntidad();
            return ut.ReturnResponse(e.CreateEntidad(HttpContext.Current.Request));
        }

        [AllowAnonymous]
        [Route("EditEntidad")]
        public HttpResponseMessage EditEntidad()
        {
            aEntidad e = new aEntidad();
            return ut.ReturnResponse(e.EditEntidad(HttpContext.Current.Request));
        }

        [AllowAnonymous]
        [Route("CreateAula")]
        public HttpResponseMessage CreateAula()
        {
            aEntidad e = new aEntidad();
            return ut.ReturnResponse(e.CreateAula(HttpContext.Current.Request));
        }
    }
}
