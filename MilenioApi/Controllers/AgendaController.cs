using MilenioApi.Action;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Agenda")]
    public class AgendaController : ApiController
    {
        aUtilities ut = new aUtilities();
        #region agenda profesional

        [HttpPost]
        [AllowAnonymous]
        [Route("CreateAgendaProfesional")]
        public HttpResponseMessage CreateAgendaProfesional()
        {
            aAgenda s = new aAgenda();
            return ut.ReturnResponse(s.CreateAgendaProfesional(HttpContext.Current.Request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("EditAgendaProfesional")]
        public HttpResponseMessage EditAgendaProfesional()
        {
            aAgenda s = new aAgenda();
            return ut.ReturnResponse(s.EditAgendaProfesional(HttpContext.Current.Request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GetAgendaProfesional")]
        public HttpResponseMessage GetAgendaProfesional()
        {
            aAgenda s = new aAgenda();
            return ut.ReturnResponse(s.GetAgendaProfesional(HttpContext.Current.Request));
        }
        #endregion

        #region horario agenda
        [HttpPost]
        [AllowAnonymous]
        [Route("CreateHorarioAgenda")]
        public HttpResponseMessage CreateHorarioAgenda()
        {
            aAgenda s = new aAgenda();
            return ut.ReturnResponse(s.CreateHorarioAgenda(HttpContext.Current.Request));
        }

        #endregion

    }
}
