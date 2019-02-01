using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Web;
using WebApi.Jwt;

namespace MilenioApi.Action
{
    public class aProfesionalcs
    {
        private TokenController tk = new TokenController();
        aUtilities autil = new aUtilities();
        ClaimsPrincipal cp = new ClaimsPrincipal();

        #region Crud
        /// <summary>
        /// Matodo para crear prefesionales
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic CreateProfesional(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Guid tipoidentificacion = Guid.Parse(httpRequest.Form["tipoidentificacion"]);
                        int identificacion = int.Parse(httpRequest.Form["identificacion"]);
                        string nombre = Convert.ToString(httpRequest.Form["nombre"]);
                        string primer_apellido = Convert.ToString(httpRequest.Form["primerapellido"]);
                        string segundo_apellido = Convert.ToString(httpRequest.Form["segundoapellido"]);
                        string registro_profesional = Convert.ToString(httpRequest.Form["registroprofesional"]);
                        string sexo = Convert.ToString(httpRequest.Form["sexo"]);
                        DateTime fnacimiento = Convert.ToDateTime(httpRequest.Form["fechanacimiento"]);

                        ///FOTO
                        ///
                        String foto = string.Empty;
                        if (httpRequest.Files.Count > 0)
                        {
                            var _foto = httpRequest.Files;
                            Byte[] Content = new BinaryReader(_foto[0].InputStream).ReadBytes(_foto[0].ContentLength);
                            foto = Convert.ToBase64String(Content);
                        }

                        string estadocivil = Convert.ToString(httpRequest.Form["estadocivil"]);
                        string tipo_sangre = Convert.ToString(httpRequest.Form["tiposangre"]);
                        Guid ubicacion = Guid.Parse(httpRequest.Form["ubicacion"]);
                        DateTime fcontratacion = Convert.ToDateTime(httpRequest.Form["fechacontratacion"]);
                        string observaciones = Convert.ToString(httpRequest.Form["observaciones"]);
                        Guid tipo_vinculacion = Guid.Parse(httpRequest.Form["tipovinculacion"]);

                        Profesional pr;

                        pr = ent.Profesional.Where(p => p.NumeroIdentificacion == identificacion && p.Id_Entidad == entidad).SingleOrDefault();

                        if (pr == null)
                        {
                            pr = ent.Profesional.Where(p => p.Registro_Profesional == registro_profesional && p.Id_Entidad == entidad).SingleOrDefault();
                            if (pr == null)
                            {
                                pr = new Profesional();
                                pr.Id_Profesional = Guid.NewGuid();
                                pr.Id_Entidad = entidad;
                                pr.Nombres = nombre;
                                pr.Primer_Apellido = primer_apellido;
                                pr.Segundo_Apellido = segundo_apellido;
                                pr.TipoIdentificacion_Id = tipoidentificacion;
                                pr.NumeroIdentificacion = identificacion;
                                pr.Ubicacion_Id = ubicacion;
                                pr.Sexo = sexo;
                                pr.FNacimiento = fnacimiento;
                                pr.Registro_Profesional = registro_profesional;
                                pr.Foto = foto;
                                pr.Estado_Civil = estadocivil;
                                pr.Estado = true;
                                pr.TipoSangre = tipo_sangre;
                                pr.Fecha_Contratacion = fcontratacion;
                                pr.Id_Tipo_Vinculacion = tipo_vinculacion;
                                pr.Observaciones = observaciones;
                                pr.Created_At = DateTime.Now;
                                pr.Updated_At = DateTime.Now;
                                pr.Usuario_Update = usuario;

                                ent.Profesional.Add(pr);
                                ent.SaveChanges();

                                //se genera el codigo del mensaje de retorno exitoso
                                return ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
                            }
                            else
                            {
                                //registro prfesional existe
                                return ret = autil.MensajeRetorno(ref ret, 3, string.Empty, null);
                            }
                        }
                        else
                        {
                            //cedula existe
                            return ret = autil.MensajeRetorno(ref ret, 5, string.Empty, null);
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

        /// <summary>
        /// Metodo para editar profesionales
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic EditProfesional(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Guid id_profesional = Guid.Parse(httpRequest.Form["id_profesional"]);
                        Guid tipoidentificacion = Guid.Parse(httpRequest.Form["tipoidentificacion"]);
                        int identificacion = int.Parse(httpRequest.Form["identificacion"]);
                        string nombre = Convert.ToString(httpRequest.Form["nombre"]);
                        string primer_apellido = Convert.ToString(httpRequest.Form["primerapellido"]);
                        string segundo_apellido = Convert.ToString(httpRequest.Form["segundoapellido"]);
                        string registro_profesional = Convert.ToString(httpRequest.Form["registroprofesional"]);
                        string sexo = Convert.ToString(httpRequest.Form["sexo"]);
                        DateTime fnacimiento = Convert.ToDateTime(httpRequest.Form["fechanacimiento"]);

                        ///FOTO
                        ///
                        String foto = string.Empty;
                        if (httpRequest.Files.Count > 0)
                        {
                            var _foto = httpRequest.Files;
                            Byte[] Content = new BinaryReader(_foto[0].InputStream).ReadBytes(_foto[0].ContentLength);
                            foto = Convert.ToBase64String(Content);
                        }

                        string estadocivil = Convert.ToString(httpRequest.Form["estadocivil"]);
                        string tipo_sangre = Convert.ToString(httpRequest.Form["tiposangre"]);
                        Guid ubicacion = Guid.Parse(httpRequest.Form["ubicacion"]);                        
                        DateTime fcontratacion = Convert.ToDateTime(httpRequest.Form["fechacontratacion"]);
                        string observaciones = Convert.ToString(httpRequest.Form["observaciones"]);
                        Guid tipo_vinculacion = Guid.Parse(httpRequest.Form["tipovinculacion"]);
                        bool estado = Convert.ToBoolean(httpRequest.Form["estado"]);

                        Profesional pr;

                        pr = ent.Profesional.Where(p => p.NumeroIdentificacion == identificacion && p.Id_Entidad == entidad).SingleOrDefault();

                        if (pr == null)
                        {
                            pr = ent.Profesional.Where(p => p.Registro_Profesional == registro_profesional && p.Id_Entidad == entidad && p.Id_Profesional != id_profesional).SingleOrDefault();
                            if (pr == null)
                            {
                                pr = ent.Profesional.Where(t => t.Id_Profesional == id_profesional).SingleOrDefault();
                                pr.Id_Entidad = entidad;
                                pr.Nombres = nombre;
                                pr.Primer_Apellido = primer_apellido;
                                pr.Segundo_Apellido = segundo_apellido;
                                pr.TipoIdentificacion_Id = tipoidentificacion;                                
                                
                                pr.Ubicacion_Id = ubicacion;
                                pr.Sexo = sexo;
                                pr.FNacimiento = fnacimiento;
                                pr.Registro_Profesional = registro_profesional;
                                pr.Foto = foto;
                                pr.Estado_Civil = estadocivil;
                                pr.Estado = estado;
                                pr.TipoSangre = tipo_sangre;
                                pr.Fecha_Contratacion = fcontratacion;
                                pr.Id_Tipo_Vinculacion = tipo_vinculacion;
                                pr.Observaciones = observaciones;
                                pr.Updated_At = DateTime.Now;
                                pr.Usuario_Update = usuario;
                                ent.SaveChanges();

                                //se genera el codigo del mensaje de retorno exitoso
                                return ret = autil.MensajeRetorno(ref ret, 20, string.Empty, null);
                            }
                            else
                            {
                                //registro prfesional existe
                                return ret = autil.MensajeRetorno(ref ret, 3, string.Empty, null);
                            }
                        }
                        else
                        {
                            //cedula existe
                            return ret = autil.MensajeRetorno(ref ret, 5, string.Empty, null);
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

       /// <summary>
       /// Metodo para agregarle usuario al profesional
       /// </summary>
       /// <param name="httpRequest"></param>
       /// <returns></returns>
        public Basic AddUser(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                        Guid id_profesional = Guid.Parse(httpRequest.Form["id_profesional"]);
                        Guid id_usuario = Guid.Parse(httpRequest.Form["usuario"]);

                        Profesional pr = ent.Profesional.Where(t => t.Id_Profesional == id_profesional).SingleOrDefault();
                        pr.Id_Usuario = id_usuario;
                        ent.SaveChanges();

                        //se genera el codigo del mensaje de retorno exitoso
                        return ret = autil.MensajeRetorno(ref ret, 20, string.Empty, null);
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

        #region consultas

        

        public List<Profesional> GetProfesionalesEntidad(HttpRequest httpRequest)
        {
            List<Profesional> lp = new List<Profesional>();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        lp = ent.Profesional.Where(e => e.Id_Entidad == entidad).ToList();

                        return lp;
                    }
                    else
                    {
                        return lp;
                    }
                }
                catch (Exception)
                {
                    return lp;
                }
            }
        }

        public List<Profesional> GetProfesionalesEntidadEstado(HttpRequest httpRequest)
        {
            List<Profesional> lp = new List<Profesional>();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["token"]));
                    bool estado = Convert.ToBoolean(httpRequest.Form["estado"]);
                    if (cp != null)
                    {
                        Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                        lp = ent.Profesional.Where(e => e.Id_Entidad == entidad && e.Estado == estado).ToList();

                        return lp;
                    }
                    else
                    {
                        return lp;
                    }
                }
                catch (Exception)
                {
                    return lp;
                }
            }
        }

        #endregion
    }
}