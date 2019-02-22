using MilenioApi.Action;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Consultorio")]
    public class ConsultorioController : ApiController
    {
        aUtilities ut = new aUtilities();
        [HttpPost]
        [AllowAnonymous]
        [Route("CreateConsultorio")]
        public HttpResponseMessage CreateConsultorio()
        {
            aConsultorio s = new aConsultorio();
            return ut.ReturnResponse(s.CreateConsultorio(HttpContext.Current.Request));
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("EditConsultorio")]
        public HttpResponseMessage EditConsultorio()
        {
            aConsultorio s = new aConsultorio();
            return ut.ReturnResponse(s.EditConsultorio(HttpContext.Current.Request));
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("GetConsultorio")]
        public HttpResponseMessage GetConsultorio()
        {
            aConsultorio s = new aConsultorio();
            return ut.ReturnResponse(s.GetConsultorio(HttpContext.Current.Request));
        }


        #region Relaciondas
        [HttpPost]
        [AllowAnonymous]
        [Route("CreateConsultorioEspecialidad")]
        public HttpResponseMessage CreateConsultorioEspecialidad()
        {
            aConsultorio s = new aConsultorio();
            return ut.ReturnResponse(s.CreateConsultorioEspecialidad(HttpContext.Current.Request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("EditConsultorioEspecialidad")]
        public HttpResponseMessage EditConsultorioEspecialidad()
        {
            aConsultorio s = new aConsultorio();
            return ut.ReturnResponse(s.EditConsultorioEspecialidad(HttpContext.Current.Request));
        }

        #endregion
    }
}
