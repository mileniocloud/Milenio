using MilenioApi.Action;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Entidad")]
    public class EntidadController : ApiController
    {
        aUtilities ut = new aUtilities();

        [HttpPost]
        [AllowAnonymous]
        [Route("GetEntidades")]
        public HttpResponseMessage GetEntidades()
        {
            aEntidad s = new aEntidad();
            return ut.ReturnResponse(s.GetEntidades(HttpContext.Current.Request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GetEntidadesEdit")]
        public HttpResponseMessage GetEntidadesEdit()
        {
            aEntidad s = new aEntidad();
            return ut.ReturnResponse(s.GetEntidadesEdit(HttpContext.Current.Request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("CreateEntidad")]
        public HttpResponseMessage CreateEntidad()
        {
            aEntidad s = new aEntidad();
            return ut.ReturnResponse(s.CreateEntidad(HttpContext.Current.Request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("EditEntidad")]
        public HttpResponseMessage EditEntidad()
        {
            aEntidad s = new aEntidad();
            return ut.ReturnResponse(s.EditEntidad(HttpContext.Current.Request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("CreateEspecialidadEntidad")]
        public HttpResponseMessage CreateEspecialidadEntidad()
        {
            aEntidad s = new aEntidad();
            return ut.ReturnResponse(s.CreateEspecialidadEntidad(HttpContext.Current.Request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("EditEspecialidadEntidad")]
        public HttpResponseMessage EditEspecialidadEntidad()
        {
            aEntidad s = new aEntidad();
            return ut.ReturnResponse(s.EditEspecialidadEntidad(HttpContext.Current.Request));
        }
    }
}
