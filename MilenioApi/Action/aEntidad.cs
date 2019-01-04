using MilenioApi.Controllers;
using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;

namespace MilenioApi.Action
{
    public class aEntidad
    {
        TokenController tk = new TokenController();
        public Return CreateEntidad(HttpRequest httpRequest)
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
                        Guid ep = new Guid();
                        if (!string.IsNullOrEmpty(httpRequest.Form["EntidadPadre"]))
                            ep = Guid.Parse(httpRequest.Form["EntidadPadre"]);

                        var retv = ent.sp_create_entidad(Convert.ToInt32(httpRequest.Form["Nit"]), Convert.ToString(httpRequest.Form["Nombre"]), Convert.ToInt32(httpRequest.Form["CodigoEntidad"]), Convert.ToInt32(httpRequest.Form["CodigoDane"]), Convert.ToDateTime(httpRequest.Form["FinicioFiscal"]), Convert.ToDateTime(httpRequest.Form["FfinFiscal"]), ep, Convert.ToInt32(httpRequest.Form["PobladoId"]), Convert.ToString(httpRequest.Form["Direccion"]), Convert.ToString(httpRequest.Form["Latitud"]), Convert.ToString(httpRequest.Form["Longitud"]), file).FirstOrDefault();
                        string valor = (string)retv;
                        ret = Newtonsoft.Json.JsonConvert.DeserializeObject<Return>(valor);
                    }
                    return ret;
                }
                else
                {
                    ret.Codigo = "00";
                    ret.Message = "Token Invalido";
                    return ret;
                }
            }
            catch (Exception ex)
            {
                ret.Codigo = "99";
                ret.Message = "Error: " + ex.Message;
                return ret;
            }
        }
    }
}