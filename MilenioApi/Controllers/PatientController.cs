using MilenioApi.Action;
using MilenioApi.Models;
using System.Net.Http;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Patient")]
    public class PatientController : ApiController
    {
        aUtilities ut = new aUtilities();

        /// <summary>
        /// Metodo para consultar si un paciente existe, regresa null si no existe
        /// </summary>
        /// <remarks>
        /// PARAMETRO: tipoidentificacion [STRING] [REQUERIDO]<br />
        /// PARAMETRO: identificacion [STRING] [REQUERIDO] <br />   
        /// </remarks>
        /// <returns>Regresa un id del paciente </returns>
        [HttpGet]
        [Route("ValidatePatient")]
        public HttpResponseMessage ValidatePatient(PatientModel t)
        {
            aPatient s = new aPatient();
            return ut.ReturnResponse(s.ValidatePatient(t));
        }

        /// <summary>
        /// Metodo para crear pacientes
        /// </summary>
        /// <remarks>
        /// PARAMETRO: tipoidentificacion [STRING] [REQUERIDO]<br />
        /// PARAMETRO: identificacion [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: nombres [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: apellidos [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: fechanacimiento [DATE] [REQUERIDO] <br />   
        /// PARAMETRO: celular [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: telefono [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: email [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: direccion [STRING] [REQUERIDO] <br />
        /// </remarks>
        /// <returns>Regresa informacion indicando que sucedio </returns>
        [HttpPost]        
        [Route("CreateTemporalPatient")]
        public HttpResponseMessage CreateTemporalPatient(PatientModel t)
        {
            aPatient s = new aPatient();
            return ut.ReturnResponse(s.CreateTemporalPatient(t));
        }

        /// <summary>
        /// Metodo para crear pacientes
        /// </summary>
        /// <remarks>
        /// PARAMETRO: tipoidentificacion [STRING] [REQUERIDO]<br />
        /// PARAMETRO: identificacion [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: nombres [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: apellidos [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: fechanacimiento [DATE] [REQUERIDO] <br />   
        /// PARAMETRO: celular [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: telefono [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: email [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: direccion [STRING] [REQUERIDO] <br />
        /// </remarks>
        /// <returns>Regresa informacion indicando que sucedio </returns>
        [HttpPost]
        [Authorize]
        [Route("CreatePatient")]
        public HttpResponseMessage CreatePatient(PatientModel t)
        {
            aPatient s = new aPatient();
            return ut.ReturnResponse(s.CreatePatient(t));
        }
    }
}
