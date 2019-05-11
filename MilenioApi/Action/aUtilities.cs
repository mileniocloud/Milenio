using MilenioApi.DAO;
using MilenioApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MilenioApi.Action
{
    public class aUtilities
    {
        public Response MensajeRetorno(ref GenericError ge)
        {
            Response ret = new Response();
            ret.status = HttpStatusCode.InternalServerError;
            ret.response_code = ge.Codigo;
            ret.message = ge.Message;
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
        public Response MensajeRetorno(ref Response ret, int idmensje, string custom, Guid? id, string rute, HttpStatusCode status = HttpStatusCode.OK)
        {
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                GenericError ge = (from g in ent.GenericError
                                   where g.codigo_id == idmensje
                                   select g).SingleOrDefault();

                ret.status = status;
                ret.response_code = ge.Codigo;
                ret.message = ge.Message + " " + custom;
                ret.rute = rute;
            }

            return ret;
        }
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

        public HttpResponseMessage ReturnResponseApi(HttpResponseMessage o)
        {
            HttpResponseMessage httpResponseMessage = null;
            httpResponseMessage = new HttpResponseMessage(o.StatusCode);

            return httpResponseMessage;
        }

        public List<ErrorFields> ValidateObject(object t)
        {
            List<ErrorFields> rl = new List<ErrorFields>();
            try
            {
                ValidationContext context = new ValidationContext(t, null, null);
                List<ValidationResult> results = new List<ValidationResult>();
                bool valid = Validator.TryValidateObject(t, context, results, true);

                if (!valid)
                {
                    foreach (ValidationResult vr in results)
                    {
                        ErrorFields r = new ErrorFields();
                        // r.field = vr.MemberNames.First();
                        r.message = vr.ErrorMessage;
                        rl.Add(r);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorFields r = new ErrorFields();
                r.message = ex.Message;
                rl.Add(r);
            }
            return rl;
        }

        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ12345678901234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }

            return res.ToString();
        }

        public string Encrypt(string token)
        {
            Byte[] stringBytes = System.Text.Encoding.Unicode.GetBytes(token);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
            //return Convert.ToBase64String(cipherTextBytes);
        }

        public string Decrypt(string encryptedText)
        {
            int numberChars = encryptedText.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(encryptedText.Substring(i, 2), 16);
            }
            return System.Text.Encoding.Unicode.GetString(bytes);
        }
        public bool SendMail(string email, AlternateView emailbody, string Subject)
        {
            try
            {
                var userCredentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SMTPUserName"].ToString(), ConfigurationManager.AppSettings["SMTPPassword"]);

                if (System.Convert.ToBoolean(ConfigurationManager.AppSettings["EmailAlertEnabled"]))
                {
                    SmtpClient smtp = new SmtpClient
                    {
                        Host = Convert.ToString(ConfigurationManager.AppSettings["SMTPHost"]),

                        Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]),

                        EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPEnableSsl"]),

                        DeliveryMethod = SmtpDeliveryMethod.Network,

                        Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPTimeout"]),

                        UseDefaultCredentials = false
                    };

                    smtp.Credentials = userCredentials;

                    MailMessage message = new MailMessage();

                    message.From = new MailAddress(ConfigurationManager.AppSettings["SenderEmailAddress"], ConfigurationManager.AppSettings["SenderDisplayName"]);
                    message.Subject = Subject;
                    message.IsBodyHtml = true;
                    message.AlternateViews.Add(emailbody);
                    message.To.Add(email);

                    smtp.Send(message);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void GetErrorDetail(ref List<ErrorFields> err, int idmensje, string proceso, DateTime fecha, DateTime horadesde, DateTime horahasta, string consultorio, string especialidad)
        {
            using (MilenioCloudEntities ent = new MilenioCloudEntities())
            {
                GenericError ge = (from g in ent.GenericError
                                   where g.codigo_id == idmensje
                                   select g).SingleOrDefault();


                ErrorFields ef = new ErrorFields();
                ef.field = proceso;
                string mensaje = string.Format(ge.Message, fecha.ToShortDateString(), horadesde.ToString("HH:mm"), horahasta.ToString("HH:mm"), consultorio, especialidad);
                ef.message = mensaje;
                err.Add(ef);
            }
        }
    }
}