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
    public class aEntidad
    {
        TokenController tk = new TokenController();
        ClaimsPrincipal cp = new ClaimsPrincipal();
        aUtilities autil = new aUtilities();

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
                String file = string.Empty;
                if (httpRequest.Files.Count > 0)
                {
                    var foto = httpRequest.Files;
                    Byte[] Content = new BinaryReader(foto[0].InputStream).ReadBytes(foto[0].ContentLength);
                    file = Convert.ToBase64String(Content);
                }

                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    int Nit = Convert.ToInt32(httpRequest.Form["Nit"]);
                    string Nombre = Convert.ToString(httpRequest.Form["Nombre"]);
                    int CodigoEntidad = Convert.ToInt32(httpRequest.Form["CodigoEntidad"]);
                    int CodigoDane = Convert.ToInt32(httpRequest.Form["CodigoDane"]);
                    DateTime FiniFiscal = DateTime.Parse(httpRequest.Form["FiniFiscal"]);
                    DateTime FfinFiscal = DateTime.Parse(httpRequest.Form["FfinFiscal"]);

                    Guid? EntidadPadre = null;
                    if (string.IsNullOrEmpty(Convert.ToString(httpRequest.Form["EntidadPadre"])))
                        EntidadPadre = Guid.Parse(Convert.ToString(httpRequest.Form["EntidadPadre"]));

                    //validamos que ese nit ya exista
                    int vNit = (from et in ent.Entidad
                                where et.Nit == Nit
                                select et).Count();

                    if (vNit == 0)
                    {

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
                        ent.Ubicacion.Add(ub);

                        Entidad et = new Entidad();
                        et.Codigo_Id = Guid.NewGuid();
                        et.Nit = Nit;
                        et.Nombre = Nombre;
                        et.CodigoDane = CodigoDane;
                        et.CodigoEntidad = CodigoEntidad;
                        et.Foto = file;
                        et.FiniFiscal = FiniFiscal;
                        et.FfinFiscal = FfinFiscal;
                        et.Entidad_Padre = EntidadPadre;
                        et.Ubicacion_Id = codigo_ubicacion;
                        et.Created_At = DateTime.Now;
                        et.Updated_At = DateTime.Now;
                        ent.Entidad.Add(et);

                        //se envia a crear todo
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
        /// Permite la edicion de los datos basicos de la entidad
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic EditEntidad(HttpRequest httpRequest)
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
                        //se saca el id de la entidad del token
                        string entidad = cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault();
                        Guid entidad_id = Guid.Parse(entidad);

                        string Nombre = Convert.ToString(httpRequest.Form["Nombre"]);
                        int CodigoEntidad = Convert.ToInt32(httpRequest.Form["CodigoEntidad"]);
                        int CodigoDane = Convert.ToInt32(httpRequest.Form["CodigoDane"]);
                        DateTime FiniFiscal = DateTime.Parse(httpRequest.Form["FiniFiscal"]);
                        DateTime FfinFiscal = DateTime.Parse(httpRequest.Form["FfinFiscal"]);

                        Guid? EntidadPadre = null;
                        if (string.IsNullOrEmpty(Convert.ToString(httpRequest.Form["EntidadPadre"])))
                            EntidadPadre = Guid.Parse(Convert.ToString(httpRequest.Form["EntidadPadre"]));

                        Entidad et = (from t in ent.Entidad
                                      where t.Codigo_Id == entidad_id
                                      select t).SingleOrDefault();
                        if (et != null)
                        {
                            et.Nombre = Nombre;
                            et.CodigoDane = CodigoDane;
                            et.CodigoEntidad = CodigoEntidad;
                            et.Foto = file;
                            et.FiniFiscal = FiniFiscal;
                            et.FfinFiscal = FfinFiscal;
                            et.Entidad_Padre = EntidadPadre;
                            et.Created_At = DateTime.Now;
                            et.Updated_At = DateTime.Now;
                            ent.Entidad.Add(et);

                            //se envia a crear todo
                            ent.SaveChanges();
                            //se genera el codigo del mensaje de retorno exitoso
                            ret = autil.MensajeRetorno(ref ret, 20, string.Empty, null);
                        }
                        else
                        {
                            //datos invalidos
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
                //error general
                ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                return ret;
            }
        }

        #endregion

        /// <summary>
        /// Metodo para crear aulas
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic CreateAula(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        string Nombre = Convert.ToString(httpRequest.Form["Nombre"]);
                        int Codigo = Convert.ToInt32(httpRequest.Form["Codigo"]);

                        //se saca el id de la entidad del token
                        string entidad = cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault();
                        Guid entidad_id = Guid.Parse(entidad);

                        //se saca el usiario que esta creando la persona del token
                        Guid? user_id = null;
                        string usid = cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                        if (!string.IsNullOrEmpty(usid))
                            user_id = Guid.Parse(usid);

                        int vaulacod = (from au in ent.Aula
                                        where au.CodigoAula == Codigo
                                        && au.Entidad_Id == entidad_id
                                        select au).Count();

                        int vaulanomb = (from au in ent.Aula
                                         where au.Nombre == Nombre
                                         && au.Entidad_Id == entidad_id
                                         select au).Count();
                        if (vaulanomb == 0)
                        {
                            if (vaulacod == 0)
                            {
                                Aula a = new Aula();
                                a.Codigo_Id = Guid.NewGuid();
                                a.Nombre = Nombre;
                                a.CodigoAula = Codigo;
                                a.Estado = true;
                                a.Entidad_Id = entidad_id;
                                a.Usuario_Update = user_id;
                                a.Created_At = DateTime.Now;
                                a.Updated_At = DateTime.Now;

                                ent.Aula.Add(a);
                                ent.SaveChanges();
                                //codigo exitoso
                                ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
                            }
                            else
                            {
                                //codigo aula existe
                                ret = autil.MensajeRetorno(ref ret, 22, string.Empty, null);
                            }
                        }
                        else
                        {
                            //Nombre aula existe
                            ret = autil.MensajeRetorno(ref ret, 23, string.Empty, null);
                        }
                    }
                }
                else
                {
                    //token invalido
                    ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
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
        /// Metodo para editar las aulas
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic EditAula(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Guid Aula_Id = Guid.Parse(httpRequest.Form["Aula_Id"]);
                        bool estado = bool.Parse(httpRequest.Form["Estado"]);
                        string Nombre = Convert.ToString(httpRequest.Form["Nombre"]);
                        int Codigo = Convert.ToInt32(httpRequest.Form["Codigo"]);

                        //se saca el id de la entidad del token
                        string entidad = cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault();
                        Guid entidad_id = Guid.Parse(entidad);

                        //se saca el usiario que esta creando la persona del token
                        Guid? user_id = null;
                        string usid = cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                        if (!string.IsNullOrEmpty(usid))
                            user_id = Guid.Parse(usid);

                        int vaulacod = (from au in ent.Aula
                                        where au.CodigoAula == Codigo
                                        && au.Codigo_Id != Aula_Id
                                        && au.Entidad_Id == entidad_id
                                        select au).Count();

                        int vaulanomb = (from au in ent.Aula
                                         where au.Nombre == Nombre
                                         && au.Codigo_Id != Aula_Id
                                         && au.Entidad_Id == entidad_id
                                         select au).Count();
                        if (vaulanomb == 0)
                        {
                            if (vaulacod == 0)
                            {
                                Aula a = (from au in ent.Aula
                                          where au.Codigo_Id == Aula_Id
                                          select au).SingleOrDefault();
                                if (a != null)
                                {
                                    a.Nombre = Nombre;
                                    a.CodigoAula = Codigo;
                                    a.Estado = estado;
                                    a.Entidad_Id = entidad_id;
                                    a.Updated_At = DateTime.Now;
                                    a.Usuario_Update = user_id;
                                    ent.SaveChanges();
                                    //codigo exitoso
                                    ret = autil.MensajeRetorno(ref ret, 20, string.Empty, null);
                                }
                                else
                                {
                                    //Datos no coinciden
                                    ret = autil.MensajeRetorno(ref ret, 15, string.Empty, null);
                                }
                            }
                            else
                            {
                                //codigo aula existe
                                ret = autil.MensajeRetorno(ref ret, 22, string.Empty, null);
                            }
                        }
                        else
                        {
                            //Nombre aula existe
                            ret = autil.MensajeRetorno(ref ret, 23, string.Empty, null);
                        }
                    }
                }
                else
                {
                    //token invalido
                    ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
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
        /// Metodo que consulta todas las aulas de una entidad
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public List<Basic> GetAula(HttpRequest httpRequest)
        {
            List<Basic> ret = new List<Basic>();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                if (cp != null)
                {
                    //se saca el id de la entidad del token
                    string entidad = cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault();
                    Guid entidad_id = Guid.Parse(entidad);
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        ret = (from a in ent.Aula
                               where a.Entidad_Id == entidad_id
                               select new Basic
                               {
                                   id = a.Codigo_Id,
                                   nombre = a.Nombre,
                                   codigoint = a.CodigoAula,
                                   Estado = a.Estado.Value
                               }).ToList();
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                return ret;
            }
        }

        /// <summary>
        /// metodo para crear telefono para entidad
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic CreateTelefonoEntidad(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));

                if (cp != null)
                {
                    //se saca el id de la entidad del token
                    string entidad = cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault();
                    Guid entidad_id = Guid.Parse(entidad);

                    //se saca el usiario que esta creando la persona del token
                    Guid? user_id = null;
                    string usid = cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(usid))
                        user_id = Guid.Parse(usid);

                    int Numero = Convert.ToInt32(httpRequest.Form["Numero"]);
                    string Descripcion = Convert.ToString(httpRequest.Form["Numero"]);
                    string Tipo = Convert.ToString(httpRequest.Form["Numero"]);

                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        int vtelefono = (from t in ent.Telefono
                                         where t.Entidad_Id == entidad_id
                                         && t.Numero == Numero
                                         select t).Count();

                        if (vtelefono == 0)
                        {
                            Telefono t = new Telefono();
                            t.Codigo_Id = Guid.NewGuid();
                            t.Entidad_Id = entidad_id;
                            t.Numero = Numero;
                            t.Descripcion = Descripcion;
                            t.Tipo = Tipo;
                            t.Created_At = DateTime.Now;
                            t.Updated_At = DateTime.Now;
                            t.Usuario_Update = user_id;

                            ent.Telefono.Add(t);
                            ent.SaveChanges();

                            //Registro Exitoso
                            ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
                        }
                        else
                        {
                            //Numero existe
                            ret = autil.MensajeRetorno(ref ret, 28, string.Empty, null);
                        }
                    }
                }
                else
                {
                    //token invalido
                    ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
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
        /// Metodo para crear telefono para personas
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic CreateTelefonoPersona(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));

                if (cp != null)
                {
                    //se saca el id de la entidad del token
                    string entidad = cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault();
                    Guid entidad_id = Guid.Parse(entidad);

                    //se saca el usiario que esta creando la persona del token
                    Guid? user_id = null;
                    string usid = cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(usid))
                        user_id = Guid.Parse(usid);

                    int Numero = Convert.ToInt32(httpRequest.Form["Numero"]);
                    string Descripcion = Convert.ToString(httpRequest.Form["Numero"]);
                    string Tipo = Convert.ToString(httpRequest.Form["Numero"]);
                    Guid Persona_Id = Guid.Parse(httpRequest.Form["Persona_Id"]);

                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        int vtelefono = (from t in ent.Telefono
                                         where t.Persona_Id == Persona_Id
                                         && t.Numero == Numero
                                         select t).Count();

                        if (vtelefono == 0)
                        {
                            Telefono t = new Telefono();
                            t.Codigo_Id = Guid.NewGuid();
                            t.Persona_Id = Persona_Id;
                            t.Numero = Numero;
                            t.Descripcion = Descripcion;
                            t.Tipo = Tipo;
                            t.Created_At = DateTime.Now;
                            t.Updated_At = DateTime.Now;
                            t.Usuario_Update = user_id;

                            ent.Telefono.Add(t);
                            ent.SaveChanges();

                            //Registro Exitoso
                            ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
                        }
                        else
                        {
                            //Numero existe
                            ret = autil.MensajeRetorno(ref ret, 28, string.Empty, null);
                        }
                    }
                }
                else
                {
                    //token invalido
                    ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
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
    }
}