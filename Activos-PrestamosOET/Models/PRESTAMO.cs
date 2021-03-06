//------------------------------------------------------------------------------
// <auto-generated>
//     Este c�digo se gener� a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicaci�n.
//     Los cambios manuales en este archivo se sobrescribir�n si se regenera el c�digo.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Activos_PrestamosOET.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Web.Models.Validation;
    public partial class PRESTAMO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRESTAMO()
        {
            this.EQUIPO_SOLICITADO = new HashSet<EQUIPO_SOLICITADO>();
            this.ACTIVOes = new HashSet<ACTIVO>();
        }

        public string ID { get; set; }
        [DisplayName("N�mero de boleta")]
        public Nullable<long> NUMERO_BOLETA { get; set; }
        [DisplayName("Motivo")]
        public string MOTIVO { get; set; }
        [DisplayName("Fecha de solicitud")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime FECHA_SOLICITUD { get; set; }
        [DisplayName("Inicio del pr�stamo")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Required(ErrorMessage = "La fecha de inicio del pr�stamo es requerida")]
        public System.DateTime FECHA_RETIRO { get; set; }
        [DisplayName("Periodo pr�stamo")]
        [Required(ErrorMessage = "El periodo de uso es requerido")]
        public int PERIODO_USO { get; set; }
        [DisplayName("Software requerido")]
        [StringLength(250)]
        public string SOFTWARE_REQUERIDO { get; set; }
        [DisplayName("Observaciones del solicitante")]
        [StringLength(250)]
        public string OBSERVACIONES_SOLICITANTE { get; set; }
        [DisplayName("Observaciones de quien aprueba")]
        [StringLength(250)]
        public string OBSERVACIONES_APROBADO { get; set; }
        [DisplayName("Observaciones al finalizar el pr�stamo")]
        [StringLength(250)]
        public string OBSERVACIONES_RECIBIDO { get; set; }
        [DisplayName("Curso")]
        public string SIGLA_CURSO { get; set; }
        public short Estado { get; set; }
        [DisplayName("Solicitante")]
        public string USUARIO_SOLICITA { get; set; }
        [DisplayName("Quien lo aprueba")]
        public string USUARIO_APRUEBA { get; set; }
        [DisplayName("Curso")]
        public int V_COURSESCOURSES { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EQUIPO_SOLICITADO> EQUIPO_SOLICITADO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACTIVO> ACTIVOes { get; set; }
        public virtual ActivosUser ActivosUser { get; set; }
        public virtual ActivosUser ActivosUser1 { get; set; }
        public virtual V_COURSES V_COURSES { get; set; }
    }
}
