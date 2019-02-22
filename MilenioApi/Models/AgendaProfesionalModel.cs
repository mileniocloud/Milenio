using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MilenioApi.Models
{
    public class AgendaProfesionalModel
    {
        public Guid Id_Agenda_Profesional { get; set; }
        public DateTime Fecha_Desde { get; set; }
        public DateTime Fecha_Hasta { get; set; }
        public Guid Id_Consultorio { get; set; }
        public Guid Id_Especialidad { get; set; }
        public Guid Id_Profesional { get; set; }
        public Guid Id_Entidad { get; set; }
        public bool Estado { get; set; }
    }
}