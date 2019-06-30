using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MilenioApi.Action;
using MilenioApi.Models;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/especialidad")]
    public class SpecialtyController : ApiController
    {
        aUtilities ut = new aUtilities();
        #region especialidad_cup
        /// <summary>
        /// Crea la asociación de cups por especialidad
        /// </summary>
        /// <remarks>
        /// PARAMETRO: idspecialty [STRING] id especialidad <br />
        /// PARAMETRO: idcup [STRING] lista de cups <br />
        /// </remarks> 
        /// <param name="t"></param>
        /// <returns>regresa un mensaje de exito o error</returns>
        [HttpPost]
        [Authorize]
        [Route("CreateCupXSpecialty")]
        public HttpResponseMessage CreateCupXSpecialty(SpecialtyCupModel t)
        {
            aSpecialty s = new aSpecialty();
            return ut.ReturnResponse(s.CreateCupXSpecialty(t));
        }

        /// <summary>
        /// edita la asociación de cups por especialidad
        /// </summary>
        /// <remarks>
        /// PARAMETRO: idspecialty [STRING] id especialidad <br />
        /// PARAMETRO: idcup [STRING] lista de cups <br />
        /// </remarks> 
        /// <param name="t"></param>
        /// <returns>regresa un mensaje de exito o error</returns>
        [HttpPost]
        [Authorize]
        [Route("EditCupXSpecialty")]
        public HttpResponseMessage EditCupXSpecialty(SpecialtyCupModel t)
        {
            aSpecialty s = new aSpecialty();
            return ut.ReturnResponse(s.EditCupXSpecialty(t));
        }

        /// <summary>
        /// Consulta la asociación de cups por especialidad
        /// </summary>
        /// <remarks>
        /// PARAMETRO: No recibe parametros<br />
        /// </remarks> 
        /// <param name="t"></param>
        /// <returns>regresa lista de la asociación de cups por especialidad</returns>
        [HttpPost]
        [Authorize]
        [Route("getCupXSpecialty")]
        public HttpResponseMessage getCupXSpecialty(SpecialtyCupModel t)
        {
            aSpecialty s = new aSpecialty();
            return ut.ReturnResponse(s.GetCupXSpecialty(t));
        }
        #endregion
    }
}
