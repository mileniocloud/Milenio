using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace MilenioApi.Action
{

    public class aUser
    {
        aUtilities autil = new aUtilities();
        ClaimsPrincipal cp = new ClaimsPrincipal();
        TokenValidationHandler tvh = new TokenValidationHandler();

        #region USER - Create - ActInactivate - lists

        public object CreateUser(UserModel model)
        {
            string pasos = "0";
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));

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

                                        //seccion para obtener los roles que le estan agregando al usuario
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

                                        //salvando todos los cambios
                                        ent.SaveChanges();
                                        //se genera el codigo del mensaje de retorno exitoso
                                        rp = autil.MensajeRetorno(ref rp, 2, string.Empty, null, HttpStatusCode.OK);
                                    }
                                    else
                                    {
                                        //Registro profesional existe
                                        return rp = autil.MensajeRetorno(ref rp, 3, string.Empty, null, HttpStatusCode.OK);
                                    }
                                }
                                else
                                {
                                    //Email existe
                                    return rp = autil.MensajeRetorno(ref rp, 6, string.Empty, null, HttpStatusCode.OK);
                                }
                            }
                            else
                            {
                                //Cedula existe
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
                catch (Exception ex)
                {
                    //error general
                    rp = autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
                    return rp;
                }
            }
        }

        public object EditUser(UserModel model)
        {
            Response rp = new Response();
            string pasos = "0";
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));

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
                catch (Exception ex)
                {
                    //error general
                    rp = autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
                    return rp;
                }
            }
        }

        public object ValidateUser(UserModel model)
        {
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));

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
                catch (Exception ex)
                {
                    //error general
                    rp = autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
                    return rp;
                }
            }
        }

        public object CreateEntityUser(UserModel model)
        {
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));

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
                catch (Exception ex)
                {
                    //error general
                    rp = autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
                    return rp;
                }
            }
        }

        public object GetUserEdit(UserModel model)
        {
            Response rp = new Response();
            try
            {
                cp = tvh.getprincipal(Convert.ToString(model.token));

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

                            rp.data = r;
                        }
                    }

                    return autil.MensajeRetorno(ref rp, 9, string.Empty, null, HttpStatusCode.OK);
                }

            }
            catch (Exception ex)
            {
                //error general
                return autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }

        public object GetUser(UserModel model)
        {
            Response rp = new Response();
            try
            {
                cp = tvh.getprincipal(Convert.ToString(model.token));

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
                    rp.data = rl;

                    //retorna un response, con el campo data lleno con la respuesta.               
                    return autil.MensajeRetorno(ref rp, 9, null, null, HttpStatusCode.OK);

                }

            }
            catch (Exception ex)
            {
                //error general
                return autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }


        #endregion

        #region Profile
        public object UserProfile(UserModel model)
        {
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));

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
                                         Id_Tipo_Profesional = us.Tipo_Profesional.Nombre,
                                         t.Registro_Profesional,
                                         t.Poblado.Municipio_Id,
                                         Departamento_Id = us.Poblado.Municipio.Departamento.Dane_Id,
                                         Estado = us.Entidad_Usuario.Where(tt => tt.Id_Usuario == us.Id_Usuario && tt.Id_Entidad == entidad).Select(tt => tt.Estado).SingleOrDefault()
                                     }).SingleOrDefault();

                            rp.data = r;
                        }
                    }

                    return autil.MensajeRetorno(ref rp, 9, string.Empty, null, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                //error general
                string tok = string.Empty;
                if(model != null)
                    tok = model.token;
                else
                tok = "vacio";

                return autil.MensajeRetorno(ref rp, 4, ex.Message + "--token: " + tok, null, HttpStatusCode.InternalServerError);
            }
        }

        public object EditProfile(UserModel model)
        {
            Response rp = new Response();
            try
            {
                cp = tvh.getprincipal(Convert.ToString(model.token));

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
            catch (Exception ex)
            {
                //error general
                return autil.MensajeRetorno(ref rp, 4, ex.Message + "--token: " + model.token, null, HttpStatusCode.InternalServerError);
            }
        }
        #endregion
    }
}