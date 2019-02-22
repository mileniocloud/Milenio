using MilenioApi.Action;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Listas")]
    public class ListasGenericasController : ApiController
    {
        aUtilities ut = new aUtilities();

        [HttpPost]
        [AllowAnonymous]
        [Route("GetPoblado")]
        public HttpResponseMessage GetPoblado()
        {
            aListasGenericas s = new aListasGenericas();
            return ut.ReturnResponse(s.GetPoblado(HttpContext.Current.Request));
        }
    }
}
