using MilenioApi.Action;
using MilenioApi.Models;
using System.Web;
using System.Web.Http;
using System.Collections.Generic;
using System.Net.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Persona")]
    public class PersonaController : ApiController
    {
        aUtilities ut = new aUtilities();

        [AllowAnonymous]
        [Route("CreatePersona")]
        public HttpResponseMessage CreatePersona()
        {
            aPersona e = new aPersona();
            return ut.ReturnResponse(e.CreatePersona(HttpContext.Current.Request));
        }
                
        [AllowAnonymous]
        [Route("CreatePersonaMaster")]
        public HttpResponseMessage CreatePersonaMaster()
        {
            aPersona e = new aPersona();
            return ut.ReturnResponse(e.CreatePersonaMaster(HttpContext.Current.Request));
        }

        [AllowAnonymous]
        [Route("AgregaPersonaEntidad")]
        public HttpResponseMessage AgregaPersonaEntidad()
        {
            aPersona e = new aPersona();
            return ut.ReturnResponse(e.AgregaPersonaEntidad(HttpContext.Current.Request));
        }

        [AllowAnonymous]
        [Route("ActInactivaRolPersona")]
        public HttpResponseMessage ActInactivaRolPersona()
        {
            aPersona e = new aPersona();
            return ut.ReturnResponse(e.ActInactivaRolPersona(HttpContext.Current.Request));
        }

        [AllowAnonymous]
        [Route("ActInactivaPersona")]
        public HttpResponseMessage ActInactivaPersona()
        {
            aPersona e = new aPersona();
            return ut.ReturnResponse(e.ActInactivaPersona(HttpContext.Current.Request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GetRoles")]
        public HttpResponseMessage GetRoles()
        {
            aPersona e = new aPersona();
            return ut.ReturnResponse(e.GetRoles(HttpContext.Current.Request));
        }

        [AllowAnonymous]
        [Route("GetRolesPersonaEntidad")]
        public HttpResponseMessage GetRolesPersonaEntidad()
        {
            aPersona e = new aPersona();
            return e.GetRolesPersonaEntidad(HttpContext.Current.Request);
        }

        [AllowAnonymous]
        [Route("EditPersona")]
        public HttpResponseMessage EditPersona()
        {
            aPersona e = new aPersona();
            return ut.ReturnResponse(e.EditPersona(HttpContext.Current.Request));
        }

        [AllowAnonymous]
        [Route("EditaUbicacionPersona")]
        public HttpResponseMessage EditaUbicacionPersona()
        {
            aPersona e = new aPersona();
            return ut.ReturnResponse(e.EditaUbicacion(HttpContext.Current.Request));
        }
    }
}
