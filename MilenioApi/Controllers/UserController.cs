using MilenioApi.Action;
using MilenioApi.Models;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MilenioApi.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        aUtilities ut = new aUtilities();

        #region Create - ActInactivate - list

        /// <summary>
        /// Metodo crear usuarios del sistema
        /// </summary>
        /// <remarks>
        /// PARAMETRO: typedocument [STRING] [REQUERIDO]<br />
        /// PARAMETRO: document  [STRING] [REQUERIDO] <br />
        /// PARAMETRO: fullname [STRING] [REQUERIDO] <br />
        /// PARAMETRO: firstlastname  [STRING] [REQUERIDO]<br />
        /// PARAMETRO: secondlastname  [STRING] [REQUERIDO]<br />
        /// PARAMETRO: gender [STRING] <br />
        /// PARAMETRO: birthdate [DATETIME] [FORTMAT DD/MM/YYYY] [REQUERIDO]<br />
        /// PARAMETRO: photo [STRING] <br />
        /// PARAMETRO: civilstatus  [STRING] <br />
        /// PARAMETRO: bloodtype [STRING] <br />
        /// PARAMETRO: neighborhood  [INT] [REQUERIDO]<br />
        /// PARAMETRO: address [STRING] <br />
        /// PARAMETRO: telephone  [STRING] <br />
        /// PARAMETRO: dateofhire [DATETIME] [FORTMAT DD/MM/YYYY] [REQUERIDO]<br />
        /// PARAMETRO: observation [STRING] <br />
        /// PARAMETRO: linktype [STRING] [REQUERIDO]<br />
        /// PARAMETRO: serviceprovider [BOOL] [1 "O" 0] [REQUERIDO]<br />
        /// PARAMETRO: login [STRING] [REQUERIDO]<br />
        /// PARAMETRO: password [STRING] [REQUERIDO]<br />
        /// PARAMETRO: email [STRING] [REQUERIDO]<br />
        /// PARAMETRO: habeas  [BOOL] [1 "O" 0] [REQUERIDO]<br />
        /// PARAMETRO: photohabeas [STRING] <br />
        /// PARAMETRO: typeprofessional  [GUID] <br />
        /// PARAMETRO: registryprofessional  [STRING] <br />        
        /// PARAMETRO: roles [STRING] [ID DE ROLES SEPARADOS POR ","] <br />   
        /// </remarks>
        /// <returns>Regresa un objeto indicando el estado de la transaccion </returns>
        [HttpPost]
        [Authorize]
        [Route("CreateUser")]
        public HttpResponseMessage CreateUser(UserModel t)
        {
            aUser s = new aUser();
            return ut.ReturnResponse(s.CreateUser(t));
        }

        /// <summary>
        /// Metodo para editar una entidad
        /// </summary>
        /// <remarks>        
        /// PARAMETRO: gender [STRING] <br />
        /// PARAMETRO: birthdate [DATETIME] [FORTMAT DD/MM/YYYY] [REQUERIDO]<br />
        /// PARAMETRO: photo [STRING] <br />
        /// PARAMETRO: civilstatus  [STRING] <br />
        /// PARAMETRO: bloodtype [STRING] <br />
        /// PARAMETRO: neighborhood  [INT] [REQUERIDO]<br />
        /// PARAMETRO: address [STRING] <br />
        /// PARAMETRO: telephone  [STRING] <br />
        /// PARAMETRO: dateofhire [DATETIME] [FORTMAT DD/MM/YYYY] [REQUERIDO]<br />
        /// PARAMETRO: observation [STRING] <br />
        /// PARAMETRO: linktype [STRING] [REQUERIDO]<br />
        /// PARAMETRO: serviceprovider [BOOL] [1 "O" 0] [REQUERIDO]<br />
        /// PARAMETRO: login [STRING] [REQUERIDO]<br />
        /// PARAMETRO: password [STRING] [REQUERIDO]<br />
        /// PARAMETRO: email [STRING] [REQUERIDO]<br />
        /// PARAMETRO: habeas  [BOOL] [1 "O" 0] [REQUERIDO]<br />
        /// PARAMETRO: photohabeas [STRING] <br />
        /// PARAMETRO: typeprofessional  [GUID] <br />
        /// PARAMETRO: registryprofessional  [STRING] <br />        
        /// PARAMETRO: roles [STRING] [ID DE ROLES SEPARADOS POR ","] <br />   
        /// </remarks>
        /// <returns>Regresa un objeto indicando el estado de la transaccion </returns>
        [HttpPost]
        [Authorize]
        [Route("EditUser")]
        public HttpResponseMessage EditUser(UserModel t)
        {
            aUser s = new aUser();
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
        [Authorize]
        [Route("CreateEntityUser")]
        public HttpResponseMessage CreateEntityUser(UserModel t)
        {
            aUser s = new aUser();
            return ut.ReturnResponse(s.CreateEntityUser(t));
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
        [Authorize]
        [Route("ValidateUser")]
        public HttpResponseMessage ValidateUser(UserModel t)
        {
            aUser s = new aUser();
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
        [Authorize]
        [Route("GetUser")]
        public HttpResponseMessage GetUser(UserModel t)
        {
            aUser s = new aUser();
            return ut.ReturnResponse(s.GetUser(t));
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
        [Authorize]
        [Route("GetUserEdit")]
        public HttpResponseMessage GetUserEdit(UserModel t)
        {
            aUser s = new aUser();
            return ut.ReturnResponse(s.GetUserEdit(t));
        }
              
        #endregion

        #region Profile

        /// <summary>
        /// Metodo consultar el perfil del usuario
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <returns>Regresa informacion completa del usuario </returns>
        [HttpGet]
        [Authorize]
        [Route("UserProfile")]
        public HttpResponseMessage UserProfile()
        {
            UserModel t = new UserModel();
            aUser s = new aUser();
            return ut.ReturnResponse(s.UserProfile(t));

        }

        /// <summary>
        /// Metodo editar el perfil del usuario
        /// </summary>
        /// <remarks>
        /// PARAMETRO: telephone [STRING] <br />
        /// PARAMETRO: photo [STRING] <br />
        /// PARAMETRO: neighborhood [STRING] <br />
        /// PARAMETRO: address [STRING] <br />
        /// PARAMETRO: Email [STRING] <br />
        /// PARAMETRO: civilstatus [STRING]
        /// </remarks>
        /// <returns>Regresa informacion completa del usuario </returns>
        [HttpPost]
        [Authorize]
        [Route("EditProfile")]
        public HttpResponseMessage EditProfile(UserModel t)
        {
            aUser s = new aUser();
            return ut.ReturnResponse(s.EditProfile(t));
        }
        #endregion
    }
}
