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
    
    public partial class Ubicacion
    {
        public System.Guid Codigo_Id { get; set; }
        public int Poblado_Id { get; set; }
        public string Direccion { get; set; }
        public Nullable<System.DateTime> Created_At { get; set; }
        public Nullable<System.DateTime> Updated_At { get; set; }
        public Nullable<System.Guid> Usuario_Update { get; set; }
    
        public virtual Poblado Poblado { get; set; }
    }
}
