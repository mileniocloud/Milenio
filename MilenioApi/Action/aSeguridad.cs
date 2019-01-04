using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
using WebApi.Jwt;
using System.Collections.Generic;

namespace MilenioApi.Action
{
    public class aSeguridad
    {
        TokenController tk = new TokenController();
        aUtilities autil = new aUtilities();

        #region Manejo Cuentas        
        public Return Login(HttpRequest httpRequest)
        {
            Return ret = new Return();
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {                
                try
                {
                    string login = Convert.ToString(httpRequest.Form["Login"]);
                    string pass = Convert.ToString(httpRequest.Form["Password"]);
                    pass = autil.Sha(pass);

                    Persona p = (from pr in ent.Persona
                                 where pr.Login == login
                                 && pr.Password == pass
                                 select pr).SingleOrDefault();

                    if (p != null)
                    {
                        List<Rol> roles = p.Entidad_Persona_Rol.Select(x => x.Rol).ToList();
                        List<Entidad> entidades = p.Entidad_Persona.Select(y => y.Entidad).ToList();

                        JwtManager.GenerateToken(p.Login, entidades, p.Codigo_Id.ToString(), roles);
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return ret;
        }
        public bool CheckUser(string username, string password)
        {
            // should check in the database
            return true;
        }

       

        #endregion
    }
}