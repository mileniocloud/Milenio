using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
using WebApi.Jwt;
using System.Collections.Generic;
using System.Security.Claims;


namespace MilenioApi.Action
{
    public class aSeguridad
    {
        private TokenController tk = new TokenController();
        aUtilities autil = new aUtilities();
        ClaimsPrincipal cp = new ClaimsPrincipal();

        #region Manejo Cuentas        
        public oPersonaModel Login(HttpRequest httpRequest)
        {
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                oPersonaModel op = new oPersonaModel();
                try
                {
                    string login = Convert.ToString(httpRequest.Form["user"]);
                    string pass = Convert.ToString(httpRequest.Form["password"]);
                    pass = autil.Sha(pass);

                    Persona p = (from pr in ent.Persona
                                 where pr.Login == login
                                 && pr.Password == pass
                                 select pr).SingleOrDefault();

                    int cantr = p.Entidad_Persona_Rol.Where(t => t.Estado == true).Count();

                    if (p != null)
                    {
                        if (cantr != 0)
                        {
                            op.Persona_Id = p.Codigo_Id;
                            op.Login = p.Login;
                            op.Email = p.Email;
                            op.nombre = p.Nombres + " " + p.Apellidos;

                            List<ComboModel> entidades = (from t in p.Entidad_Persona_Rol
                                                          where t.Estado == true
                                                          select new ComboModel
                                                          {
                                                              Id = t.Entidad.Codigo_Id,
                                                              Value = t.Entidad.Nombre
                                                          }).ToList();

                            op.Entidades = entidades.GroupBy(et => et.Id).Select(g => g.First()).ToList();
                        }
                        else
                        {
                            //USIARIO INACTIVO
                            Basic b = new Basic();
                            autil.MensajeRetorno(ref b, 19, string.Empty, null);

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

        public oPersonaModel LoginEntidad(HttpRequest httpRequest)
        {
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                oPersonaModel op = new oPersonaModel();
                try
                {
                    string login = Convert.ToString(httpRequest.Form["user"]);
                    string pass = Convert.ToString(httpRequest.Form["password"]);
                    Guid entidad = Guid.Parse(httpRequest.Form["Entidad_Id"]);

                    pass = autil.Sha(pass);
                    Persona p = (from ep in ent.Persona
                                 where ep.Login == login
                                 && ep.Password == pass
                                 select ep).SingleOrDefault();

                    int cantr = p.Entidad_Persona_Rol.Where(t => t.Estado == true && t.Entidad_Id == entidad).Count();

                    if (p != null && p.Entidad_Persona_Rol.Where(t => t.Entidad_Id == entidad).Count() > 0)
                    {
                        if (cantr != 0)
                        {
                            op.Persona_Id = p.Codigo_Id;
                            op.Login = p.Login;
                            op.Email = p.Email;
                            op.nombre = p.Nombres + " " + p.Apellidos;

                            List<ComboModel> roles = (from t in p.Entidad_Persona_Rol
                                                      where t.Estado == true
                                                      select new ComboModel
                                                      {
                                                          Id = t.Rol.Codigo_Id,
                                                          Value = t.Rol.Nombre
                                                      }).ToList();

                            List<ComboModel> entidades = (from t in p.Entidad_Persona_Rol
                                                          where t.Estado == true
                                                          select new ComboModel
                                                          {
                                                              Id = t.Entidad.Codigo_Id,
                                                              Value = t.Entidad.Nombre
                                                          }).Distinct().ToList();

                            op.Roles = roles.GroupBy(rl => rl.Id).Select(g => g.First()).ToList();
                            op.Entidades = entidades.GroupBy(et => et.Id).Select(g => g.First()).ToList();
                            op.token = JwtManager.GenerateToken(p.Login, roles, p.Codigo_Id.ToString(), entidades, entidad);

                        }
                        else
                        {
                            //Usuario Inactivo
                            Basic b = new Basic();
                            autil.MensajeRetorno(ref b, 19, string.Empty, null);

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
                    autil.MensajeRetorno(ref b, 4, ex.Message, null);

                    op.Codigo = b.Codigo;
                    op.custom = b.custom;
                    op.Message = b.Message;

                    return op;
                }

                return op;
            }
        }
        /// <summary>
        /// Este metodo permite que un usuario se cambie de colegio en cualquier momento,
        /// se valida con el token para tener el usuario y el id de la entidad
        /// con eso se verifica si el esta agregado a esa entidad
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public oPersonaModel CambioEntidad(HttpRequest httpRequest)
        {
            oPersonaModel op = new oPersonaModel();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                    if (cp != null)
                    {
                        Guid entidadcambio = Guid.Parse(httpRequest.Form["Entidad_Id"]);
                        //se saca el usuario que esta cambiandose
                        Guid usid = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Persona p = (from pr in ent.Entidad_Persona_Rol
                                     where pr.Persona_Id == usid
                                     && pr.Entidad_Id == entidadcambio
                                     select pr.Persona).FirstOrDefault();

                        if (p != null)
                        {

                            List<ComboModel> roles = (from t in p.Entidad_Persona_Rol
                                                      select new ComboModel
                                                      {
                                                          Id = t.Rol.Codigo_Id,
                                                          Value = t.Rol.Nombre
                                                      }).ToList();

                            List<ComboModel> entidades = (from t in p.Entidad_Persona_Rol
                                                          select new ComboModel
                                                          {
                                                              Id = t.Entidad.Codigo_Id,
                                                              Value = t.Entidad.Nombre
                                                          }).Distinct().ToList();


                            //consulta exitosa
                            op.Roles = roles.GroupBy(rl => rl.Id).Select(g => g.First()).ToList();
                            op.Entidades = entidades.GroupBy(et => et.Id).Select(g => g.First()).ToList();
                            op.token = JwtManager.GenerateToken(p.Login, roles, p.Codigo_Id.ToString(), entidades, entidadcambio);

                            Basic b = new Basic();
                            autil.MensajeRetorno(ref b, 9, string.Empty, null);

                            op.Persona_Id = usid;
                            op.Codigo = b.Codigo;
                            op.custom = b.custom;
                            op.Message = b.Message;
                        }
                        else
                        {
                            //usuario no asignado a entidad
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
                        //Token invalido
                        Basic b = new Basic();
                        autil.MensajeRetorno(ref b, 1, string.Empty, null);

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
                    autil.MensajeRetorno(ref b, 4, ex.Message, null);

                    op.Codigo = b.Codigo;
                    op.custom = b.custom;
                    op.Message = b.Message;

                    return op;
                }

                return op;
            }
            #endregion
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
                string Login = Convert.ToString(httpRequest.Form["Login"]);
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Persona p = (from t in ent.Persona
                                 where t.Login == Login
                                 select t).SingleOrDefault();

                    if (p != null)
                    {
                        p.Cambiar_Clave = true;
                        //metodo para enviar reseteo de contraseña
                        //p.Email;
                        ent.SaveChanges();
                        ret = autil.MensajeRetorno(ref ret, 21, string.Empty, null);
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
                //error general
                ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                return ret;
            }
        }

        public Basic CambioClave(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                if (cp != null)
                {
                    Guid Persona_Id = Guid.Parse(httpRequest.Form["Persona_Id"]);
                    string PassWord = Convert.ToString(httpRequest.Form["Password"]);

                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Persona p = (from t in ent.Persona
                                     where t.Codigo_Id == Persona_Id
                                     select t).SingleOrDefault();

                        if (p != null)
                        {
                            PassWord = autil.Sha(PassWord);
                            p.Password = PassWord;
                            ent.SaveChanges();
                            ret = autil.MensajeRetorno(ref ret, 21, string.Empty, null);
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

    }
}