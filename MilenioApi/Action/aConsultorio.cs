using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public object CreateConsultorio(ConsultorioModel model)
        {
            Response ret = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(model.token);
                    if (cp != null)
                    {
                        Response b = new Response();
                        List<ErrorFields> rel = autil.ValidateObject(model);
                        if (rel.Count == 0)
                        {
                            //con estas dos lineas se saca el id del usuario y el id de la entidad del token
                            Guid entidad = Guid.Parse(cp.Claims.Where(cd => cd.Type == ClaimTypes.PrimaryGroupSid).Select(cd => cd.Value).SingleOrDefault());
                            Guid usuario = Guid.Parse(cp.Claims.Where(cd => cd.Type == ClaimTypes.NameIdentifier).Select(cd => cd.Value).SingleOrDefault());
                            ////
                                                       
                            //AQUI SE TOMA EL OBJETO ENVIADO DESDE EL FRONT
                            //Y SE COPIA AL OBJETO USER
                            Consultorio cl = new Consultorio();
                            Copier.CopyPropertiesTo(model, cl);
                            //
                            //VALIDAMOS SI EL CONSULTORIO YA EXISTE
                            Consultorio c = ent.Consultorio.Where(t => t.Id_Entidad == entidad && t.Nombre == model.Nombre).SingleOrDefault();

                            if (c == null)
                            {
                                cl.Fecha_Create = DateTime.Now;
                                cl.Fecha_Update = DateTime.Now;
                                cl.Usuario_Create = usuario;
                                cl.Usuario_Update = usuario;

                                ent.Consultorio.Add(cl);
                                ent.SaveChanges();
                                //se genera el codigo del mensaje de retorno exitoso
                                return ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null, HttpStatusCode.OK);
                            }
                            else
                            {
                                //consultorio existe
                                return ret = autil.MensajeRetorno(ref ret, 25, string.Empty, null, HttpStatusCode.OK);
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
                        ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null, HttpStatusCode.OK);
                        return ret;
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null, HttpStatusCode.BadRequest);
                    return ret;
                }
            }
        }

        public object EditConsultorio(ConsultorioModel model)
        {
            Response ret = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(model.token));
                    if (cp != null)
                    {
                        Response b = new Response();
                        List<ErrorFields> rel = autil.ValidateObject(model);
                        if (rel.Count == 0)
                        {
                            Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                            Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                            //AQUI SE TOMA EL OBJETO ENVIADO DESDE EL FRONT
                            //Y SE COPIA AL OBJETO USER
                            Consultorio cl = new Consultorio();
                            Copier.CopyPropertiesTo(model, cl);
                            //

                            List<Consultorio> lc = ent.Consultorio.Where(t => t.Id_Entidad == entidad).ToList();

                            if (lc.Count != 0)
                            {
                                //VALIDAMOS QUE NO EXISTA UN CONSULTORIO CON EL MISMO NOMBRE EN LA ENTIDAD
                                int nombexiste = lc.Where(r => r.Nombre == model.Nombre && r.Id_Consultorio != model.Id_Consultorio).Count();
                                if (nombexiste == 0)
                                {
                                    Consultorio c = lc.Where(f => f.Id_Consultorio == model.Id_Consultorio).SingleOrDefault();
                                    c.Nombre = model.Nombre;
                                    c.Descripcion = model.Descripcion;
                                    c.Estado = model.Estado;
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
                            //fallo campos requeridos
                            return autil.MensajeRetorno(ref b, 33, string.Empty, null, rel);
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

        public object GetConsultorio(ConsultorioModel model)
        {
            List<Consultorio> lc = new List<Consultorio>();
            List<ConsultorioModel> lcm = new List<ConsultorioModel>();
            Response ret = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(model.token));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        lc = ent.Consultorio.Where(t => t.Id_Entidad == entidad).ToList();

                        if (lc.Count != 0)
                        {
                            //busca por nombre
                            if (string.IsNullOrEmpty(model.Nombre))
                            {
                                lc = lc.Where(t => t.Nombre.Contains(model.Nombre)).ToList();
                            }

                            //busca por descripcion
                            if (string.IsNullOrEmpty(model.Descripcion))
                            {
                                lc = lc.Where(t => t.Descripcion.Contains(model.Descripcion)).ToList();
                            }
                            foreach (var i in lc)
                            {
                                ConsultorioModel cm = new ConsultorioModel();
                                Copier.CopyPropertiesTo(i, cm);
                                lcm.Add(cm);
                            }
                        }
                    }
                    else
                    {
                        //token invalido
                        return autil.MensajeRetorno(ref ret, 1, string.Empty, null);
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

        public object CreateConsultorioEspecialidad(ConsultorioModel model)
        {
            Response ret = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(model.token));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Consultorio_Especialidad ce = ent.Consultorio_Especialidad
                                                     .Where(c => c.Id_Consultorio == model.Id_Consultorio
                                                     && c.Id_Especialidad == model.Id_Especialidad
                                                     && c.Id_Entidad == entidad).SingleOrDefault();

                        if (ce == null)
                        {
                            ce = new Consultorio_Especialidad();
                            ce.Id_Consultorio = model.Id_Consultorio;
                            ce.Id_Especialidad = model.Id_Especialidad;
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

        public object EditConsultorioEspecialidad(ConsultorioModel model)
        {
            Response ret = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(model.token));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Consultorio_Especialidad ce = ent.Consultorio_Especialidad
                                                     .Where(c => c.Id_Consultorio == model.Id_Consultorio
                                                     && c.Id_Especialidad == model.Id_Especialidad
                                                     && c.Id_Entidad == entidad).SingleOrDefault();

                        if (ce != null)
                        {
                            ce.Estado = model.Estado;
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