
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
    
public partial class Especialidad_Entidad
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Especialidad_Entidad()
    {

        this.Consultorio_Especialidad = new HashSet<Consultorio_Especialidad>();

        this.Especialidad_Cup_Entidad = new HashSet<Especialidad_Cup_Entidad>();

        this.Especialidad_Profesional = new HashSet<Especialidad_Profesional>();

    }


    public System.Guid Id_Especialidad { get; set; }

    public System.Guid Id_Entidad { get; set; }

    public bool Estado { get; set; }

    public System.Guid Usuario_Create { get; set; }

    public System.DateTime Fecha_Create { get; set; }

    public System.Guid Usuario_Update { get; set; }

    public System.DateTime Fecha_Update { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Consultorio_Especialidad> Consultorio_Especialidad { get; set; }

    public virtual Entidad Entidad { get; set; }

    public virtual Especialidad Especialidad { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Especialidad_Cup_Entidad> Especialidad_Cup_Entidad { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Especialidad_Profesional> Especialidad_Profesional { get; set; }

}

}
