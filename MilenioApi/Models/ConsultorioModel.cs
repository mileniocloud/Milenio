using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class ConsultorioModel
    {
        public Guid Id_Consultorio { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
}
