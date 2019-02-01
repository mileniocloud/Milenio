using MilenioApi.Action;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Entidad")]
    public class EntidadController : ApiController
    {
        aUtilities ut = new aUtilities();
        [AllowAnonymous]
        [Route("Espcinsert")]
        public void Espcinsert()
        {
            aEntidad e = new aEntidad();
            e.Espcinsert();
        }


        //[AllowAnonymous]
        //[Route("CreateEntidad")]
        //public HttpResponseMessage CreateEntidad()
        //{
        //    aEntidad e = new aEntidad();
        //    return ut.ReturnResponse(e.CreateEntidad(HttpContext.Current.Request));
        //}

        //[AllowAnonymous]
        //[Route("EditEntidad")]
        //public HttpResponseMessage EditEntidad()
        //{
        //    aEntidad e = new aEntidad();
        //    return ut.ReturnResponse(e.EditEntidad(HttpContext.Current.Request));
        //}

        //[AllowAnonymous]
        //[Route("CreateAula")]
        //public HttpResponseMessage CreateAula()
        //{
        //    aEntidad e = new aEntidad();
        //    return ut.ReturnResponse(e.CreateAula(HttpContext.Current.Request));
        //}
        //[AllowAnonymous]
        //[Route("EditAula")]
        //public HttpResponseMessage EditAula()
        //{
        //    aEntidad e = new aEntidad();
        //    return ut.ReturnResponse(e.EditAula(HttpContext.Current.Request));
        //}

        //[AllowAnonymous]
        //[Route("GetAula")]
        //public HttpResponseMessage GetAula()
        //{
        //    aEntidad e = new aEntidad();
        //    return ut.ReturnResponse(e.GetAula(HttpContext.Current.Request));
        //}
    }
}
