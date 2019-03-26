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
    public class aListasGenericas
    {
        TokenValidationHandler tvh = new TokenValidationHandler();
        ClaimsPrincipal cp = new ClaimsPrincipal();
        aUtilities autil = new aUtilities();

        public PobladoModel GetPoblado(HttpRequest httpRequest)
        {
            PobladoModel pm = new PobladoModel();
            try
            {
                cp = tvh.getprincipal(Convert.ToString(httpRequest.Form["token"]));
                if (cp != null)
                {
                    using (MilenioCloudEntities ent = new MilenioCloudEntities())
                    {
                        //List<Basic> dp = ent.Departamento.Select(l => new ComboModel { id = l.Codigo_Id, value = l.Nombre, codigo = l.Dane_Id.ToString() }).ToList();
                        //List<Basic> mp = ent.Municipio.Select(l => new Basic { codigostring = l.Dane_Id.ToString(), value = l.Nombre, codigo = l.Departamento_Id.ToString() }).ToList();
                        //List<Basic> pb = ent.Poblado.Select(l => new Basic { codigostring = l.Poblado_Id.ToString(), value = l.Nombre, codigo = l.Municipio_Id.ToString() }).ToList();

                        //pm.Departamento.AddRange(dp);
                        //pm.Municipio.AddRange(mp);
                        //pm.Poblado.AddRange(pb);

                        return pm;
                    }
                }
                else
                    return pm;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}