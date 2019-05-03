using MilenioApi.Action;
using MilenioApi.Models;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/agenda")]
    public class ScheduleController : ApiController
    {
        aUtilities ut = new aUtilities();
        #region agenda profesional
        /// <summary>
        /// Metodo para crear la agenda general del medico
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("createprofetionalschedule")]
        public HttpResponseMessage CreateAgendaProfesional(ProfetionalScheduleModel t)
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.CreateAgendaProfesional(t));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("EditAgendaProfesional")]
        public HttpResponseMessage EditAgendaProfesional()
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.EditAgendaProfesional(HttpContext.Current.Request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GetAgendaProfesional")]
        public HttpResponseMessage GetAgendaProfesional()
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.GetAgendaProfesional(HttpContext.Current.Request));
        }
        #endregion

        #region horario agenda
        [HttpPost]
        [Authorize]
        [Route("CreateScheduleAgenda")]
        public HttpResponseMessage CreateScheduleAgenda(ScheduleAgendaModel t)
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.CreateScheduleAgenda(t));
        }

        [HttpPost]
        [Authorize]
        [Route("EditScheduleAgenda")]
        public HttpResponseMessage EditScheduleAgenda(ScheduleAgendaModel t)
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.EditScheduleAgenda(t));
        }

        #endregion


        #region detalle agenda

        [HttpPost]
        [Authorize]
        [Route("CreateScheduleDetail")]
        public HttpResponseMessage CreateScheduleDetail(ScheduleDetailModel t)
        {
            aSchedule s = new aSchedule();
            return ut.ReturnResponse(s.CreateScheduleDetail(t));
        }

        #endregion
    }
}
