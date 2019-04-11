using MilenioApi.Action;
using MilenioApi.Models;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Entidad")]
    public class EntityController : ApiController
    {
        aUtilities ut = new aUtilities();


        [HttpPost]
        [AllowAnonymous]
        [Route("GetEntidades")]
        public HttpResponseMessage GetEntidades(EntidadModel t)
        {
            aEntity s = new aEntity();
            return ut.ReturnResponse(s.GetEntidades(t));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GetEntidadesEdit")]
        public HttpResponseMessage GetEntidadesEdit(EntidadModel t)
        {
            aEntity s = new aEntity();
            return ut.ReturnResponse(s.GetEntidadesEdit(t));
        }

        // <summary>
        // Metodo para crear entidades
        // </summary>
        // <remarks>
        // PARAMETRO: nit [INT] [REQUERIDO]<br/>
        // PARAMETRO: name [STRING] [REQUERIDO]<br/>
        // PARAMETRO: organization [STRING] [REQUERIDO]<br/>
        // PARAMETRO: neighborhood [STRING][REQUERIDO]<br/>
        // PARAMETRO: address [STRING][REQUERIDO]<br/>
        // PARAMETRO: email [STRING][REQUERIDO]<br/>
        // PARAMETRO: entitycode [STRING][REQUERIDO]<br/>
        // PARAMETRO: priorityatention [STRING][REQUERIDO]<br/>
        // PARAMETRO: taxpayer [STRING][REQUERIDO]<br/>
        // PARAMETRO: photo [STRING][REQUERIDO]<br/>
        // PARAMETRO: opening [STRING][REQUERIDO]<br/>
        // PARAMETRO: closing [STRING][REQUERIDO]<br/>
        // PARAMETRO: status [STRING][REQUERIDO]<br/>

        // </remarks>
        /// <returns>Regresa informacion indicando el exito de la operacion </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("CreateEntidad")]
        public HttpResponseMessage CreateEntidad(EntidadModel t)
        {
            aEntity s = new aEntity();
            return ut.ReturnResponse(s.CreateEntidad(t));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("EditEntidad")]
        public HttpResponseMessage EditEntidad(EntidadModel t)
        {
            aEntity s = new aEntity();
            return ut.ReturnResponse(s.EditEntidad(t));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("CreateEspecialidadEntidad")]
        public HttpResponseMessage CreateEspecialidadEntidad(EntidadModel t)
        {
            aEntity s = new aEntity();
            return ut.ReturnResponse(s.CreateEspecialidadEntidad(t));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("EditEspecialidadEntidad")]
        public HttpResponseMessage EditEspecialidadEntidad(EntidadModel t)
        {
            aEntity s = new aEntity();
            return ut.ReturnResponse(s.EditEspecialidadEntidad(t));
        }
    }
}
