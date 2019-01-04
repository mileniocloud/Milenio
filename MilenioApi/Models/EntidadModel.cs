using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class EntidadModel
    {
        public int Nit { get; set; }
        public  Guid Id { get; set; }
        public string Nombre { get; set; }
        public int CodigoEntidad { get; set; }
        public int CodigoDane { get; set; }
        public DateTime FinicioFiscal { get; set; }
        public DateTime FfinFiscal { get; set; }
        public Guid? EntidadPadre { get; set; }
        public int UbicacionId { get; set; }

        public int PobladoId { get; set; }
        public string Direccion { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Token { get; set; }

    }
}