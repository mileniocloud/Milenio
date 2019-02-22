using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace MilenioApi.Action
{
    public class aConsultorio
    {
        private TokenController tk = new TokenController();
        aUtilities autil = new aUtilities();
        #region CRUD
        ClaimsPrincipal cp = new ClaimsPrincipal();
        /// <summary>
        /// Metodo para crear consultorios
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic CreateConsultorio(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(cd => cd.Type == ClaimTypes.PrimaryGroupSid).Select(cd => cd.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(cd => cd.Type == ClaimTypes.NameIdentifier).Select(cd => cd.Value).SingleOrDefault());
                        string nombre = Convert.ToString(httpRequest.Form["nombre"]);
                        string descripcion = Convert.ToString(httpRequest.Form["descripcion"]);
                        bool estado = Convert.ToBoolean(int.Parse(httpRequest.Form["estado"]));

                        Consultorio c = ent.Consultorio.Where(t => t.Id_Entidad == entidad && t.Nombre == nombre).SingleOrDefault();

                        if (c == null)
                        {
                            c = new Consultorio();
                            c.Id_Consultorio = Guid.NewGuid();
                            c.Id_Entidad = entidad;
                            c.Nombre = nombre;
                            c.Descripcion = descripcion;
                            c.Fecha_Create = DateTime.Now;
                            c.Fecha_Update = DateTime.Now;
                            c.Usuario_Create = usuario;
                            c.Usuario_Update = usuario;

                            ent.Consultorio.Add(c);
                            ent.SaveChanges();
                            //se genera el codigo del mensaje de retorno exitoso
                            return ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
                        }
                        else
                        {
                            //consultorio existe
                            return ret = autil.MensajeRetorno(ref ret, 25, string.Empty, null);
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
        /// <summary>
        /// metodo para editar consultorios
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic EditConsultorio(HttpRequest httpRequest)
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
                        string nombre = Convert.ToString(httpRequest.Form["nombre"]);
                        string descripcion = Convert.ToString(httpRequest.Form["descripcion"]);
                        bool estado = Convert.ToBoolean(int.Parse(httpRequest.Form["estado"]));
                        Guid idconsultorio = Guid.Parse(httpRequest.Form["idconsultorio"]);

                        List<Consultorio> lc = ent.Consultorio.Where(t => t.Id_Entidad == entidad).ToList();

                        if (lc.Count != 0)
                        {
                            int nombexiste = lc.Where(r => r.Nombre == nombre && r.Id_Consultorio != idconsultorio).Count();
                            if (nombexiste == 0)
                            {
                                Consultorio c = lc.Where(f => f.Id_Consultorio == idconsultorio).SingleOrDefault();
                                c.Nombre = nombre;
                                c.Descripcion = descripcion;
                                c.Fecha_Update = DateTime.Now;
                                c.Usuario_Update = usuario;

                                ent.SaveChanges();
                                //se genera el codigo del mensaje de retorno exitoso
                                ret = autil.MensajeRetorno(ref ret, 20, string.Empty, null);
                            }
                            else
                            {
                                //consultorio existe
                                ret = autil.MensajeRetorno(ref ret, 25, string.Empty, null);
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
        }
        /// <summary>
        /// Metodo para consultar los consultorios
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public List<ConsultorioModel> GetConsultorio(HttpRequest httpRequest)
        {
            List<Consultorio> lc = new List<Consultorio>();
            List<ConsultorioModel> lcm = new List<ConsultorioModel>();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        lc = ent.Consultorio.Where(t => t.Id_Entidad == entidad).ToList();

                        if (lc.Count != 0)
                        {
                            //busca por nombre
                            if (string.IsNullOrEmpty(httpRequest.Form["nombre"]))
                            {
                                string nombre = Convert.ToString(httpRequest.Form["nombre"]);
                                lc = lc.Where(t => t.Nombre.Contains(nombre)).ToList();
                            }

                            //busca por descripcion
                            if (string.IsNullOrEmpty(httpRequest.Form["descripcion"]))
                            {
                                string descripcion = Convert.ToString(httpRequest.Form["descripcion"]);
                                lc = lc.Where(t => t.Descripcion.Contains(descripcion)).ToList();
                            }

                            //busca por descripcion
                            if (string.IsNullOrEmpty(httpRequest.Form["estado"]))
                            {
                                bool estado = Convert.ToBoolean(int.Parse(httpRequest.Form["estado"]));
                                lc = lc.Where(t => t.Estado == estado).ToList();
                            }

                            foreach (var i in lc)
                            {
                                ConsultorioModel cm = new ConsultorioModel();
                                Copier.CopyPropertiesTo(i, cm);
                                lcm.Add(cm);
                            }
                        }
                    }
                    return lcm;
                }
                catch (Exception)
                {
                    return lcm;
                }
            }
        }
        #endregion

        #region Relacionadas

        public Basic CreateConsultorioEspecialidad(HttpRequest httpRequest)
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

                        Guid id_consultorio = Guid.Parse(httpRequest.Form["idconsultorio"]);
                        Guid id_especialidad = Guid.Parse(httpRequest.Form["idespecialidad"]);

                        Consultorio_Especialidad ce = ent.Consultorio_Especialidad
                                                     .Where(c => c.Id_Consultorio == id_consultorio
                                                     && c.Id_Especialidad == id_especialidad
                                                     && c.Id_Entidad == entidad).SingleOrDefault();

                        if (ce == null)
                        {
                            ce = new Consultorio_Especialidad();
                            ce.Id_Consultorio = id_consultorio;
                            ce.Id_Especialidad = id_especialidad;
                            ce.Id_Entidad = entidad;
                            ce.Estado = true;
                            ce.Usuario_Create = usuario;
                            ce.Usuario_Update = usuario;
                            ce.Fecha_Create = DateTime.Now;
                            ce.Fecha_Update = DateTime.Now;
                            ent.Consultorio_Especialidad.Add(ce);
                            ent.SaveChanges();

                            //se genera el codigo del mensaje de retorno exitoso
                            return ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
                        }
                        else
                        {
                            //se especialidad agregada
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

        public Basic EditConsultorioEspecialidad(HttpRequest httpRequest)
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

                        Guid id_consultorio = Guid.Parse(httpRequest.Form["idconsultorio"]);
                        Guid id_especialidad = Guid.Parse(httpRequest.Form["idespecialidad"]);
                        bool estado = Convert.ToBoolean(int.Parse(httpRequest.Form["estado"]));

                        Consultorio_Especialidad ce = ent.Consultorio_Especialidad
                                                     .Where(c => c.Id_Consultorio == id_consultorio
                                                     && c.Id_Especialidad == id_especialidad
                                                     && c.Id_Entidad == entidad).SingleOrDefault();

                        if (ce != null)
                        {
                            ce.Estado = estado;
                            ce.Usuario_Update = usuario;
                            ce.Fecha_Update = DateTime.Now;
                            ent.SaveChanges();

                            //se genera el codigo del mensaje de retorno exitoso
                            return ret = autil.MensajeRetorno(ref ret, 20, string.Empty, null);
                        }
                        else
                        {
                            // especialidad no agregada
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