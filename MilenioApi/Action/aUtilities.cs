using MilenioApi.DAO;
using MilenioApi.Models;
using System;
using System.Linq;

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
                ret.Message = ge.Message;
                ret.custom = custom;
                ret.id = id;
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
    }
}