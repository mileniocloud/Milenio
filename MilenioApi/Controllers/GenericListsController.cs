using MilenioApi.Action;
using MilenioApi.Models;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Lists")]
    public class GenericListsController : ApiController
    {
        aUtilities ut = new aUtilities();


        /// <summary>
        /// Metodo consultar los departamentos
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <returns>Regresa lista con los departamentos </returns>
        [HttpGet]
        [Authorize]
        [Route("GetDepartament")]
        public HttpResponseMessage GetDepartament()
        {
            aGenericLists s = new aGenericLists();
            Basic t = new Basic();
            return ut.ReturnResponse(s.GetDepartament(t));
        }

        /// <summary>
        /// Metodo consultar los municipios dado un departamento
        /// </summary>
        /// <remarks>
        /// PARAMETRO: id [int] el id del departamento <br/>
        /// </remarks>
        /// <returns>Regresa una lista con los municipios </returns>
        [HttpPost]
        [Authorize]
        [Route("GetMunicipality")]
        public HttpResponseMessage GetMunicipality(Basic t)
        {
            aGenericLists s = new aGenericLists();
            return ut.ReturnResponse(s.GetMunicipality(t));
        }

        /// <summary>
        /// Metodo consultar todos los poblado o barrios
        /// </summary>
        /// <remarks>
        /// PARAMETRO: id [int] el id del municipio <br/>
        /// </remarks>
        /// <returns>Regresa una lista con todos los poblados o barrios </returns>
        [HttpPost]
        [Authorize]
        [Route("GetNeighborhood")]
        public HttpResponseMessage GetNeighborhood(Basic t)
        {
            aGenericLists s = new aGenericLists();            
            return ut.ReturnResponse(s.GetNeighborhood(t));
        }

    }
}
