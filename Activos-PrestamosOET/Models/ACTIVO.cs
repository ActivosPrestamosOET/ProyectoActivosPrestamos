//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Activos_PrestamosOET.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ACTIVO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ACTIVO()
        {
            this.PRESTAMOes = new HashSet<PRESTAMO>();
        }
    
        public int ID { get; set; }
        public string NUMERO_SERIE { get; set; }
        public System.DateTime FECHA_COMPRA { get; set; }
        public Nullable<System.DateTime> INICIO_SERVICIO { get; set; }
        public System.DateTime FECHA_INGRESO { get; set; }
        public string FABRICANTE { get; set; }
        public int PRECIO { get; set; }
        public string DESCRIPCION { get; set; }
        public bool EXENTO { get; set; }
        public bool PRESTABLE { get; set; }
        public bool TIPO_CAPITAL { get; set; }
        public string INGRESADO_POR { get; set; }
        public string NUMERO_DOCUMENTO { get; set; }
        public string NUMERO_LOTE { get; set; }
        public int TIPO_TRANSACCIONID { get; set; }
        public int ESTADO_ACTIVOID { get; set; }
        public int TIPO_ACTIVOID { get; set; }
        public string ANFITRIONAID { get; set; }
        public string PROVEEDORIDPROVEEDOR { get; set; }
        public string COMENTARIO { get; set; }
        public bool DESECHADO { get; set; }
        public string USUARIOSIDUSUARIO { get; set; }
        public string ESTACIONID { get; set; }
        public Nullable<decimal> DEPARTAMENTOIDDEPARTAMENTO { get; set; }
        public string MODELO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRESTAMO> PRESTAMOes { get; set; }
        public virtual TIPOS_ACTIVOS TIPOS_ACTIVOS { get; set; }
    }
}
