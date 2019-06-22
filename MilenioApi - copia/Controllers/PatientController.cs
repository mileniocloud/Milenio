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
        /// PARAMETRO: typedocument [STRING] [REQUERIDO]<br />
        /// PARAMETRO: document [STRING] [REQUERIDO] <br />   
        /// </remarks>
        /// <returns>Regresa un id del paciente </returns>
        [HttpPost]
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
        /// PARAMETRO: typedocument [STRING] [REQUERIDO]<br />
        /// PARAMETRO: document [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: names [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: lastnames [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: birthday [DATE] [REQUERIDO] <br />   
        /// PARAMETRO: cell [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: phone [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: email [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: address [STRING] [REQUERIDO] <br />
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
        /// PARAMETRO: identificationtype [STRING] [REQUERIDO]<br />
        /// PARAMETRO: identification [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: names [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: lastnames [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: birthday [DATE] [REQUERIDO] <br />   
        /// PARAMETRO: cell [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: phone [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: email [STRING] [REQUERIDO] <br />   
        /// PARAMETRO: address [STRING] [REQUERIDO] <br />
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
