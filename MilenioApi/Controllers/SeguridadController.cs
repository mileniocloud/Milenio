using MilenioApi.Action;
using MilenioApi.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        /// Metodo para loguear y traer la lista de entidades
        /// </summary>
        /// <remarks>
        /// PARAMETRO: user [STRING] <br />
        /// PARAMETRO: password [STRING]
        /// </remarks>
        /// <returns>Regresa un usuario con la lista de entidades a las que pertenece </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public HttpResponseMessage Login([FromBody] LoginModel t)
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.Login(t));
        }
        /// <summary>
        /// Metodo para desloguear el usuario
        /// </summary>
        /// <remarks>
        /// PARAMETRO: token [STRING]
        /// </remarks>
        [HttpPost]
        [AllowAnonymous]
        [Route("LogOff")]
        public HttpResponseMessage LogOff([FromBody] LoginModel t)
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.LogOff(t));
        }

        /// <summary>
        /// Metodo para loguear y retornar el token + los roles
        /// </summary>
        /// <remarks>
        /// PARAMETRO: user [STRING] <br />
        /// PARAMETRO: password [STRING] <br />
        /// PARAMETRO: identidad [STRING] <br /> <br />
        /// ON ERROR REGRESA LA SIG ESTRUCTURA: {<br />
        ///"errorlist": [ <br />
        ///    {<br />
        ///      "errorlist": [],<br />
        ///      "response_code": null,<br />
        ///      "custom": null,<br />
        ///      "message": "El campo password es obligatorio.",<br />
        ///      "name": "password",<br />
        ///      "id": null<br />
        ///    }<br />
        ///  ],<br />
        ///  "response_code": "03",<br />
        ///  "custom": "",<br />
        ///  "message": "Login Invalido ",<br />
        ///  "name": null,<br />
        ///  "id": null<br />
        ///} <br /><br />
        /// </remarks>
        /// <returns>Regresa el token + los roles asociados a esa entidad seleccionada</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("LoginEntidad")]
        public HttpResponseMessage LoginEntidad([FromBody] LoginModel t)
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.LoginEntidad(t));
        }
        /// <summary>
        /// Se usa para enviar un correo que le permitira cambiar la clave
        /// </summary>
        /// <param name="t"></param>
        /// <remarks>
        /// PARAMETRO: token [STRING]
        /// </remarks>
        /// <returns>REGRESA UN MENSAJE INDICANDO QUE EL CORREO FUE ENVIADO</returns>
        [AllowAnonymous]
        [Route("OlvidoClave")]
        public HttpResponseMessage OlvidoClave([FromBody] LoginModel t)
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.OlvidoClave(t));
        }

        /// <summary>
        /// Se usa para enviar un correo que le permitira cambiar la clave
        /// </summary>
        /// <param name="t"></param>
        /// <remarks>
        /// PARAMETRO: token [STRING] <br />
        /// PARAMETRO: password [STRING] <br />
        /// </remarks>
        /// <returns>REGRESA UN MENSAJE INDICANDO QUE LA CALVE CAMBIO</returns>
        [AllowAnonymous]
        [Route("CambioClave")]
        public HttpResponseMessage CambioClave([FromBody] LoginModel t)
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.CambioClave(t));
        }
        #endregion

        #region Create - ActInactivate - list

        /// <summary>
        /// Metodo crear usuarios del sistema dada una entidad
        /// </summary>
        /// <remarks>
        /// PARAMETRO: Id_Tipo_Identificacion [STRING] [REQUERIDO]<br />
        /// PARAMETRO: Numero_Identificacion [STRING] [REQUERIDO] <br />
        /// PARAMETRO: Nombres [STRING] [REQUERIDO] <br />
        /// PARAMETRO: Primer_Apellido [STRING] [REQUERIDO]<br />
        /// PARAMETRO: Segundo_Apellido [STRING] [REQUERIDO]<br />
        /// PARAMETRO: Sexo [STRING] <br />
        /// PARAMETRO: Fecha_Nacimiento [DATETIME] [FORTMAT DD/MM/YYYY] [REQUERIDO]<br />
        /// PARAMETRO: Foto [STRING] <br />
        /// PARAMETRO: Estado_Civil [STRING] <br />
        /// PARAMETRO: Tipo_Sangre [STRING] <br />
        /// PARAMETRO: Poblado_Id [INT] [REQUERIDO]<br />
        /// PARAMETRO: Direccion [STRING] <br />
        /// PARAMETRO: Telefono [STRING] <br />
        /// PARAMETRO: Fecha_Contratacion [DATETIME] [FORTMAT DD/MM/YYYY] [REQUERIDO]<br />
        /// PARAMETRO: Observaciones [STRING] <br />
        /// PARAMETRO: Tipo_Vinculacion [STRING] [REQUERIDO]<br />
        /// PARAMETRO: Presta_Servicio [BOOL] [1 "O" 0] [REQUERIDO]<br />
        /// PARAMETRO: Login [STRING] [REQUERIDO]<br />
        /// PARAMETRO: Password [STRING] [REQUERIDO]<br />
        /// PARAMETRO: Email [STRING] [REQUERIDO]<br />
        /// PARAMETRO: Acepta_ABEAS [BOOL] [1 "O" 0] [REQUERIDO]<br />
        /// PARAMETRO: Foto_ABEAS [STRING] <br />
        /// PARAMETRO: Id_Tipo_Profesional [GUID] <br />
        /// PARAMETRO: Registro_Profesional [STRING] <br />        
        /// PARAMETRO: List_Roles [STRING] [ID DE ROLES SEPARADOS POR ","] <br />   
        /// PARAMETRO: token [STRING]
        /// </remarks>
        /// <returns>Regresa un objeto indicando el estado de la transaccion </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("CreateUser")]
        public HttpResponseMessage CreateUser(UsuarioModel t)
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.CreateUser(t));
        }

        /// <summary>
        /// Metodo para editar usuarios (al editar un usuario de edita para todas las entidades)
        /// </summary>
        /// <remarks>
        /// PARAMETRO: Id_Tipo_Identificacion [STRING] [REQUERIDO]<br />
        /// PARAMETRO: Numero_Identificacion [STRING] [REQUERIDO] <br />
        /// PARAMETRO: Nombres [STRING] [REQUERIDO] <br />
        /// PARAMETRO: Primer_Apellido [STRING] [REQUERIDO]<br />
        /// PARAMETRO: Segundo_Apellido [STRING] [REQUERIDO]<br />
        /// PARAMETRO: Sexo [STRING] <br />
        /// PARAMETRO: Fecha_Nacimiento [DATETIME] [FORTMAT DD/MM/YYYY] [REQUERIDO]<br />
        /// PARAMETRO: Foto [STRING] <br />
        /// PARAMETRO: Estado_Civil [STRING] <br />
        /// PARAMETRO: Tipo_Sangre [STRING] <br />
        /// PARAMETRO: Poblado_Id [INT] [REQUERIDO]<br />
        /// PARAMETRO: Direccion [STRING] <br />
        /// PARAMETRO: Telefono [STRING] <br />
        /// PARAMETRO: Fecha_Contratacion [DATETIME] [FORTMAT DD/MM/YYYY] [REQUERIDO]<br />
        /// PARAMETRO: Observaciones [STRING] <br />
        /// PARAMETRO: Tipo_Vinculacion [STRING] [REQUERIDO]<br />
        /// PARAMETRO: Presta_Servicio [BOOL] [1 "O" 0] [REQUERIDO]<br />
        /// PARAMETRO: Login [STRING] [REQUERIDO]<br />
        /// PARAMETRO: Password [STRING] [REQUERIDO]<br />
        /// PARAMETRO: Email [STRING] [REQUERIDO]<br />
        /// PARAMETRO: Acepta_ABEAS [BOOL] [1 "O" 0] [REQUERIDO]<br />
        /// PARAMETRO: Foto_ABEAS [STRING] <br />
        /// PARAMETRO: Id_Tipo_Profesional [GUID] <br />
        /// PARAMETRO: Registro_Profesional [STRING] <br />        
        /// PARAMETRO: List_Roles [STRING] [ID DE ROLES SEPARADOS POR ","] <br />   
        /// PARAMETRO: token [STRING]
        /// </remarks>
        /// <returns>Regresa un objeto indicando el estado de la transaccion </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("EditUser")]
        public HttpResponseMessage EditUser(UsuarioModel t)
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.EditUser(t));
        }

        /// <summary>
        /// Metodo para agregar un usuario exixtente a la entidd a la que se esta tratando de ingresar como usuario nuevo
        /// </summary>
        /// <remarks>
        /// PARAMETRO: Numero_Identificacion [STRING]
        /// PARAMETRO: token [STRING]
        /// </remarks>
        /// <returns>Regresa informacion si el usuario existe o no </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("CreateEntidadUser")]
        public HttpResponseMessage CreateEntidadUser(UsuarioModel t)
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.CreateEntidadUser(t));
        }
        /// <summary>
        /// Metodo para validar si una cedula ya existe en alguna entidad para preguntar si desean agregarlo
        /// </summary>
        /// <remarks>
        /// PARAMETRO: Numero_Identificacion [STRING]
        /// PARAMETRO: token [STRING]
        /// </remarks>
        /// <returns>Regresa informacion si el usuario existe o no </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("ValidateUser")]
        public HttpResponseMessage ValidateUser(UsuarioModel t)
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.ValidateUser(t));
        }
        /********************* CONSULTAS************************/

        /// <summary>
        /// Metodo para traer lista de usuarios, contiene logica para consultar por varios campos 
        /// 
        /// </summary>
        /// <remarks>
        /// PARAMETRO: Numero_Identificacion [STRING]
        /// PARAMETRO: token [STRING]
        /// </remarks>
        /// <returns>Regresa informacion si el usuario existe o no </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("GetUsuarios")]
        public HttpResponseMessage GetUsuarios(UsuarioModel t)
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.GetUsuarios(t));
        }
        /// <summary>
        /// Metodo para consultar un usuario por su ID
        /// </summary>
        /// <remarks>
        /// PARAMETRO: Id_Usuario [GUID] [REQUERIDO]
        /// PARAMETRO: token [STRING]
        /// </remarks>
        /// <returns>Regresa informacion completa del usuario </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("GetUsuariosEdit")]
        public HttpResponseMessage GetUsuariosEdit(UsuarioModel t)
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.GetUsuariosEdit(t));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GetTipoProfesional")]
        public HttpResponseMessage GetTipoProfesional()
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.GetTipoProfesional(HttpContext.Current.Request));
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
        public HttpResponseMessage GetRoles(UsuarioModel t)
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.GetRoles(t));
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("GetRolesUsuario")]
        public HttpResponseMessage GetRolesUsuario(UsuarioModel t)
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.GetRolesUsuario(t));
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("CreateRolUsuario")]
        public HttpResponseMessage CreateRolUsuario(UsuarioModel t)
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.CreateRolUsuario(t));
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("EditRolUsuario")]
        public HttpResponseMessage EditRolUsuario(UsuarioModel t)
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.EditRolUsuario(t));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GetNotRolesUsuario")]
        public HttpResponseMessage GetNotRolesUsuario(UsuarioModel t)
        {
            aSeguridad s = new aSeguridad();
            return ut.ReturnResponse(s.GetNotRolesUsuario(t));
        }
        #endregion
    }
}
