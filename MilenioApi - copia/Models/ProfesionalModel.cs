using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class ProfesionalModel: Basic
    {
        public Guid IdEntidad { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public int NroIdentificacion { get; set; }
        public Guid TipoIdentificacion { get; set; }
        public string RegistroProfesional { get; set; }
        public string Sexo { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public String Foto { get; set; }
        public string EstadoCivil { get; set; }
        public string TipoSangre { get; set; }
        public Guid Ubicacion { get; set; }
        public DateTime FechaContratacion { get; set; }
        public string Observaciones { get; set; }
        public Guid TipoVinculacion { get; set; }
        public Guid IdUsuario { get; set; }

    }
}