using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace MilenioApi.Action
{
    public class aPersona
    {
        TokenController tk = new TokenController();
        ClaimsPrincipal cp = new ClaimsPrincipal();
        aUtilities autil = new aUtilities();


        #region Seccion Con metodos de Personas

        /// <summary>
        /// Este metodo solo lo usan los SA para crear personas en otras entidadedes
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic CreatePersonaMaster(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                String file = string.Empty;
                if (httpRequest.Files.Count > 0)
                {
                    var foto = httpRequest.Files;
                    Byte[] Content = new BinaryReader(foto[0].InputStream).ReadBytes(foto[0].ContentLength);
                    file = Convert.ToBase64String(Content);
                }

                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    //PRIMERO SE VALIDA QUE ESA PERSONA CON LA CEDULA EMAIL O LOGIN NO EXISTA//
                    Persona p = new Persona();

                    int nroidentificacion = Convert.ToInt32(httpRequest.Form["NroIdentificacion"]);
                    string email = Convert.ToString(httpRequest.Form["Email"]);
                    string login = Convert.ToString(httpRequest.Form["Login"]);
                    string entidad = Convert.ToString(httpRequest.Form["Entidad_Id"]);
                    Guid entidad_id = Guid.Parse(entidad);

                    int vcedula = (from pr in ent.Persona
                                   from et in pr.Entidad_Persona_Rol
                                   where pr.NumeroIdentificacion == nroidentificacion
                                     && et.Entidad_Id == entidad_id
                                   select pr).Count();

                    int vcedulaotraentidad = (from pr in ent.Persona
                                              from et in pr.Entidad_Persona_Rol
                                              where pr.NumeroIdentificacion == nroidentificacion
                                                && et.Entidad_Id != entidad_id
                                              select pr).Count();

                    int vemail = (from pr in ent.Persona
                                  where pr.Email == email
                                  select pr).Count();

                    int vlogin = (from pr in ent.Persona
                                  where pr.Login == login
                                  select pr).Count();

                    if (vcedula == 0)
                    {
                        if (vemail == 0)
                        {
                            if (vlogin == 0)
                            {
                                if (vcedulaotraentidad == 0)
                                {
                                    //se saca el usiario que esta creando la persona del token
                                    Guid? user_id = null;
                                    string usid = Convert.ToString(httpRequest.Form["User_Id"]);
                                    if (!string.IsNullOrEmpty(usid))
                                        user_id = Guid.Parse(usid);


                                    //SE VALIDA EL GUID DE TIPO DE IDENTIFICACION//
                                    Guid ti = new Guid();
                                    if (!string.IsNullOrEmpty(httpRequest.Form["TipoIdentificacion"]))
                                        ti = Guid.Parse(httpRequest.Form["TipoIdentificacion"]);

                                    //SE LLENA EL OBJETO UBICACION PARA CREAR PRIMERO LA UBICACION DE LA PERSONA//
                                    Guid codigo_ubicacion = Guid.NewGuid();
                                    Ubicacion ub = new Ubicacion();
                                    ub.Codigo_Id = codigo_ubicacion;
                                    ub.Poblado_Id = Convert.ToInt32(httpRequest.Form["PobladoId"]);
                                    ub.Direccion = Convert.ToString(httpRequest.Form["Direccion"]);
                                    ub.Latitud = Convert.ToString(httpRequest.Form["Latitud"]);
                                    ub.Longitud = Convert.ToString(httpRequest.Form["Longitud"]);
                                    ub.Created_At = DateTime.Now;
                                    ub.Updated_At = DateTime.Now;
                                    ub.Usuario_Update = user_id;
                                    ent.Ubicacion.Add(ub);


                                    //SE LLENA EL OBJETO PERSONA//
                                    p = new Persona();
                                    p.Codigo_Id = Guid.NewGuid();
                                    p.NumeroIdentificacion = nroidentificacion;
                                    p.TipoIdentificacion_Id = ti;
                                    p.Nombres = Convert.ToString(httpRequest.Form["Nombres"]);
                                    p.Apellidos = Convert.ToString(httpRequest.Form["Apellidos"]);
                                    p.Sexo = Convert.ToString(httpRequest.Form["Sexo"]);
                                    p.FNacimiento = Convert.ToDateTime(httpRequest.Form["FechaNacimiento"]);
                                    p.Nacionalidad = Convert.ToString(httpRequest.Form["Nacionalidad"]);
                                    p.LibretaMilitar = Convert.ToString(httpRequest.Form["LibretaMilitar"]);
                                    p.TipoSangre = Convert.ToString(httpRequest.Form["TipoSangre"]);
                                    p.Ubicacion_Id = codigo_ubicacion;
                                    p.Foto = file;
                                    p.Estado_Persona = true;
                                    //datos para el usuario//
                                    p.Login = login;
                                    p.Password = autil.Sha(Convert.ToString(httpRequest.Form["Password"]));
                                    p.Email = email;
                                    p.Estado_Usuario = true;
                                    p.Cambiar_Clave = true;
                                    p.Created_At = DateTime.Now;
                                    p.Updated_At = DateTime.Now;
                                    p.Usuario_Update = user_id;
                                    ent.Persona.Add(p);

                                    //SE ASIGNAN LOS ROLES EN LA ENTIDAD
                                    IList<string> _Roles = httpRequest.Form["Roles"].Split(',').Reverse().ToList<string>();

                                    foreach (var i in _Roles)
                                    {
                                        Entidad_Persona_Rol ur = new Entidad_Persona_Rol();
                                        ur.Persona_Id = p.Codigo_Id;
                                        ur.Rol_Id = Guid.Parse(i);
                                        ur.Entidad_Id = entidad_id;
                                        ur.Created_At = DateTime.Now;
                                        ur.Updated_At = DateTime.Now;
                                        ur.Usuario_Update = user_id;
                                        ur.Estado = true;
                                        ent.Entidad_Persona_Rol.Add(ur);
                                    }

                                    //se envia a crear todo
                                    ent.SaveChanges();
                                    //se genera el codigo del mensaje de retorno exitoso
                                    ret = autil.MensajeRetorno(ref ret, 2, string.Empty, p.Codigo_Id);

                                }
                                else
                                {
                                    //USUARIO EXISTE EN OTRA ENTIDAD EXISTE
                                    ret = autil.MensajeRetorno(ref ret, 10, string.Empty, null);
                                }
                            }
                            else
                            {
                                //LOGIN EXISTE
                                ret = autil.MensajeRetorno(ref ret, 7, string.Empty, null);
                            }
                        }
                        else
                        {
                            //EMAIL EXISTE
                            ret = autil.MensajeRetorno(ref ret, 6, string.Empty, null);
                        }
                    }
                    else
                    {
                        //CEDULA EXISTE
                        ret = autil.MensajeRetorno(ref ret, 5, string.Empty, null);
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
        /// Este medodo lo usan las personas de una entdad para crear una persona en est entidad en donde esta conectado
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic CreatePersona(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                if (cp != null)
                {
                    String file = string.Empty;
                    if (httpRequest.Files.Count > 0)
                    {
                        var foto = httpRequest.Files;
                        Byte[] Content = new BinaryReader(foto[0].InputStream).ReadBytes(foto[0].ContentLength);
                        file = Convert.ToBase64String(Content);
                    }

                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Persona p = new Persona();

                        int nroidentificacion = Convert.ToInt32(httpRequest.Form["NroIdentificacion"]);
                        string email = Convert.ToString(httpRequest.Form["Email"]);
                        string login = Convert.ToString(httpRequest.Form["Login"]);

                        //se saca el id de la entidad del token
                        string entidad = cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault();
                        Guid entidad_id = Guid.Parse(entidad);

                        int vcedula = (from pr in ent.Persona
                                       from et in pr.Entidad_Persona_Rol
                                       where pr.NumeroIdentificacion == nroidentificacion
                                         && et.Entidad_Id == entidad_id
                                       select pr).Count();

                        int vcedulaotraentidad = (from pr in ent.Persona
                                                  from et in pr.Entidad_Persona_Rol
                                                  where pr.NumeroIdentificacion == nroidentificacion
                                                    && et.Entidad_Id != entidad_id
                                                  select pr).Count();

                        int vemail = (from pr in ent.Persona
                                      where pr.Email == email
                                      select pr).Count();

                        int vlogin = (from pr in ent.Persona
                                      where pr.Login == login
                                      select pr).Count();

                        if (vcedula == 0)
                        {
                            if (vemail == 0)
                            {
                                if (vlogin == 0)
                                {
                                    if (vcedulaotraentidad == 0)
                                    {
                                        //se saca el usiario que esta creando la persona del token
                                        Guid? user_id = null;
                                        string usid = cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                                        if (!string.IsNullOrEmpty(usid))
                                            user_id = Guid.Parse(usid);

                                        //SE VALIDA EL GUID DE TIPO DE IDENTIFICACION//
                                        Guid ti = new Guid();
                                        if (!string.IsNullOrEmpty(httpRequest.Form["TipoIdentificacion"]))
                                            ti = Guid.Parse(httpRequest.Form["TipoIdentificacion"]);

                                        //SE LLENA EL OBJETO UBICACION PARA CREAR PRIMERO LA UBICACION DE LA PERSONA//
                                        Guid codigo_ubicacion = Guid.NewGuid();
                                        Ubicacion ub = new Ubicacion();
                                        ub.Codigo_Id = codigo_ubicacion;
                                        ub.Poblado_Id = Convert.ToInt32(httpRequest.Form["PobladoId"]);
                                        ub.Direccion = Convert.ToString(httpRequest.Form["Direccion"]);
                                        ub.Latitud = Convert.ToString(httpRequest.Form["Latitud"]);
                                        ub.Longitud = Convert.ToString(httpRequest.Form["Longitud"]);
                                        ub.Created_At = DateTime.Now;
                                        ub.Updated_At = DateTime.Now;
                                        ub.Usuario_Update = user_id;
                                        ent.Ubicacion.Add(ub);


                                        //SE LLENA EL OBJETO PERSONA//
                                        p = new Persona();
                                        p.Codigo_Id = Guid.NewGuid();
                                        p.NumeroIdentificacion = nroidentificacion;
                                        p.TipoIdentificacion_Id = ti;
                                        p.Nombres = Convert.ToString(httpRequest.Form["Nombres"]);
                                        p.Apellidos = Convert.ToString(httpRequest.Form["Apellidos"]);
                                        p.Sexo = Convert.ToString(httpRequest.Form["Sexo"]);
                                        p.FNacimiento = Convert.ToDateTime(httpRequest.Form["FechaNacimiento"]);
                                        p.Nacionalidad = Convert.ToString(httpRequest.Form["Nacionalidad"]);
                                        p.LibretaMilitar = Convert.ToString(httpRequest.Form["LibretaMilitar"]);
                                        p.TipoSangre = Convert.ToString(httpRequest.Form["TipoSangre"]);
                                        p.Ubicacion_Id = codigo_ubicacion;
                                        p.Foto = file;
                                        p.Estado_Persona = true;
                                        //datos para el usuario//
                                        p.Login = login;
                                        p.Password = autil.Sha(Convert.ToString(httpRequest.Form["Password"]));
                                        p.Email = email;
                                        p.Estado_Usuario = true;
                                        p.Cambiar_Clave = true;
                                        p.Created_At = DateTime.Now;
                                        p.Updated_At = DateTime.Now;
                                        p.Usuario_Update = user_id;
                                        ent.Persona.Add(p);

                                        //SE ASIGNAN LOS ROLES EN LA ENTIDAD
                                        IList<string> _Roles = httpRequest.Form["Roles"].Split(',').Reverse().ToList<string>();

                                        foreach (var i in _Roles)
                                        {
                                            Entidad_Persona_Rol ur = new Entidad_Persona_Rol();
                                            ur.Persona_Id = p.Codigo_Id;
                                            ur.Rol_Id = Guid.Parse(i);
                                            ur.Entidad_Id = entidad_id;
                                            ur.Created_At = DateTime.Now;
                                            ur.Updated_At = DateTime.Now;
                                            ur.Usuario_Update = user_id;
                                            ur.Estado = true;
                                            ent.Entidad_Persona_Rol.Add(ur);
                                        }

                                        //se envia a crear todo
                                        ent.SaveChanges();
                                        //se genera el codigo del mensaje de retorno exitoso
                                        ret = autil.MensajeRetorno(ref ret, 2, string.Empty, p.Codigo_Id);

                                    }
                                    else
                                    {
                                        //USUARIO EXISTE EN OTRA ENTIDAD EXISTE
                                        ret = autil.MensajeRetorno(ref ret, 10, string.Empty, null);
                                    }
                                }
                                else
                                {
                                    //LOGIN EXISTE
                                    ret = autil.MensajeRetorno(ref ret, 7, string.Empty, null);
                                }
                            }
                            else
                            {
                                //EMAIL EXISTE
                                ret = autil.MensajeRetorno(ref ret, 6, string.Empty, null);
                            }
                        }
                        else
                        {
                            //CEDULA EXISTE
                            ret = autil.MensajeRetorno(ref ret, 5, string.Empty, null);
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
                //error general
                ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                return ret;
            }
        }
        /// <summary>
        /// ESTE METODO AGREGA UNA PESONA A UNA ENTIDAD EN ESPECIFICO
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic AgregaPersonaEntidad(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                int error = 0;
                string roles_asignados = string.Empty;

                if (cp != null)
                {
                    //se obtienen la persona que se asignara y la entidad a donde se asignara
                    Guid entidad = Guid.Parse(Convert.ToString(httpRequest.Form["Entidad_Id"]));
                    Guid persona = Guid.Parse(Convert.ToString(httpRequest.Form["Persona_Id"]));

                    //SE OBTIENEN LOS ROLES QUE TENDRA ESA PERSONA EN ESA ENTIDAD
                    IList<string> _Roles = httpRequest.Form["Roles"].Split(',').Reverse().ToList<string>();

                    //se saca el usiario que esta creando la persona del token
                    Guid? user_id = null;
                    string usid = cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(usid))
                        user_id = Guid.Parse(usid);

                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        foreach (var i in _Roles)
                        {
                            Entidad_Persona_Rol ur = new Entidad_Persona_Rol();
                            Guid rol_id = Guid.Parse(i);


                            //aqui  se valida que esa persona no tenga asignados esos roles que se le intentan 
                            ///asignar
                            ur = (from y in ent.Entidad_Persona_Rol
                                  where y.Entidad_Id == entidad
                                  && y.Rol_Id == rol_id
                                  select y).SingleOrDefault();

                            if (ur == null)
                            {
                                ur.Persona_Id = persona;
                                ur.Rol_Id = rol_id;
                                ur.Entidad_Id = entidad;
                                ur.Created_At = DateTime.Now;
                                ur.Updated_At = DateTime.Now;
                                ur.Usuario_Update = user_id;
                                ur.Estado = true;
                                ent.Entidad_Persona_Rol.Add(ur);
                            }
                            else
                            {
                                error = 1;
                                roles_asignados = roles_asignados + " - " + ur.Rol.Nombre;
                            }
                        }

                        if (error == 0)
                        {
                            ent.SaveChanges();
                            //se genera el codigo del mensaje de retorno exitoso
                            ret = autil.MensajeRetorno(ref ret, 11, string.Empty, null);
                        }
                        else
                        {
                            ret = autil.MensajeRetorno(ref ret, 12, string.Empty, null);
                            ret.Message = string.Format(ret.Message, roles_asignados);
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
                //error general
                ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                return ret;
            }
        }

        /* ******************EDIT****************/
        /// <summary>
        /// Permite la edicion de los datos basicos de la persona
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic EditPersona(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                if (cp != null)
                {
                    String file = string.Empty;
                    if (httpRequest.Files.Count > 0)
                    {
                        var foto = httpRequest.Files;
                        Byte[] Content = new BinaryReader(foto[0].InputStream).ReadBytes(foto[0].ContentLength);
                        file = Convert.ToBase64String(Content);
                    }

                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Persona p = new Persona();
                        ///primero se tiene el id de la persona
                        Guid Persona_Id = Guid.Parse(httpRequest.Form["Persona_Id"]);

                        //se consulta la persona
                        p = (from pr in ent.Persona
                             where pr.Codigo_Id == Persona_Id
                             select pr).SingleOrDefault();

                        if (p != null)
                        {
                            //se saca el id de la entidad del token
                            string entidad = cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault();
                            Guid entidad_id = Guid.Parse(entidad);

                            //se saca el usuario que esta editando
                            Guid? user_id = null;
                            string usid = cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                            if (!string.IsNullOrEmpty(usid))
                                user_id = Guid.Parse(usid);

                            //SE LLENA EL OBJETO PERSONA//

                            p.Nombres = Convert.ToString(httpRequest.Form["Nombres"]);
                            p.Apellidos = Convert.ToString(httpRequest.Form["Apellidos"]);
                            p.Sexo = Convert.ToString(httpRequest.Form["Sexo"]);
                            p.FNacimiento = Convert.ToDateTime(httpRequest.Form["FechaNacimiento"]);
                            p.Nacionalidad = Convert.ToString(httpRequest.Form["Nacionalidad"]);
                            p.LibretaMilitar = Convert.ToString(httpRequest.Form["LibretaMilitar"]);
                            p.TipoSangre = Convert.ToString(httpRequest.Form["TipoSangre"]);
                            p.Foto = file;
                            p.Estado_Persona = true;
                            p.Estado_Usuario = true;
                            p.Updated_At = DateTime.Now;
                            p.Usuario_Update = user_id;

                            //se envia a editar todo
                            ent.SaveChanges();
                            //se genera el codigo del mensaje de retorno exitoso
                            ret = autil.MensajeRetorno(ref ret, 20, string.Empty, p.Codigo_Id);
                        }
                        else
                        {
                            //se genera el codigo del mensaje de retorno datos no coinciden
                            ret = autil.MensajeRetorno(ref ret, 15, string.Empty, p.Codigo_Id);
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
                //error general
                ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                return ret;
            }
        }
        /// <summary>
        /// permite la edicion de la ubicacion de una persona o de una entidad
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic EditaUbicacion(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Persona p = new Persona();

                        Guid Persona_Id = Guid.Parse(httpRequest.Form["Persona_Id"]);
                        Guid Ubicacion_Id = Guid.Parse(httpRequest.Form["Ubicacion_Id"]);


                        //se saca el id de la entidad del token
                        string entidad = cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault();
                        Guid entidad_id = Guid.Parse(entidad);

                        //se saca el usiario que esta creando la persona del token
                        Guid? user_id = null;
                        string usid = cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                        if (!string.IsNullOrEmpty(usid))
                            user_id = Guid.Parse(usid);


                        //SE LLENA EL OBJETO UBICACION PARA EDITAR LA UBICACION DE LA PERSONA//

                        Ubicacion ub = new Ubicacion();

                        ub = (from b in ent.Ubicacion
                              where b.Codigo_Id == Ubicacion_Id
                              select b).SingleOrDefault();
                        if (ub != null)
                        {
                            ub.Poblado_Id = Convert.ToInt32(httpRequest.Form["PobladoId"]);
                            ub.Direccion = Convert.ToString(httpRequest.Form["Direccion"]);
                            ub.Latitud = Convert.ToString(httpRequest.Form["Latitud"]);
                            ub.Longitud = Convert.ToString(httpRequest.Form["Longitud"]);
                            ub.Created_At = DateTime.Now;
                            ub.Updated_At = DateTime.Now;
                            ub.Usuario_Update = user_id;

                            //se envia a actualizar todo
                            ent.SaveChanges();
                            //se genera el codigo del mensaje de retorno exitoso
                            ret = autil.MensajeRetorno(ref ret, 20, string.Empty, p.Codigo_Id);
                        }
                        else
                        {
                            ret = autil.MensajeRetorno(ref ret, 15, string.Empty, p.Codigo_Id);
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
                //error general
                ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                return ret;
            }
        }

        /*************************************/


        #endregion

        #region Seccion con metodos de roles

        /// <summary>
        /// Este metodo permite inactivar un rol para una persona,
        /// no se puede eliminarle un rol, solo desactivarlo
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic ActInactivaRolPersona(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                if (cp != null)
                {
                    //se obtienen la persona que se asignara y la entidad a donde se asignara
                    Guid entidad = Guid.Parse(Convert.ToString(httpRequest.Form["Entidad_Id"]));
                    Guid persona = Guid.Parse(Convert.ToString(httpRequest.Form["Persona_Id"]));
                    IList<string> _Roles = httpRequest.Form["Roles"].Split(',').Reverse().ToList<string>();
                    bool estado = bool.Parse(httpRequest.Form["Estado"]);

                    //se obtiene el usuario que esta modificando
                    Guid? user_id = null;
                    string usid = cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(usid))
                        user_id = Guid.Parse(usid);

                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        //SE ASIGNAN LOS ROLES EN LA ENTIDAD
                        foreach (var i in _Roles)
                        {
                            Guid rolid = Guid.Parse(i);
                            Entidad_Persona_Rol epr = (from e in ent.Entidad_Persona_Rol
                                                       where e.Entidad_Id == entidad
                                                       && e.Rol_Id == rolid
                                                       && e.Persona_Id == persona
                                                       select e).SingleOrDefault();
                            if (epr != null)
                            {
                                epr.Estado = estado;
                                epr.Updated_At = DateTime.Now;
                                epr.Usuario_Update = user_id;
                                ent.SaveChanges();
                            }
                            else
                            {
                                //Datos No Coinciden
                                ret = autil.MensajeRetorno(ref ret, 15, string.Empty, null);
                                break;
                            }

                        }
                    }

                    if (!estado)
                    {
                        //rol desactivado exitoso
                        ret = autil.MensajeRetorno(ref ret, 14, string.Empty, null);
                    }
                    else
                    {
                        //rol activado exitoso
                        ret = autil.MensajeRetorno(ref ret, 17, string.Empty, null);
                    }
                }
                else
                {
                    //token invalido
                    ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
                    return ret;
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
        /// Este metodo permite inactivar una persona
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic ActInactivaPersona(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                if (cp != null)
                {
                    //se obtienen la persona que se asignara y la entidad a donde se asignara
                    Guid entidad = Guid.Parse(Convert.ToString(httpRequest.Form["Entidad_Id"]));
                    Guid persona = Guid.Parse(Convert.ToString(httpRequest.Form["Persona_Id"]));
                    bool estado = bool.Parse(httpRequest.Form["Estado"]);

                    //se obtiene el usuario que esta modificando
                    Guid? user_id = null;
                    string usid = cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(usid))
                        user_id = Guid.Parse(usid);

                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        List<Entidad_Persona_Rol> lepr = (from e in ent.Entidad_Persona_Rol
                                                          where e.Entidad_Id == entidad
                                                          && e.Persona_Id == persona
                                                          select e).ToList();
                        if (lepr.Count != 0)
                        {
                            foreach (var epr in lepr)
                            {
                                epr.Estado = estado;
                                epr.Updated_At = DateTime.Now;
                                epr.Usuario_Update = user_id;
                            }
                            ent.SaveChanges();

                            if (!estado)
                            {
                                //persona desactivado exitoso
                                ret = autil.MensajeRetorno(ref ret, 14, string.Empty, null);
                            }
                            else
                            {
                                //persona activado exitoso
                                ret = autil.MensajeRetorno(ref ret, 18, string.Empty, null);
                            }
                        }
                        else
                        {
                            //Datos invalidos
                            ret = autil.MensajeRetorno(ref ret, 15, string.Empty, null);
                        }
                    }
                }
                else
                {
                    //token invalido
                    ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
                    return ret;
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
        /// Este metodo trae todos los roles activos que existen
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public List<Basic> GetRoles(HttpRequest httpRequest)
        {
            List<Basic> ret = new List<Basic>();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        ret = (from r in ent.Rol
                               where r.Estado == true
                               select new Basic
                               {
                                   id = r.Codigo_Id,
                                   Value = r.Nombre
                               }).ToList();

                        return ret;
                    }
                }
                return ret;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Este metodo trae todos los roles que tiene una persona
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public List<Basic> GetRolesPersonaEntidad(HttpRequest httpRequest)
        {
            List<Basic> ret = new List<Basic>();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));

                Guid Persona_Id = Guid.Parse(httpRequest.Form["Persona_Id"]);
                //se saca la entidad del token
                Guid Entidad_Id = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());

                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        ret = (from r in ent.Entidad_Persona_Rol
                               where r.Persona_Id == Persona_Id
                               && r.Entidad_Id == Entidad_Id
                               where r.Estado == true
                               select new Basic
                               {
                                   id = r.Rol.Codigo_Id,
                                   Value = r.Rol.Nombre
                               }).ToList();

                        return ret;
                    }
                }
                return ret;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion
    }
}