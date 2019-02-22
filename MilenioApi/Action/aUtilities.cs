using MilenioApi.DAO;
using MilenioApi.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace MilenioApi.Action
{
    public class aUtilities
    {
        public Basic MensajeRetorno(ref Basic ret, int idmensje, string custom, Guid? id)
        {
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                GenericError ge = (from g in ent.GenericError
                                   where g.codigo_id == idmensje
                                   select g).SingleOrDefault();

                ret.Codigo = ge.Codigo;
                ret.Message = ge.Message + "-" + custom;
                ret.custom = custom;
                ret.id = id;
            }

            return ret;
        }
        public Basic MensajeRetorno(ref GenericError ge)
        {
            Basic ret = new Basic();
            ret.Codigo = ge.Codigo;
            ret.Message = ge.Message;
            return ret;
        }

        public string Sha(string pass)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(pass);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            String hash = System.Text.Encoding.ASCII.GetString(data);

            return hash;
        }

        /// <summary>
        /// Metodo que convierte la respuesta en un HttpResponseMessage
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>

        public HttpResponseMessage ReturnResponse(object o)
        {
            HttpResponseMessage httpResponseMessage = null;
            StringContent content = null;

            content = new StringContent(JsonConvert.SerializeObject(o, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy" }));
            httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponseMessage.Content = content;
            return httpResponseMessage;
        }
    }
}