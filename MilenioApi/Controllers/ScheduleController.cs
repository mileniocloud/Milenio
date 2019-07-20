using MilenioApi.Action;
using MilenioApi.Models;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Collections.Generic;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/agenda")]
    public class ScheduleController : ApiController
    {
        aUtilities ut = new aUtilities();
        #region agenda profesional
        /// <summary>
        /// Crea la agenda profesional para un medico, una entidad y una especialidad
        /// </summary>
        /// <remarks>
        /// PARAMETRO: fromdate [STRING] fecha desde cuando inicia el periodo <br />
        /// PARAMETRO: todate [STRING] fecha fin del periodo <br />
        /// PARAMETRO: idspeciality [STRING] id de la especialidad <br />
        /// PARAMETRO: idprofetional [STRING] id del medico <br />
        /// </remarks> 
        /// <param name="t"></param>
        /// <returns>regresa un mensaje de exito o error</returns>
        [HttpPost]
        [Authorize]
        [Route("createprofetionalschedule")]
        public HttpResponseMessage CreateAgendaProfesional(ProfetionalScheduleModel t)
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.CreateAgendaProfesional(t));
        }

        /// <summary>
        /// Edita la agenda profesional para un medico, una entidad y una especialidad
        /// </summary>
        /// <remarks>
        /// PARAMETRO: idprofetionalschedule [STRING] id de la agenda profesional <br />
        /// PARAMETRO: fromdate [STRING] fecha desde cuando inicia el periodo <br />
        /// PARAMETRO: todate [STRING] fecha fin del periodo <br />
        /// PARAMETRO: idspeciality [STRING] id de la especialidad <br />
        /// PARAMETRO: idprofetional [STRING] id del medico <br />
        /// </remarks> 
        /// <param name="t"></param>
        /// <returns>regresa un mensaje de exito o error</returns>
        [HttpPost]
        [Authorize]
        [Route("EditAgendaProfesional")]
        public HttpResponseMessage EditAgendaProfesional(ProfetionalScheduleModel t)
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.EditAgendaProfesional(t));
        }


        /// <summary>
        /// Consulta las agendas profesionales de una entidad
        /// </summary>
        /// <remarks>
        /// PARAMETRO: fromdate [DATE] fecha desde donde se quiere buscar [OPCIONAL] <br />
        /// PARAMETRO: todate [STRING] fecha desde cuando inicia el periodo <br />
        /// PARAMETRO: idprofetional [STRING] id del medico del que queremos ver la agenda <br />
        /// PARAMETRO: between [BOOL] indica si se va a buscar por un between entre dos fechas <br />
        /// PARAMETRO: idspeciality [STRING] is de la especialidad que vamos a filtrar <br />
        /// PARAMETRO: idprofetional [STRING] id del medico <br />
        /// </remarks> 
        /// <param name="t"></param>
        /// <returns>regresa una lista con las agendas</returns>
        [HttpPost]
        [Authorize]
        [Route("GetAgendaProfesional")]
        public HttpResponseMessage GetAgendaProfesional(ProfetionalScheduleModel t)
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.GetAgendaProfesional(t));
        }
        #endregion

        #region horario agenda // listo para pruebas

        /// <summary>
        /// Crea los horarios de la agenda, dada una agenda propfesional
        /// </summary>
        /// <remarks>
        /// PARAMETRO: idprofetionalschedule [STRING] id de la agenda profesional <br />
        /// PARAMETRO: idoffice [STRING] id del consultorio <br />
        /// PARAMETRO: fromhour [STRING] hora desde <br />
        /// PARAMETRO: tohour [STRING] hora hasta <br />
        /// PARAMETRO: day [STRING] dia del horario <br />
        /// PARAMETRO: duration [STRING] duracion de la cita <br />
        /// </remarks> 
        /// <param name="t"></param>
        /// <returns>regresa un mensaje de exito o error</returns>
        [HttpPost]
        [Authorize]
        [Route("CreateScheduleAgenda")]
        public HttpResponseMessage CreateScheduleAgenda(ScheduleAgendaModel t)
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.CreateScheduleAgenda(t));
        }

        /// <summary>
        /// Edita los horarios de la agenda, dada una agenda profesional
        /// </summary>
        /// <remarks>
        /// PARAMETRO: idscheduleagenda [STRING] id de horario agenda a modificar <br />
        /// PARAMETRO: idprofetionalschedule [STRING] id de la agenda profesional <br />
        /// PARAMETRO: idoffice [STRING] id del consultorio <br />
        /// PARAMETRO: fromhour [STRING] hora desde <br />
        /// PARAMETRO: tohour [STRING] hora hasta <br />
        /// PARAMETRO: day [STRING] dia del horario <br />
        /// PARAMETRO: duration [STRING] duracion de la cita <br />
        /// </remarks> 
        /// <param name="t"></param>
        /// <returns>regresa un mensaje de exito o error</returns>
        [HttpPost]
        [Authorize]
        [Route("EditScheduleAgenda")]
        public HttpResponseMessage EditScheduleAgenda(ScheduleAgendaModel t)
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.EditScheduleAgenda(t));
        }
        /// <summary>
        /// Obtiene los horarios de la agenda, dada una agenda profesional
        /// </summary>
        /// <remarks>
        /// PARAMETRO: idscheduleagenda [STRING] id de horario agenda a modificar <br />

        /// </remarks> 
        /// <param name="t"></param>
        /// <returns>regresa un una lista de horarios por agenda profesional</returns>
        [HttpPost]
        [Authorize]
        [Route("GetScheduleAgenda")]
        public HttpResponseMessage GetScheduleAgenda(ScheduleAgendaModel t)
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.GetScheduleAgenda(t));
        }

        #endregion

        #region detalle agenda // listo para pruebas
        /// <summary>
        /// Crea los detalle de las agendas dado un horario de la tabla horario agenda
        /// </summary>
        /// <remarks>
        /// PARAMETRO: idscheduleagenda [STRING] id horario agenda <br />
        /// PARAMETRO: idoffice [STRING] id del consultorio
        /// </remarks> 
        /// <param name="t"></param>
        /// <returns>regresa un mensaje de exito o una lista con errores</returns>
        [HttpPost]
        [Authorize]
        [Route("CreateScheduleDetail")]
        public HttpResponseMessage CreateScheduleDetail(ProfetionalScheduleModel t)
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.CreateScheduleDetail(t));
        }

        #endregion


        #region citas

        /// <summary>
        /// Entrega una lista con disponibilidades de citas
        /// </summary>
        /// <remarks>
        /// PARAMETRO: specialities [STRING] id de la especialidad a la cual pediran cita <br />      
        /// </remarks> 
        /// <param name="t"></param>
        /// <returns>Entrega una lista con disponibilidades de citas</returns>
        [HttpPost]
        [Route("GetAppointment")]
        public HttpResponseMessage GetAppointment(AppointmentModel t)
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.GetAppointment(t));
        }

        /// <summary>
        /// Entrega una lista con disponibilidades de citas
        /// </summary>
        /// <remarks>
        /// PARAMETRO: specialities [STRING] id de la especialidad a la cual pediran cita [requerido]<br />
        /// PARAMETRO: idprofetional [STRING] id de la especialidad a la cual pediran cita [optional]<br />
        /// </remarks> 
        /// <param name="t"></param>
        /// <returns>Entrega una lista con disponibilidades de citas</returns>
        [Authorize]
        [HttpPost]
        [Route("GetAppointmentByFilter")]
        public HttpResponseMessage GetAppointmentByFilter(AppointmentModel t)
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.GetAppointmentByFilter(t));
        }

        /// <summary>
        /// Entrega una lista con disponibilidades de citas
        /// </summary>
        /// <remarks>
        /// PARAMETRO: idpatient [STRING] id del paciente <br />  
        /// PARAMETRO: specialities [STRING] id de la especialidad que selecciono <br />  
        /// PARAMETRO: idcup [STRING] id del cup <br />  
        /// PARAMETRO: idscheduledatail [STRING] id del cup <br />  
        /// PARAMETRO: autorization [STRING] codido aprobacion<br />  
        /// PARAMETRO: id [STRING] id entidad<br /> 
        /// PARAMETRO: typequery [STRING] indica si es por EPS o particular<br /> 
        /// </remarks> 
        /// <param name="t"></param>
        /// <returns>Entrega una lista con disponibilidades de citas</returns>
        [HttpPost]
        [Route("TakeAppointment")]
        public HttpResponseMessage TakeAppointment(AppointmentModel t)
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.TakeAppointment(t));
        }

        /// <summary>
        /// Entrega una lista con disponibilidades de citas
        /// </summary>
        /// <remarks>        
        /// </remarks> 
        /// <param name="t"></param>
        /// <returns>Entrega una lista con disponibilidades de citas</returns>
        /// 
        [Authorize]
        [HttpGet]
        [Route("GetUnconfirmedAppoinmet")]
        public HttpResponseMessage GetUnconfirmedAppoinmet()
        {
            aSchedule s = new aSchedule();
            Basic t = new Basic();
            return ut.ReturnResponse(s.GetUnconfirmedAppoinmet(t));
        }

        /// <summary>
        /// Entrega una lista con disponibilidades de citas
        /// </summary>
        /// <remarks>
        /// PARAMETRO: idappointment [STRING] id del detalle de la cita <br />    
        /// </remarks> 
        /// <param name="t"></param>
        /// <returns>Entrega una lista con disponibilidades de citas</returns>
        /// 
        [Authorize]
        [HttpPost]
        [Route("ConfirmAppoinmet")]
        public HttpResponseMessage ConfirmAppoinmet(AppointmentModel t)
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.ConfirmAppoinmet(t));
        }


        /// <summary>
        /// Entrega una lista con disponibilidades de citas
        /// </summary>
        /// <remarks>
        /// PARAMETRO: idappointment [STRING] id del detalle de la cita <br />    
        /// </remarks> 
        /// <param name="t"></param>
        /// <returns>Entrega una lista con disponibilidades de citas</returns>
        /// 
        [Authorize]
        [HttpPost]
        [Route("DeleteAppoinmet")]
        public HttpResponseMessage DeleteAppoinmet(AppointmentModel t)
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.DeleteAppoinmet(t));
        }

        #endregion
    }
}
