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
    
    public partial class Persona
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Persona()
        {
            this.Entidad_Persona_Rol = new HashSet<Entidad_Persona_Rol>();
            this.Imagen = new HashSet<Imagen>();
            this.Telefono = new HashSet<Telefono>();
        }
    
        public System.Guid Codigo_Id { get; set; }
        public int NumeroIdentificacion { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Sexo { get; set; }
        public System.DateTime FNacimiento { get; set; }
        public string Nacionalidad { get; set; }
        public string LibretaMilitar { get; set; }
        public string TipoSangre { get; set; }
        public bool Estado_Persona { get; set; }
        public System.Guid Ubicacion_Id { get; set; }
        public string Foto { get; set; }
        public System.Guid TipoIdentificacion_Id { get; set; }
        public System.DateTime Created_At { get; set; }
        public Nullable<System.DateTime> Updated_At { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public Nullable<bool> Estado_Usuario { get; set; }
        public Nullable<bool> Cambiar_Clave { get; set; }
        public Nullable<System.Guid> Usuario_Update { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Entidad_Persona_Rol> Entidad_Persona_Rol { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Imagen> Imagen { get; set; }
        public virtual TipoIdentificacion TipoIdentificacion { get; set; }
        public virtual Ubicacion Ubicacion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Telefono> Telefono { get; set; }
    }
}