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
    public class aSchedule
    {
        TokenValidationHandler tvh = new TokenValidationHandler();
        ClaimsPrincipal cp = new ClaimsPrincipal();
        aUtilities autil = new aUtilities();

        #region Agenda Profesional
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


                    DateTime fecha_desde = model.Fecha_Desde;
                    DateTime fecha_hasta = model.Fecha_Hasta;

                    Agenda_Profesional ap = ent.Agenda_Profesional
                                            .Where(f =>
                                            f.Id_Profesional == model.Id_Profesional
                                            && f.Id_Entidad == entidad
                                            && f.Estado == true
                                            && f.Id_Especialidad == model.Id_Especialidad
                                            && (
                                            (f.Fecha_Desde <= fecha_desde && f.Fecha_Hasta >= fecha_desde)
                                            || (f.Fecha_Desde <= fecha_hasta && f.Fecha_Hasta >= fecha_hasta)
                                            || (f.Fecha_Desde >= fecha_desde && f.Fecha_Hasta <= fecha_hasta))
                                            ).SingleOrDefault();
                    if (ap == null)
                    {
                        ap = new Agenda_Profesional();
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
                catch (Exception ex)
                {
                    //error general
                    rp = autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
                    return rp;
                }
            }
        }

        public Basic EditAgendaProfesional(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(httpRequest.Form["token"]));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Guid id_agenda_profesional = Guid.Parse(httpRequest.Form["idagendaprofesional"]);
                        Guid id_consultorio = Guid.Parse(httpRequest.Form["idconsultorio"]);
                        Guid id_especialidad = Guid.Parse(httpRequest.Form["idespecialidad"]);
                        Guid id_profesional = Guid.Parse(httpRequest.Form["idprofesional"]);

                        bool estado = Convert.ToBoolean(int.Parse(httpRequest.Form["estado"]));

                        DateTime fecha_desde = Convert.ToDateTime(httpRequest.Form["fechadesde"]);
                        DateTime fecha_hasta = Convert.ToDateTime(httpRequest.Form["fechahasta"]);

                        Agenda_Profesional ap = ent.Agenda_Profesional
                                                .Where(f =>
                                                f.Id_Profesional == id_profesional
                                                && f.Id_Entidad == entidad
                                                && f.Estado == true
                                                && f.Id_Especialidad == id_especialidad
                                                && f.Id_Agenda_Profesional != id_agenda_profesional
                                                && (
                                                (f.Fecha_Desde <= fecha_desde && f.Fecha_Hasta >= fecha_desde)
                                                || (f.Fecha_Desde <= fecha_hasta && f.Fecha_Hasta >= fecha_hasta)
                                                || (f.Fecha_Desde >= fecha_desde && f.Fecha_Hasta <= fecha_hasta))
                                                ).SingleOrDefault();
                        if (ap != null)
                        {
                            ap = ent.Agenda_Profesional.Where(a => a.Id_Agenda_Profesional == id_agenda_profesional).SingleOrDefault();
                            ap.Fecha_Desde = fecha_desde;
                            ap.Fecha_Hasta = fecha_hasta;
                            ap.Id_Especialidad = id_especialidad;
                            ap.Estado = estado;
                            ap.Fecha_Update = DateTime.Now;
                            ap.Usuario_Update = usuario;

                            ent.SaveChanges();

                            //se genera el codigo del mensaje de retorno exitoso
                            return ret = autil.MensajeRetorno(ref ret, 20, string.Empty, null);
                        }
                        else
                        {
                            //ya existen fechas iguales creadas
                            return ret = autil.MensajeRetorno(ref ret, 26, string.Empty, null);
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


        #region Horario agenda

        public object CreateScheduleAgenda(ScheduleAgendaModel model)
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

                            Horario_Agenda ha = ent.Horario_Agenda
                                .Where(a => a.Id_Agenda_Profesional == model.Id_Agenda_Profesional
                                && a.Dia == model.Dia
                                && a.Estado == true
                                && (
                                (a.Hora_Desde <= hdesde && a.Hora_Hasta >= hdesde)
                                || (a.Hora_Desde <= hhasta && a.Hora_Hasta >= hhasta)
                                || (a.Hora_Desde >= hdesde && a.Hora_Hasta <= hhasta))
                                ).SingleOrDefault();

                            if (ha == null)
                            {
                                ha = new Horario_Agenda();
                                ha.Id_Horario_Agenda = Guid.NewGuid();
                                ha.Id_Agenda_Profesional = model.Id_Agenda_Profesional;
                                ha.Hora_Desde = hdesde;
                                ha.Hora_Hasta = hhasta;
                                ha.Dia = model.Dia;
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
                                rp = autil.MensajeRetorno(ref rp, 29, string.Empty, null);
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

                            Horario_Agenda ha = ent.Horario_Agenda
                                .Where(a => a.Id_Agenda_Profesional == model.Id_Agenda_Profesional
                                && a.Id_Horario_Agenda != model.Id_Horario_Agenda
                                && a.Dia == model.Dia
                                && (
                                (a.Hora_Desde <= hdesde && a.Hora_Hasta >= hdesde)
                                || (a.Hora_Desde <= hhasta && a.Hora_Hasta >= hhasta)
                                || (a.Hora_Desde >= hdesde && a.Hora_Hasta <= hhasta))
                                ).SingleOrDefault();

                            if (ha == null)
                            {
                                ha = ent.Horario_Agenda.Where(t => t.Id_Horario_Agenda == model.Id_Horario_Agenda).SingleOrDefault();
                                ha.Hora_Desde = hdesde;
                                ha.Hora_Hasta = hhasta;
                                ha.Dia = model.Dia;
                                ha.Estado = model.Estado;
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
                                rp = autil.MensajeRetorno(ref rp, 29, string.Empty, null);
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

        #region Detalle Agenda

        public object CreateScheduleDetail(ScheduleDetailModel model)
        {
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                    Horario_Agenda ha = ent.Horario_Agenda.Where(h => h.Id_Horario_Agenda == model.Id_Horario_Agenda).SingleOrDefault();

                    List<DateTime> fechas = new List<DateTime>();

                    for (DateTime date = ha.Agenda_Profesional.Fecha_Desde; date.Date <= ha.Agenda_Profesional.Fecha_Hasta; date = date.AddDays(1))
                    {
                        if (date.DayOfWeek.ToString() == ha.Dia)
                        {
                            ha.Hora_Desde = new DateTime(date.Year, date.Month, date.Date.Day, ha.Hora_Desde.Hour, ha.Hora_Desde.Minute, 0);
                            ha.Hora_Hasta = new DateTime(date.Year, date.Month, date.Date.Day, ha.Hora_Hasta.Hour, ha.Hora_Hasta.Minute, 0);

                            Detalle_Agenda da = ent.Detalle_Agenda.Where(
                                                d => d.Horario_Agenda.Agenda_Profesional.Id_Entidad == entidad
                                                && d.Id_Consultorio == model.Id_Consultorio
                                                && d.Fecha == date
                                                && (
                                                (d.Hora_Desde <= ha.Hora_Desde && d.Hora_Hasta >= ha.Hora_Hasta)
                                                || (d.Hora_Desde <= ha.Hora_Desde && d.Hora_Hasta >= ha.Hora_Hasta)
                                                || (d.Hora_Desde >= ha.Hora_Desde && d.Hora_Hasta <= ha.Hora_Hasta))
                                                ).SingleOrDefault();

                            if (da == null)
                            {
                                da = new Detalle_Agenda();
                                da.Id_Detalle_Agenda = Guid.NewGuid();
                                da.Id_Horario_Agenda = ha.Id_Horario_Agenda;
                                da.Hora_Desde = ha.Hora_Desde;
                                da.Hora_Hasta = ha.Hora_Hasta;
                                da.Fecha = date;
                                da.Id_Consultorio = model.Id_Consultorio;
                                da.Fecha_Create = DateTime.Now;
                                da.Fecha_Update = DateTime.Now;
                                da.Usuario_Create = usuario;
                                da.Usuario_Update = usuario;

                                ent.Detalle_Agenda.Add(da);
                                ent.SaveChanges();
                                //se genera el codigo del mensaje de retorno exitoso
                                return rp = autil.MensajeRetorno(ref rp, 2, string.Empty, null);
                            }
                            else
                            {
                                //se avisa que ya existe una cita creada par aese consultorio en esa fecha con esas horas
                                rp = autil.MensajeRetorno(ref rp, 38, string.Empty, null, HttpStatusCode.OK);
                                return rp;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //error general
                    rp = autil.MensajeRetorno(ref rp, 4, ex.Message, null, HttpStatusCode.InternalServerError);
                }
            }
            return rp;
        }
        #endregion


    }
}
