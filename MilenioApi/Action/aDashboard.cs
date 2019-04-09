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
                    Guid usuario = Guid.Parse(cp.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());

                    dm.foto = (ent.Usuario.Where(u=> u.Id_Usuario == usuario).Select(t=> t.Foto)).SingleOrDefault();

                    menu = new MenuModel();
                    menu.name = "Consultorio";
                    menu.url = "office";
                    menu.icon = "ti-clipboard";
                    lmenu.Add(menu);

                    menu = new MenuModel();
                    menu.name = "Usuarios";
                    menu.url = "user";
                    menu.icon = "ti-user";
                    lmenu.Add(menu);

                    menu = new MenuModel();
                    menu.name = "Entidades";
                    menu.url = "entity";
                    menu.icon = "ti-view-list";
                    lmenu.Add(menu);

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