using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
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
        //public LoginModel Login(HttpRequest model)
        //{
        //    using (MilenioCloudEntities ent = new MilenioCloudEntities())
        //    {
        //        LoginModel op = new LoginModel();
        //        try
        //        {
        //            string login = Convert.ToString(model.Form["user"]);
        //            string pass = Convert.ToString(model.Form["password"]);
        //            pass = autil.Sha(pass);

        //            Usuario p = ent.Usuario.Where(pr => pr.Login == login && pr.Password == pass).SingleOrDefault();

        //            if (p != null)
        //            {
        //                //validamos si tiene una session abierta
        //                if (!p.isloged)
        //                {
        //                    op.Id_User = p.Id_Usuario;
        //                    op.user = p.Login;
        //                    op.Email = p.Email;

        //                    List<ComboModel> entidades = (p.Entidad_Usuario.Where(c => c.estado == true)
        //                                            .Select(t => new ComboModel
        //                                            {
        //                                                id = t.Id_Entidad,
        //                                                value = t.Entidad.Nombre
        //                                            }).ToList());

        //                    op.Entidades = entidades.GroupBy(rl => rl.id).Select(g => g.First()).ToList();
        //                    op.token = JwtManager.GenerateToken(p.Login, p.Id_Usuario.ToString(), null, null);
        //                }
        //                else
        //                {
        //                    //si entra aqui es porque ya tiene una session abierta
        //                    Basic b = new Basic();
        //                    autil.MensajeRetorno(ref b, 31, string.Empty, null);

        //                    op.response_code = b.response_code;
        //                    op.custom = b.custom;
        //                    op.message = b.message;
        //                    return op;
        //                }
        //            }
        //            else
        //            {
        //                //login invalido
        //                Basic b = new Basic();
        //                autil.MensajeRetorno(ref b, 8, string.Empty, null);

        //                op.response_code = b.response_code;
        //                op.custom = b.custom;
        //                op.message = b.message;
        //                return op;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            //error general
        //            Basic b = new Basic();
        //            autil.MensajeRetorno(ref b, 4, ex.message + model.Form, null);

        //            op.response_code = b.response_code;
        //            op.custom = b.custom;
        //            op.message = b.message;

        //            return op;
        //        }

        //        return op;
        //    }
        //}

        public object Login(LoginModel model)
        {
            ///cambios del dia 23/03/2019
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                LoginModel lm = new LoginModel();
                Response rp = new Response();
                try
                {
                    List<ErrorFields> rel = autil.ValidateObject(model);
                    if (rel.Count == 0)
                    {
                        string pass = autil.Sha(model.Password);
                        var um = ent.Usuario.Where(pr => pr.Login == model.User && pr.Password == pass).ToList();
                        if (um.Count > 0)
                        {
                            foreach (var p in um)
                            {
                                //validamos si tiene una session abierta
                                if (!p.isloged)
                                {
                                    List<ComboModel> entidades = (p.Entidad_Usuario.Where(c => c.Estado == true)
                                                            .Select(t => new ComboModel
                                                            {
                                                                id = t.Id_Entidad,
                                                                value = t.Entidad.Nombre
                                                            }).ToList());

                                    entidades = entidades.GroupBy(rl => rl.id).Select(g => g.First()).ToList();
                                    //
                                    if (entidades.Count > 1)
                                    {
                                        var data = um.Select(t => new
                                        {
                                            id_usuario = t.Id_Usuario,
                                            login = t.Login,
                                            entidades
                                        }).SingleOrDefault();

                                        rp.data.Add(data);
                                    }
                                    else
                                    {
                                        model.Id_Entidad = entidades[0].id;
                                        return rp = (Response)LoginEntidad(model);
                                    }
                                }
                                else
                                {
                                    //si entra aqui es porque ya tiene una session abierta
                                    return autil.MensajeRetorno(ref rp, 31, string.Empty, null, HttpStatusCode.OK);
                                }
                            }
                        }
                        else
                        {
                            //login invalido                            
                            return autil.MensajeRetorno(ref rp, 8, model.User + "-" + model.Password, null, HttpStatusCode.OK);
                        }
                    }
                    else
                    {
                        //fallo campos requeridos
                        return autil.MensajeRetorno(ref rp, 8, string.Empty, null, rel, HttpStatusCode.OK);
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    return autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                }

                //retorna un response, con el campo data lleno con la respuesta.               
                return autil.MensajeRetorno(ref rp, 9, null, null, HttpStatusCode.OK);
            }
        }

        public object LoginEntidad(LoginModel model)
        {
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                Response rp = new Response();
                try
                {
                    List<ErrorFields> rel = autil.ValidateObject(model);
                    if (rel.Count == 0)
                    {
                        string pass = autil.Sha(model.Password);

                        var um = ent.Usuario.Where(pr => pr.Login == model.User && pr.Password == pass).ToList();

                        if (um.Count > 0)
                        {
                            foreach (var p in um)
                            {
                                //consultamos los roles disponibles
                                List<ComboModel> roles = ent.Rol_Usuario.Where(r => r.Id_Usuario == p.Id_Usuario && r.Id_Entidad == model.Id_Entidad && r.Estado == true).Select(t => new ComboModel
                                {
                                    id = t.Rol.Id_Rol,
                                    value = t.Rol.Nombre
                                }).ToList();

                                if (roles.Count() != 0)
                                {
                                    List<ComboModel> entidades = (p.Entidad_Usuario.Where(c => c.Estado == true && c.Entidad.Id_Entidad == model.Id_Entidad)
                                                            .Select(t => new ComboModel
                                                            {
                                                                id = t.Id_Entidad,
                                                                value = t.Entidad.Nombre
                                                            }).ToList());
                                    entidades = entidades.GroupBy(rl => rl.id).Select(g => g.First()).ToList();

                                    roles = roles.GroupBy(rl => rl.id).Select(g => g.First()).ToList();

                                    string token = JwtManager.GenerateToken(p.Login, p.Id_Usuario.ToString(), roles, model.Id_Entidad);

                                    //aqui seteamos que el usuario ya esta logueado
                                    p.isloged = true;
                                    ent.SaveChanges();

                                    var r = (from t in um
                                             select new
                                             {
                                                 id_usuario = p.Id_Usuario,
                                                 login = p.Login,
                                                 entidades,
                                                 roles,
                                                 token
                                             }).SingleOrDefault();

                                    rp.data.Add(r);
                                }
                                else
                                {
                                    //USUARIO SIN ROLES
                                    return autil.MensajeRetorno(ref rp, 13, string.Empty, null);
                                }

                            }
                        }
                        else
                        {
                            //login invalido                
                            return autil.MensajeRetorno(ref rp, 8, string.Empty, null); ;
                        }
                    }
                    else
                    {
                        //fallo campos requeridos
                        return autil.MensajeRetorno(ref rp, 8, string.Empty, null, rel);
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    return autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                }

                //retorna un response, con el campo data lleno con la respuesta.               
                return autil.MensajeRetorno(ref rp, 9, null, null, HttpStatusCode.OK);
            }
        }

        public object LogOff(LoginModel model)
        {
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                Response rp = new Response();
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(model.token));
                    if (cp != null)
                    {
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
                        Usuario us = ent.Usuario.Where(u => u.Id_Usuario == usuario).SingleOrDefault();

                        if (us != null)
                        {
                            us.isloged = false;
                            ent.SaveChanges();
                            return autil.MensajeRetorno(ref rp, 30, string.Empty, null, HttpStatusCode.OK);
                        }
                    }
                    else
                    {
                        return autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    return autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                }

                return rp;
            }
        }


        /// <summary>
        /// Metodo que envia correo con opcion para cambio de clave
        /// </summary>
        /// <returns></returns>
        public object ForgotPassword(LoginModel model)
        {
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Usuario p = ent.Usuario.Where(u => u.Login == model.User).SingleOrDefault();
                    if (p != null)
                    {
                        //metodo para enviar reseteo de contraseña                        
                        if (!string.IsNullOrEmpty(p.Email))
                        {
                            string token = JwtManager.GenerateToken(p.Login, p.Id_Usuario.ToString(), null, null);
                            string nombre = p.Nombres + " " + p.Primer_Apellido + " " + p.Segundo_Apellido;
                            string user = p.Login;

                            SendMail(p.Email, token, nombre, user);

                            ent.SaveChanges();
                            rp = autil.MensajeRetorno(ref rp, 21, string.Empty, null, HttpStatusCode.OK);
                        }
                        else
                        {
                            //mensuaje no tiene email
                            rp = autil.MensajeRetorno(ref rp, 32, string.Empty, null, HttpStatusCode.OK);
                        }
                    }
                    else
                    {
                        //mensaje login no existe
                        rp = autil.MensajeRetorno(ref rp, 15, string.Empty, null, HttpStatusCode.OK);
                    }
                }
                return rp;
            }
            catch (Exception ex)
            {
                //error general
                rp = autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                return rp;
            }
        }

        /// <summary>
        /// metodo para cambiar le la clave a un usuario
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object ChangePassword(LoginModel model)
        {
            Response ret = new Response();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(model.token));
                if (cp != null)
                {
                    Guid? user_id = null;
                    string usid = cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(usid))
                        user_id = Guid.Parse(usid);

                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Usuario p = ent.Usuario.Where(u => u.Id_Usuario == user_id).SingleOrDefault();

                        if (p != null)
                        {
                            p.Password = autil.Sha(model.Password);
                            ent.SaveChanges();
                            ret = autil.MensajeRetorno(ref ret, 10, string.Empty, null);
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

        public void SendMail(string email, string token, string nombre, string login)
        {
            try
            {
                var userCredentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SMTPUserName"].ToString(), ConfigurationManager.AppSettings["SMTPPassword"]);

                if (System.Convert.ToBoolean(ConfigurationManager.AppSettings["EmailAlertEnabled"]))
                {
                    SmtpClient smtp = new SmtpClient
                    {
                        Host = Convert.ToString(ConfigurationManager.AppSettings["SMTPHost"]),

                        Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]),

                        EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPEnableSsl"]),

                        DeliveryMethod = SmtpDeliveryMethod.Network,

                        Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPTimeout"])
                    };

                    smtp.Credentials = userCredentials;

                    MailMessage message = new MailMessage();

                    message.From = new MailAddress(ConfigurationManager.AppSettings["SenderEmailAddress"], ConfigurationManager.AppSettings["SenderDisplayName"]);

                    message.Subject = ConfigurationManager.AppSettings["EmailSubjec"];

                    message.Body = this.SetEmailBody(login, token);

                    //  message.AlternateViews.Add(body);

                    message.IsBodyHtml = true;

                    // System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(path);

                    //message.Attachments.Add(attachment);
                    message.To.Add(email);

                    smtp.Send(message);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string SetEmailBody(string subject, string token)
        {
            try
            {
                string htmlPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["EmailTemplate"]);
                string url = ConfigurationManager.AppSettings["EmailResetUrl"];

                url = url + "?token=" + token;
                var html = System.IO.File.ReadAllText(htmlPath);
                html = html.Replace("{{name}}", subject);
                html = html.Replace("{{action_url}}", url);

                StringBuilder strbuilder = new StringBuilder();

                strbuilder.Append(html);

                return strbuilder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region USER - Create - ActInactivate - lists

        public object CreateUser(UsuarioModel model)
        {
            string pasos = "0";
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(model.token));
                    if (cp != null)
                    {
                        List<ErrorFields> rel = autil.ValidateObject(model);
                        if (rel.Count == 0)
                        {
                            //AQUI SE TOMA EL OBJETO ENVIADO DESDE EL FRONT
                            //Y SE COPIA AL OBJETO USER
                            Usuario us = new Usuario();
                            Copier.CopyPropertiesTo(model, us);
                            //

                            Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                            Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                            //consulta login
                            if (ent.Usuario.Where(u => u.Login == us.Login).Count() == 0)
                            {
                                //consulta por cedula
                                if (ent.Usuario.Where(u => u.Numero_Identificacion == us.Numero_Identificacion).Count() == 0)
                                {
                                    //consulta por email
                                    if (ent.Usuario.Where(u => u.Email == us.Email).Count() == 0)
                                    {
                                        //consulta por registro profesional
                                        int regxist = 0;

                                        if (regxist == 0)
                                        {
                                            Guid id_usuario = Guid.NewGuid();
                                            us.Id_Usuario = id_usuario;

                                            ///agregado el usuario a la entidad donde se esta creando
                                            Entidad_Usuario eu = new Entidad_Usuario();
                                            eu.Id_Entidad = entidad;
                                            eu.Id_Usuario = id_usuario;
                                            eu.Estado = true;
                                            eu.Fecha_Create = DateTime.Now;
                                            eu.Fecha_Update = DateTime.Now;
                                            eu.Usuario_Create = usuario;
                                            eu.Usuario_Update = usuario;
                                            ent.Entidad_Usuario.Add(eu);
                                            /////

                                            us.Fecha_Create = DateTime.Now;
                                            us.Usuario_Create = usuario;
                                            us.Fecha_Update = DateTime.Now;
                                            us.Usuario_Update = usuario;
                                            ent.Usuario.Add(us);

                                            ///seccion para obtener los roles que le estan agregando al usuario
                                            string roles;
                                            if (!string.IsNullOrEmpty(model.List_Roles))
                                            {
                                                roles = Convert.ToString(model.List_Roles);
                                                string[] rolesArray = roles.Split(',');
                                                List<Rol_Usuario> lru = new List<Rol_Usuario>();
                                                foreach (var r in rolesArray)
                                                {
                                                    //agregando la lista de usuarios
                                                    Rol_Usuario ru = new Rol_Usuario();
                                                    ru.Id_Entidad = entidad;
                                                    ru.Id_Usuario = id_usuario;
                                                    ru.Id_Rol = Guid.Parse(r);
                                                    lru.Add(ru);
                                                }

                                                if (lru.Count > 0)
                                                {
                                                    //si hay roles que agregar, los agrega
                                                    ent.Rol_Usuario.AddRange(lru);
                                                }
                                            }

                                            ///salvando todos los cambios
                                            ent.SaveChanges();
                                            //se genera el codigo del mensaje de retorno exitoso
                                            rp = autil.MensajeRetorno(ref rp, 2, string.Empty, null, HttpStatusCode.OK);
                                        }
                                        else
                                        {
                                            ///Registro profesional existe
                                            return rp = autil.MensajeRetorno(ref rp, 3, string.Empty, null, HttpStatusCode.OK);
                                        }
                                    }
                                    else
                                    {
                                        ///Email existe
                                        return rp = autil.MensajeRetorno(ref rp, 6, string.Empty, null, HttpStatusCode.OK);
                                    }
                                }
                                else
                                {
                                    ///Cedula existe
                                    return rp = autil.MensajeRetorno(ref rp, 5, string.Empty, null, HttpStatusCode.OK);
                                }
                            }
                            else
                            {
                                //usuario ya existe
                                rp = autil.MensajeRetorno(ref rp, 7, string.Empty, null, HttpStatusCode.OK);
                            }
                            return rp;
                        }
                        else
                        {
                            //fallo campos requeridos
                            return autil.MensajeRetorno(ref rp, 33, string.Empty, null, rel, HttpStatusCode.OK);
                        }
                    }
                    else
                    {
                        //token invalido
                        rp = autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                        return rp;
                    }

                }
                catch (Exception ex)
                {
                    //error general
                    rp = autil.MensajeRetorno(ref rp, 4, ex.Message + " " + ex.InnerException + "/" + pasos, null, HttpStatusCode.InternalServerError);
                    return rp;
                }
            }
        }

        public object EditUser(UsuarioModel model)
        {
            Response rp = new Response();
            string pasos = "0";
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(model.token));
                    if (cp != null)
                    {
                        Response b = new Response();
                        List<ErrorFields> rel = autil.ValidateObject(model);
                        if (rel.Count == 0)
                        {
                            Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                            Guid? usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());


                            int lexist = ent.Usuario.Where(t => t.Login == model.Login && t.Id_Usuario != model.Id_Usuario).Count();

                            if (lexist == 0)
                            {
                                int emexist = ent.Usuario.Where(t => t.Email == model.Email && t.Id_Usuario != model.Id_Usuario).Count();

                                if (emexist == 0)
                                {
                                    int regxist = 0;
                                    if (!string.IsNullOrEmpty(model.Registro_Profesional))
                                        regxist = ent.Usuario.Where(r => r.Registro_Profesional == model.Registro_Profesional && r.Id_Usuario != model.Id_Usuario).Count();

                                    if (regxist == 0)
                                    {

                                        Guid id_usuario = Guid.NewGuid();

                                        Usuario us = ent.Usuario.Where(u => u.Id_Usuario == model.Id_Usuario).SingleOrDefault();

                                        us.Nombres = model.Nombres;
                                        us.Primer_Apellido = model.Primer_Apellido;
                                        us.Segundo_Apellido = model.Segundo_Apellido;
                                        us.Poblado_Id = model.Poblado_Id;
                                        us.Sexo = model.Sexo;
                                        us.Fecha_Nacimiento = model.Fecha_Nacimiento;
                                        us.Foto = model.Foto;
                                        us.Estado_Civil = model.Estado_Civil;
                                        us.Tipo_Sangre = model.Tipo_Sangre;
                                        us.Fecha_Contratacion = model.Fecha_Contratacion.Value;
                                        us.Tipo_Vinculacion = model.Tipo_Vinculacion;
                                        us.Observaciones = model.Observaciones;

                                        us.Acepta_ABEAS = model.Acepta_ABEAS;
                                        us.Foto_ABEAS = model.Foto_ABEAS;
                                        us.Presta_Servicio = model.Presta_Servicio;
                                        us.Id_Tipo_Profesional = model.Id_Tipo_Profesional;
                                        us.Registro_Profesional = model.Registro_Profesional;

                                        us.Login = model.Login;
                                        us.Email = model.Email;

                                        us.Fecha_Create = DateTime.Now;
                                        us.Usuario_Create = usuario;
                                        us.Fecha_Update = DateTime.Now;
                                        us.Usuario_Update = usuario;

                                        ent.SaveChanges();
                                        //se genera el codigo del mensaje de retorno exitoso
                                        rp = autil.MensajeRetorno(ref rp, 20, string.Empty, null, HttpStatusCode.OK);
                                    }
                                    else
                                    {
                                        ///Registro profesional existe
                                        return rp = autil.MensajeRetorno(ref rp, 3, string.Empty, null, HttpStatusCode.OK);
                                    }
                                }
                                else
                                {
                                    ///Email existe
                                    return rp = autil.MensajeRetorno(ref rp, 6, string.Empty, null, HttpStatusCode.OK);
                                }
                            }
                            else
                            {
                                //usuario ya existe
                                rp = autil.MensajeRetorno(ref rp, 7, string.Empty, null, HttpStatusCode.OK);
                            }
                            return rp;
                        }
                        else
                        {
                            //fallo campos requeridos
                            return autil.MensajeRetorno(ref b, 33, string.Empty, null, rel, HttpStatusCode.OK);
                        }
                    }
                    else
                    {
                        //token invalido
                        rp = autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                        return rp;
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    rp = autil.MensajeRetorno(ref rp, 4, ex.Message + " " + ex.InnerException + "/" + pasos, null, HttpStatusCode.InternalServerError);
                    return rp;
                }
            }
        }

        public object ValidateUser(UsuarioModel model)
        {
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(model.token));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Entidad_Usuario us = ent.Entidad_Usuario.Where(t => t.Usuario.Numero_Identificacion == model.Numero_Identificacion).SingleOrDefault();

                        if (us != null)
                        {
                            //La cedula si existe
                            if (us.Id_Entidad == entidad)
                                ///Cedula existe en esta entidad
                                rp = autil.MensajeRetorno(ref rp, 22, string.Empty, null, HttpStatusCode.OK);
                            else
                                ///Cedula existe en otra entidad
                                rp = autil.MensajeRetorno(ref rp, 23, string.Empty, null, HttpStatusCode.OK);
                        }
                        else
                        {
                            ///Cedula NO existe                            
                            rp = autil.MensajeRetorno(ref rp, 24, string.Empty, null, HttpStatusCode.OK);
                        }
                        return rp;
                    }
                    else
                    {
                        //token invalido
                        rp = autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                        return rp;
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    rp = autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                    return rp;
                }
            }
        }

        public object CreateEntidadUser(UsuarioModel model)
        {
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(model.token));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Usuario us = ent.Usuario.Where(u => u.Numero_Identificacion == model.Numero_Identificacion).SingleOrDefault();
                        if (us != null)
                        {
                            Entidad_Usuario eu = new Entidad_Usuario();
                            eu.Id_Entidad = entidad;
                            eu.Id_Usuario = us.Id_Usuario;
                            eu.Fecha_Create = DateTime.Now;
                            eu.Fecha_Update = DateTime.Now;
                            eu.Estado = true;
                            eu.Usuario_Create = usuario;
                            eu.Usuario_Update = usuario;

                            ent.Entidad_Usuario.Add(eu);
                            ent.SaveChanges();

                            //Exitoso
                            rp = autil.MensajeRetorno(ref rp, 2, string.Empty, null, HttpStatusCode.OK);
                        }
                        return rp;
                    }
                    else
                    {
                        //token invalido
                        rp = autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                        return rp;
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    rp = autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                    return rp;
                }
            }
        }

        public object GetUsuariosEdit(UsuarioModel model)
        {
            Response rp = new Response();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(model.token));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());

                        var vu = ent.Entidad_Usuario.Where(t => t.Id_Entidad == entidad && t.Id_Usuario == model.Id_Usuario)
                                           .Select(u => u.Usuario).ToList();

                        if (vu.Count > 0)
                        {
                            foreach (var us in vu)
                            {
                                List<ComboModel> cmr = ent.Rol_Usuario.Where(ru => ru.Id_Entidad == entidad && ru.Id_Usuario == model.Id_Usuario)
                                                       .Select(u => new ComboModel
                                                       {
                                                           id = u.Id_Rol,
                                                           value = u.Rol.Nombre
                                                       }).ToList();

                                var r = (from t in vu
                                         select new
                                         {
                                             t.Id_Tipo_Identificacion,
                                             t.Numero_Identificacion,
                                             t.Nombres,
                                             t.Primer_Apellido,
                                             t.Segundo_Apellido,
                                             t.Sexo,
                                             t.Fecha_Nacimiento,
                                             t.Foto,
                                             t.Estado_Civil,
                                             t.Tipo_Sangre,
                                             t.Poblado_Id,
                                             t.Direccion,
                                             t.Telefono,
                                             t.Fecha_Contratacion,
                                             t.Observaciones,
                                             t.Tipo_Vinculacion,
                                             t.Presta_Servicio,
                                             t.Email,
                                             t.Acepta_ABEAS,
                                             t.Foto_ABEAS,
                                             t.Id_Tipo_Profesional,
                                             t.Registro_Profesional,
                                             Rol = cmr,
                                             t.Poblado.Municipio_Id,
                                             Departamento_Id = us.Poblado.Municipio.Departamento.Dane_Id,
                                             Estado = us.Entidad_Usuario.Where(tt => tt.Id_Usuario == us.Id_Usuario && tt.Id_Entidad == entidad).Select(tt => tt.Estado).SingleOrDefault()
                                         }).SingleOrDefault();

                                rp.data.Add(r);
                            }
                        }

                        return autil.MensajeRetorno(ref rp, 9, string.Empty, null, HttpStatusCode.OK);
                    }
                }
                else
                {
                    //TOKEN INVALIDO
                    return autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                }

            }
            catch (Exception ex)
            {
                //error general
                return autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
            }
        }

        public object GetUsuarios(UsuarioModel model)
        {
            Response rp = new Response();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(model.token));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());

                        IQueryable<Usuario> us = ent.Entidad_Usuario.Where(t => t.Id_Entidad == entidad).Select(u => u.Usuario);
                        //consulta por id usuario
                        if (model.Id_Usuario != Guid.Empty)
                        {
                            us = us.Where(c => c.Id_Usuario == model.Id_Usuario);
                        }

                        //consulta por cedula
                        if (!string.IsNullOrEmpty(model.Numero_Identificacion))
                        {
                            us = us.Where(c => c.Numero_Identificacion.Contains(model.Numero_Identificacion));
                        }
                        ///consulta por nombre
                        if (!string.IsNullOrEmpty(model.Nombres))
                        {
                            us = us.Where(c => c.Nombres.Contains(model.Nombres));
                        }

                        ///consulta por primer apellido
                        if (!string.IsNullOrEmpty(model.Primer_Apellido))
                        {
                            us = us.Where(c => c.Primer_Apellido.Contains(model.Primer_Apellido));
                        }

                        ///consulta por segundo apellido
                        if (!string.IsNullOrEmpty(model.Segundo_Apellido))
                        {
                            us = us.Where(c => c.Segundo_Apellido.Contains(model.Segundo_Apellido));
                        }

                        ///consulta por login
                        if (!string.IsNullOrEmpty(model.Login))
                        {
                            us = us.Where(c => c.Login.Contains(model.Login));
                        }

                        ///consulta por email
                        if (!string.IsNullOrEmpty(model.Email))
                        {
                            us = us.Where(c => c.Email.Contains(model.Email));
                        }

                        ///consulta por fecha contratacion
                        if (!string.IsNullOrEmpty(model.Fecha_Contratacion.ToString()))
                        {
                            us = us.Where(c => c.Fecha_Contratacion == model.Fecha_Contratacion);
                        }

                        //int pageSize = Convert.ToInt32(model.Form["pageSize"]);
                        //int startingPageIndex = Convert.ToInt32(model.Form["startingPageIndex"]);
                        var rl = us.Select(u => new
                        {
                            u.Id_Usuario,
                            u.Nombres,
                            u.Primer_Apellido,
                            u.Segundo_Apellido,
                            u.Login,
                            u.Fecha_Contratacion,
                            u.Email
                        }).ToList();


                        //.OrderBy(o=> o.Nombres).Skip(startingPageIndex * pageSize).Take(pageSize).ToList();

                        rp.cantidad = rl.Count();
                        rp.pagina = 0;
                        rp.data.AddRange(rl);

                        //retorna un response, con el campo data lleno con la respuesta.               
                        return autil.MensajeRetorno(ref rp, 9, null, null, HttpStatusCode.OK);

                    }
                }
                else
                {
                    //TOKEN INVALIDO                   
                    return autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                }

            }
            catch (Exception ex)
            {
                //error general
                return autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
            }
        }

        public object UserProfile(UsuarioModel model)
        {
            Response rp = new Response();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(model.token));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        var vu = ent.Usuario.Where(t => t.Id_Usuario == usuario).ToList();

                        if (vu.Count > 0)
                        {
                            foreach (var us in vu)
                            {
                                var r = (from t in vu
                                         select new
                                         {
                                             t.Id_Tipo_Identificacion,
                                             t.Numero_Identificacion,
                                             t.Nombres,
                                             t.Primer_Apellido,
                                             t.Segundo_Apellido,
                                             t.Sexo,
                                             t.Fecha_Nacimiento,
                                             t.Foto,
                                             t.Estado_Civil,
                                             t.Tipo_Sangre,
                                             t.Poblado_Id,
                                             t.Direccion,
                                             t.Telefono,
                                             t.Fecha_Contratacion,
                                             t.Observaciones,
                                             t.Tipo_Vinculacion,
                                             t.Presta_Servicio,
                                             t.Email,
                                             t.Acepta_ABEAS,
                                             t.Foto_ABEAS,
                                             t.Id_Tipo_Profesional,
                                             t.Registro_Profesional,
                                             t.Poblado.Municipio_Id,
                                             Departamento_Id = us.Poblado.Municipio.Departamento.Dane_Id,
                                             Estado = us.Entidad_Usuario.Where(tt => tt.Id_Usuario == us.Id_Usuario && tt.Id_Entidad == entidad).Select(tt => tt.Estado).SingleOrDefault()
                                         }).SingleOrDefault();

                                rp.data.Add(r);
                            }
                        }

                        return autil.MensajeRetorno(ref rp, 9, string.Empty, null, HttpStatusCode.OK);
                    }
                }
                else
                {
                    //TOKEN INVALIDO
                    return autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                //error general
                return autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
            }
        }

        public object EditProfile(UsuarioModel model)
        {
            Response rp = new Response();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(model.token));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Usuario vu = ent.Usuario.Where(t => t.Id_Usuario == usuario).SingleOrDefault();

                        vu.Telefono = model.Telefono;
                        vu.Foto = model.Foto;
                        vu.Poblado_Id = model.Poblado_Id;
                        vu.Direccion = model.Direccion;
                        vu.Email = model.Email;
                        vu.Estado_Civil = model.Estado_Civil;

                        ent.SaveChanges();

                        return autil.MensajeRetorno(ref rp, 20, string.Empty, null, HttpStatusCode.OK);
                    }
                }
                else
                {
                    //TOKEN INVALIDO
                    return autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                //error general
                return autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region Profesional

        /// <summary>
        /// Metodo para llenar combo de tipo de profesional
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public List<ComboModel> GetTipoProfesional(HttpRequest httpRequest)
        {
            List<ComboModel> ret = new List<ComboModel>();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        ret = ent.Tipo_Profesional.Select(l => new ComboModel { id = l.Id_Tipo_Profesional, value = l.Nombre }).ToList();
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

        public Basic CreateEspecialidadProfesional(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
                        Guid idusuario = Guid.Parse(httpRequest.Form["idusuario"]);
                        Guid idespecialidad = Guid.Parse(httpRequest.Form["idespecialidad"]);

                        Especialidad_Profesional ep = ent.Especialidad_Profesional
                                                      .Where(e => e.Id_Entidad == entidad
                                                      && e.Id_Especialidad == idespecialidad
                                                      && e.Id_Usuario == idusuario).SingleOrDefault();

                        if (ep == null)
                        {
                            ep = new Especialidad_Profesional();
                            ep.Id_Entidad = entidad;
                            ep.Id_Usuario = idusuario;
                            ep.Id_Especialidad = idespecialidad;
                            ep.Estado = true;
                            ep.Fecha_Create = DateTime.Now;
                            ep.Fecha_Update = DateTime.Now;
                            ep.Usuario_Create = usuario;
                            ep.Usuario_Update = usuario;
                            ent.Especialidad_Profesional.Add(ep);
                            ent.SaveChanges();

                            //se genera el codigo del mensaje de retorno exitoso
                            return ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
                        }
                        else
                        {
                            //se especialidad ya agregada
                            return ret = autil.MensajeRetorno(ref ret, 27, string.Empty, null);
                        }
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

        public Basic EditEspecialidadProfesional(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
                        Guid idusuario = Guid.Parse(httpRequest.Form["idusuario"]);
                        Guid idespecialidad = Guid.Parse(httpRequest.Form["idespecialidad"]);
                        bool estado = Convert.ToBoolean(int.Parse(httpRequest.Form["estado"]));

                        Especialidad_Profesional ep = ent.Especialidad_Profesional
                                                      .Where(e => e.Id_Entidad == entidad
                                                      && e.Id_Especialidad == idespecialidad
                                                      && e.Id_Usuario == idusuario).SingleOrDefault();

                        if (ep != null)
                        {
                            ep.Id_Entidad = entidad;
                            ep.Id_Usuario = idusuario;
                            ep.Id_Especialidad = idespecialidad;
                            ep.Estado = estado;
                            ep.Fecha_Update = DateTime.Now;
                            ep.Usuario_Update = usuario;
                            ent.SaveChanges();

                            //se genera el codigo del mensaje de retorno exitoso
                            return ret = autil.MensajeRetorno(ref ret, 20, string.Empty, null);
                        }
                        else
                        {
                            //se especialidad no agregada
                            return ret = autil.MensajeRetorno(ref ret, 28, string.Empty, null);
                        }
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

        #region Seccion Rol

        /// <summary>
        /// Metodo para tener todos los roles activos
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object GetRoles(UsuarioModel model)
        {
            List<ComboModel> rl = new List<ComboModel>();
            Response rp = new Response();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(model.token));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        rl = ent.Rol.Where(r => r.Estado == true).Select(l => new ComboModel { id = l.Id_Rol, value = l.Nombre }).ToList();
                        rp.data.Add(rl);

                    }
                }
                else
                {
                    //TOKEN INVALIDO                   
                    return autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                }

                //retorna un response, con el campo data lleno con la respuesta.               
                return autil.MensajeRetorno(ref rp, 9, null, null, HttpStatusCode.OK);
            }

            catch (Exception ex)
            {
                //error general               
                return autil.MensajeRetorno(ref rp, 4, ex.Message + " " + ex.InnerException, null, HttpStatusCode.InternalServerError);
            }
        }
        /// <summary>
        /// Metodo para tener todos los roles de un usuario
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object GetRolesUsuario(UsuarioModel model)
        {
            List<ComboModel> rl = new List<ComboModel>();
            Response rp = new Response();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(model.token));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        rl = ent.Rol_Usuario.Where(u => u.Id_Entidad == entidad && u.Id_Usuario == model.Id_Usuario && u.Estado == true).Select(l => new ComboModel { id = l.Id_Rol, value = l.Rol.Nombre }).ToList();
                        rp.data.Add(rl);
                    }
                }
                else
                {
                    //TOKEN INVALIDO                   
                    return autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                }

                //retorna un response, con el campo data lleno con la respuesta.
                return autil.MensajeRetorno(ref rp, 9, null, null, HttpStatusCode.OK);
            }

            catch (Exception ex)
            {
                //error general               
                return autil.MensajeRetorno(ref rp, 4, ex.Message + " " + ex.InnerException, null, HttpStatusCode.InternalServerError);
            }
        }

        public object GetNotRolesUsuario(UsuarioModel model)
        {
            List<ComboModel> rl = new List<ComboModel>();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(model.token));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        List<Rol> rol = ent.Rol_Usuario.Where(u => u.Id_Entidad == entidad && u.Id_Usuario == model.Id_Usuario && u.Estado == true).Select(t => t.Rol).ToList();
                        List<Rol> _rol = ent.Rol.ToList();

                        rl = _rol.Except(rol).Select(l => new ComboModel { id = l.Id_Rol, value = l.Nombre }).ToList();

                        return rl;
                    }
                }
                else
                    return rl;
            }

            catch (Exception ex)
            {
                //error general
                Response rp = new Response();
                return autil.MensajeRetorno(ref rp, 4, ex.Message + " " + ex.InnerException, null, HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Metodo para agregar rol a un usuario
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object CreateRolUsuario(UsuarioModel model)
        {
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(model.token));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Rol_Usuario ru = ent.Rol_Usuario.Where(r => r.Id_Rol == model.Id_Rol && r.Id_Usuario == model.Id_Usuario && r.Id_Entidad == entidad).SingleOrDefault();
                        if (ru == null)
                        {
                            ru = new Rol_Usuario();
                            ru.Id_Entidad = entidad;
                            ru.Id_Usuario = model.Id_Usuario;
                            ru.Id_Rol = model.Id_Rol;
                            ru.Estado = true;
                            ru.Fecha_Create = DateTime.Now;
                            ru.Usuario_Create = usuario;
                            ru.Fecha_Update = DateTime.Now;
                            ru.Usuario_Update = usuario;
                            ent.Rol_Usuario.Add(ru);
                        }
                        else
                        {
                            ru.Estado = true;
                            ru.Fecha_Create = DateTime.Now;
                            ru.Usuario_Create = usuario;
                            ru.Fecha_Update = DateTime.Now;
                            ru.Usuario_Update = usuario;
                        }

                        ent.SaveChanges();
                        //se genera el codigo del mensaje de retorno exitoso
                        return rp = autil.MensajeRetorno(ref rp, 2, string.Empty, null, HttpStatusCode.OK);
                    }
                    else
                    {
                        //token invalido
                        rp = autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                        return rp;
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    rp = autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.OK);
                    return rp;
                }
            }
        }
        /// <summary>
        /// Metodo para elimiar rol a un usuario
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object EditRolUsuario(UsuarioModel model)
        {
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(model.token));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Rol_Usuario ru = ent.Rol_Usuario.Where(r => r.Id_Rol == model.Id_Rol && r.Id_Usuario == model.Id_Usuario && r.Id_Entidad == entidad).SingleOrDefault();
                        if (ru != null)
                        {
                            ru.Estado = bool.Parse(model.Estado.ToString());
                            ru.Usuario_Update = usuario;
                            ru.Fecha_Update = DateTime.Now;
                            ent.SaveChanges();
                        }
                        //se genera el codigo del mensaje de retorno exitoso
                        return autil.MensajeRetorno(ref rp, 20, string.Empty, null, HttpStatusCode.OK);
                    }
                    else
                    {
                        //token invalido
                        rp = autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                        return rp;
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    rp = autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                    return rp;
                }
            }
        }

        #endregion

        #region metodos basicos

        ///CREATE
        //public Basic CreateXXX(HttpRequest model)
        //{
        //    Basic ret = new Basic();
        //    using (MilenioCloudEntities ent = new MilenioCloudEntities())
        //    {
        //        try
        //        {
        //            cp = tk.ValidateToken(Convert.ToString(model.Form["token"]));
        //            if (cp != null)
        //            {
        //Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.value).SingleOrDefault());
        //Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.value).SingleOrDefault());
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
        //            ret = autil.MensajeRetorno(ref ret, 4, ex.message, null);
        //            return ret;
        //        }
        //    }
        //}

        #endregion
    }
}