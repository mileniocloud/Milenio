using MilenioApi.Action;
using MilenioApi.Models;
using System.Net.Http;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/Security")]
    public class SecurityController : ApiController
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
        [Route("Login")]
        public HttpResponseMessage Login([FromBody] LoginModel t)
        {
            aSecurity s = new aSecurity();
            return ut.ReturnResponse(s.Login(t));
        }
        /// <summary>
        /// Metodo para desloguear el usuario
        /// </summary>
        /// <remarks>
        /// PARAMETRO: token [STRING]
        /// </remarks>
        [HttpPost]
        [Authorize]
        [Route("LogOff")]
        public HttpResponseMessage LogOff([FromBody] LoginModel t)
        {
            aSecurity s = new aSecurity();
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
        [Route("LoginEntidad")]
        public HttpResponseMessage LoginEntidad([FromBody] LoginModel t)
        {
            aSecurity s = new aSecurity();
            return ut.ReturnResponse(s.LoginEntidad(t));
        }
        /// <summary>
        /// Se usa para enviar un correo que le permitira cambiar la clave
        /// </summary>
        /// <param name="t"></param>
        /// <remarks>
        /// PARAMETRO: user [STRING]
        /// </remarks>
        /// <returns>REGRESA UN MENSAJE INDICANDO QUE EL CORREO FUE ENVIADO</returns>      
        [Route("ForgotPassword")]
        public HttpResponseMessage ForgotPassword([FromBody] LoginModel t)
        {
            aSecurity s = new aSecurity();
            return ut.ReturnResponse(s.ForgotPassword(t));
        }

        /// <summary>
        /// Se usa para enviar un correo que le permitira cambiar la clave
        /// </summary>
        /// <param name="t"></param>
        /// <remarks> 
        /// PARAMETRO: password [STRING]
        /// PARAMETRO: oldpassword [STRING]
        /// </remarks>
        /// <returns>REGRESA UN MENSAJE INDICANDO QUE LA CLAVE CAMBIO</returns>   
        [Route("ChangePassword")]
        [Authorize]
        public HttpResponseMessage ChangePassword([FromBody] LoginModel t)
        {
            aSecurity s = new aSecurity();
            return ut.ReturnResponse(s.ChangePassword(t));
        }

        /// <summary>
        /// Se usa para enviar un correo que le permitira cambiar la clave
        /// </summary>
        /// <param name="t"></param>
        /// <remarks>
        /// PARAMETRO: user [STRING] - el usuario que desea cambiar la clave
        /// </remarks>
        /// <returns>REGRESA UN MENSAJE INDICANDO QUE EL CORREO FUE ENVIADO</returns>   
        [Route("ResetPassword")]
        public HttpResponseMessage ResetPassword([FromBody] LoginModel t)
        {
            aSecurity s = new aSecurity();
            return ut.ReturnResponse(s.ResetPassword(t));
        }
        #endregion       
    }
}
