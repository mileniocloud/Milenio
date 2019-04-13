using MilenioApi.Action;
using MilenioApi.Models;
using System.Net.Http;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Entity")]
    public class EntityController : ApiController
    {
        aUtilities ut = new aUtilities();

        /// <summary>
        /// Metodo para consultar entidades
        /// </summary>
        /// <remarks>
        /// PARAMETRO: nit [INT] [OPCIONAL]<br/>
        /// PARAMETRO: name [STRING] [OPCIONAL]<br/>
        /// PARAMETRO: organization [STRING] [OPCIONAL]<br/>        
        /// PARAMETRO: email [STRING][OPCIONAL]<br/>
        /// PARAMETRO: entitycode [STRING][OPCIONAL]<br/>
        /// PARAMETRO: identity [STRING][OPCIONAL]<br/>
        /// </remarks>
        /// <returns>Regresa informacion con la lista de entidades filtradas por cada uno de los parametros que e envien </returns>
        [HttpPost]
        [Authorize]
        [Route("GetEntity")]
        public HttpResponseMessage GetEntity(EntidadModel t)
        {
            aEntity s = new aEntity();
            return ut.ReturnResponse(s.GetEntity(t));
        }

        
        [HttpPost]
        [Authorize]
        [Route("GetEntityEdit")]
        public HttpResponseMessage GetEntityEdit(EntidadModel t)
        {
            aEntity s = new aEntity();
            return ut.ReturnResponse(s.GetEntityEdit(t));
        }

        /// <summary>
        /// Metodo para crear entidades
        /// </summary>
        /// <remarks>
        /// PARAMETRO: nit [INT] [REQUERIDO]<br/>
        /// PARAMETRO: name [STRING] [REQUERIDO]<br/>
        /// PARAMETRO: organization [STRING] [REQUERIDO]<br/>
        /// PARAMETRO: neighborhood [STRING][REQUERIDO]<br/>
        /// PARAMETRO: address [STRING][REQUERIDO]<br/>
        /// PARAMETRO: email [STRING][REQUERIDO]<br/>
        /// PARAMETRO: telephone [STRING][REQUERIDO]<br/>
        /// PARAMETRO: entitycode [STRING][REQUERIDO]<br/>
        /// PARAMETRO: priorityatention [STRING][REQUERIDO]<br/>
        /// PARAMETRO: taxpayer [STRING][REQUERIDO]<br/>
        /// PARAMETRO: photo [STRING][REQUERIDO]<br/>
        /// PARAMETRO: opening [STRING][REQUERIDO]<br/>
        /// PARAMETRO: closing [STRING][REQUERIDO]<br/>
        /// </remarks>
        /// <returns>Regresa informacion indicando el exito de la operacion </returns>
        [HttpPost]
        [Authorize]
        [Route("CreateEntity")]
        public HttpResponseMessage CreateEntity(EntidadModel t)
        {
            aEntity s = new aEntity();
            return ut.ReturnResponse(s.CreateEntity(t));
        }

        [HttpPost]
        [Authorize]
        [Route("EditEntity")]
        public HttpResponseMessage EditEntity(EntidadModel t)
        {
            aEntity s = new aEntity();
            return ut.ReturnResponse(s.EditEntity(t));
        }

        [HttpPost]
        [Authorize]
        [Route("CreateEspecialidadEntidad")]
        public HttpResponseMessage CreateEspecialidadEntidad(EntidadModel t)
        {
            aEntity s = new aEntity();
            return ut.ReturnResponse(s.CreateEspecialidadEntidad(t));
        }

        [HttpPost]
        [Authorize]
        [Route("EditEspecialidadEntidad")]
        public HttpResponseMessage EditEspecialidadEntidad(EntidadModel t)
        {
            aEntity s = new aEntity();
            return ut.ReturnResponse(s.EditEspecialidadEntidad(t));
        }
    }
}
