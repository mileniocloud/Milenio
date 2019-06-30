using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;

namespace MilenioApi.Action
{
    public class aSpecialty
    {
        TokenValidationHandler tvh = new TokenValidationHandler();
        ClaimsPrincipal cp = new ClaimsPrincipal();
        aUtilities autil = new aUtilities();

        #region Agenda Profesional // listo para pruebas
        public object CreateCupXSpecialty(SpecialtyCupModel model)
        {
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                    List<ErrorFields> rel = autil.ValidateObject(model);
                    if (rel.Count == 0)
                    {
                        List<Especialidad_Cup_Entidad> lace = new List<Especialidad_Cup_Entidad>();
                        if (model.cups.Count > 0)
                        {
                            foreach (var e in model.cups)
                            {
                                Guid cup = Guid.Parse(e.id);
                                List<Especialidad_Cup_Entidad> ece = ent.Especialidad_Cup_Entidad
                                               .Where(f =>
                                               f.Id_Especialidad == model.Id_Especialidad
                                               && f.Id_Cups == cup
                                               && f.Id_Entidad == entidad
                                               && f.Estado == true
                                               ).ToList();
                                if (ece.Count() == 0)
                                {

                                    Especialidad_Cup_Entidad ace_ = new Especialidad_Cup_Entidad();
                                    ace_.Id_Especialidad = model.Id_Especialidad;
                                    ace_.Id_Cups = cup;
                                    ace_.Id_Entidad = entidad;
                                    ace_.Estado = true;
                                    ace_.Usuario_Create = usuario;
                                    ace_.Fecha_Create = DateTime.Now;
                                    ace_.Usuario_Update = usuario;
                                    ace_.Fecha_Update = DateTime.Now;
                                    lace.Add(ace_);
                                }
                                else
                                {
                                    //ya existen registros iguales
                                    return rp = autil.ReturnMesagge(ref rp, 50, string.Empty, null, HttpStatusCode.OK);
                                }
                            }
                        }
                        if (lace.Count > 0)
                        {
                            //si hay especialidades que agregar, las agrega
                            ent.Especialidad_Cup_Entidad.AddRange(lace);
                            ent.SaveChanges();

                        }
                        model = new SpecialtyCupModel();
                        rp.data = GetCupXSpecialty(model);
                        //se genera el codigo del mensaje de retorno exitoso
                        return rp = autil.ReturnMesagge(ref rp, 2, string.Empty, null, HttpStatusCode.OK);
                    }
                    else
                    {
                        //fallo campos requeridos
                        return autil.ReturnMesagge(ref rp, 33, string.Empty, null, rel, HttpStatusCode.OK);
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    rp = autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
                    return rp;
                }
            }
        }

