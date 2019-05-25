using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace MilenioApi.Action
{
    public class aRol
    {
        aUtilities autil = new aUtilities();
        ClaimsPrincipal cp = new ClaimsPrincipal();
        TokenValidationHandler tvh = new TokenValidationHandler();

        #region Seccion Rol

        /// <summary>
        /// Metodo para tener todos los roles activos
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object GetRoles(UserModel model)
        {
            List<ComboModel> rl = new List<ComboModel>();
            Response rp = new Response();
            try
            {
                cp = tvh.getprincipal(Convert.ToString(model.token));

                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    rl = ent.Rol.Where(r => r.Estado == true).Select(l => new ComboModel { id = l.Id_Rol, value = l.Nombre }).ToList();
                    rp.data = rl;

                }
                //retorna un response, con el campo data lleno con la respuesta.               
                return autil.ReturnMesagge(ref rp, 9, null, null, HttpStatusCode.OK);
            }

            catch (Exception ex)
            {
                //error general               
                return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }
        /// <summary>
        /// Metodo para tener todos los roles de un usuario
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object GetRolesUsuario(UserModel model)
        {
            List<ComboModel> rl = new List<ComboModel>();
            Response rp = new Response();
            try
            {
                cp = tvh.getprincipal(Convert.ToString(model.token));
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                    rl = ent.Rol_Usuario.Where(u => u.Id_Entidad == entidad && u.Id_Usuario == model.Id_Usuario && u.Estado == true).Select(l => new ComboModel { id = l.Id_Rol, value = l.Rol.Nombre }).ToList();
                    rp.data = rl;
                }

                //retorna un response, con el campo data lleno con la respuesta.
                return autil.ReturnMesagge(ref rp, 9, null, null, HttpStatusCode.OK);
            }

            catch (Exception ex)
            {
                //error general               
                return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }

        public object GetNotRolesUsuario(UserModel model)
        {
            List<ComboModel> rl = new List<ComboModel>();
            try
            {
                cp = tvh.getprincipal(Convert.ToString(model.token));

                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                    List<Rol> rol = ent.Rol_Usuario.Where(u => u.Id_Entidad == entidad && u.Id_Usuario == model.Id_Usuario && u.Estado == true).Select(t => t.Rol).ToList();
                    List<Rol> _rol = ent.Rol.ToList();

                    rl = _rol.Except(rol).Select(l => new ComboModel { id = l.Id_Rol, value = l.Nombre }).ToList();

                    return rl;
                }
            }
            catch (Exception ex)
            {
                //error general
                Response rp = new Response();
                return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Metodo para agregar rol a un usuario
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object CreateRolUsuario(UserModel model)
        {
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));

                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                    Rol_Usuario ru = ent.Rol_Usuario.Where(r => r.Id_Rol == model.Id_Rol && r.Id_Usuario == model.Id_Usuario && r.Id_Entidad == entidad).SingleOrDefault();
                    if (ru == null)
                    {
                        ru = new Rol_Usuario();
                        ru.Id_Entidad = entidad;
                        ru.Id_Usuario = model.Id_Usuario;
                        ru.Id_Rol = model.Id_Rol;
                        ru.Estado = true;
                        ru.Fecha_Create = DateTime.Now;
                        ru.Usuario_Create = usuario;
                        ru.Fecha_Update = DateTime.Now;
                        ru.Usuario_Update = usuario;
                        ent.Rol_Usuario.Add(ru);
                    }
                    else
                    {
                        ru.Estado = true;
                        ru.Fecha_Create = DateTime.Now;
                        ru.Usuario_Create = usuario;
                        ru.Fecha_Update = DateTime.Now;
                        ru.Usuario_Update = usuario;
                    }

                    ent.SaveChanges();
                    //se genera el codigo del mensaje de retorno exitoso
                    return rp = autil.ReturnMesagge(ref rp, 2, string.Empty, null, HttpStatusCode.OK);

                }
                catch (Exception ex)
                {
                    //error general
                    rp = autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.OK);
                    return rp;
                }
            }
        }
        /// <summary>
        /// Metodo para elimiar rol a un usuario
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object EditRolUsuario(UserModel model)
        {
            Response rp = new Response();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                try
                {
                    cp = tvh.getprincipal(Convert.ToString(model.token));

                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                    Rol_Usuario ru = ent.Rol_Usuario.Where(r => r.Id_Rol == model.Id_Rol && r.Id_Usuario == model.Id_Usuario && r.Id_Entidad == entidad).SingleOrDefault();
                    if (ru != null)
                    {
                        ru.Estado = bool.Parse(model.Estado.ToString());
                        ru.Usuario_Update = usuario;
                        ru.Fecha_Update = DateTime.Now;
                        ent.SaveChanges();
                    }
                    //se genera el codigo del mensaje de retorno exitoso
                    return autil.ReturnMesagge(ref rp, 20, string.Empty, null, HttpStatusCode.OK);

                }
                catch (Exception ex)
                {
                    //error general
                    rp = autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
                    return rp;
                }
            }
        }

        #endregion
    }
}