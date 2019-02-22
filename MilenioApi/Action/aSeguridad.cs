using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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
                        op.Id_User = p.Id_Usuario;
                        op.Login = p.Login;
                        op.Email = p.Email;

                        List<ComboModel> entidades = (p.Entidad_Usuario.Where(c => c.Estado == true)
                                                .Select(t => new ComboModel
                                                {
                                                    Id = t.Id_Entidad,
                                                    Value = t.Entidad.Nombre
                                                }).ToList());

                        op.Entidades = entidades.GroupBy(rl => rl.Id).Select(g => g.First()).ToList();
                        op.token = JwtManager.GenerateToken(p.Login, p.Id_Usuario.ToString(), null, null);

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

        public LoginModel LoginEntidad(HttpRequest httpRequest)
        {
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                LoginModel op = new LoginModel();
                try
                {
                    string login = Convert.ToString(httpRequest.Form["user"]);
                    string pass = Convert.ToString(httpRequest.Form["password"]);
                    Guid entidad = Guid.Parse(httpRequest.Form["identidad"]);

                    pass = autil.Sha(pass);

                    Usuario p = ent.Usuario.Where(pr => pr.Login == login && pr.Password == pass).SingleOrDefault();

                    if (p != null)
                    {
                        //consultamos los roles disponibles
                        List<ComboModel> roles = ent.Rol_Usuario.Where(r => r.Id_Usuario == p.Id_Usuario && r.Id_Entidad == entidad && r.Estado == true).Select(t => new ComboModel
                        {
                            Id = t.Rol.Id_Rol,
                            Value = t.Rol.Nombre
                        }).ToList();

                        if (roles.Count() != 0)
                        {
                            op.Id_User = p.Id_Usuario;
                            op.Login = p.Login;
                            op.Email = p.Email;

                            List<ComboModel> entidades = (p.Entidad_Usuario.Where(c => c.Estado == true && c.Entidad.Id_Entidad == entidad)
                                                    .Select(t => new ComboModel
                                                    {
                                                        Id = t.Id_Entidad,
                                                        Value = t.Entidad.Nombre
                                                    }).ToList());

                            op.Roles = roles.GroupBy(rl => rl.Id).Select(g => g.First()).ToList();
                            op.Entidades = entidades.GroupBy(rl => rl.Id).Select(g => g.First()).ToList();
                            op.token = JwtManager.GenerateToken(p.Login, p.Id_Usuario.ToString(), roles, entidad);
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
                            string nombre = p.Nombres + " " + p.Primer_Apellido + " " + p.Segundo_Apellido;
                            string user = p.Login;

                            SendMail(p.Email, token, nombre, user);

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

        #region Create - ActInactivate - lists
        /// <summary>
        /// Metodo para crear usuarios
        /// </summary>
        public Basic CreateUser(HttpRequest httpRequest)
        {
            string pasos = "0";
            Basic ret = new Basic();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                    if (cp != null)
                    {
                        pasos = pasos + "-1-";
                        Guid entidad;
                        if (!string.IsNullOrEmpty(httpRequest.Form["entidadid"]))
                            entidad = Guid.Parse(httpRequest.Form["entidadid"]);
                        else
                            entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        pasos = pasos + "-2-";
                        Guid? usuario = null;
                        if (!string.IsNullOrEmpty(httpRequest.Form["usuarioid"]))
                            usuario = Guid.Parse(httpRequest.Form["usuarioid"]);
                        else
                            usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
                        pasos = pasos + "-3-";
                        string tipoidentificacion = Convert.ToString(httpRequest.Form["tipoidentificacion"]);
                        string identificacion = Convert.ToString(httpRequest.Form["identificacion"]);
                        string nombre = Convert.ToString(httpRequest.Form["nombre"]);
                        string primer_apellido = Convert.ToString(httpRequest.Form["primerapellido"]);
                        string segundo_apellido = Convert.ToString(httpRequest.Form["segundoapellido"]);
                        string sexo = Convert.ToString(httpRequest.Form["sexo"]);
                        bool presta_servicio = Convert.ToBoolean(int.Parse(httpRequest.Form["prestaservicio"]));
                        pasos = pasos + "-fechanacimiento-" + httpRequest.Form["fechanacimiento"].ToString();
                        DateTime fnacimiento = Convert.ToDateTime(httpRequest.Form["fechanacimiento"]);
                        pasos = pasos + "-4-";
                        ///FOTO
                        ///
                        String foto = string.Empty;
                        if (httpRequest.Files.Count > 0)
                        {
                            var _foto = httpRequest.Files;
                            Byte[] Content = new BinaryReader(_foto[0].InputStream).ReadBytes(_foto[0].ContentLength);
                            foto = Convert.ToBase64String(Content);
                        }

                        pasos = pasos + "-5-";
                        ///FOTO ABEAS
                        String fotoabeas = string.Empty;
                        if (httpRequest.Files.Count > 1)
                        {
                            var _fotoabeas = httpRequest.Files;
                            Byte[] Content = new BinaryReader(_fotoabeas[1].InputStream).ReadBytes(_fotoabeas[1].ContentLength);
                            fotoabeas = Convert.ToBase64String(Content);
                        }
                        pasos = pasos + "-6-";
                        string estadocivil = Convert.ToString(httpRequest.Form["estadocivil"]);
                        string tipo_sangre = Convert.ToString(httpRequest.Form["tiposangre"]);

                        pasos = pasos + "-7-";
                        int? poblado_id = null;
                        if (!string.IsNullOrEmpty(httpRequest.Form["barrio"]))
                            poblado_id = int.Parse(httpRequest.Form["barrio"]);
                        pasos = pasos + "-8-";
                        DateTime fcontratacion = Convert.ToDateTime(httpRequest.Form["fechacontratacion"]);
                        string observaciones = Convert.ToString(httpRequest.Form["observaciones"]);
                        Guid tipo_vinculacion = Guid.Parse(httpRequest.Form["tipovinculacion"]);
                        string direccion = Convert.ToString(httpRequest.Form["direccion"]);
                        string telefono = Convert.ToString(httpRequest.Form["telefono"]);
                        pasos = pasos + "-9-";
                        string login = Convert.ToString(httpRequest.Form["user"]);
                        string password = Convert.ToString(httpRequest.Form["password"]);
                        string email = Convert.ToString(httpRequest.Form["email"]);
                        pasos = pasos + "-10-";

                        pasos = pasos + "-abeas-" + Convert.ToString(httpRequest.Form["abeas"]);
                        pasos = pasos + "-abeas-" + Convert.ToString(httpRequest.Form["tipoprofesional"]);
                        pasos = pasos + "-abeas-" + Convert.ToString(httpRequest.Form["registroprofesional"]);

                        bool abeas = Convert.ToBoolean(int.Parse(httpRequest.Form["abeas"]));
                        Guid? tipoprofesional = Guid.Parse(httpRequest.Form["tipoprofesional"]);
                        string registroprofesional = Convert.ToString(httpRequest.Form["registroprofesional"]);
                        pasos = pasos + "-11-";
                        password = autil.Sha(password);
                        pasos = pasos + "-12-";
                        List<Usuario> lus = ent.Usuario.ToList();

                        int lexist = lus.Where(t => t.Login == login).Count();

                        if (lexist == 0)
                        {
                            int idexist = lus.Where(t => t.Numero_Identificacion == identificacion).Count();

                            if (idexist == 0)
                            {
                                int emexist = lus.Where(t => t.Email == email).Count();

                                if (emexist == 0)
                                {
                                    int regxist = 0;
                                    if (!string.IsNullOrEmpty(registroprofesional))
                                        regxist = ent.Profesional.Where(r => r.Registro_Profesional == registroprofesional).Count();

                                    if (regxist == 0)
                                    {
                                        pasos = pasos + "-13-";
                                        Guid id_usuario = Guid.NewGuid();
                                        Usuario us = new Usuario();
                                        us.Id_Usuario = id_usuario;
                                        us.Nombres = nombre;
                                        us.Primer_Apellido = primer_apellido;
                                        us.Segundo_Apellido = segundo_apellido;
                                        us.Id_Tipo_Identificacion = tipoidentificacion;
                                        us.Numero_Identificacion = identificacion;
                                        us.Poblado_Id = poblado_id;
                                        us.Direccion = direccion;
                                        us.Telefono = telefono;
                                        us.Sexo = sexo;
                                        us.Fecha_Nacimiento = fnacimiento;
                                        us.Foto = foto;
                                        us.Estado_Civil = estadocivil;
                                        us.TipoSangre = tipo_sangre;
                                        us.Fecha_Contratacion = fcontratacion;
                                        us.Id_Tipo_Vinculacion = tipo_vinculacion;
                                        us.Observaciones = observaciones;
                                        us.Presta_Servicio = presta_servicio;

                                        us.Acepta_ABEAS = abeas;
                                        us.Foto_ABEAS = fotoabeas;

                                        us.Login = login;
                                        us.Password = password;
                                        us.Email = email;
                                        pasos = pasos + "-14-";
                                        ///agregado el usuario a la entidad donde se esta creando
                                        Entidad_Usuario eu = new Entidad_Usuario();
                                        eu.Id_Entidad = entidad;
                                        eu.Id_Usuario = id_usuario;
                                        eu.Estado = true;
                                        eu.Fecha_Create = DateTime.Now;
                                        eu.Fecha_Update = DateTime.Now;
                                        eu.Usuario_Create = usuario.Value;
                                        eu.Usuario_Update = usuario.Value;
                                        ent.Entidad_Usuario.Add(eu);
                                        /////

                                        us.Fecha_Create = DateTime.Now;
                                        us.Usuario_Create = usuario;
                                        us.Fecha_Update = DateTime.Now;
                                        us.Usuario_Update = usuario;
                                        ent.Usuario.Add(us);
                                        pasos = pasos + "-15-";
                                        if (tipoprofesional != null)
                                        {
                                            pasos = pasos + "-16-";
                                            Profesional pf = new Profesional();
                                            pf.Id_Profesional = Guid.NewGuid();
                                            pf.Id_Usuario = id_usuario;
                                            pf.Id_Tipo_Profesional = tipoprofesional.Value;
                                            pf.Registro_Profesional = registroprofesional;
                                            pf.Created_At = DateTime.Now;
                                            pf.Updated_At = DateTime.Now;
                                            pf.Usuario_Create = usuario;
                                            pf.Usuario_Update = usuario;
                                            ent.Profesional.Add(pf);
                                        }
                                        pasos = pasos + "-17-";
                                        ///salvando todos los cambios
                                        ent.SaveChanges();
                                        //se genera el codigo del mensaje de retorno exitoso
                                        ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
                                    }
                                    else
                                    {
                                        ///Registro profesional existe
                                        return ret = autil.MensajeRetorno(ref ret, 3, string.Empty, null);
                                    }
                                }
                                else
                                {
                                    ///Email existe
                                    return ret = autil.MensajeRetorno(ref ret, 6, string.Empty, null);
                                }
                            }
                            else
                            {
                                ///Cedula existe
                                return ret = autil.MensajeRetorno(ref ret, 5, string.Empty, null);
                            }
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
                    ret = autil.MensajeRetorno(ref ret, 4, ex.Message + " " + ex.InnerException + "/" + pasos, null);
                    return ret;
                }
            }
        }

        /// <summary>
        /// Metodo que valida si una cedula ya existe
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic ValidateUser(HttpRequest httpRequest)
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

                        string identificacion = Convert.ToString(httpRequest.Form["identificacion"]);

                        List<Entidad_Usuario> leus = ent.Entidad_Usuario.ToList();

                        Entidad_Usuario us = leus.Where(t => t.Usuario.Numero_Identificacion == identificacion).SingleOrDefault();
                        if (us != null)
                        {
                            //La cedula si existe
                            if (us.Id_Entidad == entidad)
                                ///Cedula existe en esta entidad
                                ret = autil.MensajeRetorno(ref ret, 22, string.Empty, null);
                            else
                                ///Cedula existe en otra entidad
                                ret = autil.MensajeRetorno(ref ret, 23, string.Empty, null);
                        }
                        else
                        {
                            ///Cedula NO existe                            
                            ret = autil.MensajeRetorno(ref ret, 24, string.Empty, null);
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
        /// Metodo para editar la informacion del usuario
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic EditUser(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                    if (cp != null)
                    {
                        Guid entidad;
                        if (!string.IsNullOrEmpty(httpRequest.Form["entidadid"]))
                            entidad = Guid.Parse(httpRequest.Form["entidadid"]);
                        else
                            entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());

                        Guid? usuario = null;
                        if (!string.IsNullOrEmpty(httpRequest.Form["usuarioid"]))
                            usuario = Guid.Parse(httpRequest.Form["usuarioid"]);
                        else
                            usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Guid id_user = Guid.Parse(httpRequest.Form["iduser"]);
                        string nombre = Convert.ToString(httpRequest.Form["nombre"]);
                        string primer_apellido = Convert.ToString(httpRequest.Form["primerapellido"]);
                        string segundo_apellido = Convert.ToString(httpRequest.Form["segundoapellido"]);
                        string sexo = Convert.ToString(httpRequest.Form["sexo"]);
                        DateTime fnacimiento = Convert.ToDateTime(httpRequest.Form["fechanacimiento"]);
                        bool presta_servicio = Convert.ToBoolean(int.Parse(httpRequest.Form["prestaservicio"]));

                        ///FOTO
                        ///
                        String foto = string.Empty;
                        if (httpRequest.Files.Count > 0)
                        {
                            var _foto = httpRequest.Files;
                            Byte[] Content = new BinaryReader(_foto[0].InputStream).ReadBytes(_foto[0].ContentLength);
                            foto = Convert.ToBase64String(Content);
                        }

                        ///FOTO ABEAS
                        String fotoabeas = string.Empty;
                        if (httpRequest.Files.Count > 1)
                        {
                            var _fotoabeas = httpRequest.Files;
                            Byte[] Content = new BinaryReader(_fotoabeas[1].InputStream).ReadBytes(_fotoabeas[1].ContentLength);
                            fotoabeas = Convert.ToBase64String(Content);
                        }

                        string estadocivil = Convert.ToString(httpRequest.Form["estadocivil"]);
                        string tipo_sangre = Convert.ToString(httpRequest.Form["tiposangre"]);
                        int? poblado_id = null;
                        if (!string.IsNullOrEmpty(httpRequest.Form["barrio"]))
                            poblado_id = int.Parse(httpRequest.Form["barrio"]);
                        DateTime fcontratacion = Convert.ToDateTime(httpRequest.Form["fechacontratacion"]);
                        string observaciones = Convert.ToString(httpRequest.Form["observaciones"]);
                        Guid tipo_vinculacion = Guid.Parse(httpRequest.Form["tipovinculacion"]);
                        bool estado = Convert.ToBoolean(int.Parse(httpRequest.Form["estado"]));

                        string login = Convert.ToString(httpRequest.Form["user"]);
                        string email = Convert.ToString(httpRequest.Form["email"]);

                        bool abeas = Convert.ToBoolean(int.Parse(httpRequest.Form["abeas"]));

                        Guid? tipoprofesional = Guid.Parse(httpRequest.Form["tipoprofesional"]);
                        string registroprofesional = Convert.ToString(httpRequest.Form["registroprofesional"]);

                        List<Usuario> lus = ent.Usuario.ToList();

                        int lexist = lus.Where(t => t.Login == login && t.Id_Usuario != id_user).Count();

                        if (lexist == 0)
                        {
                            int emexist = lus.Where(t => t.Email == email && t.Id_Usuario != id_user).Count();

                            if (emexist == 0)
                            {
                                int regxist = 0;
                                if (!string.IsNullOrEmpty(registroprofesional))
                                    regxist = ent.Profesional.Where(r => r.Registro_Profesional == registroprofesional && r.Usuario.Id_Usuario != id_user).Count();

                                if (regxist == 0)
                                {
                                    Guid id_usuario = Guid.NewGuid();

                                    Usuario us = ent.Usuario.Where(u => u.Id_Usuario == id_user).SingleOrDefault();

                                    us.Nombres = nombre;
                                    us.Primer_Apellido = primer_apellido;
                                    us.Segundo_Apellido = segundo_apellido;
                                    us.Poblado_Id = poblado_id;
                                    us.Sexo = sexo;
                                    us.Fecha_Nacimiento = fnacimiento;
                                    us.Foto = foto;
                                    us.Estado_Civil = estadocivil;
                                    us.TipoSangre = tipo_sangre;
                                    us.Fecha_Contratacion = fcontratacion;
                                    us.Id_Tipo_Vinculacion = tipo_vinculacion;
                                    us.Observaciones = observaciones;

                                    us.Acepta_ABEAS = abeas;
                                    us.Foto_ABEAS = fotoabeas;
                                    us.Presta_Servicio = presta_servicio;

                                    us.Login = login;
                                    us.Email = email;

                                    us.Fecha_Create = DateTime.Now;
                                    us.Usuario_Create = usuario;
                                    us.Fecha_Update = DateTime.Now;
                                    us.Usuario_Update = usuario;

                                    if (tipoprofesional != null)
                                    {
                                        Profesional pf = ent.Profesional.Where(p => p.Id_Usuario == us.Id_Usuario).SingleOrDefault();
                                        pf.Id_Tipo_Profesional = tipoprofesional.Value;
                                        pf.Registro_Profesional = registroprofesional;
                                        pf.Updated_At = DateTime.Now;
                                        pf.Usuario_Update = usuario;
                                    }

                                    ent.SaveChanges();
                                    //se genera el codigo del mensaje de retorno exitoso
                                    ret = autil.MensajeRetorno(ref ret, 20, string.Empty, null);
                                }
                                else
                                {
                                    ///Registro profesional existe
                                    return ret = autil.MensajeRetorno(ref ret, 3, string.Empty, null);
                                }
                            }
                            else
                            {
                                ///Email existe
                                return ret = autil.MensajeRetorno(ref ret, 6, string.Empty, null);
                            }
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
        /// Metodo para agregar un usuario existente a una entidad
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic CreateEntidadUser(HttpRequest httpRequest)
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
                        string identificacion = Convert.ToString(httpRequest.Form["identificacion"]);

                        Usuario us = ent.Usuario.Where(u => u.Numero_Identificacion == identificacion).SingleOrDefault();
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
                            ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
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
        /// metodo para consultar los usuarios de una entidad
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public List<UsuarioModel> GetUsuariosEdit(HttpRequest httpRequest)
        {
            List<UsuarioModel> um = new List<UsuarioModel>();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid idusuario = Guid.Parse(httpRequest.Form["idusuario"]);

                        List<Usuario> us = ent.Entidad_Usuario.Where(t => t.Id_Entidad == entidad && t.Id_Usuario == idusuario)
                                           .Select(u => u.Usuario).ToList();

                        foreach (var i in us)
                        {
                            UsuarioModel usm = new UsuarioModel();
                            Copier.CopyPropertiesTo(i, usm);
                            usm.Id_Municipio = i.Poblado.Municipio_Id;
                            usm.Id_Departamento = i.Poblado.Municipio.Departamento.Dane_Id;
                            if (i.Presta_Servicio)
                                usm.Presta_Servicio_Int = 1;
                            else
                                usm.Presta_Servicio_Int = 0;

                            if (i.Acepta_ABEAS)
                                usm.Acepta_ABEAS_Int = 1;
                            else
                                usm.Acepta_ABEAS_Int = 0;

                            um.Add(usm);
                        }                        

                        return um.OrderBy(o=> o.Nombres).ToList();
                    }
                }
                else
                    return um;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UsuarioModel> GetUsuarios(HttpRequest httpRequest)
        {
            List<UsuarioModel> um = new List<UsuarioModel>();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());

                        IQueryable<Usuario> us = ent.Entidad_Usuario.Where(t => t.Id_Entidad == entidad).Select(u => u.Usuario);
                        //consulta por id usuario
                        if (!string.IsNullOrEmpty(httpRequest.Form["idusuario"]))
                        {
                            Guid idusuario = Guid.Parse(httpRequest.Form["idusuario"]);
                            us = us.Where(c => c.Id_Usuario == idusuario);
                        }

                        //consulta por cedula
                        if (!string.IsNullOrEmpty(httpRequest.Form["identificacion"]))
                        {
                            string nroidentificacion = Convert.ToString(httpRequest.Form["identificacion"]);
                            us = us.Where(c => c.Numero_Identificacion.Contains(nroidentificacion));
                        }
                        ///consulta por nombre
                        if (!string.IsNullOrEmpty(httpRequest.Form["nombres"]))
                        {
                            string nombres = Convert.ToString(httpRequest.Form["nombres"]);
                            us = us.Where(c => c.Nombres.Contains(nombres));
                        }

                        ///consulta por primer apellido
                        if (!string.IsNullOrEmpty(httpRequest.Form["primerapellido"]))
                        {
                            string primerapellido = Convert.ToString(httpRequest.Form["primerapellido"]);
                            us = us.Where(c => c.Primer_Apellido.Contains(primerapellido));
                        }

                        ///consulta por segundo apellido
                        if (!string.IsNullOrEmpty(httpRequest.Form["segundoapellido"]))
                        {
                            string segundoapellido = Convert.ToString(httpRequest.Form["segundoapellido"]);
                            us = us.Where(c => c.Segundo_Apellido.Contains(segundoapellido));
                        }

                        ///consulta por login
                        if (!string.IsNullOrEmpty(httpRequest.Form["login"]))
                        {
                            string login = Convert.ToString(httpRequest.Form["login"]);
                            us = us.Where(c => c.Login.Contains(login));
                        }

                        ///consulta por email
                        if (!string.IsNullOrEmpty(httpRequest.Form["email"]))
                        {
                            string email = Convert.ToString(httpRequest.Form["email"]);
                            us = us.Where(c => c.Email.Contains(email));
                        }

                        int pageSize = Convert.ToInt32(httpRequest.Form["pageSize"]);
                        int startingPageIndex = Convert.ToInt32(httpRequest.Form["startingPageIndex"]);


                        //IQueryable<Usuario> us = ent.Entidad_Usuario.Where(t => t.Id_Entidad == entidad).Select(u => u.Usuario);
                        um = us.Select(u => new UsuarioModel                        
                        {
                            Id_Usuario = u.Id_Usuario,
                            Nombres = u.Nombres,
                            Primer_Apellido = u.Primer_Apellido,
                            Segundo_Apellido = u.Segundo_Apellido,
                            Login = u.Login,
                            Fecha_Contratacion = u.Fecha_Contratacion,
                            Email = u.Email
                        }).OrderBy(o=> o.Nombres).Skip(startingPageIndex * pageSize).Take(pageSize).ToList();

                        return um;
                    }
                }
                else
                    return um;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Metodo pra llenar combo de tipo de de identificacion
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public List<ComboModel> GetTipoIdentificacion(HttpRequest httpRequest)
        {
            List<ComboModel> ret = new List<ComboModel>();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        ret = ent.Tipo_Identificacion.Select(l => new ComboModel { id = l.Id_Tipo_Identificacion, Value = l.Nombre }).ToList();
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
                        ret = ent.Tipo_Profesional.Select(l => new ComboModel { id = l.Id_Tipo_Profesional, Value = l.Nombre }).ToList();
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
                        Guid idprofesional = Guid.Parse(httpRequest.Form["idprofesional"]);
                        Guid idespecialidad = Guid.Parse(httpRequest.Form["idespecialidad"]);

                        Especialidad_Profesional ep = ent.Especialidad_Profesional
                                                      .Where(e => e.Id_Entidad == entidad
                                                      && e.Id_Especialidad == idespecialidad
                                                      && e.Id_Profesional == idprofesional).SingleOrDefault();

                        if (ep == null)
                        {
                            ep = new Especialidad_Profesional();
                            ep.Id_Entidad = entidad;
                            ep.Id_Profesional = idprofesional;
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
                        Guid idprofesional = Guid.Parse(httpRequest.Form["idprofesional"]);
                        Guid idespecialidad = Guid.Parse(httpRequest.Form["idespecialidad"]);
                        bool estado = Convert.ToBoolean(int.Parse(httpRequest.Form["estado"]));

                        Especialidad_Profesional ep = ent.Especialidad_Profesional
                                                      .Where(e => e.Id_Entidad == entidad
                                                      && e.Id_Especialidad == idespecialidad
                                                      && e.Id_Profesional == idprofesional).SingleOrDefault();

                        if (ep != null)
                        {
                            ep.Id_Entidad = entidad;
                            ep.Id_Profesional = idprofesional;
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
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public List<ComboModel> GetRoles(HttpRequest httpRequest)
        {
            List<ComboModel> ret = new List<ComboModel>();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
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
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
                        Guid idusuario = Guid.Parse(httpRequest.Form["usuario"]);

                        ret = ent.Rol_Usuario.Where(u => u.Id_Entidad == entidad && u.Id_Usuario == idusuario && u.Estado == true).Select(l => new ComboModel { id = l.Id_Rol, Value = l.Rol.Nombre }).ToList();
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

        public List<ComboModel> GetNotRolesUsuario(HttpRequest httpRequest)
        {
            List<ComboModel> ret = new List<ComboModel>();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
                        Guid idusuario = Guid.Parse(httpRequest.Form["usuario"]);

                        List<Rol> rol = ent.Rol_Usuario.Where(u => u.Id_Entidad == entidad && u.Id_Usuario == idusuario && u.Estado == true).Select(t => t.Rol).ToList();
                        List<Rol> _rol = ent.Rol.ToList();

                        ret = _rol.Except(rol).Select(l => new ComboModel { id = l.Id_Rol, Value = l.Nombre }).ToList();

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
        public Basic CreateRolUsuario(HttpRequest httpRequest)
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

                        Guid idusuario = Guid.Parse(httpRequest.Form["usuario"]);
                        Guid rol = Guid.Parse(httpRequest.Form["idrol"]);

                        Rol_Usuario ru = ent.Rol_Usuario.Where(r => r.Id_Rol == rol && r.Id_Usuario == idusuario && r.Id_Entidad == entidad).SingleOrDefault();
                        if (ru == null)
                        {
                            ru = new Rol_Usuario();
                            ru.Id_Entidad = entidad;
                            ru.Id_Usuario = idusuario;
                            ru.Id_Rol = rol;
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
                        return ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
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
        public Basic EditRolUsuario(HttpRequest httpRequest)
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

                        Guid idusuario = Guid.Parse(httpRequest.Form["usuario"]);
                        Guid rol = Guid.Parse(httpRequest.Form["idrol"]);
                        bool estado = Convert.ToBoolean(int.Parse(httpRequest.Form["estado"]));

                        Rol_Usuario ru = ent.Rol_Usuario.Where(r => r.Id_Rol == rol && r.Id_Usuario == idusuario && r.Id_Entidad == entidad).SingleOrDefault();
                        if (ru != null)
                        {
                            ru.Estado = estado;
                            ru.Usuario_Update = usuario;
                            ru.Fecha_Update = DateTime.Now;
                            ent.SaveChanges();
                        }
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
        //Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
        //Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
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