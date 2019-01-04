using MilenioApi.Action;
using MilenioApi.Models;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Entidad")]
    public class EntidadController : ApiController
    {
        [AllowAnonymous]
        [Route("CreateEntidad")]
        public Return CreateEntidad()
        {
            aEntidad e = new aEntidad();
            return e.CreateEntidad(HttpContext.Current.Request);
        }
    }
}
