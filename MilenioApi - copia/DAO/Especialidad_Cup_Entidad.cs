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
    
    public partial class Especialidad_Cup_Entidad
    {
        public System.Guid Id_Especialidad { get; set; }
        public System.Guid Id_Cups { get; set; }
        public System.Guid Id_Entidad { get; set; }
        public bool Estado { get; set; }
        public System.Guid Usuario_Create { get; set; }
        public System.DateTime Fecha_Create { get; set; }
        public System.Guid Usuario_Update { get; set; }
        public System.DateTime Fecha_Update { get; set; }
    
        public virtual Cups Cups { get; set; }
        public virtual Especialidad_Entidad Especialidad_Entidad { get; set; }
    }
}
