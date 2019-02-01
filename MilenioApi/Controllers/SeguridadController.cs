using MilenioApi.Action;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Seguridad")]
    public class SeguridadController : ApiController
    {
        aUtilities ut = new aUtilities();

        #region Login
        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public HttpResponseMessage Login()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.Login(HttpContext.Current.Request));
        }
        [AllowAnonymous]
        [Route("OlvidoClave")]
        public HttpResponseMessage OlvidoClave()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.OlvidoClave(HttpContext.Current.Request));
        }
        [AllowAnonymous]
        [Route("CambioClave")]
        public HttpResponseMessage CambioClave()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.CambioClave(HttpContext.Current.Request));
        }
        #endregion

        #region Create - ActInactivate - list
        [HttpPost]
        [AllowAnonymous]
        [Route("CreateUser")]
        public HttpResponseMessage CreateUser()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.CreateUser(HttpContext.Current.Request));
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("ActInactivateUser")]
        public HttpResponseMessage ActInactivateUser()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.ActInactivateUser(HttpContext.Current.Request));
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("GetUsuarios")]
        public HttpResponseMessage GetUsuarios()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.GetUsuarios(HttpContext.Current.Request));
        }
        #endregion
        
        #region Seccion Rol
        [HttpPost]
        [AllowAnonymous]
        [Route("GetRoles")]
        public HttpResponseMessage GetRoles()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.GetRoles(HttpContext.Current.Request));
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("GetRolesUsuario")]
        public HttpResponseMessage GetRolesUsuario()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.GetRolesUsuario(HttpContext.Current.Request));
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("AgregarRolUsuario")]
        public HttpResponseMessage AgregarRolUsuario()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.AgregarRolUsuario(HttpContext.Current.Request));
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("EliminaRolUsuario")]
        public HttpResponseMessage EliminaRolUsuario()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.EliminaRolUsuario(HttpContext.Current.Request));
        }
        #endregion
    }
}
