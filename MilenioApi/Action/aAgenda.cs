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
    public class aAgenda
    {
        TokenValidationHandler tvh = new TokenValidationHandler();
        ClaimsPrincipal cp = new ClaimsPrincipal();
        aUtilities autil = new aUtilities();

        #region Agenda Profesional
        public Basic CreateAgendaProfesional(HttpRequest httpRequest)
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

                        Guid id_consultorio = Guid.Parse(httpRequest.Form["idconsultorio"]);
                        Guid id_especialidad = Guid.Parse(httpRequest.Form["idespecialidad"]);
                        Guid id_profesional = Guid.Parse(httpRequest.Form["idprofesional"]);

                        DateTime fecha_desde = Convert.ToDateTime(httpRequest.Form["fechadesde"]);
                        DateTime fecha_hasta = Convert.ToDateTime(httpRequest.Form["fechahasta"]);

                        Agenda_Profesional ap = ent.Agenda_Profesional
                                                .Where(f =>
                                                f.Id_Profesional == id_profesional
                                                && f.Id_Entidad == entidad
                                                && f.Id_Consultorio == id_consultorio
                                                && f.Estado == true
                                                && f.Id_Especialidad == id_especialidad
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
                            ap.Id_Consultorio = id_consultorio;
                            ap.Id_Especialidad = id_especialidad;
                            ap.Id_Profesional = id_profesional;
                            ap.Id_Entidad = entidad;
                            ap.Estado = true;
                            ap.Fecha_Create = DateTime.Now;
                            ap.Fecha_Update = DateTime.Now;
                            ap.Usuario_Create = usuario;
                            ap.Usuario_Update = usuario;
                            ent.Agenda_Profesional.Add(ap);

                            ent.SaveChanges();

                            //se genera el codigo del mensaje de retorno exitoso
                            return ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
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
                                                && f.Id_Consultorio == id_consultorio
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
                            ap.Id_Consultorio = id_consultorio;
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

        public List<AgendaProfesionalModel> GetAgendaProfesional(HttpRequest httpRequest)
        {
            List<AgendaProfesionalModel> apm = new List<AgendaProfesionalModel>();
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

                            if (!string.IsNullOrEmpty(httpRequest.Form["idconsultorio"]))
                            {
                                Guid idconsultorio = Guid.Parse(httpRequest.Form["idconsultorio"]);
                                ap = ap.Where(t => t.Id_Consultorio == idconsultorio).ToList();
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
                                AgendaProfesionalModel apmm = new AgendaProfesionalModel();
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

        public Basic CreateHorarioAgenda(HttpRequest httpRequest)
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
                        DateTime hora_desde = Convert.ToDateTime(httpRequest.Form["horadesde"]);
                        DateTime hora_hasta = Convert.ToDateTime(httpRequest.Form["horahasta"]);
                        string dia = Convert.ToString(httpRequest.Form["dia"]);
                        int duracion = Convert.ToInt32(httpRequest.Form["duracion"]);

                        TimeSpan hdesde = hora_desde.TimeOfDay;
                        TimeSpan hhasta = hora_hasta.TimeOfDay;

                        Horario_Agenda ha = ent.Horario_Agenda
                            .Where(a => a.Id_Agenda_Profesional == id_agenda_profesional
                            && a.Dia == dia
                            && (
                            (a.Hora_Desde <= hdesde && a.Hora_Hasta >= hdesde)
                            || (a.Hora_Desde <= hhasta && a.Hora_Hasta >= hhasta)
                            || (a.Hora_Desde >= hdesde && a.Hora_Hasta <= hhasta))
                            ).SingleOrDefault();

                        if (ha == null)
                        {
                            ha = new Horario_Agenda();
                            ha.Id_Horario_Agenda = Guid.NewGuid();
                            ha.Id_Agenda_Profesional = id_agenda_profesional;
                            ha.Hora_Desde = hdesde;
                            ha.Hora_Hasta = hhasta;
                            ha.Dia = dia;
                            ha.Duracion = duracion;
                            ha.Fecha_Create = DateTime.Now;
                            ha.Fecha_Update = DateTime.Now;
                            ha.Usuario_Create = usuario;
                            ha.Usuario_Update = usuario;
                            ent.Horario_Agenda.Add(ha);
                            ent.SaveChanges();
                            //se genera el codigo del mensaje de retorno exitoso
                            return ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
                        }
                        else
                        {
                            //fechas iguales
                            ret = autil.MensajeRetorno(ref ret, 29, string.Empty, null);
                            return ret;
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