        public object EditCupXSpecialty(SpecialtyCupModel model)
        {
            Response rep = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                    List<ErrorFields> rel = autil.ValidateObject(model);
                    if (rel.Count == 0)
                    {
                        List<Especialidad_Cup_Entidad> laceModel = new List<Especialidad_Cup_Entidad>();
                        if (model.cups.Count > 0)
                        {
                            List<Especialidad_Cup_Entidad> ece = ent.Especialidad_Cup_Entidad.Where(f => f.Id_Entidad == entidad && f.Id_Especialidad == model.Id_Especialidad).ToList();
                            //if (ece.Count > 0)
                            //{
                            //Se crea una lista de los cups que vienen del modelo 
                            foreach (var e in model.cups)
                            {
                                Guid cup = Guid.Parse(e.id);
                                Especialidad_Cup_Entidad ace_ = new Especialidad_Cup_Entidad();
                                ace_.Id_Especialidad = model.Id_Especialidad;
                                ace_.Id_Cups = cup;
                                ace_.Id_Entidad = entidad;
                                ace_.Estado = true;
                                ace_.Usuario_Create = usuario;
                                ace_.Fecha_Create = DateTime.Now;
                                ace_.Usuario_Update = usuario;
                                ace_.Fecha_Update = DateTime.Now;
                                laceModel.Add(ace_);
                            }
                            //Preguntamos las que vienen del modelo que no tengo en bd
                            //var list = ece.Where(t => !laceModel.Any(t2 => t2.Id_Cups == t.Id_Cups && t2.Id_Especialidad == t.Id_Especialidad && t.Id_Entidad == entidad)).ToList();

                            var list = (from t in laceModel where !ece.Any(x => x.Id_Cups == t.Id_Cups && x.Id_Entidad == t.Id_Entidad && x.Id_Especialidad == t.Id_Especialidad) select t).ToList();
                            List<Especialidad_Cup_Entidad> laceNew = new List<Especialidad_Cup_Entidad>();
                            foreach (var ee in list)
                            {
                                Especialidad_Cup_Entidad ace_ = new Especialidad_Cup_Entidad();
                                ace_.Id_Especialidad = model.Id_Especialidad;
                                ace_.Id_Cups = ee.Id_Cups;
                                ace_.Id_Entidad = entidad;
                                ace_.Estado = true;
                                ace_.Usuario_Create = usuario;
                                ace_.Fecha_Create = DateTime.Now;
                                ace_.Usuario_Update = usuario;
                                ace_.Fecha_Update = DateTime.Now;
                                laceNew.Add(ace_);
                            }
                            if (laceNew.Count > 0)
                            {
                                ent.Especialidad_Cup_Entidad.AddRange(laceNew);
                            }
                            List<Especialidad_Cup_Entidad> list_ = ece.Where(t => laceModel.Any(t2 => t2.Id_Cups == t.Id_Cups && t2.Id_Especialidad == t.Id_Especialidad && t.Id_Entidad == entidad)).ToList();
                            foreach (Especialidad_Cup_Entidad ee in list_)
                            {
                                ee.Estado = true;
                                ee.Usuario_Update = usuario;
                                ee.Fecha_Update = DateTime.Now;
                                ent.SaveChanges();
                            }
                            List<Especialidad_Cup_Entidad> listP = ece.Where(t => !laceModel.Any(t2 => t2.Id_Cups == t.Id_Cups && t2.Id_Especialidad == t.Id_Especialidad && t.Id_Entidad == entidad)).ToList();
                            foreach (Especialidad_Cup_Entidad ee in listP)
                            {
                                ee.Estado = false;
                                ee.Usuario_Update = usuario;
                                ee.Fecha_Update = DateTime.Now;
                                ent.SaveChanges();
                            }
                            //ent.SaveChanges();
                            model = new SpecialtyCupModel();
                            rep.data = GetCupXSpecialty(model);
                            //se genera el codigo del mensaje de retorno exitoso
                            return rep = autil.ReturnMesagge(ref rep, 20, string.Empty, null, HttpStatusCode.OK);
                        }
                        else
                        {
                            //ya existen fechas iguales creadas
                            return rep = autil.ReturnMesagge(ref rep, 26, string.Empty, null, HttpStatusCode.OK);
                        }
                    }
                    else
                    {
                        //fallo campos requeridos
                        return autil.ReturnMesagge(ref rep, 33, string.Empty, null, rel, HttpStatusCode.OK);
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    rep = autil.ReturnMesagge(ref rep, 4, ex.Message, null);
                    return rep;
                }
            }
        }

        public object GetCupXSpecialty(SpecialtyCupModel model)
        {
            Response rp = new Response();
            cp = tvh.getprincipal(Convert.ToString(model.token));
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());


                    IQueryable<Especialidad_Cup_Entidad> ece = ent.Especialidad_Cup_Entidad.Where(a => a.Id_Entidad == entidad);

                    //busca por id cups

                    if (model.Id_Especialidad != Guid.Empty)
                    {
                        ece = ece.Where(t => t.Id_Especialidad == model.Id_Especialidad);
                    }
                    var prueba = ece.Where(z => z.Estado == true).ToList();
                    var agenda = ece.Where(c => c.Estado == true).Select(a => new
                    {
                        idspecialty = a.Id_Especialidad,
                        namespecialty = a.Especialidad_Entidad.Especialidad.Nombre,
                        CountCup = (from spcup in ent.Especialidad_Cup_Entidad
                                    where spcup.Id_Especialidad == a.Id_Especialidad && spcup.Id_Entidad == entidad && spcup.Estado == true
                                    select new { spcup.Id_Cups }).ToList().Count,
                        cups = (from spcup in ent.Especialidad_Cup_Entidad
                                where spcup.Id_Especialidad == a.Id_Especialidad && spcup.Id_Entidad == entidad && spcup.Estado == true
                                select new { id = spcup.Id_Cups, value = spcup.Cups.Descripcion }),
                        status = a.Estado

                    }).ToList();
                    var result = agenda.GroupBy(test => test.idspecialty)
                   .Select(grp => grp.First())
                   .ToList();

                    rp.cantidad = result.Where(x=> x.CountCup>0).Count();
                    rp.pagina = 0;
                    rp.data = result.Where(x => x.CountCup > 0);

                    //retorna un response, con el campo data lleno con la respuesta.               
                    return autil.ReturnMesagge(ref rp, 9, null, null, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                //error general
                return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }
        #endregion
    }
}