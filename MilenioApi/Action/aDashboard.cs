using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;

namespace MilenioApi.Action
{
    public class aDashboard
    {
        aUtilities autil = new aUtilities();
        ClaimsPrincipal cp = new ClaimsPrincipal();
        TokenValidationHandler tvh = new TokenValidationHandler();

        public object Dashboard(DashboardModel model)
        {
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                Response rp = new Response();
                DashboardModel dm = new DashboardModel();
                try
                {
                    List<MenuModel> lmenu = new List<MenuModel>();
                    MenuModel menu;
                    cp = tvh.getprincipal(Convert.ToString(model.token));

                    Guid entidad = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.PrimaryGroupSid).Select(c => c.Value).SingleOrDefault());
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());


                    Usuario us = ent.Usuario.Where(u => u.Id_Usuario == usuario).SingleOrDefault();
                    dm.foto = us.Foto;

                    List<Menu> mn = (from m in ent.Menu
                                     from r in m.Rol
                                     from ru in r.Rol_Usuario
                                     where ru.Id_Usuario == usuario
                                     && ru.Id_Entidad == entidad
                                     select m).Distinct().ToList();

                    foreach (var m in mn)
                    {
                        menu = new MenuModel();
                        menu.name = m.Titulo;
                        menu.url = m.Ruta;
                        menu.icon = m.Estilo;
                        lmenu.Add(menu);
                    }
                    //se adjunta el menu
                    dm.menu.AddRange(lmenu);

                    rp.data = dm;
                }
                catch (Exception ex)
                {
                    //error general
                    return autil.MensajeRetorno(ref rp, 4, string.Empty, null, HttpStatusCode.InternalServerError);
                }

                //retorna un response, con el campo data lleno con la respuesta.               
                return autil.MensajeRetorno(ref rp, 9, null, null, HttpStatusCode.OK);
            }
        }

    }
}