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
    public class aGenericLists
    {
        TokenValidationHandler tvh = new TokenValidationHandler();
        ClaimsPrincipal cp = new ClaimsPrincipal();
        aUtilities autil = new aUtilities();

        #region Listas

        public object GetDepartament(Basic model)
        {
            Response rp = new Response();
            try
            {
                cp = tvh.getprincipal(Convert.ToString(model.token));
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    var pb = ent.Departamento.Select(l => new
                    {
                        code = l.Dane_Id,
                        value = l.Nombre
                    }).ToList();

                    rp.data = pb;

                    return autil.MensajeRetorno(ref rp, 9, string.Empty, null, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }
        public object GetMunicipality(Basic model)
        {
            Response rp = new Response();
            try
            {
                cp = tvh.getprincipal(Convert.ToString(model.token));
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    int id = int.Parse(model.id);
                    var pb = ent.Municipio.Where(t => t.Departamento_Id == id).Select(l => new
                    {
                        code = l.Dane_Id,
                        value = l.Nombre
                    }).ToList();

                    rp.data = pb;

                    return autil.MensajeRetorno(ref rp, 9, string.Empty, null, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }
        public object GetNeighborhood(Basic model)
        {
            Response rp = new Response();
            try
            {
                cp = tvh.getprincipal(Convert.ToString(model.token));
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    int id = int.Parse(model.id);
                    var pb = ent.Poblado.Where(t => t.Municipio_Id == id).Select(l => new
                    {
                        code = l.Poblado_Id,
                        value = l.Nombre
                    }).ToList();

                    rp.data = pb;

                    return autil.MensajeRetorno(ref rp, 9, string.Empty, null, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }

        public DepartamentList GetFullDepartament()
        {
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    DepartamentList gl = new DepartamentList();
                    gl.departament = ent.Departamento.Select(l => new BasicList
                    {
                        name = l.Dane_Id.ToString(),
                        value = l.Nombre
                    }).ToList();

                    return gl;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public MunicipalityList GetFullMunicipality()
        {

            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    MunicipalityList gl = new MunicipalityList();
                    gl.municipality = ent.Municipio.Select(l => new BasicList
                    {
                        name = l.Dane_Id.ToString(),
                        value = l.Nombre,
                        keylink = l.Departamento_Id.ToString()
                    }).ToList();

                    return gl;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public NeighborhoodList GetFullNeighborhood()
        {
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    NeighborhoodList gl = new NeighborhoodList();
                    gl.neighborhood = ent.Poblado.Select(l => new BasicList
                    {
                        name = l.Poblado_Id.ToString(),
                        value = l.Nombre,
                        keylink = l.Municipio_Id.ToString()
                    }).ToList();

                    return gl;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ProfetionalTypeList GetProfetionalType()
        {
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    ProfetionalTypeList gl = new ProfetionalTypeList();
                    gl.profetionaltype = ent.Tipo_Profesional.Select(l => new BasicList
                    {
                        name = l.Id_Tipo_Profesional.ToString(),
                        value = l.Nombre
                    }).ToList();

                    return gl;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public LinkTypeList GetLinkType()
        {
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    LinkTypeList gl = new LinkTypeList();
                    gl.linktype = ent.Tipo_Vinculacion.Select(l => new BasicList
                    {
                        name = l.Id_Tipo_Vinculacion.ToString(),
                        value = l.Nombre
                    }).ToList();

                    return gl;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public RolList GetRolList()
        {
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    RolList gl = new RolList();
                    gl.rollist = ent.Rol.Select(l => new BasicList
                    {
                        name = l.Id_Rol.ToString(),
                        value = l.Nombre
                    }).ToList();

                    return gl;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }



        #endregion
    }
}