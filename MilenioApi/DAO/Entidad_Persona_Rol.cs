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
    
    public partial class Entidad_Persona_Rol
    {
        public System.Guid Rol_Id { get; set; }
        public System.Guid Persona_Id { get; set; }
        public System.Guid Entidad_Id { get; set; }
        public Nullable<bool> Estado { get; set; }
        public Nullable<System.DateTime> Created_At { get; set; }
        public Nullable<System.DateTime> Updated_At { get; set; }
        public Nullable<System.Guid> Usuario_Update { get; set; }
    
        public virtual Entidad Entidad { get; set; }
        public virtual Persona Persona { get; set; }
        public virtual Rol Rol { get; set; }
    }
}
