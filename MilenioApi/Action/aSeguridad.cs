using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
using WebApi.Jwt;
using System.Collections.Generic;

namespace MilenioApi.Action
{
    public class aSeguridad
    {
        private TokenController tk = new TokenController();
        aUtilities autil = new aUtilities();

        #region Manejo Cuentas        
        public oPersonaModel Login(HttpRequest httpRequest)
        {
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                oPersonaModel op = new oPersonaModel();
                try
                {
                    string login = Convert.ToString(httpRequest.Form["Login"]);
                    string pass = Convert.ToString(httpRequest.Form["Password"]);
                    pass = autil.Sha(pass);

                    Persona p = (from pr in ent.Persona
                                 where pr.Login == login
                                 && pr.Password == pass
                                 select pr).SingleOrDefault();

                    if (p != null)
                    {
                        op.Persona_Id = p.Codigo_Id;
                        op.Login = p.Login;
                        op.Email = p.Email;
                        op.nombre = p.Nombres + " " + p.Apellidos;

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
                                                      }).ToList();

                        op.Roles = roles;
                        op.Entidades = entidades;

                        //ret.token = JwtManager.GenerateToken(p.Login, entidades, p.Codigo_Id.ToString(), roles);
                    }
                    else
                    {
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

        public oPersonaModel LoginEntidad(HttpRequest httpRequest)
        {
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                oPersonaModel op = new oPersonaModel();
                try
                {
                    string login = Convert.ToString(httpRequest.Form["Login"]);
                    string pass = Convert.ToString(httpRequest.Form["Password"]);
                    Guid entidad = Guid.Parse(httpRequest.Form["Entidad_Id"]);

                    pass = autil.Sha(pass);

                    Persona p = (from pr in ent.Persona
                                 where pr.Login == login
                                 && pr.Password == pass
                                 select pr).SingleOrDefault();

                    if (p != null)
                    {
                        op.Persona_Id = p.Codigo_Id;
                        op.Login = p.Login;
                        op.Email = p.Email;
                        op.nombre = p.Nombres + " " + p.Apellidos;

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

                        op.Roles = roles.GroupBy(rl => rl.Id).Select(g => g.First()).ToList();
                        op.Entidades = entidades.GroupBy(et => et.Id).Select(g => g.First()).ToList();
                        op.token = JwtManager.GenerateToken(p.Login, roles, p.Codigo_Id.ToString(), entidades, entidad);
                    }
                    else
                    {
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

        #endregion
    }
}