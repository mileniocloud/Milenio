using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
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
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic CreateEntidad(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                    int Nit = Convert.ToInt32(httpRequest.Form["Nit"]);

                    //validamos que ese nit ya exista
                    int vNit = ent.Entidad.Where(t => t.Nit == Nit).Count();
                    if (vNit == 0)
                    {
                        ///FOTO
                        String file = string.Empty;
                        if (httpRequest.Files.Count > 0)
                        {
                            var foto = httpRequest.Files;
                            Byte[] Content = new BinaryReader(foto[0].InputStream).ReadBytes(foto[0].ContentLength);
                            file = Convert.ToBase64String(Content);
                        }

                        string Nombre = Convert.ToString(httpRequest.Form["nombre"]);
                        string organizacion = Convert.ToString(httpRequest.Form["organizacion"]);
                        Guid id_Poblado = Guid.Parse(httpRequest.Form["idpoblado"]);
                        string direccion = Convert.ToString(httpRequest.Form["direccion"]);
                        string email = Convert.ToString(httpRequest.Form["email"]);
                        string codentidad = Convert.ToString(httpRequest.Form["codentidad"]);
                        bool atencionprioritaria = Convert.ToBoolean(int.Parse(httpRequest.Form["atencionprioritaria"]));
                        bool contribuyente = Convert.ToBoolean(int.Parse(httpRequest.Form["contribuyente"]));
                        string horadesde = Convert.ToString(httpRequest.Form["horadesde"]);
                        string horahasta = Convert.ToString(httpRequest.Form["horahasta"]);

                        Entidad et = new Entidad();
                        Guid identidad = Guid.NewGuid();
                        et.Id_Entidad = identidad;
                        et.Nit = Nit;
                        et.Nombre = Nombre;
                        et.Organizacion = organizacion;
                        et.Id_Poblado = id_Poblado;
                        et.Direccion = direccion;
                        et.Email = email;
                        et.CodigoEntidad = codentidad;
                        et.Atencion_Prioritaria = atencionprioritaria;
                        et.Contribuyente = contribuyente;
                        et.Hora_Desde = horadesde;
                        et.Hora_Hasta = horahasta;
                        et.Fecha_Create = DateTime.Now;
                        et.Fecha_Update = DateTime.Now;
                        et.Foto = file;

                        ent.Entidad.Add(et);
                        //se envia a crear todo

                        //se consulta en el webconfig el usuario SA y el rol SA
                        Guid sauser = Guid.Parse(ConfigurationManager.AppSettings["idsuperuser"]);
                        Guid sarol = Guid.Parse(ConfigurationManager.AppSettings["idrolsuperuser"]);

                        //se agrega el usuario SA a la entidad
                        Entidad_Usuario eu = new Entidad_Usuario();
                        eu.Id_Entidad = identidad;
                        eu.Id_Usuario = sauser;
                        eu.Estado = true;
                        eu.Usuario_Create = usuario;
                        eu.Usuario_Update = usuario;
                        eu.Fecha_Create = DateTime.Now;
                        eu.Fecha_Update = DateTime.Now;
                        ent.Entidad_Usuario.Add(eu);

                        ///se agrega el rol SA a ese usuario para esa entidad
                        Rol_Usuario ru = new Rol_Usuario();
                        ru.Id_Rol = sarol;
                        ru.Id_Usuario = sauser;
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
                        ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
                    }
                    else
                    {
                        //Nit EXISTE
                        ret = autil.MensajeRetorno(ref ret, 3, string.Empty, null);
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
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                    int Nit = Convert.ToInt32(httpRequest.Form["Nit"]);
                    Guid identidad = Guid.Parse(httpRequest.Form["identidad"]);

                    //validamos que ese nit ya exista
                    int vNit = ent.Entidad.Where(t => t.Nit == Nit && t.Id_Entidad != identidad).Count();
                    if (vNit == 0)
                    {
                        ///FOTO
                        String file = string.Empty;
                        if (httpRequest.Files.Count > 0)
                        {
                            var foto = httpRequest.Files;
                            Byte[] Content = new BinaryReader(foto[0].InputStream).ReadBytes(foto[0].ContentLength);
                            file = Convert.ToBase64String(Content);
                        }

                        string Nombre = Convert.ToString(httpRequest.Form["nombre"]);
                        string organizacion = Convert.ToString(httpRequest.Form["organizacion"]);
                        Guid id_Poblado = Guid.Parse(httpRequest.Form["idpoblado"]);
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
                        et.Id_Poblado = id_Poblado;
                        et.Direccion = direccion;
                        et.Email = email;
                        et.CodigoEntidad = codentidad;
                        et.Atencion_Prioritaria = atencionprioritaria;
                        et.Contribuyente = contribuyente;
                        et.Hora_Desde = horadesde;
                        et.Hora_Hasta = horahasta;
                        et.Fecha_Update = DateTime.Now;
                        et.Foto = file;

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

                        //consulta por nombre
                        if (!string.IsNullOrEmpty(httpRequest.Form["nombre"]))
                        {
                            string nombre = Convert.ToString(httpRequest.Form["nombre"]);
                            et = et.Where(c => c.Nombre == nombre);
                        }

                        //consulta por nit
                        if (!string.IsNullOrEmpty(httpRequest.Form["nombre"]))
                        {
                            int nit = Convert.ToInt32(httpRequest.Form["nit"]);
                            et = et.Where(c => c.Nit == nit);
                        }

                        //consulta por cofigo entidad
                        if (!string.IsNullOrEmpty(httpRequest.Form["codigoentidad"]))
                        {
                            string codigoentidad = Convert.ToString(httpRequest.Form["codigoentidad"]);
                            et = et.Where(c => c.CodigoEntidad == codigoentidad);
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
                    return ret;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EntidadModel> GetEntidadesFiltro(HttpRequest httpRequest)
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

                        //consulta por nombre
                        if (!string.IsNullOrEmpty(httpRequest.Form["nombre"]))
                        {
                            string nombre = Convert.ToString(httpRequest.Form["nombre"]);
                            et = et.Where(c => c.Nombre == nombre);
                        }

                        //consulta por nit
                        if (!string.IsNullOrEmpty(httpRequest.Form["nombre"]))
                        {
                            int nit = Convert.ToInt32(httpRequest.Form["nit"]);
                            et = et.Where(c => c.Nit == nit);
                        }

                        //consulta por cofigo entidad
                        if (!string.IsNullOrEmpty(httpRequest.Form["codigoentidad"]))
                        {
                            string codigoentidad = Convert.ToString(httpRequest.Form["codigoentidad"]);
                            et = et.Where(c => c.CodigoEntidad == codigoentidad);
                        }

                        foreach (var i in et.ToList())
                        {
                            EntidadModel em = new EntidadModel();
                            Copier.CopyPropertiesTo(i, em);
                            ret.Add(em);
                        }

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