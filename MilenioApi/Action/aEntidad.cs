using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;

namespace MilenioApi.Action
{
    public class aEntidad
    {
        TokenValidationHandler tvh = new TokenValidationHandler();
        ClaimsPrincipal cp = new ClaimsPrincipal();
        aUtilities autil = new aUtilities();

        #region Especialidad

        #endregion

        #region Seccion de entidad

        /// <summary>
        /// Metodo permite la creacion de entidaddes
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object CreateEntidad(EntidadModel model)
        {
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));
                    if (cp != null)
                    {
                        Response b = new Response();
                        List<ErrorFields> rel = autil.ValidateObject(model);
                        if (rel.Count == 0)
                        {
                            Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                            //validamos que ese nit ya exista
                            int vNit = ent.Entidad.Where(t => t.Nit == model.Nit).Count();
                            if (vNit == 0)
                            {
                                //AQUI SE TOMA EL OBJETO ENVIADO DESDE EL FRONT
                                //Y SE COPIA AL OBJETO USER
                                Entidad et = new Entidad();
                                Copier.CopyPropertiesTo(model, et);
                                //                               
                                Guid identidad = Guid.NewGuid();
                                et.Id_Entidad = identidad;
                                et.Fecha_Create = DateTime.Now;
                                et.Fecha_Update = DateTime.Now;
                                ent.Entidad.Add(et);
                                //se envia a crear todo

                                //se consulta en el webconfig el usuario SA y el rol SA
                                Guid sauser = Guid.Parse(ConfigurationManager.AppSettings["idsuperuser"]);
                                Guid sarol = Guid.Parse(ConfigurationManager.AppSettings["idrolsuperuser"]);

                                //se genera un usuario para enviar por email a la entidad
                                Guid idusuario = Guid.NewGuid();
                                Usuario um = new Usuario();
                                um.Id_Usuario = idusuario;
                                um.Nombres = et.Nombre.Replace(" ", "");
                                um.Primer_Apellido = et.Nit.ToString().Replace(".", "").Replace(" ", "");
                                um.Segundo_Apellido = et.Organizacion;
                                um.Id_Tipo_Identificacion = "NIT";
                                um.Numero_Identificacion = et.Nit.ToString().Replace(".", "").Replace(" ", "");
                                um.Fecha_Contratacion = DateTime.Now;
                                um.Tipo_Vinculacion = "NA";
                                um.Presta_Servicio = false;
                                um.Login = et.Nombre.Replace(" ", "");
                                string password = autil.CreatePassword(8);
                                um.Password = autil.Sha(password);
                                um.Email = et.Email;
                                um.Acepta_ABEAS = true;
                                ent.Usuario.Add(um);


                                //se agrega el usuario que se creo a la entidad
                                Entidad_Usuario eu = new Entidad_Usuario();
                                eu.Id_Entidad = identidad;
                                eu.Id_Usuario = idusuario;
                                eu.Estado = true;
                                eu.Usuario_Create = usuario;
                                eu.Usuario_Update = usuario;
                                eu.Fecha_Create = DateTime.Now;
                                eu.Fecha_Update = DateTime.Now;
                                ent.Entidad_Usuario.Add(eu);

                                ///se agrega el rol SA a ese usuario para esa entidad
                                Rol_Usuario ru = new Rol_Usuario();
                                ru.Id_Rol = sarol;
                                ru.Id_Usuario = idusuario;
                                ru.Id_Entidad = identidad;
                                ru.Estado = true;
                                ru.Usuario_Create = usuario;
                                ru.Usuario_Update = usuario;
                                ru.Fecha_Create = DateTime.Now;
                                ru.Fecha_Update = DateTime.Now;
                                ent.Rol_Usuario.Add(ru);

                                //se guarda todo
                                ent.SaveChanges();
                                //se genera el codigo del mensaje de retorno exitoso
                                rp = autil.MensajeRetorno(ref rp, 2, string.Empty, null, HttpStatusCode.OK);
                            }
                            else
                            {
                                //Nit EXISTE
                                rp = autil.MensajeRetorno(ref rp, 3, string.Empty, null, HttpStatusCode.OK);
                            }
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
        /// Metodo para editar entidades
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object EditEntidad(EntidadModel model)
        {
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    cp = tvh.getprincipal(model.token);
                    if (cp != null)
                    {
                        List<ErrorFields> rel = autil.ValidateObject(model);
                        if (rel.Count == 0)
                        {
                            Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                            //validamos que ese nit ya exista
                            int vNit = ent.Entidad.Where(t => t.Nit == model.Nit && t.Id_Entidad != model.Id_Entidad).Count();
                            if (vNit == 0)
                            {
                                Entidad et = ent.Entidad.Where(en => en.Id_Entidad == model.Id_Entidad).SingleOrDefault();
                                et.Nit = model.Nit;
                                et.Nombre = model.Nombre;
                                et.Organizacion = model.Organizacion;
                                et.Poblado_Id = model.Poblado_Id;
                                et.Direccion = model.Direccion;
                                et.Email = model.Email;
                                et.CodigoEntidad = model.CodigoEntidad;
                                et.Atencion_Prioritaria = model.Atencion_Prioritaria;
                                et.Contribuyente = model.Contribuyente;
                                et.Hora_Desde = model.Hora_Desde;
                                et.Hora_Hasta = model.Hora_Hasta;
                                et.Fecha_Update = DateTime.Now;
                                et.Foto = model.Foto;

                                //se envia a editar todo
                                ent.SaveChanges();
                                //se genera el codigo del mensaje de retorno exitoso
                                rp = autil.MensajeRetorno(ref rp, 20, string.Empty, null, HttpStatusCode.OK);
                            }
                            else
                            {
                                //Nit EXISTE
                                rp = autil.MensajeRetorno(ref rp, 3, string.Empty, null, HttpStatusCode.OK);
                            }
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
                return rp;
            }
            catch (Exception ex)
            {
                //error general
                rp = autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                return rp;
            }
        }

        public object GetEntidades(EntidadModel model)
        {
            List<EntidadModel> rl = new List<EntidadModel>();
            Response rp = new Response();
            try
            {
                cp = tvh.getprincipal(model.token);
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        IQueryable<Entidad> et = ent.Entidad;
                        //consulta por nit
                        if (model.Nit != 0)
                        {
                            et = et.Where(c => c.Nit == model.Nit);
                        }

                        //consulta por nombre
                        if (!string.IsNullOrEmpty(model.Nombre))
                        {
                            et = et.Where(c => c.Nombre.Contains(model.Nombre));
                        }

                        //consulta por organizacion
                        if (!string.IsNullOrEmpty(model.Organizacion))
                        {
                            et = et.Where(c => c.Organizacion.Contains(model.Organizacion));
                        }

                        //consulta por cofigo entidad
                        if (!string.IsNullOrEmpty(model.CodigoEntidad))
                        {
                            et = et.Where(c => c.CodigoEntidad.Contains(model.CodigoEntidad));
                        }

                        //consulta por cofigo email
                        if (!string.IsNullOrEmpty(model.Email))
                        {
                            et = et.Where(c => c.Email.Contains(model.Email));
                        }

                        rl = et.Select(u => new EntidadModel
                        {
                            Id_Entidad = u.Id_Entidad,
                            Nit = u.Nit,
                            Nombre = u.Nombre,
                            Organizacion = u.Organizacion,
                            Email = u.Email,
                            CodigoEntidad = u.CodigoEntidad

                        }).ToList();//.Take(pageSize).Skip(startingPageIndex * pageSize)
                        
                        rp.data = rl;
                        rp.cantidad = rl.Count();
                    }
                    //retorna un response, con el campo data lleno con la respuesta.               
                    return autil.MensajeRetorno(ref rp, 9, null, null, HttpStatusCode.OK);
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

        public object GetEntidadesEdit(EntidadModel model)
        {
            List<EntidadModel> rl = new List<EntidadModel>();
            Response rp = new Response();
            try
            {
                cp = tvh.getprincipal(model.token);
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        IQueryable<Entidad> et = ent.Entidad;

                        //consulta por cofigo entidad
                        if (model.Id_Entidad != Guid.Empty)
                        {
                            et = et.Where(c => c.Id_Entidad == model.Id_Entidad);
                        }

                        foreach (var i in et.ToList())
                        {
                            EntidadModel em = new EntidadModel();
                            em.Id_Municipio = i.Poblado.Municipio_Id;
                            em.Id_Departamento = i.Poblado.Municipio.Departamento_Id;
                            Copier.CopyPropertiesTo(i, em);
                            rl.Add(em);
                        }

                        return rl;
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
                rp = autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                return rp;
            }
        }
        #endregion

        #region Seccion Relacionadas

        public object CreateEspecialidadEntidad(EntidadModel model)
        {
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(model.token);
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Especialidad_Entidad ee = ent.Especialidad_Entidad.Where(t => t.Id_Entidad == entidad && t.Id_Especialidad == model.Id_Especialidad).SingleOrDefault();

                        if (ee == null)
                        {
                            ee = new Especialidad_Entidad();
                            ee.Id_Entidad = entidad;
                            ee.Id_Especialidad = model.Id_Especialidad;
                            ee.Estado = true;
                            ee.Usuario_Create = usuario;
                            ee.Usuario_Update = usuario;
                            ee.Fecha_Create = DateTime.Now;
                            ee.Fecha_Update = DateTime.Now;
                            ent.Especialidad_Entidad.Add(ee);
                            ent.SaveChanges();

                            //se genera el codigo del mensaje de retorno exitoso
                            return rp = autil.MensajeRetorno(ref rp, 2, string.Empty, null, HttpStatusCode.OK);
                        }
                        else
                        {
                            //especialidad ya fue agregada
                            return rp = autil.MensajeRetorno(ref rp, 27, string.Empty, null, HttpStatusCode.OK);
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
                    rp = autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                    return rp;
                }
            }
        }

        public object EditEspecialidadEntidad(EntidadModel model)
        {
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(model.token);
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Especialidad_Entidad ee = ent.Especialidad_Entidad.Where(t => t.Id_Entidad == entidad && t.Id_Especialidad == model.Id_Especialidad).SingleOrDefault();

                        if (ee != null)
                        {
                            ee.Estado = model.Estado;
                            ee.Usuario_Update = usuario;
                            ee.Fecha_Update = DateTime.Now;
                            ent.SaveChanges();

                            //se genera el codigo del mensaje de retorno exitoso
                            return rp = autil.MensajeRetorno(ref rp, 20, string.Empty, null, HttpStatusCode.OK);
                        }
                        else
                        {
                            //especialidad ya fue agregada
                            return rp = autil.MensajeRetorno(ref rp, 28, string.Empty, null, HttpStatusCode.OK);
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
                    rp = autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                    return rp;
                }
            }
        }

        #endregion
    }
}