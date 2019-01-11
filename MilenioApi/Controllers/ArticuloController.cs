using MilenioApi.Action;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Articulo")]
    public class ArticuloController : ApiController
    {
        aUtilities ut = new aUtilities();

        [AllowAnonymous]
        [Route("CreateCategoria")]
        public HttpResponseMessage CreateCategoria()
        {
            aArticulo a = new aArticulo();
            return ut.ReturnResponse(a.CreateCategoria(HttpContext.Current.Request));
        }
        [AllowAnonymous]
        [Route("EditCategoria")]
        public HttpResponseMessage EditCategoria()
        {
            aArticulo a = new aArticulo();
            return ut.ReturnResponse(a.EditCategoria(HttpContext.Current.Request));
        }


        [AllowAnonymous]
        [Route("CreateSubCategoria")]
        public HttpResponseMessage CreateSubCategoria()
        {
            aArticulo a = new aArticulo();
            return ut.ReturnResponse(a.CreateSubCategoria(HttpContext.Current.Request));
        }

        [AllowAnonymous]
        [Route("EditSubCategoria")]
        public HttpResponseMessage EditSubCategoria()
        {
            aArticulo a = new aArticulo();
            return ut.ReturnResponse(a.EditSubCategoria(HttpContext.Current.Request));
        }
    }
}
