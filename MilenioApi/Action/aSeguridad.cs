using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Web;
using WebApi.Jwt;

namespace MilenioApi.Action
{
    public class aSeguridad
    {
        private TokenController tk = new TokenController();
        aUtilities autil = new aUtilities();
        ClaimsPrincipal cp = new ClaimsPrincipal();

        #region Login        
        public LoginModel Login(HttpRequest httpRequest)
        {
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                LoginModel op = new LoginModel();
                try
                {
                    string login = Convert.ToString(httpRequest.Form["user"]);
                    string pass = Convert.ToString(httpRequest.Form["password"]);
                    pass = autil.Sha(pass);

                    Usuario p = ent.Usuario.Where(pr => pr.Login == login && pr.Password == pass).SingleOrDefault();

                    if (p != null)
                    {
                        int cantr = p.Rol.Where(t => t.Estado == true).Count();
                        if (cantr != 0)
                        {
                            op.Id_User = p.Id_Usuario;
                            op.Login = p.Login;
                            op.Email = p.Email;

                            List<ComboModel> roles = (p.Rol.Where(c => c.Estado == true)
                                                    .Select(t => new ComboModel
                                                    {
                                                        Id = t.Id_Rol,
                                                        Value = t.Nombre
                                                    }).ToList());

                            op.Roles = roles.GroupBy(rl => rl.Id).Select(g => g.First()).ToList();
                            op.token = JwtManager.GenerateToken(p.Login, p.Id_Usuario.ToString(), roles, p.Id_Entidad);
                        }
                        else
                        {
                            //USUARIO SIN ROLES
                            Basic b = new Basic();
                            autil.MensajeRetorno(ref b, 13, string.Empty, null);

                            op.Codigo = b.Codigo;
                            op.custom = b.custom;
                            op.Message = b.Message;
                            return op;
                        }
                    }
                    else
                    {
                        //login invalido
                        Basic b = new Basic();
                        autil.MensajeRetorno(ref b, 8, string.Empty, null);

                        op.Codigo = b.Codigo;
                        op.custom = b.custom;
                        op.Message = b.Message;
                        return op;
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    Basic b = new Basic();
                    autil.MensajeRetorno(ref b, 4, ex.Message + httpRequest.Form, null);

                    op.Codigo = b.Codigo;
                    op.custom = b.custom;
                    op.Message = b.Message;

                    return op;
                }

                return op;
            }
        }

        /// <summary>
        /// Metodo que envia correo con opcion para cambio de clave
        /// </summary>
        /// <returns></returns>
        public Basic OlvidoClave(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                string Login = Convert.ToString(httpRequest.Form["user"]);
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Usuario p = ent.Usuario.Where(u => u.Login == Login).SingleOrDefault();
                    if (p != null)
                    {
                        //metodo para enviar reseteo de contraseña                        
                        if (!string.IsNullOrEmpty(p.Email))
                        {
                            string token = JwtManager.GenerateToken(p.Login, p.Id_Usuario.ToString(), null, null);
                            SendMail(p.Email, token);
                            p.Cambiar_Clave = true;
                            ent.SaveChanges();
                            ret = autil.MensajeRetorno(ref ret, 21, string.Empty, null);
                        }
                        else
                        {
                            ///mensuaje no tiene email
                            ret = autil.MensajeRetorno(ref ret, 32, string.Empty, null);
                        }
                    }
                    else
                    {
                        ///mensaje login no existe
                        ret = autil.MensajeRetorno(ref ret, 15, string.Empty, null);
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                //error general
                ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                return ret;
            }
        }

