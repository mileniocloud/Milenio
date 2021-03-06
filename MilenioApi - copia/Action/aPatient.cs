﻿using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Web;

namespace MilenioApi.Action
{
    public class aPatient : Basic
    {
        aUtilities autil = new aUtilities();
        ClaimsPrincipal cp = new ClaimsPrincipal();
        TokenValidationHandler tvh = new TokenValidationHandler();

        public object CreateTemporalPatient(PatientModel model)
        {
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    //se valida por cedula
                    if (ent.Paciente.Where(p => p.Id_Tipo_Identificacion == model.Id_Tipo_Identificacion && p.Numero_Identificacion == model.Numero_Identificacion).Count() == 0)
                    {
                        Paciente pc = new Paciente();
                        pc.Id_Paciente = Guid.NewGuid();
                        pc.Id_Tipo_Identificacion = model.Id_Tipo_Identificacion;
                        pc.Numero_Identificacion = model.Numero_Identificacion;
                        pc.Nombres = model.Nombres;
                        pc.Apellidos = model.Apellidos;
                        pc.Fecha_Nacimiento = model.Fecha_Nacimiento;
                        pc.Celular = model.Celular;
                        pc.Telefono = model.Telefono;
                        pc.Email = model.Email;
                        pc.Direccion = model.Direccion;
                        pc.Confirmado = false;

                        ent.Paciente.Add(pc);
                        ent.SaveChanges();
                    }
                    else
                    {
                        //cedula existe
                        return rp = autil.ReturnMesagge(ref rp, 45, string.Empty, null, HttpStatusCode.OK);
                    }

                    return this.ValidatePatient(model);
                }
            }
            catch (Exception ex)
            {
                //error general
                return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }

        public object CreatePatient(PatientModel model)
        {
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    //se valida por cedula
                    if (ent.Paciente.Where(p => p.Id_Tipo_Identificacion == model.Id_Tipo_Identificacion && p.Numero_Identificacion == model.Numero_Identificacion).Count() == 0)
                    {
                        //se valida por email
                        if (ent.Paciente.Where(p => p.Email == model.Email).Count() == 0)
                        {
                            //se valida por celular
                            if (ent.Paciente.Where(p => p.Celular == model.Celular).Count() == 0)
                            {
                                Paciente pc = new Paciente();
                                pc.Id_Paciente = Guid.NewGuid();
                                pc.Id_Tipo_Identificacion = model.Id_Tipo_Identificacion;
                                pc.Numero_Identificacion = model.Numero_Identificacion;
                                pc.Nombres = model.Nombres;
                                pc.Apellidos = model.Apellidos;
                                pc.Fecha_Nacimiento = model.Fecha_Nacimiento;
                                pc.Celular = model.Celular;
                                pc.Telefono = model.Telefono;
                                pc.Email = model.Email;
                                pc.Direccion = model.Direccion;
                                pc.Confirmado = true;

                                ent.Paciente.Add(pc);
                                ent.SaveChanges();
                            }
                            else
                            {
                                //celular existe
                                return rp = autil.ReturnMesagge(ref rp, 46, string.Empty, null, HttpStatusCode.OK);
                            }
                        }
                        else
                        {
                            // Email existe
                            return rp = autil.ReturnMesagge(ref rp, 6, string.Empty, null, HttpStatusCode.OK);
                        }
                    }
                    else
                    {
                        //cedula existe
                        return rp = autil.ReturnMesagge(ref rp, 45, string.Empty, null, HttpStatusCode.OK);
                    }

                    //retorna un mensaje de exito
                    return autil.ReturnMesagge(ref rp, 2, null, null, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                //error general
                return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }


        //este metodo solo se usa para la solicitud de citas
        public object ValidatePatient(PatientModel model)
        {
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    List<object> response = new List<object>();
                    Guid identidad = Guid.Parse(model.id);
                    Cups cup = new Cups();
                    Especialidad ep = new Especialidad();
                    if (model.Tipo_Cita == "0")
                    {
                        Guid codcup = Guid.Parse(model.Codigo_Cup);
                        cup = ent.Cups.Where(h => h.Id_Cups == codcup).SingleOrDefault();
                    }

                    ep = ent.Especialidad.Where(g => g.Id_Especialidad == model.Id_Especialidad).SingleOrDefault();
                    Entidad et = ent.Entidad.Where(e => e.Id_Entidad == identidad).SingleOrDefault();

                    var paciente = ent.Paciente.Where(p => p.Id_Tipo_Identificacion == model.Id_Tipo_Identificacion && p.Numero_Identificacion == model.Numero_Identificacion)
                                          .Select(c => new
                                          {
                                              idpatient = c.Id_Paciente,
                                              typedocument = c.Id_Tipo_Identificacion,
                                              document = c.Numero_Identificacion,
                                              names = c.Nombres,
                                              lastnames = c.Apellidos,
                                              cups = model.Codigo_Cup,
                                              typequery = model.Tipo_Cita,
                                              model.Id_Especialidad,
                                              autorization = model.Cod_Aprobacion,
                                              entityname = et.Nombre,
                                              cupname = cup.Descripcion,
                                              speciality = ep.Nombre,
                                              id = model.id
                                          }).ToList();
                    if (paciente.Count > 0)
                        response.Add(paciente[0]);

                    if (response.Count != 0)
                    {
                        //si el paciente exite, le regresa las disponibilidades
                        aSchedule sh = new aSchedule();
                        AppointmentModel am = new AppointmentModel();
                        am.id = Guid.Parse(model.id);
                        am.Id_Especialidad = model.Id_Especialidad;
                        var ap = sh.GetAppointment(am);
                        response.Add(((MilenioApi.Models.Response)ap).data);
                    }

                    rp.data = response;

                    //retorna un response, con el id del paciente
                    return autil.ReturnMesagge(ref rp, 48, null, null, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                //error general
                return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }
    }
}