using MilenioApi.Action;
using MilenioApi.Models;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Office")]
    public class OfficeController : ApiController
    {
        aUtilities ut = new aUtilities();

        /// <summary>
        /// Metodo para crear un consultorio
        /// </summary>
        /// <remarks>
        /// PARAMETRO: name [STRING] [REQUERIDO] <br />
        /// PARAMETRO: description [STRING] [REQUERIDO]<br />
        /// PARAMETRO: status [BOOL] [REQUERIDO]<br />
        /// </remarks>
        /// <returns>Regresa un objeto informado exito de la operacion </returns>
        [HttpPost]
        [Authorize]
        [Route("CreateOffice")]
        public HttpResponseMessage CreateOffice(OfficeModel t)
        {
            aOffice s = new aOffice();
            return ut.ReturnResponse(s.CreateOffice(t));
        }

        /// <summary>
        /// Metodo para editar un consultorio
        /// </summary>
        /// <remarks>
        /// PARAMETRO: name [STRING] [REQUERIDO] <br />
        /// PARAMETRO: description [STRING] [REQUERIDO]<br />
        /// PARAMETRO: status [BOOL] [REQUERIDO]<br />
        /// PARAMETRO: idoffice [GUID] [REQUERIDO]<br />
        /// </remarks>
        /// <returns>Regresa un objeto informado exito de la operacion </returns>
        [HttpPost]
        [Authorize]
        [Route("EditOffice")]
        public HttpResponseMessage EditOffice(OfficeModel t)
        {
            aOffice s = new aOffice();
            return ut.ReturnResponse(s.EditOffice(t));
        }

        /// <summary>
        /// Metodo para consultar consultorios, 
        /// </summary>
        /// <remarks>
        /// PARAMETRO: name [STRING] [OPTIONAL] (si se pasa este valor, se buscara consultorios con este nombre) <br />
        /// PARAMETRO: description [STRING] [OPTIONAL] (si se pasa este valor se buscara consultorios con esa descripcion)<br />
        /// PARAMETRO: page [STRING] Este valor indica en que pagina del grid estamos actualmente<br />
        /// PARAMETRO: pagesize [STRING] Este valor indica la cantidad de datos que queremos traer<br />
        /// </remarks>
        /// <returns>Regresa una lista con los consultorios que cumplan con los parametros de busqueda</returns>
        [HttpPost]
        [Authorize]
        [Route("GetOffice")]
        public HttpResponseMessage GetOffice(OfficeModel t)
        {
            aOffice s = new aOffice();
            return ut.ReturnResponse(s.GetOffice(t));
        }

        /// <summary>
        /// Metodo para consultar consultorios, 
        /// </summary>
        /// <remarks>
        /// PARAMETRO: idoffice [STRING]
        /// </remarks>
        /// <returns>Regresa la data del consultorio consultado y una lista de las especialidades de ese consultorio</returns>
        [HttpPost]
        [Authorize]
        [Route("GetEditOffice")]
        public HttpResponseMessage GetEditOffice(OfficeModel t)
        {
            aOffice s = new aOffice();
            return ut.ReturnResponse(s.GetEditOffice(t));
        }

        /// <summary>
        /// Metodo para consultar consultorios, 
        /// </summary>
        /// <remarks>
        /// PARAMETRO: idoffice [STRING]
        /// </remarks>
        /// <returns>Regresa info indicando que se elimino el registro o que no se pudo</returns>
        [HttpPost]
        [Authorize]
        [Route("DeleteOffice")]
        public HttpResponseMessage DeleteOffice(OfficeModel t)
        {
            aOffice s = new aOffice();
            return ut.ReturnResponse(s.DeleteOffice(t));
        }


        #region Relaciondas

        /// <summary>
        /// Metodo para agregar una especialidad a un consultorio
        /// </summary>
        /// <remarks>
        /// PARAMETRO: Id_Consultorio [GUID][REQUERIDO]<br />
        /// PARAMETRO: Id_Especialidad [GUID[REQUERIDO]]<br />  
        /// PARAMETRO: token [STRING] [REQUERIDO]
        /// </remarks>
        /// <returns>Regresa un objeto indicando el resultado de la operacion</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("CreateConsultorioEspecialidad")]
        public HttpResponseMessage CreateConsultorioEspecialidad(OfficeModel t)
        {
            aOffice s = new aOffice();
            return ut.ReturnResponse(s.CreateConsultorioEspecialidad(t));
        }

        /// <summary>
        /// Metodo para editar el estado de una especialidad en un consultorio
        /// </summary>
        /// <remarks>
        /// PARAMETRO: Id_Consultorio [GUID][REQUERIDO]<br />
        /// PARAMETRO: Id_Especialidad [GUID][REQUERIDO]<br />  
        /// PARAMETRO: Estado [STRING] [REQUERIDO]
        /// PARAMETRO: token [STRING] [REQUERIDO]
        /// 
        /// </remarks>
        /// <returns>Regresa un objeto indicando el resultado de la operacion</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("EditConsultorioEspecialidad")]
        public HttpResponseMessage EditConsultorioEspecialidad(OfficeModel t)
        {
            aOffice s = new aOffice();
            return ut.ReturnResponse(s.EditConsultorioEspecialidad(t));
        }

        #endregion
    }
}
