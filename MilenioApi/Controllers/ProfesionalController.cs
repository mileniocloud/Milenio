using MilenioApi.Action;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Profesional")]
    public class ProfesionalController : ApiController
    {
        aUtilities ut = new aUtilities();

        //#region Crud
        //[HttpPost]
        //[AllowAnonymous]
        //[Route("CreateProfesional")]
        //public HttpResponseMessage CreateProfesional()
        //{
        //    aProfesionalcs s = new aProfesionalcs();
        //    return ut.ReturnResponse(s.CreateProfesional(HttpContext.Current.Request));
        //}
        //[HttpPost]
        //[AllowAnonymous]
        //[Route("EditProfesional")]
        //public HttpResponseMessage EditProfesional()
        //{
        //    aProfesionalcs s = new aProfesionalcs();
        //    return ut.ReturnResponse(s.EditProfesional(HttpContext.Current.Request));
        //}
        //#endregion
    }
}
