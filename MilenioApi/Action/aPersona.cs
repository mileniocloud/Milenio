using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;

namespace MilenioApi.Action
{
    public class aPersona
    {
        TokenController tk = new TokenController();
        aUtilities autil = new aUtilities();
        public Return CreatePersona(HttpRequest httpRequest)
        {
            Return ret = new Return();
            try
            {
                if (tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"])) != null)
                {
                    String file = string.Empty;
                    if (httpRequest.Files.Count > 0)
                    {
                        var foto = httpRequest.Files;
                        Byte[] Content = new BinaryReader(foto[0].InputStream).ReadBytes(foto[0].ContentLength);
                        file = Convert.ToBase64String(Content);
                    }

                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        //PRIMERO SE VALIDA QUE ESA PERSONA CON LA CEDULA EMAIL O LOGIN NO EXISTA//
                        Persona p = new Persona();

                        int nroidentificacion = Convert.ToInt32(httpRequest.Form["NroIdentificacion"]);
                        string email = Convert.ToString(httpRequest.Form["Email"]);
                        string login = Convert.ToString(httpRequest.Form["Login"]);
                        Guid entidad_id = Guid.Parse(httpRequest.Form["Entidad_Id"]);

                        Guid? user_id = null;
                        if (!string.IsNullOrEmpty(httpRequest.Form["User_Id"]))
                            user_id = Guid.Parse(httpRequest.Form["User_Id"]);

                        Guid? rol_id = null;
                        if (!string.IsNullOrEmpty(httpRequest.Form["Roles"]))
                            rol_id = Guid.Parse(httpRequest.Form["Roles"]);

                        int vcedula = (from pr in ent.Persona
                                       where pr.NumeroIdentificacion == nroidentificacion
                                       select pr).Count();

                        int vemail = (from pr in ent.Persona
                                      where pr.Email == email
                                      select pr).Count();

                        int vlogin = (from pr in ent.Persona
                                      where pr.Login == login
                                      select pr).Count();

                        if (vcedula == 0)
                        {
                            if (vemail == 0)
                            {
                                if (vlogin == 0)
                                {
                                    //SE VALIDA EL GUID DE TIPO DE IDENTIFICACION//
                                    Guid ti = new Guid();
                                    if (!string.IsNullOrEmpty(httpRequest.Form["TipoIdentificacion"]))
                                        ti = Guid.Parse(httpRequest.Form["TipoIdentificacion"]);

                                    //SE LLENA EL OBJETO UBICACION PARA CREAR PRIMERO LA UBICACION DE LA PERSONA//
                                    Guid codigo_ubicacion = Guid.NewGuid();
                                    Ubicacion ub = new Ubicacion();
                                    ub.Codigo_Id = codigo_ubicacion;
                                    ub.Poblado_Id = Convert.ToInt32(httpRequest.Form["PobladoId"]);
                                    ub.Direccion = Convert.ToString(httpRequest.Form["Direccion"]);
                                    ub.Latitud = Convert.ToString(httpRequest.Form["Latitud"]);
                                    ub.Longitud = Convert.ToString(httpRequest.Form["Longitud"]);
                                    ub.Created_At = DateTime.Now;
                                    ub.Updated_At = DateTime.Now;
                                    ub.Usuario_Update = user_id;
                                    ent.Ubicacion.Add(ub);


                                    //SE LLENA EL OBJETO PERSONA//
                                    p = new Persona();
                                    p.Codigo_Id = Guid.NewGuid();
                                    p.NumeroIdentificacion = nroidentificacion;
                                    p.TipoIdentificacion_Id = ti;
                                    p.Nombres = Convert.ToString(httpRequest.Form["Nombres"]);
                                    p.Apellidos = Convert.ToString(httpRequest.Form["Apellidos"]);
                                    p.Sexo = Convert.ToString(httpRequest.Form["Sexo"]);
                                    p.FNacimiento = Convert.ToDateTime(httpRequest.Form["FechaNacimiento"]);
                                    p.Nacionalidad = Convert.ToString(httpRequest.Form["Nacionalidad"]);
                                    p.LibretaMilitar = Convert.ToString(httpRequest.Form["LibretaMilitar"]);
                                    p.TipoSangre = Convert.ToString(httpRequest.Form["TipoSangre"]);
                                    p.Ubicacion_Id = codigo_ubicacion;
                                    p.Foto = file;
                                    p.Estado_Persona = true;
                                    //datos para el usuario//
                                    p.Login = login;
                                    p.Password = autil.Sha(Convert.ToString(httpRequest.Form["Password"]));
                                    p.Email = email;
                                    p.Estado_Usuario = true;
                                    p.Cambiar_Clave = true;
                                    p.Created_At = DateTime.Now;
                                    p.Updated_At = DateTime.Now;
                                    p.Usuario_Update = user_id;
                                    ent.Persona.Add(p);

                                    //ASIGNAMOS LA PERSONA A LA ENTIDAD//
                                    Entidad_Persona_Rol ep = new Entidad_Persona_Rol();
                                    ep.Persona_Id = p.Codigo_Id;
                                    ep.Entidad_Id = entidad_id;
                                    ep.Created_At = DateTime.Now;
                                    ep.Updated_At = DateTime.Now;
                                    ep.Estado = true;
                                    ep.Usuario_Update = user_id;
                                    ent.Entidad_Persona_Rol.Add(ep);

                                    //SI TIENE ROLL SE LO ASIGNAMOS
                                    if (rol_id != null)
                                    {
                                        Entidad_Persona_Rol ur = new Entidad_Persona_Rol();
                                        ur.Persona_Id = p.Codigo_Id;
                                        ur.Rol_Id = rol_id.Value;
                                        ur.Entidad_Id = entidad_id;
                                        ur.Created_At = DateTime.Now;
                                        ur.Updated_At = DateTime.Now;
                                        ur.Usuario_Update = user_id;
                                        ur.Estado = true;
                                        ent.Entidad_Persona_Rol.Add(ur);
                                    }

                                    //se envia a crear todo
                                    ent.SaveChanges();
                                    //se genera el codigo del mensaje de retorno exitoso
                                    ret = autil.MensajeRetorno(ref ret, 2, string.Empty, p.Codigo_Id);
                                }
                                else
                                {
                                    //LOGIN EXISTE
                                    ret = autil.MensajeRetorno(ref ret, 7, string.Empty, null);
                                }
                            }
                            else
                            {
                                //EMAIL EXISTE
                                ret = autil.MensajeRetorno(ref ret, 6, string.Empty, null);
                            }
                        }
                        else
                        {
                            //CEDULA EXISTE
                            ret = autil.MensajeRetorno(ref ret, 5, string.Empty, null);
                        }
                    }
                    return ret;
                }
                else
                {
                    ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
                    return ret;
                }
            }
            catch (Exception ex)
            {
                ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                return ret;
            }
        }
    }
}