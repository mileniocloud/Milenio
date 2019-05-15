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
    public class aEntity
    {
        TokenValidationHandler tvh = new TokenValidationHandler();
        ClaimsPrincipal cp = new ClaimsPrincipal();
        aUtilities autil = new aUtilities();

        #region Seccion de entidad

        public object CreateEntity(EntidadModel model)
        {
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));
                    if (cp != null)
                    {
                        Response b = new Response();
                        List<ErrorFields> rel = autil.ValidateObject(model);
                        if (rel.Count == 0)
                        {
                            Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                            //validamos que ese nit ya exista
                            int vNit = ent.Entidad.Where(t => t.Nit == model.Nit).Count();
                            if (vNit == 0)
                            {
                                //AQUI SE TOMA EL OBJETO ENVIADO DESDE EL FRONT
                                //Y SE COPIA AL OBJETO USER
                                Entidad et = new Entidad();
                                Copier.CopyPropertiesTo(model, et);
                                //                               
                                Guid identidad = Guid.NewGuid();
                                et.Id_Entidad = identidad;

                                if (string.IsNullOrEmpty(et.Foto))
                                    et.Foto = ConfigurationManager.AppSettings["entitydefoultphoto"];

                                et.Fecha_Create = DateTime.Now;
                                et.Fecha_Update = DateTime.Now;
                                ent.Entidad.Add(et);

                                //se controla las especialidades que se van a agregar
                                if (model.specialities.Count > 0)
                                {
                                    List<Especialidad_Entidad> lee = new List<Especialidad_Entidad>();
                                    foreach (var e in model.specialities)
                                    {
                                        Especialidad_Entidad ce = new Especialidad_Entidad();
                                        ce.Id_Entidad = identidad;
                                        ce.Id_Especialidad = Guid.Parse(e.id);
                                        ce.Estado = true;
                                        ce.Usuario_Create = usuario;
                                        ce.Usuario_Update = usuario;
                                        ce.Fecha_Create = DateTime.Now;
                                        ce.Fecha_Update = DateTime.Now;
                                        lee.Add(ce);
                                    }
                                    if (lee.Count > 0)
                                    {
                                        //si hay especialidades que agregar, las agrega
                                        ent.Especialidad_Entidad.AddRange(lee);
                                    }
                                }

                                //se consulta en el webconfig el usuario SA y el rol SA
                                //Guid sauser = Guid.Parse(ConfigurationManager.AppSettings["idsuperuser"]);
                                Guid sarol = Guid.Parse(ConfigurationManager.AppSettings["idrolsuperuser"]);
                                Guid idtipovinculacion = Guid.Parse(ConfigurationManager.AppSettings["idtipovinculacion"]);

                                //se genera un usuario para enviar por email a la entidad
                                Guid idusuario = Guid.NewGuid();
                                Usuario um = new Usuario();
                                um.Id_Usuario = idusuario;
                                um.Nombres = et.Nombre.Replace(" ", "");
                                um.Primer_Apellido = et.Nit.ToString().Replace(".", "").Replace(" ", "");
                                um.Fecha_Nacimiento = DateTime.Now;
                                um.Segundo_Apellido = et.Organizacion;
                                um.Id_Tipo_Identificacion = "NIT";
                                um.Numero_Identificacion = et.Nit.ToString().Replace(".", "").Replace(" ", "");
                                um.Fecha_Contratacion = DateTime.Now;
                                um.Id_Tipo_Vinculacion = idtipovinculacion;
                                um.Presta_Servicio = false;
                                um.Login = et.Nit.ToString().Replace(".", "").Replace(" ", "");
                                string password = autil.CreatePassword(8);
                                um.Password = autil.Sha(password);
                                um.Foto = model.Foto;
                                um.Email = et.Email;
                                um.Acepta_ABEAS = true;
                                um.Fecha_Create = DateTime.Now;
                                um.Fecha_Update = DateTime.Now;
                                ent.Usuario.Add(um);


                                //se agrega el usuario que se creo a la entidad
                                Entidad_Usuario eu = new Entidad_Usuario();
                                eu.Id_Entidad = identidad;
                                eu.Id_Usuario = idusuario;
                                eu.Estado = true;
                                eu.Usuario_Create = usuario;
                                eu.Usuario_Update = usuario;
                                eu.Fecha_Create = DateTime.Now;
                                eu.Fecha_Update = DateTime.Now;
                                ent.Entidad_Usuario.Add(eu);

                                ///se agrega el rol SA a ese usuario para esa entidad
                                Rol_Usuario ru = new Rol_Usuario();
                                ru.Id_Rol = sarol;
                                ru.Id_Usuario = idusuario;
                                ru.Id_Entidad = identidad;
                                ru.Estado = true;
                                ru.Usuario_Create = usuario;
                                ru.Usuario_Update = usuario;
                                ru.Fecha_Create = DateTime.Now;
                                ru.Fecha_Update = DateTime.Now;
                                ent.Rol_Usuario.Add(ru);

                                //se guarda todo
                                ent.SaveChanges();
                                model = new EntidadModel();
                                rp.data = GetEntity(model);
                                if (!autil.SendMail(et.Email, (SetWelcomeEmailBody(et.Nombre, um.Login, password)), ConfigurationManager.AppSettings["WelcomeSubjec"]))
                                {
                                    //se genera el codigo del mensaje indicando que se creo la entidad pero no se
                                    //pudo eviar el correo.
                                    rp = autil.MensajeRetorno(ref rp, 37, string.Empty, null, HttpStatusCode.OK);
                                }
                                else
                                {
                                    //se genera el codigo del mensaje de retorno exitoso
                                    rp = autil.MensajeRetorno(ref rp, 2, string.Empty, null, HttpStatusCode.OK);
                                }
                            }
                            else
                            {
                                //Nit EXISTE
                                rp = autil.MensajeRetorno(ref rp, 36, string.Empty, null, HttpStatusCode.OK);
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
                        rp = autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                        return rp;
                    }
                }
                return rp;
            }
            catch (Exception ex)
            {
                //error general
                rp = autil.MensajeRetorno(ref rp, 4, ex.Message + " " + ex.InnerException, null, HttpStatusCode.InternalServerError);
                return rp;
            }
        }
 
        public object EditEntity(EntidadModel model)
        {
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    cp = tvh.getprincipal(model.token);
                    List<ErrorFields> rel = autil.ValidateObject(model);
                    if (rel.Count == 0)
                    {
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        List<ComboListModel> listEnt = ent.Especialidad_Entidad.Where(ee => ee.Id_Entidad == model.Id_Entidad).Select(x=> new ComboListModel { id= x.Id_Especialidad.ToString(), value =x.Especialidad.Nombre }).ToList();

                     
                        if (listEnt.Count > 0)
                        {
                            var list = listEnt.Where(t => !model.specialities.Any(t2 => t2.id == t.id)).ToList();

                            foreach (var ee in list)
                            {
                                Guid idesp = Guid.Parse(ee.id);
                                Especialidad_Entidad ce = ent.Especialidad_Entidad.Where(en => en.Id_Entidad == model.Id_Entidad && en.Id_Especialidad == idesp).SingleOrDefault();
                                if (ce.Estado == true)
                                {
                                    ce.Estado = false;
                                    ce.Usuario_Update = usuario;
                                    ce.Fecha_Update = DateTime.Now;
                                }
                            }
                        }


                        if (model.specialities.Count > 0)
                        {
                            List<Especialidad_Entidad> lee = new List<Especialidad_Entidad>();
                            foreach (var e in model.specialities)
                            {
                                Guid idespeciality = Guid.Parse(e.id);
                                List<Especialidad_Entidad> espId = ent.Especialidad_Entidad.Where(en => en.Id_Entidad == model.Id_Entidad && en.Id_Especialidad == idespeciality).ToList();
                                if (espId.Count == 0)
                                {
                                    Especialidad_Entidad ce = new Especialidad_Entidad();

                                    ce.Id_Entidad = model.Id_Entidad;
                                    ce.Id_Especialidad = Guid.Parse(e.id);
                                    ce.Estado = true;
                                    ce.Usuario_Create = usuario;
                                    ce.Usuario_Update = usuario;
                                    ce.Fecha_Create = DateTime.Now;
                                    ce.Fecha_Update = DateTime.Now;
                                    lee.Add(ce);
                                }
                                else if(espId.Where(en => en.Estado ==false).ToList().Count > 0)
                                {
                                    
                                    Especialidad_Entidad ce = ent.Especialidad_Entidad.Where(en => en.Id_Entidad == model.Id_Entidad && en.Id_Especialidad == idespeciality).SingleOrDefault();
                                    ce.Estado = true;
                                    ce.Usuario_Update = usuario;
                                    ce.Fecha_Update = DateTime.Now;
                                }
                                
                                
                                
                            }
                            if (lee.Count > 0)
                            {
                                //si hay especialidades que agregar, las agrega
                                ent.Especialidad_Entidad.AddRange(lee);
                            }
                        }

                        //validamos que ese nit ya exista
                        int vNit = ent.Entidad.Where(t => t.Nit == model.Nit && t.Id_Entidad != model.Id_Entidad).Count();
                        if (vNit == 0)
                        {
                            Entidad et = ent.Entidad.Where(en => en.Id_Entidad == model.Id_Entidad).SingleOrDefault();
                            et.Nit = model.Nit;
                            et.Nombre = model.Nombre;
                            et.Organizacion = model.Organizacion;
                            et.Poblado_Id = model.Poblado_Id;
                            et.Direccion = model.Direccion;
                            et.Email = model.Email;
                            et.CodigoEntidad = model.CodigoEntidad;
                            et.Atencion_Prioritaria = model.Atencion_Prioritaria;
                            et.Contribuyente = model.Contribuyente;
                            et.Hora_Desde = model.Hora_Desde;
                            et.Hora_Hasta = model.Hora_Hasta;
                            et.Fecha_Update = DateTime.Now;
                            et.Foto = model.Foto;
                            et.Telefono = model.Telefono;
                            //se envia a editar todo
                            ent.SaveChanges();
                            //se genera el codigo del mensaje de retorno exitoso
                            model = new EntidadModel();
                            rp.data = GetEntity(model);
                            rp = autil.MensajeRetorno(ref rp, 20, string.Empty, null, HttpStatusCode.OK);
                        }
                        else
                        {
                            //Nit EXISTE
                            rp = autil.MensajeRetorno(ref rp, 3, string.Empty, null, HttpStatusCode.OK);
                        }
                    }
                    else
                    {
                        //fallo campos requeridos
                        return autil.MensajeRetorno(ref rp, 33, string.Empty, null, rel, HttpStatusCode.OK);
                    }
                }
                return rp;
            }
            catch (Exception ex)
            {
                //error general
                rp = autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
                return rp;
            }
        }

        public object GetEntity(EntidadModel model)
        {
            List<EntidadModel> rl = new List<EntidadModel>();
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    IQueryable<Entidad> et = ent.Entidad;

                    //consulta por nit
                    if (model.Id_Entidad != Guid.Empty)
                    {
                        et = et.Where(c => c.Id_Entidad == model.Id_Entidad);
                    }

                    //consulta por nit
                    if (!string.IsNullOrEmpty(model.Nit))
                    {
                        et = et.Where(c => c.Nit == model.Nit);
                    }

                    //consulta por nombre
                    if (!string.IsNullOrEmpty(model.Nombre))
                    {
                        et = et.Where(c => c.Nombre.Contains(model.Nombre));
                    }

                    //consulta por organizacion
                    if (!string.IsNullOrEmpty(model.Organizacion))
                    {
                        et = et.Where(c => c.Organizacion.Contains(model.Organizacion));
                    }

                    //consulta por codigo entidad
                    if (!string.IsNullOrEmpty(model.CodigoEntidad))
                    {
                        et = et.Where(c => c.CodigoEntidad.Contains(model.CodigoEntidad));
                    }

                    //consulta por email
                    if (!string.IsNullOrEmpty(model.Email))
                    {
                        et = et.Where(c => c.Email.Contains(model.Email));
                    }
                    

                    var list = et.Select(u => new
                    {
                        identity = u.Id_Entidad,
                        nit = u.Nit,
                        name = u.Nombre,
                        organization = u.Organizacion,
                        address = u.Direccion,
                        neighborhood = u.Poblado_Id,
                        email = u.Email,
                        telephone = u.Telefono,
                        entitycode = u.CodigoEntidad,
                        priorityatention = u.Atencion_Prioritaria,
                        taxpayer = u.Contribuyente,
                        photo = u.Foto,
                        opening = u.Hora_Desde,
                        closing = u.Hora_Hasta,
                        specialities = (from specialities_ in ent.Especialidad
                                        join entityEsp in ent.Especialidad_Entidad on specialities_.Id_Especialidad equals entityEsp.Id_Especialidad
                                        where entityEsp.Id_Entidad == u.Id_Entidad && entityEsp.Estado == true
                                        select new
                                        {
                                            id = specialities_.Id_Especialidad,
                                            value = specialities_.Nombre
                                        }).ToList(),
                        notspecialities = ent.Especialidad.Select(es => new ComboModel { id = es.Id_Especialidad, value =es.Nombre }).ToList()

                       
                    }).ToList();//.Take(pageSize).Skip(startingPageIndex * pageSize)

                    rp.data = list;
                    rp.cantidad = list.Count();
                }
                //retorna un response, con el campo data lleno con la respuesta.               
                return autil.MensajeRetorno(ref rp, 9, null, null, HttpStatusCode.OK);
            }

            catch (Exception ex)
            {
                //error general
                rp = autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
                return rp;
            }
        }

        public object GetEntityEdit(EntidadModel model)
        {
            List<EntidadModel> rl = new List<EntidadModel>();
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    //todas las especialidades
                    List<Especialidad> gall = ent.Especialidad.ToList();
                    //trae todas las especialidades que SI tiene la entidad
                    List<Especialidad> gesp = ent.Especialidad_Entidad.Where(t => t.Id_Entidad == model.Id_Entidad).Select(y => y.Especialidad).ToList();

                    //llena la lista que se regresa en el objeto con las especialidades que SI tiene
                    EspecialityList gel = new EspecialityList();
                    //gel.specialities = gesp.Select(l => new ComboListModel
                    //{
                    //    id = l.Id_Especialidad.ToString(),
                    //    value = l.Nombre

                    //}).ToList();
                 
                    ////esta es la lista de especialidades que NO tiene la entidad
                    //EspecialityList gnel = new EspecialityList();                
                    //gnel.specialities = gall.Except(gesp).Select(l => new ComboListModel
                    //{
                    //    id = l.Id_Especialidad.ToString(),
                    //    value = l.Nombre

                    //}).ToList();

                    var list = ent.Entidad.Where(e=> e.Id_Entidad == model.Id_Entidad).Select(u => new
                    {
                        identity = u.Id_Entidad,
                        nit = u.Nit,
                        name = u.Nombre,
                        organization = u.Organizacion,
                        email = u.Email,
                        entitycode = u.CodigoEntidad,
                        specialities = gel
                        //notspecialities = gnel

                    }).ToList();//.Take(pageSize).Skip(startingPageIndex * pageSize)

                    rp.data = list;
                    rp.cantidad = list.Count();
                }
                //retorna un response, con el campo data lleno con la respuesta.               
                return autil.MensajeRetorno(ref rp, 9, null, null, HttpStatusCode.OK);

            }

            catch (Exception ex)
            {
                //error general
                rp = autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
                return rp;
            }
        }

        #endregion

        #region Seccion Relacionadas

        public object CreateEspecialidadEntidad(EntidadModel model)
        {
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(model.token);
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Especialidad_Entidad ee = ent.Especialidad_Entidad.Where(t => t.Id_Entidad == entidad).SingleOrDefault();

                        if (ee == null)
                        {
                            ee = new Especialidad_Entidad();
                            ee.Id_Entidad = entidad;
                            //ee.Id_Especialidad = model.Id_Especialidad;
                            ee.Estado = true;
                            ee.Usuario_Create = usuario;
                            ee.Usuario_Update = usuario;
                            ee.Fecha_Create = DateTime.Now;
                            ee.Fecha_Update = DateTime.Now;
                            ent.Especialidad_Entidad.Add(ee);
                            ent.SaveChanges();

                            //se genera el codigo del mensaje de retorno exitoso
                            return rp = autil.MensajeRetorno(ref rp, 2, string.Empty, null, HttpStatusCode.OK);
                        }
                        else
                        {
                            //especialidad ya fue agregada
                            return rp = autil.MensajeRetorno(ref rp, 27, string.Empty, null, HttpStatusCode.OK);
                        }

                    }
                    else
                    {
                        //token invalido
                        rp = autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                        return rp;
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

        public object EditEspecialidadEntidad(EntidadModel model)
        {
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(model.token);
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Especialidad_Entidad ee = ent.Especialidad_Entidad.Where(t => t.Id_Entidad == entidad).SingleOrDefault();

                        if (ee != null)
                        {
                            //ee.Estado = model.Estado.Value;
                            ee.Usuario_Update = usuario;
                            ee.Fecha_Update = DateTime.Now;
                            ent.SaveChanges();

                            //se genera el codigo del mensaje de retorno exitoso
                            return rp = autil.MensajeRetorno(ref rp, 20, string.Empty, null, HttpStatusCode.OK);
                        }
                        else
                        {
                            //especialidad ya fue agregada
                            return rp = autil.MensajeRetorno(ref rp, 28, string.Empty, null, HttpStatusCode.OK);
                        }

                    }
                    else
                    {
                        //token invalido
                        rp = autil.MensajeRetorno(ref rp, 1, string.Empty, null, HttpStatusCode.OK);
                        return rp;
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

        #endregion


        #region Coreo
        private AlternateView SetWelcomeEmailBody(string subject, string login, string password)
        {
            try
            {
                //se arma el correo que se envia para el ambio de clave
                string plantilla = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["WelcomeTemplate"]);

                var html = System.IO.File.ReadAllText(plantilla);
                html = html.Replace("{{name}}", subject);
                html = html.Replace("{{user}}", login);
                html = html.Replace("{{password}}", password);

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