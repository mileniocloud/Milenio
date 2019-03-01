using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class EntidadModel
    {
        public Guid Id_Entidad { get; set; }
        public int Nit { get; set; }
        public string Nombre { get; set; }
        public string Organizacion { get; set; }
        public int Poblado_Id { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string CodigoEntidad { get; set; }
        public bool Atencion_Prioritaria { get; set; }
        public bool Contribuyente { get; set; }
        public string Foto { get; set; }
        public string Hora_Desde { get; set; }
        public string Hora_Hasta { get; set; }


        public int Id_Municipio { get; set; }
        public int Id_Departamento { get; set; }

        public string Response_Code { get; set; }
        public string Message { get; set; }

    }
}