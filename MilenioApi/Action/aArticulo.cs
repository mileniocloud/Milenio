using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace MilenioApi.Action
{
    public class aArticulo
    {
        TokenController tk = new TokenController();
        ClaimsPrincipal cp = new ClaimsPrincipal();
        aUtilities autil = new aUtilities();

        /// <summary>
        /// Metodo que permite crear una categoria
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic CreateCategoria(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                if (cp != null)
                {
                    //se saca el id de la entidad del token
                    string entidad = cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault();
                    Guid entidad_id = Guid.Parse(entidad);

                    //se saca el usiario que esta creando la persona del token
                    Guid? user_id = null;
                    string usid = cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(usid))
                        user_id = Guid.Parse(usid);

                    string Nombre = Convert.ToString(httpRequest.Form["Nombre"]);
                    string Descripcion = Convert.ToString(httpRequest.Form["Descripcion"]);
                    int referencia = Convert.ToInt32(httpRequest.Form["Referencia"]);

                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        int vref = (from c in ent.Categoria
                                    where c.Entidad_Id == entidad_id
                                    && c.Referencia == referencia
                                    select c).Count();

                        int vnomb = (from c in ent.Categoria
                                     where c.Entidad_Id == entidad_id
                                     && c.Nombre == Nombre
                                     select c).Count();

                        if (vref == 0)
                        {
                            if (vnomb == 0)
                            {
                                Categoria ct = new Categoria();
                                ct.Codigo_Id = Guid.NewGuid();
                                ct.Nombre = Nombre;
                                ct.Descripcion = Descripcion;
                                ct.Referencia = referencia;
                                ct.Entidad_Id = entidad_id;
                                ct.Estado = true;
                                ct.Created_At = DateTime.Now;
                                ct.Updated_At = DateTime.Now;
                                ct.Usuario_Update = user_id;

                                ent.Categoria.Add(ct);
                                ent.SaveChanges();
                                //ingresado exitoso
                                ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
                            }
                            else
                            {
                                //nombre categoria existe
                                ret = autil.MensajeRetorno(ref ret, 24, string.Empty, null);
                            }
                        }
                        else
                        {
                            //referencia categoria existe
                            ret = autil.MensajeRetorno(ref ret, 25, string.Empty, null);
                        }
                    }
                }
                else
                {
                    //token invalido
                    ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
                }

                return ret;
            }
            catch (Exception ex)
            {
                //error general
                ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                return ret;
            }
        }

        /// <summary>
        /// Metodo que permite editar una categoria
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic EditCategoria(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                if (cp != null)
                {
                    //se saca el id de la entidad del token
                    string entidad = cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault();
                    Guid entidad_id = Guid.Parse(entidad);

                    //se saca el usiario que esta creando la persona del token
                    Guid? user_id = null;
                    string usid = cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(usid))
                        user_id = Guid.Parse(usid);

                    string Nombre = Convert.ToString(httpRequest.Form["Nombre"]);
                    string Descripcion = Convert.ToString(httpRequest.Form["Descripcion"]);
                    int referencia = Convert.ToInt32(httpRequest.Form["Referencia"]);
                    Guid Categoria_Id = Guid.Parse(httpRequest.Form["Categoria_Id"]);
                    bool Estado = bool.Parse(httpRequest.Form["Estado"]);

                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        int vref = (from c in ent.Categoria
                                    where c.Entidad_Id == entidad_id
                                    && c.Referencia == referencia
                                    && c.Codigo_Id != Categoria_Id
                                    select c).Count();

                        int vnomb = (from c in ent.Categoria
                                     where c.Entidad_Id == entidad_id
                                     && c.Nombre == Nombre
                                     && c.Codigo_Id != Categoria_Id
                                     select c).Count();

                        if (vref == 0)
                        {
                            if (vnomb == 0)
                            {
                                Categoria ct = (from y in ent.Categoria
                                                where y.Codigo_Id == Categoria_Id
                                                select y).SingleOrDefault();

                                if (ct != null)
                                {
                                    ct.Nombre = Nombre;
                                    ct.Descripcion = Descripcion;
                                    ct.Referencia = referencia;
                                    ct.Entidad_Id = entidad_id;
                                    ct.Estado = Estado;
                                    ct.Updated_At = DateTime.Now;
                                    ct.Usuario_Update = user_id;

                                    ent.SaveChanges();
                                    //Mensaje exitoso
                                    ret = autil.MensajeRetorno(ref ret, 20, string.Empty, null);
                                }
                                else
                                {
                                    //datos invalidos
                                    ret = autil.MensajeRetorno(ref ret, 15, string.Empty, null);
                                }
                            }
                            else
                            {
                                //nombre categoria existe
                                ret = autil.MensajeRetorno(ref ret, 24, string.Empty, null);
                            }
                        }
                        else
                        {
                            //referencia categoria existe
                            ret = autil.MensajeRetorno(ref ret, 25, string.Empty, null);
                        }
                    }
                }
                else
                {
                    //token invalido
                    ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
                }

                return ret;
            }
            catch (Exception ex)
            {
                //error general
                ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                return ret;
            }
        }
        /// <summary>
        /// Metodo que permite la creacion de sub categorias
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic CreateSubCategoria(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                if (cp != null)
                {
                    //se saca el usiario que esta creando la persona del token
                    Guid? user_id = null;
                    string usid = cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(usid))
                        user_id = Guid.Parse(usid);

                    string Nombre = Convert.ToString(httpRequest.Form["Nombre"]);
                    string Descripcion = Convert.ToString(httpRequest.Form["Descripcion"]);
                    int referencia = Convert.ToInt32(httpRequest.Form["Referencia"]);
                    Guid Categoria_Id = Guid.Parse(httpRequest.Form["Categoria_Id"]);

                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        int vref = (from c in ent.Subcategoria
                                    where
                                    c.Categoria_Id == Categoria_Id &&
                                    c.Referencia == referencia
                                    select c).Count();

                        int vnomb = (from c in ent.Subcategoria
                                     where
                                     c.Categoria_Id == Categoria_Id
                                     && c.Nombre == Nombre
                                     select c).Count();

                        if (vref == 0)
                        {
                            if (vnomb == 0)
                            {

                                Subcategoria ct = new Subcategoria();
                                ct.Codigo_Id = Guid.NewGuid();
                                ct.Nombre = Nombre;
                                ct.Descripcion = Descripcion;
                                ct.Referencia = referencia;
                                ct.Categoria_Id = Categoria_Id;
                                ct.Estado = true;
                                ct.Created_At = DateTime.Now;
                                ct.Updated_At = DateTime.Now;
                                ct.Usuario_Update = user_id;

                                ent.Subcategoria.Add(ct);
                                ent.SaveChanges();
                                //ingresado con exito
                                ret = autil.MensajeRetorno(ref ret, 2, string.Empty, null);
                            }
                            else
                            {
                                //nombre subcategoria existe
                                ret = autil.MensajeRetorno(ref ret, 26, string.Empty, null);
                            }
                        }
                        else
                        {
                            //referencia categoria existe
                            ret = autil.MensajeRetorno(ref ret, 27, string.Empty, null);
                        }
                    }
                }
                else
                {
                    //token invalido
                    ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
                }

                return ret;
            }
            catch (Exception ex)
            {
                //error general
                ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                return ret;
            }
        }
        /// <summary>
        /// Metodo que permite editar una Subcategoria
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public Basic EditSubCategoria(HttpRequest httpRequest)
        {
            Basic ret = new Basic();
            try
            {
                cp = tk.ValidateToken(Convert.ToString(httpRequest.Form["Token"]));
                if (cp != null)
                {
                    //se saca el id de la entidad del token
                    string entidad = cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault();
                    Guid entidad_id = Guid.Parse(entidad);

                    //se saca el usiario que esta creando la persona del token
                    Guid? user_id = null;
                    string usid = cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(usid))
                        user_id = Guid.Parse(usid);

                    string Nombre = Convert.ToString(httpRequest.Form["Nombre"]);
                    string Descripcion = Convert.ToString(httpRequest.Form["Descripcion"]);
                    int referencia = Convert.ToInt32(httpRequest.Form["Referencia"]);
                    Guid SubCategoria_Id = Guid.Parse(httpRequest.Form["SubCategoria_Id"]);
                    bool Estado = bool.Parse(httpRequest.Form["Estado"]);

                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        int vref = (from c in ent.Subcategoria
                                    where
                                    c.Referencia == referencia
                                    && c.Codigo_Id != SubCategoria_Id
                                    select c).Count();

                        int vnomb = (from c in ent.Subcategoria
                                     where c.Nombre == Nombre
                                     && c.Codigo_Id != SubCategoria_Id
                                     select c).Count();

                        if (vref == 0)
                        {
                            if (vnomb == 0)
                            {
                                Subcategoria ct = (from y in ent.Subcategoria
                                                   where y.Codigo_Id == SubCategoria_Id
                                                   select y).SingleOrDefault();

                                if (ct != null)
                                {
                                    ct.Nombre = Nombre;
                                    ct.Descripcion = Descripcion;
                                    ct.Referencia = referencia;
                                    ct.Estado = Estado;
                                    ct.Updated_At = DateTime.Now;
                                    ct.Usuario_Update = user_id;

                                    ent.SaveChanges();
                                    //Mensaje exitoso
                                    ret = autil.MensajeRetorno(ref ret, 20, string.Empty, null);
                                }
                                else
                                {
                                    //datos invalidos
                                    ret = autil.MensajeRetorno(ref ret, 15, string.Empty, null);
                                }
                            }
                            else
                            {
                                //nombre Subcategoria existe
                                ret = autil.MensajeRetorno(ref ret, 26, string.Empty, null);
                            }
                        }
                        else
                        {
                            //referencia Subcategoria existe
                            ret = autil.MensajeRetorno(ref ret, 27, string.Empty, null);
                        }
                    }
                }
                else
                {
                    //token invalido
                    ret = autil.MensajeRetorno(ref ret, 1, string.Empty, null);
                }

                return ret;
            }
            catch (Exception ex)
            {
                //error general
                ret = autil.MensajeRetorno(ref ret, 4, ex.Message, null);
                return ret;
            }
        }
    }
}