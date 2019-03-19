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
        TokenController tk = new TokenController();
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
                    cp = tk.ValidateToken(Convert.ToString(model.token));
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
                rp = autil.MensajeRetorno(ref rp, 4, ex.Message, null,HttpStatusCode.InternalServerError);
                return rp;
            }
        }

        /// <summary>
        /// Metodo para editar entidades
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic EditEntidad(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                    if (cp != null)
                    {
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        int Nit = Convert.ToInt32(httpRequest.Form["Nit"]);
                        Guid identidad = Guid.Parse(httpRequest.Form["identidad"]);

                        //validamos que ese nit ya exista
                        int vNit = ent.Entidad.Where(t => t.Nit == Nit && t.Id_Entidad != identidad).Count();
                        if (vNit == 0)
                        {
                            string Nombre = Convert.ToString(httpRequest.Form["nombre"]);
                            string organizacion = Convert.ToString(httpRequest.Form["organizacion"]);
                            int id_Poblado = Convert.ToInt32(httpRequest.Form["idpoblado"]);
                            string direccion = Convert.ToString(httpRequest.Form["direccion"]);
                            string email = Convert.ToString(httpRequest.Form["email"]);
                            string codentidad = Convert.ToString(httpRequest.Form["codentidad"]);
                            bool atencionprioritaria = Convert.ToBoolean(int.Parse(httpRequest.Form["atencionprioritaria"]));
                            bool contribuyente = Convert.ToBoolean(int.Parse(httpRequest.Form["contribuyente"]));
                            string horadesde = Convert.ToString(httpRequest.Form["horadesde"]);
                            string horahasta = Convert.ToString(httpRequest.Form["horahasta"]);

                            Entidad et = ent.Entidad.Where(e => e.Id_Entidad == identidad).SingleOrDefault();
                            et.Nit = Nit;
                            et.Nombre = Nombre;
                            et.Organizacion = organizacion;
                            et.Poblado_Id = id_Poblado;
                            et.Direccion = direccion;
                            et.Email = email;
                            et.CodigoEntidad = codentidad;
                            et.Atencion_Prioritaria = atencionprioritaria;
                            et.Contribuyente = contribuyente;
                            et.Hora_Desde = horadesde;
                            et.Hora_Hasta = horahasta;
                            et.Fecha_Update = DateTime.Now;

                            String file = string.Empty;
                            if (httpRequest.Files.Count > 0)
                            {
                                var foto = httpRequest.Files;
                                Byte[] Content = new BinaryReader(foto[0].InputStream).ReadBytes(foto[0].ContentLength);
                                file = Convert.ToBase64String(Content);
                                et.Foto = file;
                            }

                            //se envia a editar todo
                            ent.SaveChanges();
                            //se genera el codigo del mensaje de retorno exitoso
                            ret = autil.MensajeRetorno(ref ret, 20, string.Empty, null);
                        }
                        else
                        {
                            //Nit EXISTE
                            ret = autil.MensajeRetorno(ref ret, 3, string.Empty, null);
                        }
                    }
                    else
                    {
                        //token invalido
                        ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
                        return ret;
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

        public List<EntidadModel> GetEntidades(HttpRequest httpRequest)
        {
            List<EntidadModel> ret = new List<EntidadModel>();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        IQueryable<Entidad> et = ent.Entidad;
                        //consulta por nit
                        if (!string.IsNullOrEmpty(httpRequest.Form["Nit"]))
                        {
                            int nit = Convert.ToInt32(httpRequest.Form["Nit"]);
                            et = et.Where(c => c.Nit == nit);
                        }

                        //consulta por nombre
                        if (!string.IsNullOrEmpty(httpRequest.Form["nombre"]))
                        {
                            string nombre = Convert.ToString(httpRequest.Form["nombre"]);
                            et = et.Where(c => c.Nombre.Contains(nombre));
                        }

                        //consulta por organizacion
                        if (!string.IsNullOrEmpty(httpRequest.Form["organizacion"]))
                        {
                            string organizacion = Convert.ToString(httpRequest.Form["organizacion"]);
                            et = et.Where(c => c.Organizacion.Contains(organizacion));
                        }

                        //consulta por cofigo entidad
                        if (!string.IsNullOrEmpty(httpRequest.Form["codigoentidad"]))
                        {
                            string codigoentidad = Convert.ToString(httpRequest.Form["codigoentidad"]);
                            et = et.Where(c => c.CodigoEntidad.Contains(codigoentidad));
                        }

                        //consulta por cofigo email
                        if (!string.IsNullOrEmpty(httpRequest.Form["email"]))
                        {
                            string email = Convert.ToString(httpRequest.Form["email"]);
                            et = et.Where(c => c.Email.Contains(email));
                        }

                        ret = et.Select(u => new EntidadModel
                        {
                            Id_Entidad = u.Id_Entidad,
                            Nit = u.Nit,
                            Nombre = u.Nombre,
                            Organizacion = u.Organizacion,
                            Email = u.Email,
                            CodigoEntidad = u.CodigoEntidad

                        }).ToList();//.Take(pageSize).Skip(startingPageIndex * pageSize)


                        return ret;
                    }
                }
                else
                {
                    //TOKEN INVALIDO
                    EntidadModel u = new EntidadModel();
                    Basic rep = new Basic();
                    //token invalido
                    rep = autil.MensajeRetorno(ref rep, 1, string.Empty, null);
                    u.Response_Code = rep.response_code;
                    u.Message = rep.message;
                    ret.Add(u);
                    return ret;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EntidadModel> GetEntidadesEdit(HttpRequest httpRequest)
        {
            List<EntidadModel> ret = new List<EntidadModel>();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        IQueryable<Entidad> et = ent.Entidad;

                        //consulta por cofigo entidad
                        if (!string.IsNullOrEmpty(httpRequest.Form["identidad"]))
                        {
                            Guid codigoentidad = Guid.Parse(httpRequest.Form["identidad"]);
                            et = et.Where(c => c.Id_Entidad == codigoentidad);
                        }

                        foreach (var i in et.ToList())
                        {
                            EntidadModel em = new EntidadModel();
                            em.Id_Municipio = i.Poblado.Municipio_Id;
                            em.Id_Departamento = i.Poblado.Municipio.Departamento_Id;
                            Copier.CopyPropertiesTo(i, em);
                            ret.Add(em);
                        }

                        return ret;
                    }
                }
                else
                {
                    //TOKEN INVALIDO
                    EntidadModel u = new EntidadModel();
                    Basic rep = new Basic();
                    //token invalido
                    rep = autil.MensajeRetorno(ref rep, 1, string.Empty, null);
                    u.Response_Code = rep.response_code;
                    u.Message = rep.message;
                    ret.Add(u);
                    return ret;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Seccion Relacionadas

        public Basic CreateEspecialidadEntidad(HttpRequest httpRequest)
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

                        Guid id_especialidad = Guid.Parse(httpRequest.Form["idespecialidad"]);

                        Especialidad_Entidad ee = ent.Especialidad_Entidad.Where(t => t.Id_Entidad == entidad && t.Id_Especialidad == id_especialidad).SingleOrDefault();

                        if (ee == null)
                        {
                            ee = new Especialidad_Entidad();
                            ee.Id_Entidad = entidad;
                            ee.Id_Especialidad = id_especialidad;
                            ee.Estado = true;
                            ee.Usuario_Create = usuario;
                            ee.Usuario_Update = usuario;
                            ee.Fecha_Create = DateTime.Now;
                            ee.Fecha_Update = DateTime.Now;
                            ent.Especialidad_Entidad.Add(ee);
                            ent.SaveChanges();

                            //se genera el codigo del mensaje de retorno exitoso
                            return ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
                        }
                        else
                        {
                            //especialidad ya fue agregada
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

        public Basic EditEspecialidadEntidad(HttpRequest httpRequest)
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

                        Guid id_especialidad = Guid.Parse(httpRequest.Form["idespecialidad"]);
                        bool estado = Convert.ToBoolean(int.Parse(httpRequest.Form["estado"]));

                        Especialidad_Entidad ee = ent.Especialidad_Entidad.Where(t => t.Id_Entidad == entidad && t.Id_Especialidad == id_especialidad).SingleOrDefault();

                        if (ee != null)
                        {
                            ee.Estado = estado;
                            ee.Usuario_Update = usuario;
                            ee.Fecha_Update = DateTime.Now;
                            ent.SaveChanges();

                            //se genera el codigo del mensaje de retorno exitoso
                            return ret = autil.MensajeRetorno(ref ret, 20, string.Empty, null);
                        }
                        else
                        {
                            //especialidad ya fue agregada
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
    }
}