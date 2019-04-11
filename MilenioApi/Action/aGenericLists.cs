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

        #endregion
    }
}