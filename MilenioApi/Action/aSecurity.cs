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

namespace MilenioApi.Action
{
    public class aSecurity
    {
        aUtilities autil = new aUtilities();
        ClaimsPrincipal cp = new ClaimsPrincipal();
        TokenValidationHandler tvh = new TokenValidationHandler();

        #region Login

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
                                            // login = t.Login,
                                            entidades
                                        }).SingleOrDefault();

                                        rp.data = data;
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
                            return autil.MensajeRetorno(ref rp, 8, string.Empty, null, HttpStatusCode.OK);
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
                    return autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
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
                        //si la entidad viene vacia, quiere decir que esta haciendo loguin normal y envia 
                        //al metodo de login a ver si trae entidades o si solo hace loguin porque trae una sola entidad
                        if (model.Id_Entidad == null || model.Id_Entidad == Guid.Empty)
                        {
                            return Login(model);
                        }
                        else
                        {
                            //si trae entidad llena, entonces ya hace el metodo de logueo
                            string pass = autil.Sha(model.Password);
                            var um = ent.Usuario.Where(pr => pr.Login == model.User && pr.Password == pass).ToList();

                            if (um.Count > 0)
                            {
                                foreach (var p in um)
                                {
                                    //consultamos los roles disponibles
                                    List<string> roles = ent.Rol_Usuario.Where(r => r.Id_Usuario == p.Id_Usuario && r.Id_Entidad == model.Id_Entidad && r.Estado == true)
                                                        .Select(t => t.Rol.Nombre).ToList();

                                    if (roles.Count() != 0)
                                    {
                                        List<ComboModel> entidades = (p.Entidad_Usuario.Where(c => c.Estado == true && c.Entidad.Id_Entidad == model.Id_Entidad)
                                                                .Select(t => new ComboModel
                                                                {
                                                                    id = t.Id_Entidad,
                                                                    value = t.Entidad.Nombre
                                                                }).ToList());

                                        entidades = entidades.GroupBy(rl => rl.id).Select(g => g.First()).ToList();

                                        roles = roles.GroupBy(rl => rl).Select(g => g.First()).ToList();

                                        string token = tvh.GenerateToken(p.Login, p.Id_Usuario.ToString(), roles, model.Id_Entidad);

                                        //aqui seteamos que el usuario ya esta logueado
                                        // p.isloged = true;
                                        ent.SaveChanges();

                                        var r = (from t in um
                                                 select new
                                                 {
                                                     //login = p.Login,
                                                     //entidades,
                                                     // roles,
                                                     token
                                                 }).SingleOrDefault();

                                        rp.data = r;
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
                    return autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
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
                    cp = tvh.getprincipal(Convert.ToString(model.token));

                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
                    Usuario us = ent.Usuario.Where(u => u.Id_Usuario == usuario).SingleOrDefault();

                    if (us != null)
                    {
                        us.isloged = false;
                        ent.SaveChanges();
                        return autil.MensajeRetorno(ref rp, 30, string.Empty, null, HttpStatusCode.OK);
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    return autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
                }

                return rp;
            }
        }

        public void TimeLogOff(Guid idusuario)
        {
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                Usuario us = ent.Usuario.Where(u => u.Id_Usuario == idusuario).SingleOrDefault();

                if (us != null)
                {
                    us.isloged = false;
                    ent.SaveChanges();
                }
            }
        }

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

                        if (!string.IsNullOrEmpty(p.Email))
                        {
                            //se genera un token que sera enviado al correo para validar cuando trate de cambiar la clave
                            string token = Convert.ToBase64String(Encoding.Unicode.GetBytes(tvh.GenerateToken(p.Login, p.Id_Usuario.ToString(), null, null)));
                            string nombre = p.Nombres + " " + p.Primer_Apellido + " " + p.Segundo_Apellido;
                            string user = p.Login;

                            //se envia el correo para cambio de clave
                            autil.SendMail(p.Email, this.SetForgotEmailBody(user, token), ConfigurationManager.AppSettings["EmailSubjec"]);

                            //se guarda el toen que se genero para envirlo al url y luego
                            //cuando traten de cambiar la clave, se consulta con la base de datos y se compara
                            //para aluego borrarlo al cambiar la clave
                            p.Token_Password_Change = token;
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
                rp = autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
                return rp;
            }
        }

        private AlternateView SetForgotEmailBody(string subject, string token)
        {
            try
            {
                //se arma el correo que se envia para el ambio de clave
                string plantilla = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["EmailTemplate"]);
                string url = ConfigurationManager.AppSettings["EmailResetUrl"];

                url = url + "?token=" + token;
                var html = System.IO.File.ReadAllText(plantilla);
                html = html.Replace("{{name}}", subject);
                html = html.Replace("{{action_url}}", url);

                AlternateView av = AlternateView.CreateAlternateViewFromString(html, null, "text/html");

                //create the LinkedResource (embedded image)
                LinkedResource logo = new LinkedResource(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["LogoPath"]));
                logo.ContentId = "companylogo";
                //add the LinkedResource to the appropriate view
                av.LinkedResources.Add(logo);

                return av;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object ChangePassword(LoginModel model)
        {
            Response ret = new Response();
            try
            {
                cp = tvh.getprincipal(Convert.ToString(model.token));
                Guid user_id = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Usuario p = ent.Usuario.Where(u => u.Id_Usuario == user_id).SingleOrDefault();

                    if (p != null)
                    {
                        string oldp = autil.Sha(model.OldPassword);

                        if (oldp == p.Password)
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
                    else
                    {
                        ret = autil.MensajeRetorno(ref ret, 35, string.Empty, null);
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                //Error General
                ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                return ret;
            }
        }

        public object ResetPassword(LoginModel model)
        {
            Response ret = new Response();
            try
            {
                cp = tvh.getprincipal(Convert.ToString(Encoding.Unicode.GetString(Convert.FromBase64String(model.token))));

                Guid? user_id = null;
                string usid = cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(usid))
                    user_id = Guid.Parse(usid);

                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Usuario p = ent.Usuario.Where(u => u.Id_Usuario == user_id).SingleOrDefault();

                    if (p != null)
                    {
                        if (p.Token_Password_Change == model.token)
                        {
                            p.Password = autil.Sha(model.Password);
                            p.Token_Password_Change = null;
                            ent.SaveChanges();
                            ret = autil.MensajeRetorno(ref ret, 10, string.Empty, null);
                        }
                        else
                        {
                            ret = autil.MensajeRetorno(ref ret, 34, string.Empty, null);
                        }
                    }
                    else
                    {
                        ret = autil.MensajeRetorno(ref ret, 15, string.Empty, null);
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                //Error General
                ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                return ret;
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
        //            cp = tvh.getprincipal(Convert.ToString(model.Form["token"]));
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