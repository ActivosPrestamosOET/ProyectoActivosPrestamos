//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Activos_PrestamosOET.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PRESTAMO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRESTAMO()
        {
            this.EQUIPO_SOLICITADO = new HashSet<EQUIPO_SOLICITADO>();
            this.ACTIVOes = new HashSet<ACTIVO>();
        }
        public string ID { get; set; }
        public Nullable<decimal> NUMERO_BOLETA { get; set; }
        public string MOTIVO { get; set; }
        public Nullable<System.DateTime> FECHA_SOLICITUD { get; set; }
        public Nullable<System.DateTime> FECHA_RETIRO { get; set; }
        public Nullable<decimal> PERIODO_USO { get; set; }
        public string SOFTWARE_REQUERIDO { get; set; }
        public string OBSERVACIONES_SOLICITANTE { get; set; }
        public string OBSERVACIONES_APROBADO { get; set; }
        public string OBSERVACIONES_RECIBIDO { get; set; }
        public string SIGLA_CURSO { get; set; }
        public short Estado { get; set; }
        public string CED_SOLICITA { get; set; }
        public string CED_APRUEBA { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EQUIPO_SOLICITADO> EQUIPO_SOLICITADO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACTIVO> ACTIVOes { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }
    }
}
