using MilenioApi.Action;
using MilenioApi.Models;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Collections.Generic;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/especialidad")]
    public class SpecialtyController : ApiController
    {
        aUtilities ut = new aUtilities();
        #region especialidad_cup
        /// <summary>
        /// Consulta la asociación de cups por especialidad
        /// </summary>
        /// <remarks>
        /// PARAMETRO: Pendiente<br />
        /// </remarks> 
        /// <param name="t"></param>
        /// <returns>regresa un mensaje de exito o error</returns>
        [HttpPost]
        [Authorize]
        [Route("getCupXSpecialty")]
        public HttpResponseMessage getCupXSpecialty()
        {
            //Pendiente realizar cambios
            object l = new object();
            return ut.ReturnResponse(l);
        }
        #endregion
    }
}
