using MilenioApi.Action;
using MilenioApi.Models;
using System.Net.Http;
using System.Web;
using System.Web.Http;


namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Dashboard")]
    public class DashboardController : ApiController
    {
        aUtilities ut = new aUtilities();
        /// <summary>
        /// Metodo para construir el dashboard
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <returns>Regresa el json con el menu </returns>
        [HttpPost]
        [Authorize]
        [Route("Dashboard")]
        public HttpResponseMessage Dashboard(DashboardModel t)
        {
            aDashboard s = new aDashboard();
            return ut.ReturnResponse(s.Dashboard(t));
        }
    }
}