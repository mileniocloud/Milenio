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
    public class aSchedule
    {
        TokenValidationHandler tvh = new TokenValidationHandler();
        ClaimsPrincipal cp = new ClaimsPrincipal();
        aUtilities autil = new aUtilities();

        #region Agenda Profesional // listo para pruebas
        public object CreateAgendaProfesional(ProfetionalScheduleModel model)
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
                        DateTime fecha_desde = model.Fecha_Desde;
                        DateTime fecha_hasta = model.Fecha_Hasta;

                        List<Agenda_Profesional> lap = ent.Agenda_Profesional
                                                .Where(f =>
                                                f.Id_Profesional == model.Id_Profesional
                                                && f.Id_Entidad == entidad
                                                && f.Estado == true
                                               // && f.Id_Especialidad == model.Id_Especialidad
                                                && (
                                                (f.Fecha_Desde <= fecha_desde && f.Fecha_Hasta >= fecha_desde)
                                                || (f.Fecha_Desde <= fecha_hasta && f.Fecha_Hasta >= fecha_hasta)
                                                || (f.Fecha_Desde >= fecha_desde && f.Fecha_Hasta <= fecha_hasta))
                                                ).ToList();
                        if (lap.Count() == 0)
                        {
                            Agenda_Profesional ap = new Agenda_Profesional();
                            ap.Id_Agenda_Profesional = Guid.NewGuid();
                            ap.Fecha_Desde = fecha_desde;
                            ap.Fecha_Hasta = fecha_hasta;
                          //  ap.Id_Especialidad = model.Id_Especialidad;
                            ap.Id_Profesional = model.Id_Profesional;
                            ap.Id_Entidad = entidad;
                            ap.Estado = true;
                            ap.Fecha_Create = DateTime.Now;
                            ap.Fecha_Update = DateTime.Now;
                            ap.Usuario_Create = usuario;
                            ap.Usuario_Update = usuario;
                            if (model.specialities.Count > 0)
                            {
                                foreach (var e in model.specialities)
                                {
                                    Guid idespeciality = Guid.Parse(e.id);
                                    Especialidad_Entidad ces = ent.Especialidad_Entidad.Where(h => h.Id_Especialidad == idespeciality && h.Id_Entidad == entidad).SingleOrDefault();
                                    ap.Especialidad_Entidad.Add(ces);
                                }
                            }
                            ent.Agenda_Profesional.Add(ap);
                            
                               
                            
                            ent.SaveChanges();
                            model = new ProfetionalScheduleModel();
                            rp.data = GetAgendaProfesional(model);
                            //se genera el codigo del mensaje de retorno exitoso
                            return rp = autil.ReturnMesagge(ref rp, 2, string.Empty, null, HttpStatusCode.OK);
                        }
                        else
                        {
                            //ya existen fechas iguales creadas
                            return rp = autil.ReturnMesagge(ref rp, 26, string.Empty, null, HttpStatusCode.OK);
                        }
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

        public object EditAgendaProfesional(ProfetionalScheduleModel model)
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
                        DateTime fecha_desde = model.Fecha_Desde;
                        DateTime fecha_hasta = model.Fecha_Hasta;
                        List<Agenda_Profesional> lap = ent.Agenda_Profesional
                                                .Where(f =>
                                                f.Id_Profesional == model.Id_Profesional
                                                && f.Id_Agenda_Profesional != model.Id_Agenda_Profesional
                                                && f.Id_Entidad == entidad
                                              //  && f.Estado == true
                                              //  && f.Id_Especialidad == model.Id_Especialidad
                                                && (
                                                (f.Fecha_Desde <= fecha_desde && f.Fecha_Hasta >= fecha_desde)
                                                || (f.Fecha_Desde <= fecha_hasta && f.Fecha_Hasta >= fecha_hasta)
                                                || (f.Fecha_Desde >= fecha_desde && f.Fecha_Hasta <= fecha_hasta))
                                                ).ToList();
                        if (lap.Count() == 0)
                        {
                            Agenda_Profesional ap = ent.Agenda_Profesional.Where(t => t.Id_Agenda_Profesional == model.Id_Agenda_Profesional).Single();
                            ap.Fecha_Desde = fecha_desde;
                            ap.Fecha_Hasta = fecha_hasta;
                           // ap.Id_Especialidad = model.Id_Especialidad;
                            ap.Id_Profesional = model.Id_Profesional;
                            ap.Id_Entidad = entidad;
                            ap.Estado = model.Estado;
                            ap.Fecha_Update = DateTime.Now;
                            ap.Usuario_Update = usuario;
                            List<Especialidad> Ea = (from e in ent.Agenda_Profesional
                                                     from ep in e.Especialidad_Entidad
                                                     where e.Id_Agenda_Profesional == model.Id_Agenda_Profesional
                                                     select ep.Especialidad).ToList();
                            List<ComboListModel> listEa = Ea.Select(l => new ComboListModel
                            {
                                id = l.Id_Especialidad.ToString(),
                                value = l.Nombre

                            }).OrderBy(o => o.value).ToList();
                            if (listEa.Count > 0)
                            {
                                var list = listEa.Where(t => !model.specialities.Any(t2 => t2.id == t.id)).ToList();
                                if (list.Count() > 0)
                                {
                                    foreach (var ee in list)
                                    {
                                        Guid idespeciality = Guid.Parse(ee.id);
                                        Especialidad_Entidad ces = ent.Especialidad_Entidad.Where(h => h.Id_Especialidad == idespeciality && h.Id_Entidad == entidad).SingleOrDefault();
                                        ap.Especialidad_Entidad.Remove(ces);
                                    }
                                }
                            }
                            if (model.specialities.Count > 0)
                            {
                                foreach (var e in model.specialities)
                                {
                                    var listx = listEa.Where(x => x.id == e.id).ToList();
                                    if (listx.Count == 0)
                                    {
                                        Guid idespeciality = Guid.Parse(e.id);
                                        Especialidad_Entidad ces = ent.Especialidad_Entidad.Where(h => h.Id_Especialidad == idespeciality && h.Id_Entidad == entidad).SingleOrDefault();
                                        ap.Especialidad_Entidad.Add(ces);
                                    }
                                }
                            }
                            //ent.Agenda_Profesional.Add(ap);
                            ent.SaveChanges();
                            model = new ProfetionalScheduleModel();
                            rep.data = GetAgendaProfesional(model);
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

        public object GetAgendaProfesional(ProfetionalScheduleModel model)
        {
            Response rp = new Response();
            cp = tvh.getprincipal(Convert.ToString(model.token));
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());


                    IQueryable<Agenda_Profesional> ap = ent.Agenda_Profesional.Where(a => a.Id_Entidad == entidad);

                    //busca por id profesional
                    if (model.Id_Profesional != Guid.Empty)
                    {
                        ap = ap.Where(t => t.Id_Profesional == model.Id_Profesional);
                    }

                    ////por especialidad
                    //if (model.Id_Especialidad != Guid.Empty)
                    //{
                    //    ap = ap.Where(t => t.Id_Especialidad == model.Id_Especialidad);
                    //}

                    //entre dos fechas
                    if (model.between)
                    {
                        if (model.Fecha_Desde.Year > 2000 && model.Fecha_Hasta.Year > 2000)
                        {
                            DateTime fecha_desde = model.Fecha_Desde;
                            DateTime fecha_hasta = model.Fecha_Hasta;
                            ap = ap.Where(f =>
                                        f.Fecha_Desde <= fecha_desde && f.Fecha_Hasta >= fecha_desde
                                        || f.Fecha_Desde <= fecha_hasta && f.Fecha_Hasta >= fecha_hasta
                                        || f.Fecha_Desde >= fecha_desde && f.Fecha_Hasta <= fecha_hasta);
                        }
                    }
                    else
                    {
                        //PARA BUSCAR POR FECHAS EN ESPECIFICO
                        if (model.Fecha_Desde.Year > 2000)
                        {
                            ap = ap.Where(t => t.Fecha_Desde == model.Fecha_Desde);
                        }

                        if (model.Fecha_Hasta.Year > 2000)
                        {
                            ap = ap.Where(t => t.Fecha_Hasta == model.Fecha_Hasta);
                        }
                    }

                    var agenda = ap.Select(a => new
                    {
                        idschedule = a.Id_Agenda_Profesional,
                        fromdate = a.Fecha_Desde,
                        todate = a.Fecha_Hasta,
                       // idspeciality = a.Id_Especialidad,
                        idprofetional = a.Id_Profesional,
                        status = a.Estado,
                       // speciality = a.Especialidad_Entidad.Especialidad.Nombre,
                        profetional = a.Usuario.Nombres + " " + a.Usuario.Primer_Apellido + " " + a.Usuario.Segundo_Apellido,
                        cantidad = (from horary in ent.Horario_Agenda
                                    join detalle in ent.Detalle_Agenda on horary.Id_Horario_Agenda equals detalle.Id_Horario_Agenda
                                    where horary.Id_Agenda_Profesional == a.Id_Agenda_Profesional
                                    select new
                                    {
                                        horary.Id_Agenda_Profesional
                                    }).FirstOrDefault(),
                        //specialities = ent.Especialidad_Entidad.Where(x => x.Id_Entidad == entidad).Select(g => new { id = g.Especialidad.Id_Especialidad, value = g.Especialidad.Nombre })


                    }).ToList();

                    rp.cantidad = agenda.Count();
                    rp.pagina = 0;
                    rp.data = agenda;

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

        #region Horario agenda // sale a pruebas

        public object CreateScheduleAgenda(ScheduleAgendaModel model)
        {
            Response rp = new Response();
            List<ErrorFields> el = new List<ErrorFields>();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                    //validando los campos requeridos
                    List<ErrorFields> rel = autil.ValidateObject(model);
                    if (rel.Count == 0)
                    {
                        //acamos el ID de la especialidad que se esta manejando
                        //Guid id_especialidad = Guid.Empty;//ent.Agenda_Profesional.Where(a => a.Id_Agenda_Profesional == model.Id_Agenda_Profesional).Select(g => g.Id_Especialidad).SingleOrDefault();
                        //validamos que el consultorio seleccionado tenga esa especialidad asignada
                        //int ve = ent.Consultorio_Especialidad.Where(c => c.Id_Especialidad == id_especialidad && c.Id_Consultorio == model.Id_Consultorio && c.Id_Entidad == entidad).Count();

                        //si trae algo es porque ese consultorio si contiene esa especialidad
                        //if (ve != 0)
                        //{
                            //se saca la cantidad de minutos disponibles
                            TimeSpan td = model.Hora_Hasta.Subtract(model.Hora_Desde);
                            //se divide la cantidad de minutos disponibles para saber cuantas citas puede atender
                            double cant_consultas = td.TotalMinutes / model.Duracion;

                            //aqui validamos si la cantidad de citas que puede atender es entera, es decir
                            //que puede atender citas completas durante esas horas y ninguna cita queda incompleta
                            if (Math.Abs(cant_consultas % 1) <= (Double.Epsilon * 100))
                            {
                                DateTime hdesde = model.Hora_Desde;
                                DateTime hhasta = model.Hora_Hasta;

                                List<Horario_Agenda> lha = ent.Horario_Agenda
                                     .Where(a => a.Id_Agenda_Profesional == model.Id_Agenda_Profesional
                                     && a.Dia == model.Dia
                                     && a.Id_Consultorio == model.Id_Consultorio
                                     && a.Estado == true
                                     && (
                                     (a.Hora_Desde < hdesde && a.Hora_Hasta > hdesde)//cuando la hora desde que envian esta dentro de un rango
                                     || (a.Hora_Desde < hhasta && a.Hora_Hasta > hhasta) //cuando la hora hasta que envian esta dentro de un rango
                                     || (a.Hora_Desde >= hdesde && a.Hora_Hasta <= hhasta))//cuando los rangos enviados son mayores que los rangos creados o son iguales
                                     ).ToList();

                                if (lha.Count() == 0)
                                {
                                    Horario_Agenda ha = new Horario_Agenda();
                                    ha.Id_Horario_Agenda = Guid.NewGuid();
                                    ha.Id_Agenda_Profesional = model.Id_Agenda_Profesional;
                                    ha.Hora_Desde = hdesde;
                                    ha.Hora_Hasta = hhasta;
                                    ha.Dia = model.Dia;
                                    ha.Id_Consultorio = model.Id_Consultorio;
                                    ha.Estado = true;
                                    ha.Duracion = model.Duracion;
                                    ha.Fecha_Create = DateTime.Now;
                                    ha.Fecha_Update = DateTime.Now;
                                    ha.Usuario_Create = usuario;
                                    ha.Usuario_Update = usuario;
                                    ent.Horario_Agenda.Add(ha);
                                    ent.SaveChanges();
                                //se genera el codigo del mensaje de retorno exitoso
                                ScheduleAgendaModel model_ = new ScheduleAgendaModel();
                                model_.Id_Agenda_Profesional = model.Id_Agenda_Profesional;
                                rp.data = GetScheduleAgenda(model_);
                                return rp = autil.ReturnMesagge(ref rp, 2, string.Empty, null);
                                }
                                else
                                {
                                    //fechas iguales
                                    rp = autil.ReturnMesagge(ref rp, 40, string.Empty, null);
                                    return rp;
                                }
                            }
                            else
                            {
                                //se avisa que la cantidad de consultas disponible no coinciden
                                rp = autil.ReturnMesagge(ref rp, 38, string.Empty, null, HttpStatusCode.OK);
                                return rp;
                            }
                        //}
                        //else
                        //{
                        //    //se avisa que el consultorio no contiene esa especialidad asignada
                        //    rp = autil.ReturnMesagge(ref rp, 39, string.Empty, null, HttpStatusCode.OK);
                        //    return rp;
                        //}
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
                    rp = autil.ReturnMesagge(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                    return rp;
                }
            }
        }

        public object EditScheduleAgenda(ScheduleAgendaModel model)
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
                        //acamos el ID de la especialidad que se esta manejando
                        var ep = ent.Agenda_Profesional.Where(e => e.Id_Agenda_Profesional == model.Id_Agenda_Profesional).Select(s => s.Especialidad_Entidad).ToList();
                        //Guid id_especialidad = Guid.Empty;//ent.Agenda_Profesional.Where(a => a.Id_Agenda_Profesional == model.Id_Agenda_Profesional).Select(g => g.Id_Especialidad).SingleOrDefault();
                        ////validamos que el consultorio seleccionado tenga esa especialidad asignada
                        //int ve = ent.Consultorio_Especialidad.Where(c => c.Id_Especialidad == id_especialidad && c.Id_Consultorio == model.Id_Consultorio && c.Id_Entidad == entidad).Count();
                        //if (ve != 0)
                        //{
                            //se saca la cantidad de minutos disponibles
                            TimeSpan td = model.Hora_Hasta.Subtract(model.Hora_Desde);
                            //se divide la cantidad de minutos disponibles para saber cuantas citas puede atender
                            double cant_consultas = td.TotalMinutes / model.Duracion;

                            //aqui validamos si la cantidad de citas que puede atender es entera, es decir
                            //que puede atender citas completas durante esas horas y ninguna cita queda incompleta
                            if (Math.Abs(cant_consultas % 1) <= (Double.Epsilon * 100))
                            {
                                DateTime hdesde = model.Hora_Desde;
                                DateTime hhasta = model.Hora_Hasta;

                                List<Horario_Agenda> lha = ent.Horario_Agenda
                                     .Where(a => a.Id_Agenda_Profesional == model.Id_Agenda_Profesional
                                     && a.Id_Horario_Agenda != model.Id_Horario_Agenda
                                     && a.Id_Consultorio == model.Id_Consultorio
                                     && a.Estado == true
                                     && a.Dia == model.Dia
                                     //&& (
                                     //    (a.Hora_Desde < hdesde && a.Hora_Hasta > hdesde)//cuando la hora desde que envian esta dentro de un rango
                                     //    || (a.Hora_Desde < hhasta && a.Hora_Hasta > hhasta) //cuando la hora hasta que envian esta dentro de un rango
                                     //    || (a.Hora_Desde >= hdesde && a.Hora_Hasta <= hhasta))//cuando los rangos enviados son mayores que los rangos creados o son iguales
                                         ).ToList();

                                if (lha.Count() == 0)
                                {
                                    Horario_Agenda ha = ent.Horario_Agenda.Where(t => t.Id_Horario_Agenda == model.Id_Horario_Agenda).SingleOrDefault();
                                    ha.Hora_Desde = hdesde;
                                    ha.Hora_Hasta = hhasta;
                                    ha.Dia = model.Dia;
                                    ha.Estado = model.Estado;
                                    ha.Id_Consultorio = model.Id_Consultorio;
                                    ha.Duracion = model.Duracion;
                                    ha.Fecha_Update = DateTime.Now;
                                    ha.Usuario_Update = usuario;

                                    ent.SaveChanges();
                                ScheduleAgendaModel model_ = new ScheduleAgendaModel();
                                model_.Id_Agenda_Profesional = model.Id_Agenda_Profesional;
                                rp.data = GetScheduleAgenda(model_);
                                //se genera el codigo del mensaje de retorno exitoso
                                return rp = autil.ReturnMesagge(ref rp, 20, string.Empty, null);
                                }
                                else
                                {
                                    //fechas iguales
                                    rp = autil.ReturnMesagge(ref rp, 40, string.Empty, null);
                                    return rp;
                                }
                            }
                            else
                            {
                                //se avisa que la cantidad de consultas disponible no coinciden
                                rp = autil.ReturnMesagge(ref rp, 38, string.Empty, null, HttpStatusCode.OK);
                                return rp;
                            }
                        //}
                        //else
                        //{
                        //    //se avisa que el consultorio no contiene esa especialidad asignada
                        //    rp = autil.ReturnMesagge(ref rp, 39, string.Empty, null, HttpStatusCode.OK);
                        //    return rp;
                        //}
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
                    rp = autil.ReturnMesagge(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                    return rp;
                }
            }
        }
        public object GetScheduleAgenda(ScheduleAgendaModel model)
        {
            Response rp = new Response();
            cp = tvh.getprincipal(Convert.ToString(model.token));
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());


                    IQueryable<Horario_Agenda> ap = ent.Horario_Agenda.Where(a => a.Id_Agenda_Profesional == model.Id_Agenda_Profesional);

                    //busca por hora desde
                    if (model.Hora_Desde.Year > 2000)
                    {
                        ap = ap.Where(t => t.Hora_Desde == model.Hora_Desde);
                    }
                    //busca por hora hasta
                    if (model.Hora_Hasta.Year > 2000)
                    {
                        ap = ap.Where(t => t.Hora_Hasta == model.Hora_Hasta);
                    }
                    //busca por dia
                    if (model.Dia != string.Empty)
                    {
                        ap = ap.Where(t => t.Dia == model.Dia);
                    }
                    //busca por duracion
                    if (model.Duracion > 0)
                    {
                        ap = ap.Where(t => t.Duracion == model.Duracion);
                    }
                    //busca por consultorio
                    if (model.Id_Consultorio != Guid.Empty)
                    {
                        ap = ap.Where(t => t.Id_Consultorio == model.Id_Consultorio);
                    }


                    List<ScheduleAgendaModel> agenda = ap.Select(a => new ScheduleAgendaModel
                    {
                        Id_Horario_Agenda = a.Id_Horario_Agenda,
                        Id_Agenda_Profesional = a.Id_Agenda_Profesional,
                        Hora_Desde = a.Hora_Desde,
                        Hora_Hasta = a.Hora_Hasta,
                        Dia = a.Dia,
                        Duracion = a.Duracion,
                        Id_Consultorio = a.Id_Consultorio,
                        Consultorio = a.Consultorio.Nombre,
                        Estado = a.Estado,
                        cantidad = a.Detalle_Agenda.Count()
                   

                    }).ToList();
                    //List<ScheduleAgendaModel> list = new List<ScheduleAgendaModel>();
                    //foreach (Models.ScheduleAgendaModel mc in list)
                    //{
                    //    mc.Hora_Desde = Convert. mc.Hora_Desde
                    //}

                    rp.cantidad = agenda.Count();
                    rp.pagina = 0;
                    rp.data = agenda;

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

        #region Detalle Agenda // sale a pruebas

        public object CreateScheduleDetail(ProfetionalScheduleModel _model)
        {
            Response rp = new Response();
            List<ErrorFields> er = new List<ErrorFields>();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(_model.token));

                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                    List<Horario_Agenda> m = ent.Horario_Agenda.Where(i => i.Id_Agenda_Profesional == _model.Id_Agenda_Profesional).ToList();

                    bool fallavalidacion = false;
                    if (m.Count() > 0)
                    {
                        foreach (var ha in m)
                        {
                            if (fallavalidacion)
                                break;

                            //seteamos las horas con la fehca de hoy para que solo tome la parte de tiempo y no fecha.
                            DateTime t1 = DateTime.Today.AddHours(ha.Hora_Desde.Hour).AddMinutes(ha.Hora_Desde.Minute);
                            DateTime t2 = DateTime.Today.AddHours(ha.Hora_Hasta.Hour).AddMinutes(ha.Hora_Hasta.Minute);

                            //se saca la cantidad de minutos disponibles
                            TimeSpan td = t2.Subtract(t1);
                            //se divide la cantidad de minutos disponibles para saber cuantas citas puede atender
                            double cant_consultas = td.TotalMinutes / ha.Duracion;

                            //consultamos el nombre del consultorio y de la especialidad
                            string especialidad = "";//ha.Agenda_Profesional.Especialidad_Entidad.Especialidad.Nombre;
                            string consultorio = ent.Consultorio.Where(t => t.Id_Consultorio == ha.Id_Consultorio).Select(s => s.Nombre).SingleOrDefault();
                            Guid id_especialidad = Guid.Empty;//ha.Agenda_Profesional.Especialidad_Entidad.Especialidad.Id_Especialidad;

                            //lista donde se almacenan los errores
                            List<DateTime> fechas = new List<DateTime>();

                            //aqui validamos si la cantidad de citas que puede atender es entera, es decir
                            //que puede atender citas completas durante esas horas y ninguna cita queda incompleta
                            if (Math.Abs(cant_consultas % 1) <= (Double.Epsilon * 100))
                            {
                                for (DateTime date = ha.Agenda_Profesional.Fecha_Desde; date.Date <= ha.Agenda_Profesional.Fecha_Hasta; date = date.AddDays(1))
                                {
                                    if (fallavalidacion)
                                        break;

                                    if (traslateDay(date.DayOfWeek.ToString()) == ha.Dia)
                                    {
                                        //se setean variables con la fecha de la cita y las horas.
                                        DateTime hdesde = new DateTime(date.Year, date.Month, date.Date.Day, ha.Hora_Desde.Hour, ha.Hora_Desde.Minute, 0);
                                        DateTime hhasta = new DateTime(date.Year, date.Month, date.Date.Day, ha.Hora_Hasta.Hour, ha.Hora_Hasta.Minute, 0);

                                        for (int i = 0; i < cant_consultas; i++)
                                        {
                                            //se coloca como hora hasta, la hora desde mas la porcion de tiempo de la cita
                                            hhasta = hdesde.AddMinutes(ha.Duracion);

                                            Detalle_Agenda da = ent.Detalle_Agenda.Where(
                                                            d => d.Horario_Agenda.Agenda_Profesional.Id_Entidad == entidad
                                                            && d.Fecha == date
                                                            && d.Horario_Agenda.Id_Consultorio == ha.Id_Consultorio
                                                            // && d.Horario_Agenda.Agenda_Profesional.Id_Especialidad == id_especialidad
                                                            && (
                                                            (d.Hora_Desde <= hdesde && d.Hora_Hasta >= hhasta)
                                                            || (d.Hora_Desde <= hdesde && d.Hora_Hasta >= hhasta)
                                                            || (d.Hora_Desde >= hdesde && d.Hora_Hasta <= hhasta))
                                                            ).SingleOrDefault();

                                            if (da == null)
                                            {
                                                da = new Detalle_Agenda();
                                                da.Id_Detalle_Agenda = Guid.NewGuid();
                                                da.Id_Horario_Agenda = ha.Id_Horario_Agenda;
                                                da.Hora_Desde = hdesde;
                                                da.Hora_Hasta = hhasta;
                                                da.Fecha = date;
                                                da.Asignada = false;
                                                da.Fecha_Create = DateTime.Now;
                                                da.Fecha_Update = DateTime.Now;
                                                da.Usuario_Create = usuario;
                                                da.Usuario_Update = usuario;

                                                ent.Detalle_Agenda.Add(da);
                                                //se coloca como hora desde, la hora hasta donde se termino la cita anterior.
                                                hdesde = hhasta;
                                            }
                                            else
                                            {
                                                autil.GetAgendaErrorDetail(ref er, 29, "Detalle Agenda", date, hdesde, hhasta, consultorio, especialidad);
                                                //se coloca como hora desde, la hora hasta donde se termino la cita anterior.
                                                // se coloca aqui tambien por si da error, mueva las horas
                                                // hdesde = hhasta;
                                                fallavalidacion = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //se coloca aqui porque si falla alguna validacion, no se guardara nada
                        if (!fallavalidacion)
                            ent.SaveChanges();
                        var model = new ProfetionalScheduleModel();
                        rp.data = GetAgendaProfesional(model);
                        rp = autil.ReturnMesagge(ref rp, 2, string.Empty, null);
                    }
                    else
                    {
                        rp = autil.ReturnMesagge(ref rp, 43, string.Empty, null);
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    rp = autil.ReturnMesagge(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                }
            }
            
            rp.data = er;
            return rp;
        }
        private string traslateDay(string day) {
            switch (day)
            {
                case "Monday":
                    return "Lunes";
                case "Tuesday":
                    return "Martes";
                case "Wednesday":
                    return "Miercoles";
                case "Thursday":
                    return "Jueves";
                case "Friday":
                    return "Viernes";
                case "Saturday":
                    return "Sábado";
                case "Sunday":
                    return "Domingo";
                default:
                    return day;
            }
        }
        #endregion

        #region Disponibilidad // sale a pruebas

        public object GetAppointment(AppointmentModel model)
        {
            Response rep = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    Guid entidad = model.id;
                    List<ErrorFields> rel = autil.ValidateObject(model);
                    if (rel.Count == 0)
                    {
                        if (model.Mes == 0)
                            model.Mes = DateTime.Today.Month;

                        //List<Consultorio_Especialidad> lce = (from ce in ent.Consultorio_Especialidad
                        //                                      where
                        //                                      ce.Estado == true
                        //                                      && ce.Consultorio.Estado == true
                        //                                      && ce.Id_Especialidad == model.Id_Especialidad
                        //                                      && ce.Id_Entidad == entidad
                        //                                      select ce).ToList();


                        List<Detalle_Agenda> ldet = (from ap in ent.Agenda_Profesional
                                                     from ha in ap.Horario_Agenda
                                                     from da in ha.Detalle_Agenda
                                                     //join ce in ent.Consultorio_Especialidad
                                                     //on new { ap.Id_Especialidad, ap.Id_Entidad }
                                                     //equals new { ce.Id_Especialidad, ce.Id_Entidad }
                                                     where
                                                     ap.Estado == true
                                                     && ap.Id_Entidad == entidad
                                                     && ha.Estado == true
                                                     && da.Asignada == false
                                                     && da.Fecha >= DateTime.Today
                                                     && da.Fecha.Month == model.Mes
                                                     //&& ap.Especialidad_Entidad.Estado == true
                                                     //&& ce.Consultorio.Estado == true
                                                     //&& ce.Estado == true
                                                     select da).OrderBy(d => d.Hora_Desde).ToList();                       

                        List<CalendarModel> lcm = new List<CalendarModel>();
                        foreach (var i in ldet.GroupBy(g => new { g.Fecha, g.Hora_Desde, g.Hora_Hasta, /*g.Horario_Agenda.Agenda_Profesional.Id_Especialidad*/ }))
                        {
                            CalendarModel cm = new CalendarModel();
                            cm.title = i.Key.Fecha.ToString("dd/MM/yyyy") + " " + i.Key.Hora_Desde.ToString("HH:mm");
                            cm.fecha = i.Key.Fecha.ToString("dd/MM/yyyy");
                            cm.start = i.Key.Fecha.ToString("yyyy-MM-dd") + " " + i.Key.Hora_Desde.ToString("HH:mm");
                            cm.end = i.Key.Fecha.ToString("yyyy-MM-dd") + " " + i.Key.Hora_Hasta.ToString("HH:mm");
                            cm.draggable = "false";
                            cm.resizable.afterEnd = "true";
                            cm.resizable.beforeStart = "true";
                            cm.color.primary = "#ad2121";
                            cm.color.secondary = "#FAE3E3";
                            //cm.profetional = ldet.Where(d => d.Fecha == i.Key.Fecha && d.Hora_Desde == i.Key.Hora_Desde && d.Hora_Hasta == i.Key.Hora_Hasta && d.Horario_Agenda.Agenda_Profesional.Id_Especialidad == i.Key.Id_Especialidad)
                                //.Select(u => new ComboModel
                                //{
                                //    id = u.Id_Detalle_Agenda,
                                //    value = u.Horario_Agenda.Agenda_Profesional.Usuario.Nombres + " " + u.Horario_Agenda.Agenda_Profesional.Usuario.Primer_Apellido + " " + u.Horario_Agenda.Agenda_Profesional.Usuario.Segundo_Apellido
                                //}).ToList();

                            lcm.Add(cm);
                        }

                        lcm = lcm.OrderBy(t => t.start).ToList();
                        if (lcm.Count() != 0)
                        {
                            //si tiene agenda e infrorma los resultados
                            rep.data = lcm;
                            autil.ReturnMesagge(ref rep, 41, string.Empty, null, rel, HttpStatusCode.OK);
                            List<string> list = new List<string>();
                            list.Add(lcm[0].fecha);
                            rep.message = autil.ReplaceCustomMesaggeText(list, rep.message);
                            return rep;
                        }
                        else
                        {
                            //informa que no hay disponibilidad
                            return autil.ReturnMesagge(ref rep, 42, string.Empty, null, rel, HttpStatusCode.OK);
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
                    throw ex;
                }
            }
        }

        #endregion

        #region Citas

        public object TakeAppointment(AppointmentModel model)
        {
            Response rep = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    Guid entidad = model.id;
                    List<ErrorFields> rel = autil.ValidateObject(model);
                    if (rel.Count == 0)
                    {
                        Guid idcup = ent.Especialidad_Cup_Entidad.Where(g => g.Id_Especialidad == model.Id_Especialidad && g.Id_Entidad == model.Id_Entidad && g.Cups.Codigo == model.Codigo_Cup).Select(t => t.Id_Cups).SingleOrDefault();

                        Cita c = new Cita();
                        c.Id_Cita = Guid.NewGuid();
                        c.Id_Entidad = model.Id_Entidad;
                        c.Id_Especialidad = model.Id_Especialidad;
                        c.Id_Detalle_Agenda = model.Id_Detalle_Agenda;
                        c.Id_Cup = idcup;

                        return rep;
                    }
                    else
                    {
                        //fallo campos requeridos
                        return autil.ReturnMesagge(ref rep, 33, string.Empty, null, rel, HttpStatusCode.OK);
                    }

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        #endregion
    }
}
