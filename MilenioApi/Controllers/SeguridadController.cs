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
        /// <summary>
        /// Metodo para loguear
        /// </summary>
        /// <param name="user">String</param>
        /// <param name="password">String</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]        
        public HttpResponseMessage Login()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.Login(HttpContext.Current.Request));
        }


        /// <summary>
        /// metodo para desloguear un usuario
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("LogOff")]
        public HttpResponseMessage LogOff()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.LogOff(HttpContext.Current.Request));
        }

        /// <summary>
        /// Metodo para loguear con la entidad seleccionada
        /// </summary>
        /// <param name="user">String</param>
        /// <param name="password">String</param>
        /// <param name="identidad">String</param>
        /// <returns>info del usuario y token</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("LoginEntidad")]
        
        public HttpResponseMessage LoginEntidad()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.LoginEntidad(HttpContext.Current.Request));
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
        [Route("CreateEntidadUser")]
        public HttpResponseMessage CreateEntidadUser()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.CreateEntidadUser(HttpContext.Current.Request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("EditUser")]
        public HttpResponseMessage EditUser()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.EditUser(HttpContext.Current.Request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ValidateUser")]
        public HttpResponseMessage ValidateUser()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.ValidateUser(HttpContext.Current.Request));
        }
        /********************* CONSULTAS************************/
        [HttpPost]
        [AllowAnonymous]
        [Route("GetUsuarios")]
        public HttpResponseMessage GetUsuarios()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.GetUsuarios(HttpContext.Current.Request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GetUsuariosEdit")]
        public HttpResponseMessage GetUsuariosEdit()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.GetUsuariosEdit(HttpContext.Current.Request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GetTipoProfesional")]
        public HttpResponseMessage GetTipoProfesional()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.GetTipoProfesional(HttpContext.Current.Request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GetTipoIdentificacion")]
        public HttpResponseMessage GetTipoIdentificacion()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.GetTipoIdentificacion(HttpContext.Current.Request));
        }
        #endregion

        #region Especialidad profesional

        [HttpPost]
        [AllowAnonymous]
        [Route("CreateEspecialidadProfesional")]
        public HttpResponseMessage CreateEspecialidadProfesional()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.CreateEspecialidadProfesional(HttpContext.Current.Request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("EditEspecialidadProfesional")]
        public HttpResponseMessage EditEspecialidadProfesional()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.EditEspecialidadProfesional(HttpContext.Current.Request));
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
        [Route("CreateRolUsuario")]
        public HttpResponseMessage CreateRolUsuario()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.CreateRolUsuario(HttpContext.Current.Request));
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("EditRolUsuario")]
        public HttpResponseMessage EditRolUsuario()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.EditRolUsuario(HttpContext.Current.Request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GetNotRolesUsuario")]
        public HttpResponseMessage GetNotRolesUsuario()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.GetNotRolesUsuario(HttpContext.Current.Request));
        }
        #endregion
    }
}
