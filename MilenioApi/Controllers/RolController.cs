using MilenioApi.Action;
using MilenioApi.Models;
using System.Net.Http;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Rol")]
    public class RolController : ApiController
    {
        aUtilities ut = new aUtilities();

        #region Seccion Rol
        [HttpPost]
        [Authorize]
        [Route("GetRoles")]
        public HttpResponseMessage GetRoles(UserModel t)
        {
            aRol s = new aRol();
            return ut.ReturnResponse(s.GetRoles(t));
        }
        [HttpPost]
        [Authorize]
        [Route("GetRolesUsuario")]
        public HttpResponseMessage GetRolesUsuario(UserModel t)
        {
            aRol s = new aRol();
            return ut.ReturnResponse(s.GetRolesUsuario(t));
        }
        [HttpPost]
        [Authorize]
        [Route("CreateRolUsuario")]
        public HttpResponseMessage CreateRolUsuario(UserModel t)
        {
            aRol s = new aRol();
            return ut.ReturnResponse(s.CreateRolUsuario(t));
        }
        [HttpPost]
        [Authorize]
        [Route("EditRolUsuario")]
        public HttpResponseMessage EditRolUsuario(UserModel t)
        {
            aRol s = new aRol();
            return ut.ReturnResponse(s.EditRolUsuario(t));
        }

        [HttpPost]
        [Authorize]
        [Route("GetNotRolesUsuario")]
        public HttpResponseMessage GetNotRolesUsuario(UserModel t)
        {
            aRol s = new aRol();
            return ut.ReturnResponse(s.GetNotRolesUsuario(t));
        }
        #endregion
    }
}
