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
    public class aOffice
    {
        private TokenValidationHandler tvh = new TokenValidationHandler();
        aUtilities autil = new aUtilities();
        #region CRUD
        ClaimsPrincipal cp = new ClaimsPrincipal();

        public object CreateOffice(OfficeModel model)
        {
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(model.token);

                    Response b = new Response();
                    List<ErrorFields> rel = autil.ValidateObject(model);
                    if (rel.Count == 0)
                    {
                        //con estas dos lineas se saca el id del usuario y el id de la entidad del token
                        Guid entidad = Guid.Parse(cp.Claims.Where(cd => cd.Type == ClaimTypes.PrimaryGroupSid).Select(cd => cd.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(cd => cd.Type == ClaimTypes.NameIdentifier).Select(cd => cd.Value).SingleOrDefault());
                        ////
                     
                        //VALIDAMOS SI EL CONSULTORIO YA EXISTE
                        Consultorio c = ent.Consultorio.Where(t => t.Id_Entidad == entidad && t.Nombre == model.Nombre).SingleOrDefault();

                        if (c == null)
                        {
                            //AQUI SE TOMA EL OBJETO ENVIADO DESDE EL FRONT
                            //Y SE COPIA AL OBJETO CONSULTORIO
                            Consultorio cons = new Consultorio();
                            Copier.CopyPropertiesTo(model, cons);
                            Guid id_Consultorio = Guid.NewGuid();
                            cons.Id_Consultorio = id_Consultorio;
                            cons.Id_Entidad = entidad;
                            cons.Estado = true;
                            cons.Usuario_Create = usuario;
                            cons.Usuario_Update = usuario;
                            cons.Fecha_Create = DateTime.Now;
                            cons.Fecha_Update = DateTime.Now;
                            ent.Consultorio.Add(cons);
                            string especialildad;
                            if (!string.IsNullOrEmpty(model.list_Especialidad))
                            {
                                especialildad = Convert.ToString(model.list_Especialidad);
                                string[] especialidadArray = especialildad.Split(',');
                                List<Consultorio_Especialidad> lce = new List<Consultorio_Especialidad>();
                                foreach (var esp in especialidadArray)
                                {
                                    Consultorio_Especialidad ce = new Consultorio_Especialidad();
                                    if (ce != null)
                                    {
                                        ce.Id_Consultorio = id_Consultorio;
                                        ce.Id_Especialidad = Guid.Parse(esp);
                                        ce.Id_Entidad = entidad;
                                        ce.Estado = true;
                                        ce.Usuario_Create = usuario;
                                        ce.Usuario_Update = usuario;
                                        ce.Fecha_Create = DateTime.Now;
                                        ce.Fecha_Update = DateTime.Now;
                                        lce.Add(ce);
                                    }
                                }

                                if (lce.Count > 0)
                                {
                                    //si hay especialidades que agregar, las agrega
                                    ent.Consultorio_Especialidad.AddRange(lce);
                                }
                            }
                            ent.SaveChanges();
                            //se genera el codigo del mensaje de retorno exitoso
                            return rp = autil.ReturnMesagge(ref rp, 2, string.Empty, null, HttpStatusCode.OK);
                        }
                        else
                        {
                            //consultorio existe
                            return rp = autil.ReturnMesagge(ref rp, 25, string.Empty, null, HttpStatusCode.OK);
                        }
                    }
                    else
                    {
                        //fallo campos requeridos
                        return autil.ReturnMesagge(ref b, 33, string.Empty, null, rel, HttpStatusCode.OK);
                    }

                }
                catch (Exception ex)
                {
                    //error general
                    rp = autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.BadRequest);
                    return rp;
                }
            }
        }

        public object EditOffice(OfficeModel model)
        {
            Response rp = new Response();
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
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        //AQUI SE TOMA EL OBJETO ENVIADO DESDE EL FRONT
                        //Y SE COPIA AL OBJETO USER
                        Consultorio cl = new Consultorio();
                        Copier.CopyPropertiesTo(model, cl);
                        //

                        //VALIDAMOS QUE NO EXISTA UN CONSULTORIO CON EL MISMO NOMBRE EN LA ENTIDAD
                        int nombexiste = ent.Consultorio.Where(r => r.Nombre == model.Nombre && r.Id_Consultorio != model.Id_Consultorio).Count();
                        if (nombexiste == 0)
                        {
                            Consultorio c = ent.Consultorio.Where(f => f.Id_Consultorio == model.Id_Consultorio).SingleOrDefault();
                            c.Nombre = model.Nombre;
                            c.Descripcion = model.Descripcion;
                            c.Estado = model.Estado;
                            c.Fecha_Update = DateTime.Now;
                            c.Usuario_Update = usuario;

                            ent.SaveChanges();
                            //se genera el codigo del mensaje de retorno exitoso
                            rp = autil.ReturnMesagge(ref rp, 20, string.Empty, null);
                        }
                        else
                        {
                            //consultorio existe
                            rp = autil.ReturnMesagge(ref rp, 25, string.Empty, null);
                        }
                        return rp;
                    }
                    else
                    {
                        //fallo campos requeridos
                        return autil.ReturnMesagge(ref b, 33, string.Empty, null, rel);
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    rp = autil.ReturnMesagge(ref rp, 4, string.Empty, null);
                    return rp;
                }
            }
        }

        public object GetEditOffice(OfficeModel model)
        {
            List<Consultorio> lc = new List<Consultorio>();
            List<OfficeModel> lcm = new List<OfficeModel>();
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        lc = ent.Consultorio.Where(t => t.Id_Entidad == entidad).ToList();

                        if (lc.Count != 0)
                        {
                            var rl = lc.Where(o => o.Id_Consultorio == model.Id_Consultorio).Select(u => new
                            {
                                idoffice = u.Id_Consultorio,
                                name = u.Nombre,
                                description = u.Descripcion,
                                status = u.Estado,
                                especiality = u.Consultorio_Especialidad.Where(e=> e.Estado = true).Select(t=> t.Id_Especialidad)
                            }).ToList();
                            rp.cantidad = rl.Count();
                            rp.pagina = 0;
                            rp.data = rl;
                        }
                    }
                    else
                    {
                        //token invalido
                        return autil.ReturnMesagge(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                    }
                    return autil.ReturnMesagge(ref rp, 9, null, null, HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.BadRequest);
                }
            }
        }
        public object GetOffice(OfficeModel model)
        {
            List<Consultorio> lc = new List<Consultorio>();
            List<OfficeModel> lcm = new List<OfficeModel>();
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        lc = ent.Consultorio.Where(t => t.Id_Entidad == entidad).ToList();

                        if (lc.Count != 0)
                        {
                            //busca por nombre
                            if (!string.IsNullOrEmpty(model.Nombre))
                            {
                                lc = lc.Where(t => t.Nombre.Contains(model.Nombre)).ToList();
                            }

                            //busca por descripcion
                            if (!string.IsNullOrEmpty(model.Descripcion))
                            {
                                lc = lc.Where(t => t.Descripcion.Contains(model.Descripcion)).ToList();
                            }
                            var rl = lc.Select(u => new
                            {
                                idoffice = u.Id_Consultorio,
                                name = u.Nombre,
                                description = u.Descripcion,
                                status = u.Estado
                            }).ToList();
                            rp.cantidad = rl.Count();
                            rp.pagina = 0;
                            rp.data = rl;
                        }
                    }
                    else
                    {
                        //token invalido
                        return autil.ReturnMesagge(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                    }
                    return autil.ReturnMesagge(ref rp, 9, null, null, HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.BadRequest);
                }
            }
        }
        #endregion

        #region Relacionadas

        public object CreateConsultorioEspecialidad(OfficeModel model)
        {
            Response ret = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());


                        int vCons = ent.Consultorio.Where(t => t.Nombre == model.Nombre && t.Id_Entidad == entidad).Count();
                        Consultorio cons = new Consultorio();
                        if (vCons == 0)
                        {
                            Copier.CopyPropertiesTo(model, cons);
                            Guid id_Consultorio = Guid.NewGuid();
                            cons.Id_Consultorio = id_Consultorio;
                            cons.Id_Entidad = entidad;
                            cons.Estado = true;
                            cons.Usuario_Create = usuario;
                            cons.Usuario_Update = usuario;
                            cons.Fecha_Create = DateTime.Now;
                            cons.Fecha_Update = DateTime.Now;
                            ent.Consultorio.Add(cons);
                            string especialildad;
                            if (!string.IsNullOrEmpty(model.list_Especialidad))
                            {
                                especialildad = Convert.ToString(model.list_Especialidad);
                                string[] especialidadArray = especialildad.Split(',');
                                List<Consultorio_Especialidad> lce = new List<Consultorio_Especialidad>();
                                foreach (var esp in especialidadArray)
                                {
                                    Consultorio_Especialidad ce = new Consultorio_Especialidad();
                                    if (ce != null)
                                    {
                                        ce.Id_Consultorio = id_Consultorio;
                                        ce.Id_Especialidad = Guid.Parse(esp);
                                        ce.Id_Entidad = entidad;
                                        ce.Estado = true;
                                        ce.Usuario_Create = usuario;
                                        ce.Usuario_Update = usuario;
                                        ce.Fecha_Create = DateTime.Now;
                                        ce.Fecha_Update = DateTime.Now;
                                        lce.Add(ce);
                                    }

                                }

                                if (lce.Count > 0)
                                {
                                    //si hay especialidades que agregar, las agrega
                                    ent.Consultorio_Especialidad.AddRange(lce);
                                }
                            }

                            ent.SaveChanges();

                            //se genera el codigo del mensaje de retorno exitoso
                            ret = autil.ReturnMesagge(ref ret, 2, string.Empty, null, HttpStatusCode.OK);
                        }
                        else
                        {
                            //Retorna consultorio ya existente
                            return ret = autil.ReturnMesagge(ref ret, 25, string.Empty, null, HttpStatusCode.OK);
                        }
                    }
                    else
                    {
                        //token invalido
                        ret = autil.ReturnMesagge(ref ret, 1, string.Empty, null, HttpStatusCode.OK);

                    }
                    return ret;
                }
                catch (Exception ex)
                {
                    //error general
                    ret = autil.ReturnMesagge(ref ret, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                    return ret;
                }
            }
        }

        public object EditConsultorioEspecialidad(OfficeModel model)
        {
            Response ret = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Consultorio con = ent.Consultorio
                                                     .Where(c => c.Id_Consultorio == model.Id_Consultorio
                                                     && c.Id_Entidad == entidad).SingleOrDefault();

                        if (con != null)
                        {
                            Copier.CopyPropertiesTo(model, con);
                            con.Estado = model.Estado;
                            con.Usuario_Update = usuario;
                            con.Fecha_Update = DateTime.Now;
                            string especialildad;
                            if (!string.IsNullOrEmpty(model.list_Especialidad))
                            {
                                especialildad = Convert.ToString(model.list_Especialidad);
                                string[] especialidadArray = especialildad.Split(',');
                                List<Consultorio_Especialidad> lce = new List<Consultorio_Especialidad>();
                                foreach (var esp in especialidadArray)
                                {
                                    Consultorio_Especialidad ce = ent.Consultorio_Especialidad.Where(ces => ces.Id_Consultorio == model.Id_Consultorio
                                                     && ces.Id_Entidad == entidad && ces.Id_Especialidad == Guid.Parse(esp)).SingleOrDefault();
                                    if (ce != null)
                                    {
                                        ce.Id_Consultorio = model.Id_Consultorio;
                                        ce.Id_Especialidad = Guid.Parse(esp);
                                        ce.Id_Entidad = entidad;
                                        ce.Estado = true;
                                        ce.Usuario_Create = usuario;
                                        ce.Usuario_Update = usuario;
                                        ce.Fecha_Create = DateTime.Now;
                                        ce.Fecha_Update = DateTime.Now;
                                        lce.Add(ce);
                                    }

                                }

                                if (lce.Count > 0)
                                {
                                    //si hay especialidades que agregar, las agrega
                                    ent.Consultorio_Especialidad.AddRange(lce);
                                }
                            }
                            ent.SaveChanges();

                            //se genera el codigo del mensaje de retorno exitoso
                            ret = autil.ReturnMesagge(ref ret, 2, string.Empty, null, HttpStatusCode.OK);
                        }
                        else
                        {
                            //Retorna consultorio no existente
                            return ret = autil.ReturnMesagge(ref ret, 25, string.Empty, null, HttpStatusCode.OK);
                        }
                    }
                    else
                    {
                        //token invalido
                        ret = autil.ReturnMesagge(ref ret, 1, string.Empty, null, HttpStatusCode.OK);

                    }
                    return ret;
                }
                catch (Exception ex)
                {
                    //error general
                    ret = autil.ReturnMesagge(ref ret, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                    return ret;
                }
            }
        }

        #endregion

    }
}