        /// <summary>
        /// metodo para cambiar le la clave a un usuario
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic CambioClave(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                if (cp != null)
                {
                    Guid? user_id = null;
                    string usid = cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(usid))
                        user_id = Guid.Parse(usid);
                    string PassWord = Convert.ToString(httpRequest.Form["Password"]);

                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Usuario p = ent.Usuario.Where(u => u.Id_Usuario == user_id).SingleOrDefault();

                        if (p != null)
                        {
                            PassWord = autil.Sha(PassWord);
                            p.Password = PassWord;
                            ent.SaveChanges();
                            ret = autil.MensajeRetorno(ref ret, 20, string.Empty, null);
                        }
                        else
                        {
                            ret = autil.MensajeRetorno(ref ret, 15, string.Empty, null);
                        }
                    }
                    return ret;
                }
                else
                {
                    //token invalido
                    ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
                    return ret;
                }
            }
            catch (Exception ex)
            {
                //Error General
                ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                return ret;
            }
        }

        private bool SendMail(string email, string token)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("luisfernandose@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Cambio de clave";
                mail.Body = "<form action='http://192.168.195.57:4200/reset-password&?token='" + token + "' id='register-form' role='form' autocomplete='off' class='form' method='post'>" +
                           "<div class='form-group'>" +
                           "<input name = 'recover-submit' class='btn btn-lg btn-primary btn-block' value='Reset Password' type='submit'>" +
                           "</div> <input type = 'hidden' class='hide' name='token' id='" + token + "' value='" + token + "'> </form>";

                mail.IsBodyHtml = true;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("luisfernandose@gmail.com", "M4t145J4v13r");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion

        #region Create - ActInactivate - list
        /// <summary>
        /// Metodo para crear usuarios
        /// </summary>
        public Basic CreateUser(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                    if (cp != null)
                    {
                        string login = Convert.ToString(httpRequest.Form["user"]);
                        string password = Convert.ToString(httpRequest.Form["password"]);
                        string email = Convert.ToString(httpRequest.Form["email"]);
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
                        password = autil.Sha(password);

                        Usuario us = ent.Usuario.Where(c => c.Login == login).SingleOrDefault();
                        if (us == null)
                        {
                            us = new Usuario();
                            us.Id_Usuario = Guid.NewGuid();
                            us.Login = login;
                            us.Password = password;
                            us.Email = email;
                            us.Id_Entidad = entidad;
                            us.Estdo = true;
                            us.Fecha_Update = DateTime.Now;
                            us.Usuario_Update = usuario;
                            ent.Usuario.Add(us);
                            ent.SaveChanges();
                            //se genera el codigo del mensaje de retorno exitoso
                            ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
                        }
                        else
                        {
                            //usuario ya existe
                            ret = autil.MensajeRetorno(ref ret, 7, string.Empty, null);
                        }
                        return ret;
                    }
                    else
                    {
                        //token invalido
                        ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
                        return ret;
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                    return ret;
                }
            }
        }
        /// <summary>
        /// metodo para activar o desactivar un usuario
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic ActInactivateUser(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                    if (cp != null)
                    {
                        Guid usuario = Guid.Parse(httpRequest.Form["usuario"]);
                        bool estado = bool.Parse(httpRequest.Form["estado"]);
                        Guid Id_usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Usuario us = ent.Usuario.Where(u => u.Id_Usuario == usuario).SingleOrDefault();
                        us.Estdo = estado;
                        us.Usuario_Update = Id_usuario;
                        us.Fecha_Update = DateTime.Now;
                        ent.SaveChanges();
                        //se genera el codigo del mensaje de retorno exitoso
                        ret = autil.MensajeRetorno(ref ret, 20, string.Empty, null);

                        return ret;
                    }
                    else
                    {
                        //token invalido
                        ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
                        return ret;
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                    return ret;
                }
            }
        }

        /// <summary>
        /// metodo para consultar los usuarios de una entidad
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public List<UsuarioModel> GetUsuarios(HttpRequest httpRequest)
        {
            List<UsuarioModel> ret = new List<UsuarioModel>();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        ret = (ent.Usuario.Where(r => r.Id_Entidad == entidad)
                               .Select(l => new UsuarioModel
                               {
                                   id = l.Id_Usuario,
                                   Login = l.Login,
                                   Email = l.Email,
                                   Estado = l.Estdo,
                                   CambiarClave = l.Cambiar_Clave
                               }).ToList());

                        return ret;
                    }
                }
                else
                    return ret;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Seccion Rol

        /// <summary>
        /// Metodo para tener todos los roles activos
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public List<ComboModel> GetRoles(HttpRequest httpRequest)
        {
            List<ComboModel> ret = new List<ComboModel>();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        ret = ent.Rol.Where(r => r.Estado == true).Select(l => new ComboModel { id = l.Id_Rol, Value = l.Nombre }).ToList();
                        return ret;
                    }
                }
                else
                    return ret;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Metodo para tener todos los roles de un usuario
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public List<ComboModel> GetRolesUsuario(HttpRequest httpRequest)
        {
            List<ComboModel> ret = new List<ComboModel>();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Guid usuario = Guid.Parse(httpRequest.Form["usuario"]);
                        ret = ent.Rol.Where(r => r.Id_Rol == usuario).Select(l => new ComboModel { id = l.Id_Rol, Value = l.Nombre }).ToList();
                        return ret;
                    }
                }
                else
                    return ret;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Metodo para agregar rol a un usuario
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic AgregarRolUsuario(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                    if (cp != null)
                    {
                        Guid usuario = Guid.Parse(httpRequest.Form["usuario"]);
                        Guid rol = Guid.Parse(httpRequest.Form["rol"]);

                        Rol r = ent.Rol.Where(rl => rl.Id_Rol == rol).SingleOrDefault();
                        Usuario us = ent.Usuario.Where(u => u.Id_Usuario == usuario).SingleOrDefault();

                        us.Rol.Add(r);
                        ent.SaveChanges();

                        //se genera el codigo del mensaje de retorno exitoso
                        return ret = autil.MensajeRetorno(ref ret, 20, string.Empty, null);
                    }
                    else
                    {
                        //token invalido
                        ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
                        return ret;
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                    return ret;
                }
            }
        }
        /// <summary>
        /// Metodo para elimiar rol a un usuario
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic EliminaRolUsuario(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                    if (cp != null)
                    {
                        Guid usuario = Guid.Parse(httpRequest.Form["usuario"]);
                        Guid rol = Guid.Parse(httpRequest.Form["rol"]);

                        Rol r = ent.Rol.Where(rl => rl.Id_Rol == rol).SingleOrDefault();
                        Usuario us = ent.Usuario.Where(u => u.Id_Usuario == usuario).SingleOrDefault();

                        us.Rol.Remove(r);
                        ent.SaveChanges();

                        //se genera el codigo del mensaje de retorno exitoso
                        return ret = autil.MensajeRetorno(ref ret, 20, string.Empty, null);
                    }
                    else
                    {
                        //token invalido
                        ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
                        return ret;
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                    return ret;
                }
            }
        }

        #endregion

        #region metodos basicos

        ///CREATE
        //public Basic CreateXXX(HttpRequest httpRequest)
        //{
        //    Basic ret = new Basic();
        //    using (MilenioCloudEntities ent = new MilenioCloudEntities())
        //    {
        //        try
        //        {
        //            cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
        //            if (cp != null)
        //            {

        //                //se genera el codigo del mensaje de retorno exitoso
        //                return ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
        //            }
        //            else
        //            {
        //                //token invalido
        //                ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
        //                return ret;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            //error general
        //            ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
        //            return ret;
        //        }
        //    }
        //}

        #endregion
    }
}