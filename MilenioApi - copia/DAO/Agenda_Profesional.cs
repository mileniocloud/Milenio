//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MilenioApi.DAO
{
    using System;
    using System.Collections.Generic;
    
    public partial class Agenda_Profesional
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Agenda_Profesional()
        {
            this.Horario_Agenda = new HashSet<Horario_Agenda>();
            this.Especialidad_Entidad = new HashSet<Especialidad_Entidad>();
        }
    
        public System.Guid Id_Agenda_Profesional { get; set; }
        public System.DateTime Fecha_Desde { get; set; }
        public System.DateTime Fecha_Hasta { get; set; }
        public System.Guid Id_Profesional { get; set; }
        public System.Guid Id_Entidad { get; set; }
        public bool Estado { get; set; }
        public System.DateTime Fecha_Create { get; set; }
        public System.DateTime Fecha_Update { get; set; }
        public System.Guid Usuario_Create { get; set; }
        public System.Guid Usuario_Update { get; set; }
    
        public virtual Usuario Usuario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Horario_Agenda> Horario_Agenda { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Especialidad_Entidad> Especialidad_Entidad { get; set; }
    }
}