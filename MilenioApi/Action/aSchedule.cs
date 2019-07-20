using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
                        rp = autil.ReturnMesagge(ref rp, 49, string.Empty, null);
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    rp = autil.ReturnMesagge(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                }
            }

            //rp.data = er;
            return rp;
        }
        public object GetScheduleDetail(ScheduleDetailModel model)
        {
            Response rp = new Response();
            cp = tvh.getprincipal(Convert.ToString(model.token));
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
                    Horario_Agenda ap = ent.Horario_Agenda.Where(a => a.Id_Agenda_Profesional == model.Id_Agenda_Profesional).SingleOrDefault();
                    //DateTime horaIni = model.Fecha_Desde.Date + model.Hora_Desde.TimeOfDay;
                    //DateTime horaFin = model.Fecha_Hasta.Date + model.Hora_Hasta.TimeOfDay;
                    IQueryable<Detalle_Agenda> dt = ent.Detalle_Agenda.Where(z => z.Id_Horario_Agenda == ap.Id_Horario_Agenda);
                    //busca por duracion
                    if (model.Fecha_Desde != null && model.Fecha_Desde.Year>2000)
                    {
                        dt = dt.Where(t => t.Fecha >= model.Fecha_Desde);
                    }
                    if (model.Fecha_Hasta != null && model.Fecha_Hasta.Year > 2000)
                    {
                        dt = dt.Where(t => t.Fecha <= model.Fecha_Hasta);
                    }
                    //if (model.Hora_Desde != null && model.Hora_Desde.Year > 2000)
                    //{
                    //    dt = dt.Where(t => t.Hora_Desde >= horaIni);
                    //}
                    //if (model.Hora_Hasta != null && model.Hora_Hasta.Year > 2000)
                    //{
                    //    dt = dt.Where(t => t.Hora_Hasta <= t.Hora_Hasta);
                    //}
                    List<ScheduleDetailModel> detalleAgenda = dt.Select(a => new ScheduleDetailModel
                    {
                        Id_Agenda_Profesional = a.Horario_Agenda.Id_Agenda_Profesional,
                        Id_Detalle_Agenda = a.Id_Detalle_Agenda,
                        Id_Horario_Agenda = a.Id_Horario_Agenda,
                        Hora_Desde = a.Hora_Desde,
                        Hora_Hasta = a.Hora_Hasta,
                        Fecha = a.Fecha,
                        Id_Consultorio=a.Horario_Agenda.Id_Consultorio,
                        Name_Consultorio = a.Horario_Agenda.Consultorio.Nombre


                    }).OrderBy(s=> s.Fecha).ThenBy(n => n.Hora_Desde).ToList();
                    var detalle = new List<object>();
                    foreach (var item in detalleAgenda)
                    {
                        if (model.Hora_Desde != null && model.Hora_Desde.Year > 2000)
                        {
                            if (item.Hora_Desde.TimeOfDay >= model.Hora_Desde.TimeOfDay)
                            {
                                if (model.Hora_Hasta != null && model.Hora_Hasta.Year > 2000)
                                {
                                    if (item.Hora_Hasta.TimeOfDay <= model.Hora_Hasta.TimeOfDay)
                                    {
                                        detalle.Add(item);
                                    }
                                }
                                else
                                {
                                    detalle.Add(item);
                                }
                            }
                        }
                        else {
                            if (model.Hora_Hasta != null && model.Hora_Hasta.Year > 2000)
                            {
                                if (item.Hora_Hasta.TimeOfDay <= model.Hora_Hasta.TimeOfDay)
                                {
                                    detalle.Add(item);
                                }
                            }
                            else {
                                detalle.Add(item);
                            }
                        }
                    }
                    rp.cantidad = detalle.Count();
                    rp.pagina = 0;
                    rp.data = detalle;

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

        public object EditProfesionalScheduleDetail(List<ScheduleDetailModel> model) {
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                List<string> lsError = new List<string>();
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model[0].token));
                    
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
                    foreach (var item in model)
                    {
                        Detalle_Agenda da = ent.Detalle_Agenda.Where(x => x.Id_Detalle_Agenda == item.Id_Detalle_Agenda).SingleOrDefault();
                        List<Detalle_Agenda> dapro = ent.Detalle_Agenda.Where(d => d.Id_Suplente == item.id_Suplente
                        && d.Fecha == da.Fecha && d.Hora_Desde == da.Hora_Desde && d.Hora_Hasta == da.Hora_Hasta).ToList();

                        List<Agenda_Profesional> lap = (from da_ in ent.Detalle_Agenda
                                   join ha_ in ent.Horario_Agenda on da_.Id_Horario_Agenda equals ha_.Id_Horario_Agenda
                                   join ap in ent.Agenda_Profesional on ha_.Id_Agenda_Profesional equals ap.Id_Agenda_Profesional
                                   where da_.Id_Detalle_Agenda == item.Id_Detalle_Agenda
                                   && ap.Id_Profesional == item.id_Suplente && ap.Fecha_Desde >= da.Fecha && ap.Fecha_Hasta <= da.Fecha &&
                                   ha_.Hora_Desde >= da.Hora_Desde && ha_.Hora_Hasta <= da.Hora_Hasta
                                   select (
                                   ap

                                   )
                        ).ToList();
                        if (lap.Count == 0 && dapro.Count == 0)
                        {
                            da.Id_Suplente = item.id_Suplente;
                            ent.SaveChanges();
                        }
                        else {
                            lsError.Add("Fecha: " + da.Fecha.ToShortDateString() + " con hora inicial: " + da.Hora_Desde.ToLongTimeString() + " y con hora final: " + da.Hora_Hasta.ToLongTimeString() + " no fue posible asignar al profesional elegido ya que tiene una agenda propuesta o generada para estos registros");
                        }
                    }
                }
                
                catch (Exception ex) {
                    rp = autil.ReturnMesagge(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                }
                if (lsError.Count == 0)
                {
                    //se genera el codigo del mensaje de retorno exitoso
                    return rp = autil.ReturnMesagge(ref rp, 20, string.Empty, null);
                }
                else
                {
                    rp.data = lsError;
                    //retorna un response, con los datos que no fueron modificados en data.               
                    return autil.ReturnMesagge(ref rp, 9, null, null, HttpStatusCode.OK);
                }
            }
        }
        private string traslateDay(string day)
        {
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

                        DateTime ft = DateTime.Now.AddMonths(2);


                        //List<Consultorio> ce = ent.Consultorio_Especialidad.Where(c => c.Estado == true && c.Id_Especialidad == model.Id_Especialidad && c.Id_Entidad == entidad && c.Consultorio.Estado == true).Select(O => O.Consultorio).ToList();
                        List<Guid> ce = ent.Consultorio_Especialidad.Where(c => c.Estado == true && c.Id_Especialidad == model.Id_Especialidad && c.Id_Entidad == entidad && c.Consultorio.Estado == true).Select(O => O.Consultorio.Id_Consultorio).ToList();

                        List<Detalle_Agenda> ldet = (from ee in ent.Especialidad_Entidad
                                                     from ap in ee.Agenda_Profesional
                                                     from ha in ap.Horario_Agenda
                                                     from da in ha.Detalle_Agenda
                                                     where
                                                     ee.Id_Especialidad == model.Id_Especialidad
                                                     && ee.Id_Entidad == entidad
                                                     && ap.Estado == true
                                                     && ha.Estado == true
                                                     && da.Asignada == false
                                                     && da.Fecha >= DateTime.Today
                                                     && da.Fecha <= ft
                                                     && ce.Contains(ha.Consultorio.Id_Consultorio)
                                                     select da).OrderBy(d => d.Hora_Desde).ToList();


                        List<CalendarModel> lcm = new List<CalendarModel>();
                        // consultamos todos los usuarios de esa entidad que son medicos
                        List<Usuario> user = ent.Entidad_Usuario.Where(i => i.Id_Entidad == model.id && i.Usuario.Presta_Servicio == true).Select(r => r.Usuario).ToList();
                        foreach (var i in ldet.GroupBy(g => new { g.Fecha, g.Hora_Desde, g.Hora_Hasta }))
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
                            Guid? idsuplente = ldet.Where(d => d.Fecha == i.Key.Fecha && d.Hora_Desde == i.Key.Hora_Desde && d.Hora_Hasta == i.Key.Hora_Hasta).Select(y => y.Id_Suplente).SingleOrDefault();

                            //si el id de suplente llega vacio retornamos los datos del doctor que originalmente atendera
                            if (idsuplente == null)
                            {
                                cm.profetional = ldet.Where(d => d.Fecha == i.Key.Fecha && d.Hora_Desde == i.Key.Hora_Desde && d.Hora_Hasta == i.Key.Hora_Hasta)
                                .Select(u => new ComboModel
                                {
                                    id = u.Id_Detalle_Agenda,
                                    value = u.Horario_Agenda.Agenda_Profesional.Usuario.Nombres + " " + u.Horario_Agenda.Agenda_Profesional.Usuario.Primer_Apellido + " " + u.Horario_Agenda.Agenda_Profesional.Usuario.Segundo_Apellido
                                }).ToList();
                            }
                            else
                            {
                                string profesional = user.Where(j => j.Id_Usuario == idsuplente).Select(f => f.Nombres + " " + f.Primer_Apellido + " " + f.Segundo_Apellido).SingleOrDefault();
                                //si existe un id de suplente mostramos esos datos del doctor que sera el que atiende ahora
                                cm.profetional = ldet.Where(d => d.Fecha == i.Key.Fecha && d.Hora_Desde == i.Key.Hora_Desde && d.Hora_Hasta == i.Key.Hora_Hasta)
                                .Select(u => new ComboModel
                                {
                                    id = u.Id_Detalle_Agenda,
                                    value = profesional
                                }).ToList();
                            }

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

        public object GetAppointmentByFilter(AppointmentModel model)
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
                        if (model.Mes == 0)
                            model.Mes = DateTime.Today.Month;

                        DateTime ft = new DateTime(DateTime.Today.Year, model.Mes, 1);

                        List<Guid> ce = ent.Consultorio_Especialidad.Where(c => c.Estado == true && c.Id_Especialidad == model.Id_Especialidad && c.Id_Entidad == entidad && c.Consultorio.Estado == true).Select(O => O.Consultorio.Id_Consultorio).ToList();

                        List<Detalle_Agenda> ldet = new List<Detalle_Agenda>();

                        IQueryable<Detalle_Agenda> ldet2 = (from ee in ent.Especialidad_Entidad
                                                            from ap in ee.Agenda_Profesional
                                                            from ha in ap.Horario_Agenda
                                                            from da in ha.Detalle_Agenda
                                                            where
                                                            ee.Id_Especialidad == model.Id_Especialidad
                                                            && ee.Id_Entidad == entidad
                                                            && ap.Estado == true
                                                            && ha.Estado == true
                                                            && da.Asignada == model.tipo_consulta
                                                            && da.Fecha.Month == ft.Month
                                                            && ce.Contains(ha.Consultorio.Id_Consultorio)
                                                            select da);

                        if (model.tipo_consulta == true)
                            ldet2.Select(m => m.Cita.Where(n => n.Confirmada == true));

                        //si bucan por profesional
                        if (model.Id_Profetional != null && model.Id_Profetional != Guid.Empty)
                        {
                            //agenda del medico;
                            ldet2 = ldet2.Where(u => u.Id_Suplente == null && u.Horario_Agenda.Agenda_Profesional.Id_Profesional == model.Id_Profetional);
                            //agenda del medico cuando es suplente
                            ldet2 = ldet2.Where(u => u.Id_Suplente == model.Id_Profetional);
                        }

                        //si buscan por paciente
                        if (!string.IsNullOrEmpty(model.Id_Tipo_Identificacion) && !string.IsNullOrEmpty(model.Numero_Identificacion))
                        {
                            ldet2 = (from b in ldet2
                                     from t in b.Cita
                                     where t.Paciente.Id_Tipo_Identificacion == model.Id_Tipo_Identificacion
                                     && t.Paciente.Numero_Identificacion == model.Numero_Identificacion
                                     select b);
                        }

                        //llevamos el iquereable a una lista completa
                        ldet = ldet2.ToList();

                        List<CalendarModel> lcm = new List<CalendarModel>();
                        // consultamos todos los usuarios de esa entidad que son medicos
                        List<Usuario> user = ent.Entidad_Usuario.Where(i => i.Id_Entidad == entidad && i.Usuario.Presta_Servicio == true).Select(r => r.Usuario).ToList();
                        foreach (var i in ldet.GroupBy(g => new { g.Fecha, g.Hora_Desde, g.Hora_Hasta }))
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
                            Guid? idsuplente = ldet.Where(d => d.Fecha == i.Key.Fecha && d.Hora_Desde == i.Key.Hora_Desde && d.Hora_Hasta == i.Key.Hora_Hasta).Select(y => y.Id_Suplente).SingleOrDefault();

                            //si el id de suplente llega vacio retornamos los datos del doctor que originalmente atendera
                            if (idsuplente == null)
                            {
                                cm.profetional = ldet.Where(d => d.Fecha == i.Key.Fecha && d.Hora_Desde == i.Key.Hora_Desde && d.Hora_Hasta == i.Key.Hora_Hasta)
                                .Select(u => new ComboModel
                                {
                                    id = u.Id_Detalle_Agenda,
                                    value = u.Horario_Agenda.Agenda_Profesional.Usuario.Nombres + " " + u.Horario_Agenda.Agenda_Profesional.Usuario.Primer_Apellido + " " + u.Horario_Agenda.Agenda_Profesional.Usuario.Segundo_Apellido
                                }).ToList();
                            }
                            else
                            {
                                string profesional = user.Where(j => j.Id_Usuario == idsuplente).Select(f => f.Nombres + " " + f.Primer_Apellido + " " + f.Segundo_Apellido).SingleOrDefault();
                                //si existe un id de suplente mostramos esos datos del doctor que sera el que atiende ahora
                                cm.profetional = ldet.Where(d => d.Fecha == i.Key.Fecha && d.Hora_Desde == i.Key.Hora_Desde && d.Hora_Hasta == i.Key.Hora_Hasta)
                                .Select(u => new ComboModel
                                {
                                    id = u.Id_Detalle_Agenda,
                                    value = profesional
                                }).ToList();
                            }

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

                    List<ErrorFields> rel = autil.ValidateObject(model);
                    if (rel.Count == 0)
                    {
                        //se valida si esa cita ya fue tomada
                        Cita ct = ent.Cita.Where(t => t.Id_Detalle_Agenda == model.Id_Detalle_Agenda).SingleOrDefault();
                        if (ct == null)
                        {
                            //se consulta el paciente
                            Paciente pc = ent.Paciente.Where(o => o.Id_Paciente == model.Id_Paciente).SingleOrDefault();
                            Cita c = new Cita();
                            c.Id_Cita = Guid.NewGuid();
                            //si el tipo de cita es privada
                            if (model.Tipo_Cita == "1")
                            {
                                c.Id_Cup = null;
                                c.Cod_Aprobacion = null;
                            }
                            else // si es por EPM
                            {
                                c.Id_Cup = model.Id_Cup;
                                c.Cod_Aprobacion = model.Cod_Aprobacion;
                            }

                            c.Id_Paciente = model.Id_Paciente;
                            c.Id_Entidad = model.id;
                            c.Id_Especialidad = model.Id_Especialidad;

                            c.Id_Detalle_Agenda = model.Id_Detalle_Agenda;

                            c.Tipo_Cita = model.Tipo_Cita;

                            //si el paciente esta confirmado, la cita tambien se guarda confirmada
                            if (pc.Confirmado)
                                c.Confirmada = true;
                            else
                                c.Confirmada = false;

                            c.Fecha_Create = DateTime.Now;

                            ent.Cita.Add(c);

                            //se coloca el detalle de la cita que ya fue tomada
                            Detalle_Agenda dau = ent.Detalle_Agenda.Where(s => s.Id_Detalle_Agenda == model.Id_Detalle_Agenda).SingleOrDefault();
                            dau.Asignada = true;

                            ent.SaveChanges();
                            autil.ReturnMesagge(ref rep, 47, string.Empty, null, rel, HttpStatusCode.OK);


                            Detalle_Agenda da = ent.Detalle_Agenda.Where(t => t.Id_Detalle_Agenda == model.Id_Detalle_Agenda).SingleOrDefault();
                            List<string> list = new List<string>();
                            list.Add(da.Fecha.ToShortDateString());
                            list.Add(da.Hora_Desde.ToShortTimeString());
                            list.Add(da.Hora_Hasta.ToShortTimeString());
                            string email = new string(pc.Email.Take(3).ToArray());
                            string emailserver = pc.Email.Substring(pc.Email.LastIndexOf('@') + 1);
                            list.Add(email + "*******@" + emailserver);
                            rep.message = autil.ReplaceCustomMesaggeText(list, rep.message);

                            Entidad et = ent.Entidad.Where(e => e.Id_Entidad == model.id).SingleOrDefault();
                            Especialidad esp = ent.Especialidad.Where(e => e.Id_Especialidad == model.Id_Especialidad).SingleOrDefault();

                            if (pc != null)
                            {
                                autil.SendMail(pc.Email, (SendConfirmation(pc.Nombres + " " + pc.Apellidos, et.Nombre, da.Fecha.ToShortDateString() + " " + da.Hora_Desde.ToShortTimeString(), esp.Nombre)), ConfigurationManager.AppSettings["WelcomeSubjec"]);
                            }
                        }
                        else
                        {
                            autil.ReturnMesagge(ref rep, 51, string.Empty, null, rel, HttpStatusCode.OK);
                        }
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

        public object GetUnconfirmedAppoinmet(Basic model)
        {
            Response rep = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());


                    DateTime fechaconsulta = DateTime.Today.AddDays(1);

                    var citas = ent.Cita.Where(t => t.Confirmada == false
                    //&& (t.Detalle_Agenda.Fecha <= fechaconsulta
                    && t.Id_Entidad == entidad).Select(u => new AppointmentConfirmationModel
                    {
                        Id_Appointment = u.Id_Cita,
                        Fecha_Cita = u.Detalle_Agenda.Fecha,
                        Hora_Desde = u.Detalle_Agenda.Hora_Desde,
                        Hora_Hasta = u.Detalle_Agenda.Hora_Hasta,
                        Nombre_paciente = u.Paciente.Nombres + " " + u.Paciente.Apellidos,
                        Id_Paciente = u.Paciente.Id_Paciente,
                        Telefono = u.Paciente.Telefono,
                        Celular = u.Paciente.Celular,
                        Correo = u.Paciente.Email
                    }).OrderBy(o => o.Fecha_Cita).ToList();

                    rep.data = citas;
                    return autil.ReturnMesagge(ref rep, 9, null, null, HttpStatusCode.OK);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public object ConfirmAppoinmet(AppointmentModel model)
        {
            Response rep = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                    Cita ct = ent.Cita.Where(t => t.Id_Cita == model.Id_Cita).SingleOrDefault();

                    if (ct != null)
                    {
                        ct.Confirmada = true;
                        ct.Paciente.Confirmado = true;
                        ent.SaveChanges();
                    }
                    return autil.ReturnMesagge(ref rep, 52, null, null, HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public object DeleteAppoinmet(AppointmentModel model)
        {
            Response rep = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                    Cita ct = ent.Cita.Where(t => t.Id_Cita == model.Id_Cita).SingleOrDefault();

                    if (ct != null)
                    {
                        Detalle_Agenda da = ct.Detalle_Agenda;

                        da.Asignada = false;
                        ent.Cita.Remove(ct);
                        ent.SaveChanges();
                    }
                    return autil.ReturnMesagge(ref rep, 53, null, null, HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }



        #endregion


        #region Coreo
        private AlternateView SendConfirmation(string nombre, string entidad, string fecha, string especialidad)
        {
            try
            {
                //se arma el correo que se envia para el ambio de clave
                string plantilla = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["CreacionCita"]);

                var html = System.IO.File.ReadAllText(plantilla);
                html = html.Replace("{{name}}", nombre);
                html = html.Replace("{{entidad}}", entidad);
                html = html.Replace("{{fecha}}", fecha);
                html = html.Replace("{{especialidad}}", especialidad);

                AlternateView av = AlternateView.CreateAlternateViewFromString(html, null, "text/html");

                //create the LinkedResource (embedded image)
                LinkedResource logo = new LinkedResource(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["LogoPath"]));
                logo.ContentId = "companylogo";
                //add the LinkedResource to the appropriate view
                av.LinkedResources.Add(logo);

                return av;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
