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
                    }).OrderBy(o => o.value).ToList();

                    rp.data = pb;

                    return autil.ReturnMesagge(ref rp, 9, string.Empty, null, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
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
                    }).OrderBy(o => o.value).ToList();

                    rp.data = pb;

                    return autil.ReturnMesagge(ref rp, 9, string.Empty, null, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
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
                    }).OrderBy(o => o.value).ToList();

                    rp.data = pb;

                    return autil.ReturnMesagge(ref rp, 9, string.Empty, null, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
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
                        id = l.Dane_Id.ToString(),
                        value = l.Nombre
                    }).OrderBy(o => o.value).ToList();

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
                        id = l.Dane_Id.ToString(),
                        value = l.Nombre,
                        keylink = l.Departamento_Id.ToString()
                    }).OrderBy(o => o.value).ToList();

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
                        id = l.Poblado_Id.ToString(),
                        value = l.Nombre,
                        keylink = l.Municipio_Id.ToString()
                    }).OrderBy(o => o.value).ToList();

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
                        id = l.Id_Tipo_Profesional.ToString(),
                        value = l.Nombre
                    }).OrderBy(o => o.value).ToList();

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
                        id = l.Id_Tipo_Vinculacion.ToString(),
                        value = l.Nombre
                    }).OrderBy(o => o.value).ToList();

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
                        id = l.Id_Rol.ToString(),
                        value = l.Nombre
                    }).OrderBy(o => o.value).ToList();

                    return gl;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public object GetEspecialityList()
        {
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    //EspecialityList gl = new EspecialityList();
                    var gl = ent.Especialidad.Select(l => new ComboModel
                    {
                        id = l.Id_Especialidad,
                        value = l.Nombre

                    }).OrderBy(o => o.value).ToList();

                    return gl;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public object GetEspecialityListByEntity(Basic model)
        {
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Guid entity = Guid.Parse(model.id);
                    EspecialityList esp = new EspecialityList();
                    esp.specialities = ent.Especialidad_Entidad.Where(e => e.Id_Entidad == entity && e.Estado == true).Select(l => new ComboModel
                    {
                        id = l.Id_Especialidad,
                        value = l.Especialidad.Nombre

                    }).OrderBy(o => o.value).ToList();

                    return esp;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public object GetEspecialityListByEntity(Guid entity)
        {
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    EspecialityList gl = new EspecialityList();
                    gl.specialities = ent.Especialidad_Entidad.Where(e => e.Id_Entidad == entity && e.Estado == true && e.Especialidad_Profesional.Count() >= 1).Select(l => new ComboModel
                    {
                        id = l.Id_Especialidad,
                        value = l.Especialidad.Nombre

                    }).OrderBy(o => o.value).ToList();

                    return gl;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public object GetProfessionalListByEntity(Guid entity)
        {
            Response rp = new Response();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    ProfetionalSpecialtyList gl = new ProfetionalSpecialtyList();
                    gl.profetionalspecialty = ent.Especialidad_Profesional.Where(e => e.Id_Entidad == entity && e.Estado == true).Select(l => new BasicList
                    {
                        id = l.Id_Usuario.ToString(),
                        value = l.Usuario.Nombres + " " + l.Usuario.Primer_Apellido + " " + l.Usuario.Segundo_Apellido,
                        keylink = l.Id_Especialidad.ToString()
                    }).OrderBy(o => o.value).ToList();

                    return gl;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public object GetListsEntityForm(Basic model)
        {
            Response rp = new Response();
            aGenericLists gl = new aGenericLists();
            try
            {
                cp = tvh.getprincipal(Convert.ToString(model.token));

                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    List<object> listas = new List<object>();

                    listas.Add(gl.GetFullDepartament());
                    listas.Add(gl.GetFullMunicipality());
                    listas.Add(gl.GetFullNeighborhood());
                    listas.Add(gl.GetEspecialityList());

                    rp.data = listas;
                    //retorna un response, con el campo data lleno con la respuesta.               
                    return autil.ReturnMesagge(ref rp, 9, null, null, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                //error general
                return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }

        public object GetListsSheduleForm(Basic model)
        {

            Response rp = new Response();
            aGenericLists gl = new aGenericLists();
            try
            {
                cp = tvh.getprincipal(Convert.ToString(model.token));
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    List<object> listas = new List<object>();

                    listas.Add(gl.GetEspecialityListByEntity(entidad));
                    listas.Add(gl.GetProfessionalListByEntity(entidad));


                    rp.data = listas;
                    //retorna un response, con el campo data lleno con la respuesta.               
                    return autil.ReturnMesagge(ref rp, 9, null, null, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                //error general
                return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }

        public object GetListsUserForm(Basic model)
        {
            Response rp = new Response();
            aGenericLists gl = new aGenericLists();
            try
            {
                cp = tvh.getprincipal(Convert.ToString(model.token));

                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    List<object> listas = new List<object>();

                    listas.Add(gl.GetFullDepartament());
                    listas.Add(gl.GetFullMunicipality());
                    listas.Add(gl.GetFullNeighborhood());
                    listas.Add(gl.GetProfetionalType());
                    listas.Add(gl.GetLinkType());
                    listas.Add(gl.GetRolList());

                    rp.data = listas;
                    //retorna un response, con el campo data lleno con la respuesta.               
                    return autil.ReturnMesagge(ref rp, 9, null, null, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                //error general
                return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }

        public object GetServiceProviderList(Basic model)
        {
            Response rp = new Response();
            aGenericLists gl = new aGenericLists();
            try
            {
                cp = tvh.getprincipal(Convert.ToString(model.token));

                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());

                    var docs = ent.Entidad_Usuario.Where(u => u.Id_Entidad == entidad && u.Estado == true && u.Usuario.Presta_Servicio == true)
                                .Select(t => new
                                {
                                    id = t.Usuario.Id_Usuario,
                                    value = t.Usuario.Nombres + " " + t.Usuario.Primer_Apellido + " " + t.Usuario.Segundo_Apellido
                                }).OrderBy(o => o.value).ToList();

                    rp.data = docs;
                    //retorna un response, con el campo data lleno con la respuesta.               
                    return autil.ReturnMesagge(ref rp, 9, null, null, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                //error general
                return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }

        public object GetEntityByMunicipality()
        {
            Response rp = new Response();
            aGenericLists gl = new aGenericLists();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    var en = (from e in ent.Entidad
                              select new
                              {
                                  grup = e.Poblado.Municipio.Nombre,
                                  entities = new
                                  {
                                      id = e.Id_Entidad,
                                      value = e.Nombre
                                  }
                              }).ToList();
                    List<Object> r = new List<object>();
                    foreach (var i in en.Select(t => t.grup).Distinct())
                    {
                        var t = new
                        {
                            group = i,
                            entities = en.Where(y => y.grup == i).Select(s => s.entities).ToList()
                        };
                        r.Add(t);
                    }

                    return rp.data = r;
                }
            }
            catch (Exception ex)
            {
                //error general
                return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }


        public object GetCupsByEspeciality(PatientModel model)
        {
            Response rp = new Response();
            aGenericLists gl = new aGenericLists();
            try
            {
                using (MilenioCloudEntities ent = new MilenioCloudEntities())
                {
                    Guid id = Guid.Parse(model.id);
                    var en = (from e in ent.Especialidad_Cup_Entidad
                              where e.Id_Especialidad == model.Id_Especialidad
                              && e.Id_Entidad == id
                              select new ComboModel
                              {
                                  id = e.Id_Cups,
                                  value = e.Cups.Codigo + "-" + e.Cups.Descripcion
                              }).ToList();


                    return rp.data = en;
                }
            }
            catch (Exception ex)
            {
                //error general
                return autil.ReturnMesagge(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
            }
        }
        #endregion
    }
}