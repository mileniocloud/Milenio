using MilenioApi.Action;
using MilenioApi.Models;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Persona")]
    public class PersonaController : ApiController
    {        
        [AllowAnonymous]
        [Route("CreatePersona")]
        public Return CreatePersona()
        {
            aPersona e = new aPersona();
            return e.CreatePersona(HttpContext.Current.Request);
        }
    }
}
