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
    
    public partial class Usuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuario()
        {
            this.Agenda_Profesional = new HashSet<Agenda_Profesional>();
            this.Entidad_Usuario = new HashSet<Entidad_Usuario>();
            this.Especialidad_Profesional = new HashSet<Especialidad_Profesional>();
        }
    
        public System.Guid Id_Usuario { get; set; }
        public string Id_Tipo_Identificacion { get; set; }
        public string Numero_Identificacion { get; set; }
        public string Nombres { get; set; }
        public string Primer_Apellido { get; set; }
        public string Segundo_Apellido { get; set; }
        public string Sexo { get; set; }
        public Nullable<System.DateTime> Fecha_Nacimiento { get; set; }
        public string Foto { get; set; }
        public string Estado_Civil { get; set; }
        public string Tipo_Sangre { get; set; }
        public Nullable<int> Poblado_Id { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public System.DateTime Fecha_Contratacion { get; set; }
        public string Observaciones { get; set; }
        public System.Guid Id_Tipo_Vinculacion { get; set; }
        public bool Presta_Servicio { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool Acepta_ABEAS { get; set; }
        public string Foto_ABEAS { get; set; }
        public Nullable<System.Guid> Id_Tipo_Profesional { get; set; }
        public string Registro_Profesional { get; set; }
        public Nullable<System.Guid> Usuario_Create { get; set; }
        public System.DateTime Fecha_Create { get; set; }
        public Nullable<System.Guid> Usuario_Update { get; set; }
        public System.DateTime Fecha_Update { get; set; }
        public bool isloged { get; set; }
        public string Token_Password_Change { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Agenda_Profesional> Agenda_Profesional { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Entidad_Usuario> Entidad_Usuario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Especialidad_Profesional> Especialidad_Profesional { get; set; }
        public virtual Poblado Poblado { get; set; }
        public virtual Tipo_Profesional Tipo_Profesional { get; set; }
        public virtual Tipo_Vinculacion Tipo_Vinculacion { get; set; }
    }
}
