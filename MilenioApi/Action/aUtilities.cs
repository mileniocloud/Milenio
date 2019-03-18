using MilenioApi.DAO;
using MilenioApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace MilenioApi.Action
{
    public class aUtilities
    {
        public Response MensajeRetorno(ref Response ret, int idmensje, string custom, Guid? id, List<ErrorFields> el, HttpStatusCode status = HttpStatusCode.OK)
        {
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                GenericError ge = (from g in ent.GenericError
                                   where g.codigo_id == idmensje
                                   select g).SingleOrDefault();

                ret.status = status;
                ret.response_code = ge.Codigo;
                ret.message = ge.Message + " " + custom;

                ret.error.AddRange(el);
            }

            return ret;
        }

        public Response MensajeRetorno(ref Response ret, int idmensje, string custom, Guid? id, HttpStatusCode status = HttpStatusCode.OK)
        {
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                GenericError ge = (from g in ent.GenericError
                                   where g.codigo_id == idmensje
                                   select g).SingleOrDefault();

                ret.status = status;
                ret.response_code = ge.Codigo;
                ret.message = ge.Message + " " + custom;
            }

            return ret;
        }


        public Basic MensajeRetorno(ref Basic ret, int idmensje, string custom, Guid? id, HttpStatusCode status = HttpStatusCode.OK)
        {
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                GenericError ge = (from g in ent.GenericError
                                   where g.codigo_id == idmensje
                                   select g).SingleOrDefault();

                
                ret.response_code = ge.Codigo;
                ret.message = ge.Message + " " + custom;
            }

            return ret;
        }

        public Basic MensajeRetorno(ref Basic ret, int idmensje, string custom, Guid? id, List<ErrorFields> el, HttpStatusCode status = HttpStatusCode.OK)
        {
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                GenericError ge = (from g in ent.GenericError
                                   where g.codigo_id == idmensje
                                   select g).SingleOrDefault();

                
                ret.response_code = ge.Codigo;
                ret.message = ge.Message + " " + custom;
            }

            return ret;
        }
        public Response MensajeRetorno(ref GenericError ge)
        {
            Response ret = new Response();
            ret.status = HttpStatusCode.InternalServerError;
            ret.response_code = ge.Codigo;
            ret.message = ge.Message;
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

        public List<ErrorFields> ValidateObject(object t)
        {
            List<ErrorFields> rl = new List<ErrorFields>();

            ValidationContext context = new ValidationContext(t, null, null);
            List<ValidationResult> results = new List<ValidationResult>();
            bool valid = Validator.TryValidateObject(t, context, results, true);

            if (!valid)
            {
                foreach (ValidationResult vr in results)
                {
                    ErrorFields r = new ErrorFields();
                    r.field = vr.MemberNames.First();
                    r.message = vr.ErrorMessage;
                    rl.Add(r);
                }
            }
            return rl;
        }
    }
}