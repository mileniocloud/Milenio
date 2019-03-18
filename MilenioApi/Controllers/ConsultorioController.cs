using MilenioApi.Action;
using MilenioApi.Models;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Consultorio")]
    public class ConsultorioController : ApiController
    {
        aUtilities ut = new aUtilities();

        /// <summary>
        /// Metodo para crear un consultorio
        /// </summary>
        /// <remarks>
        /// PARAMETRO: Nombre [STRING] [REQUERIDO] <br />
        /// PARAMETRO: Descripcion [STRING] [REQUERIDO]<br />
        /// PARAMETRO: Estado [BOOL] [REQUERIDO]<br />
        /// PARAMETRO: token [STRING] [REQUERIDO]
        /// </remarks>
        /// <returns>Regresa un objeto informado exito de la operacion </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("CreateConsultorio")]
        public HttpResponseMessage CreateConsultorio(ConsultorioModel t)
        {
            aConsultorio s = new aConsultorio();
            return ut.ReturnResponse(s.CreateConsultorio(t));
        }

        /// <summary>
        /// Metodo para editar un consultorio
        /// </summary>
        /// <remarks>
        /// PARAMETRO: Nombre [STRING] [REQUERIDO]<br />
        /// PARAMETRO: Descripcion [STRING] [REQUERIDO]<br />
        /// PARAMETRO: Estado [BOOL] [REQUERIDO]<br />
        /// PARAMETRO: token [STRING] [REQUERIDO]
        /// </remarks>
        /// <returns>Regresa un objeto informado exito de la operacion </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("EditConsultorio")]
        public HttpResponseMessage EditConsultorio(ConsultorioModel t)
        {
            aConsultorio s = new aConsultorio();
            return ut.ReturnResponse(s.EditConsultorio(t));
        }

        /// <summary>
        /// Metodo para editar un consultorio
        /// </summary>
        /// <remarks>
        /// PARAMETRO: Nombre [STRING]<br />
        /// PARAMETRO: Descripcion [STRING]<br />  
        /// PARAMETRO: token [STRING] [REQUERIDO]
        /// </remarks>
        /// <returns>Regresa una lista con los consultorios que cumplan con los parametros de busqueda</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("GetConsultorio")]
        public HttpResponseMessage GetConsultorio(ConsultorioModel t)
        {
            aConsultorio s = new aConsultorio();
            return ut.ReturnResponse(s.GetConsultorio(t));
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
        public HttpResponseMessage CreateConsultorioEspecialidad(ConsultorioModel t)
        {
            aConsultorio s = new aConsultorio();
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
        public HttpResponseMessage EditConsultorioEspecialidad(ConsultorioModel t)
        {
            aConsultorio s = new aConsultorio();
            return ut.ReturnResponse(s.EditConsultorioEspecialidad(t));
        }

        #endregion
    }
}
