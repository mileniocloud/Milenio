
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
    
public partial class Subcategoria
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Subcategoria()
    {

        this.Articulo = new HashSet<Articulo>();

    }


    public System.Guid Codigo_Id { get; set; }

    public int Referencia { get; set; }

    public string Nombre { get; set; }

    public string Descripcion { get; set; }

    public Nullable<bool> Estado { get; set; }

    public System.Guid Categoria_Id { get; set; }

    public System.DateTime Created_At { get; set; }

    public Nullable<System.DateTime> Updated_At { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Articulo> Articulo { get; set; }

    public virtual Categoria Categoria { get; set; }

}

}
