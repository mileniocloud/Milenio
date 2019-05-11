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
                                                && f.Id_Especialidad == model.Id_Especialidad
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
                            ap.Id_Especialidad = model.Id_Especialidad;
                            ap.Id_Profesional = model.Id_Profesional;
                            ap.Id_Entidad = entidad;
                            ap.Estado = true;
                            ap.Fecha_Create = DateTime.Now;
                            ap.Fecha_Update = DateTime.Now;
                            ap.Usuario_Create = usuario;
                            ap.Usuario_Update = usuario;
                            ent.Agenda_Profesional.Add(ap);

                            ent.SaveChanges();

                            //se genera el codigo del mensaje de retorno exitoso
                            return rp = autil.MensajeRetorno(ref rp, 2, string.Empty, null, HttpStatusCode.OK);
                        }
                        else
                        {
                            //ya existen fechas iguales creadas
                            return rp = autil.MensajeRetorno(ref rp, 26, string.Empty, null, HttpStatusCode.OK);
                        }
                    }
                    else
                    {
                        //fallo campos requeridos
                        return autil.MensajeRetorno(ref rp, 33, string.Empty, null, rel, HttpStatusCode.OK);
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    rp = autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
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
                                                && f.Estado == true
                                                && f.Id_Especialidad == model.Id_Especialidad
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
                            ap.Id_Especialidad = model.Id_Especialidad;
                            ap.Id_Profesional = model.Id_Profesional;
                            ap.Id_Entidad = entidad;
                            ap.Estado = model.Estado;
                            ap.Fecha_Update = DateTime.Now;
                            ap.Usuario_Update = usuario;

                            ent.SaveChanges();

                            //se genera el codigo del mensaje de retorno exitoso
                            return rep = autil.MensajeRetorno(ref rep, 2, string.Empty, null, HttpStatusCode.OK);
                        }
                        else
                        {
                            //ya existen fechas iguales creadas
                            return rep = autil.MensajeRetorno(ref rep, 26, string.Empty, null, HttpStatusCode.OK);
                        }
                    }
                    else
                    {
                        //fallo campos requeridos
                        return autil.MensajeRetorno(ref rep, 33, string.Empty, null, rel, HttpStatusCode.OK);
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    rep = autil.MensajeRetorno(ref rep, 4, ex.Message, null);
                    return rep;
                }
            }
        }

        public List<ProfetionalScheduleModel> GetAgendaProfesional(HttpRequest httpRequest)
        {
            List<ProfetionalScheduleModel> apm = new List<ProfetionalScheduleModel>();
            List<Agenda_Profesional> ap = new List<Agenda_Profesional>();
            try
            {
                cp = tvh.getprincipal(Convert.ToString(httpRequest.Form["token"]));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
                        bool between = Convert.ToBoolean(int.Parse(httpRequest.Form["between"]));

                        ap = ent.Agenda_Profesional.Where(a => a.Id_Entidad == entidad).ToList();

                        if (ap.Count != 0)
                        {
                            //busca por nombre
                            if (!string.IsNullOrEmpty(httpRequest.Form["idprofesional"]))
                            {
                                Guid idprofesional = Guid.Parse(httpRequest.Form["idprofesional"]);
                                ap = ap.Where(t => t.Id_Profesional == idprofesional).ToList();
                            }

                            if (!string.IsNullOrEmpty(httpRequest.Form["idespecialidad"]))
                            {
                                Guid idespecialidad = Guid.Parse(httpRequest.Form["idespecialidad"]);
                                ap = ap.Where(t => t.Id_Especialidad == idespecialidad).ToList();
                            }

                            if (!string.IsNullOrEmpty(httpRequest.Form["estado"]))
                            {
                                bool estado = Convert.ToBoolean(int.Parse(httpRequest.Form["estado"]));
                                ap = ap.Where(t => t.Estado == estado).ToList();
                            }

                            if (between)
                            {
                                if (!string.IsNullOrEmpty(httpRequest.Form["fechadesde"]) && !string.IsNullOrEmpty(httpRequest.Form["fechahasta"]))
                                {
                                    DateTime fecha_desde = Convert.ToDateTime(httpRequest.Form["fechadesde"]);
                                    DateTime fecha_hasta = Convert.ToDateTime(httpRequest.Form["fechahasta"]);
                                    ap = ap.Where(f =>
                                                f.Fecha_Desde <= fecha_desde && f.Fecha_Hasta >= fecha_desde
                                                || f.Fecha_Desde <= fecha_hasta && f.Fecha_Hasta >= fecha_hasta
                                                || f.Fecha_Desde >= fecha_desde && f.Fecha_Hasta <= fecha_hasta).ToList();
                                }
                            }
                            else
                            {
                                //PARA BUSCAR POR FECHAS EN ESPECIFICO
                                if (!string.IsNullOrEmpty(httpRequest.Form["fechadesde"]))
                                {
                                    DateTime fecha_desde = Convert.ToDateTime(httpRequest.Form["fechadesde"]);
                                    ap = ap.Where(t => t.Fecha_Desde == fecha_desde).ToList();
                                }

                                if (!string.IsNullOrEmpty(httpRequest.Form["fechahasta"]))
                                {
                                    DateTime fecha_hasta = Convert.ToDateTime(httpRequest.Form["fechahasta"]);
                                    ap = ap.Where(t => t.Fecha_Hasta == fecha_hasta).ToList();
                                }
                            }

                            foreach (var i in ap)
                            {
                                ProfetionalScheduleModel apmm = new ProfetionalScheduleModel();
                                Copier.CopyPropertiesTo(i, apmm);
                                apm.Add(apmm);
                            }

                        }

                    }
                    return apm;
                }
                else
                    return apm;
            }
            catch (Exception)
            {
                return apm;
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
                        Guid id_especialidad = ent.Agenda_Profesional.Where(a => a.Id_Agenda_Profesional == model.Id_Agenda_Profesional).Select(g => g.Id_Especialidad).SingleOrDefault();
                        //validamos que el consultorio seleccionado tenga esa especialidad asignada
                        int ve = ent.Consultorio_Especialidad.Where(c => c.Id_Especialidad == id_especialidad && c.Id_Consultorio == model.Id_Consultorio && c.Id_Entidad == entidad).Count();

                        //si trae algo es porque ese consultorio si contiene esa especialidad
                        if (ve != 0)
                        {
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
                                    return rp = autil.MensajeRetorno(ref rp, 2, string.Empty, null);
                                }
                                else
                                {
                                    //fechas iguales
                                    rp = autil.MensajeRetorno(ref rp, 40, string.Empty, null);
                                    return rp;
                                }
                            }
                            else
                            {
                                //se avisa que la cantidad de consultas disponible no coinciden
                                rp = autil.MensajeRetorno(ref rp, 38, string.Empty, null, HttpStatusCode.OK);
                                return rp;
                            }
                        }
                        else
                        {
                            //se avisa que el consultorio no contiene esa especialidad asignada
                            rp = autil.MensajeRetorno(ref rp, 39, string.Empty, null, HttpStatusCode.OK);
                            return rp;
                        }
                    }
                    else
                    {
                        //fallo campos requeridos
                        return autil.MensajeRetorno(ref rp, 33, string.Empty, null, rel, HttpStatusCode.OK);
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
                        Guid id_especialidad = ent.Agenda_Profesional.Where(a => a.Id_Agenda_Profesional == model.Id_Agenda_Profesional).Select(g => g.Id_Especialidad).SingleOrDefault();
                        //validamos que el consultorio seleccionado tenga esa especialidad asignada
                        int ve = ent.Consultorio_Especialidad.Where(c => c.Id_Especialidad == id_especialidad && c.Id_Consultorio == model.Id_Consultorio && c.Id_Entidad == entidad).Count();
                        if (ve != 0)
                        {
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
                                     && (
                                         (a.Hora_Desde < hdesde && a.Hora_Hasta > hdesde)//cuando la hora desde que envian esta dentro de un rango
                                         || (a.Hora_Desde < hhasta && a.Hora_Hasta > hhasta) //cuando la hora hasta que envian esta dentro de un rango
                                         || (a.Hora_Desde >= hdesde && a.Hora_Hasta <= hhasta))//cuando los rangos enviados son mayores que los rangos creados o son iguales
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
                                    //se genera el codigo del mensaje de retorno exitoso
                                    return rp = autil.MensajeRetorno(ref rp, 20, string.Empty, null);
                                }
                                else
                                {
                                    //fechas iguales
                                    rp = autil.MensajeRetorno(ref rp, 40, string.Empty, null);
                                    return rp;
                                }
                            }
                            else
                            {
                                //se avisa que la cantidad de consultas disponible no coinciden
                                rp = autil.MensajeRetorno(ref rp, 38, string.Empty, null, HttpStatusCode.OK);
                                return rp;
                            }
                        }
                        else
                        {
                            //se avisa que el consultorio no contiene esa especialidad asignada
                            rp = autil.MensajeRetorno(ref rp, 39, string.Empty, null, HttpStatusCode.OK);
                            return rp;
                        }
                    }
                    else
                    {
                        //fallo campos requeridos
                        return autil.MensajeRetorno(ref rp, 33, string.Empty, null, rel, HttpStatusCode.OK);
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

        #region Detalle Agenda // sale a pruebas

        public object CreateScheduleDetail(ScheduleDetailModel _model)
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

                    foreach (var ha in m)
                    {
                        if (fallavalidacion)
                            break;

                        //se saca la cantidad de minutos disponibles
                        TimeSpan td = ha.Hora_Hasta.Subtract(ha.Hora_Desde);
                        //se divide la cantidad de minutos disponibles para saber cuantas citas puede atender
                        double cant_consultas = td.TotalMinutes / ha.Duracion;

                        //consultamos el nobre del consultorio y de la especialidad
                        string especialidad = ha.Agenda_Profesional.Especialidad_Entidad.Especialidad.Nombre;
                        string consultorio = ent.Consultorio.Where(t => t.Id_Consultorio == ha.Id_Consultorio).Select(s => s.Nombre).SingleOrDefault();

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

                                if (date.DayOfWeek.ToString() == ha.Dia)
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
                                            autil.GetErrorDetail(ref er, 29, "Detalle Agenda", date, hdesde, hhasta, consultorio, especialidad);
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
                }
                catch (Exception ex)
                {
                    //error general
                    rp = autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                }
            }
            rp = autil.MensajeRetorno(ref rp, 2, string.Empty, null);
            rp.data = er;
            return rp;
        }
        #endregion


        #region citas

        public object GetAppointment(AppointmentModel model)
        {
            Response rep = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    List<ErrorFields> rel = autil.ValidateObject(model);
                    if (rel.Count == 0)
                    {
                        if (model.Mes == 0)
                            model.Mes = DateTime.Today.Month;

                        var daa = (from e in ent.Detalle_Agenda
                                                    where e.Horario_Agenda.Agenda_Profesional.Especialidad_Entidad.Especialidad.Codigo == model.Codigo_Especilidad
                                                    && e.Horario_Agenda.Agenda_Profesional.Especialidad_Entidad.Id_Entidad == entidad
                                                    && e.Asignada == false
                                                    && e.Fecha >= DateTime.Today
                                                    && e.Fecha.Month == model.Mes
                                                    select new {
                                                        fecha = e.Fecha,
                                                        fromhour = e.Hora_Desde,
                                                        tohour = e.Hora_Desde,
                                                        doctor = e.Horario_Agenda.Agenda_Profesional
                                                    }).ToList();
                       

                        return rep;
                    }
                    else
                    {
                        //fallo campos requeridos
                        return autil.MensajeRetorno(ref rep, 33, string.Empty, null, rel, HttpStatusCode.OK);
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
