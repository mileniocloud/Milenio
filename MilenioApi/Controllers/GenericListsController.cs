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


        // <summary>
        // Metodo para consultar lo basico del formulario de usuarios
        // </summary>
        // <remarks>
        // </remarks>
        // <returns>Regresa vrias listas con la informacion necesaria para el formulario de usuarios </returns>
        [HttpGet]
        [Authorize]
        [Route("GetListsUserForm")]
        public HttpResponseMessage GetListsUserForm()
        {
            aGenericLists s = new aGenericLists();
            Basic t = new Basic();
            return ut.ReturnResponse(s.GetListsUserForm(t));
        }


        // <summary>
        // Metodo para consultar lo basico del formulario de usuarios
        // </summary>
        // <remarks>
        // </remarks>
        // <returns>Regresa varias listas con la informacion necesaria para el formulario de entidades </returns>
        [HttpGet]
        [Authorize]
        [Route("GetListsEntityForm")]
        public HttpResponseMessage GetListsEntityForm()
        {
            aGenericLists s = new aGenericLists();
            Basic t = new Basic();
            return ut.ReturnResponse(s.GetListsEntityForm(t));
        }
        // <summary>
        // Metodo para consultar lo basico del formulario de agenda
        // </summary>
        // <remarks>
        // </remarks>
        // <returns>Regresa varias listas con la informacion necesaria para el formulario de agenda </returns>
        [HttpGet]
        [Authorize]
        [Route("GetListsSheduleForm")]
        public HttpResponseMessage GetListsSheduleForm()
        {
            aGenericLists s = new aGenericLists();
            Basic t = new Basic();
            return ut.ReturnResponse(s.GetListsSheduleForm(t));
        }

        // <summary>
        // Metodo para consultar los doctores de una entidad
        // </summary>
        // <remarks>
        // </remarks>
        // <returns>Regresa una lista de Id - Value con los nombre de los prestadores de servicios de una entidad </returns>
        [HttpGet]
        [Authorize]
        [Route("GetServiceProviderList")]
        public HttpResponseMessage GetServiceProviderList()
        {
            aGenericLists s = new aGenericLists();
            Basic t = new Basic();
            return ut.ReturnResponse(s.GetServiceProviderList(t));
        }



        // <summary>
        // Metodo para consultar las especialidades de una entidad
        // </summary>
        // <remarks>
        // PARAMETRO: id [STRING] el id de la entidad 
        // </remarks>
        // <returns>Regresa una lista de Id - Value con las especialidades de una entidad </returns>
        [HttpPost]
        [Route("GetEspecialityListByEntity")]
        public HttpResponseMessage GetEspecialityListByEntity(Basic t)
        {
            aGenericLists s = new aGenericLists();
            return ut.ReturnResponse(s.GetEspecialityListByEntity(t));
        }

        // <summary>
        // Metodo para consultar las especialidades agrupadas por municipio
        // </summary>
        // <remarks>
        // </remarks>
        // <returns>Regresa una lista agrupada de entidades</returns>
        [HttpPost]
        [Route("GetEntityByMunicipality")]
        public HttpResponseMessage GetEntityByMunicipality()
        {
            aGenericLists s = new aGenericLists();
            return ut.ReturnResponse(s.GetEntityByMunicipality());
        }

       
        
    }
}
