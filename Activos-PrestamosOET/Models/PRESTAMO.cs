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
    using System.ComponentModel.DataAnnotations;
    public enum Estadito
    {
        //Todos,
        Pendiente,
        Aprobado,
        Denegado,
        Cancelado
    }
    public partial class PRESTAMO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRESTAMO()
        {
            this.EQUIPO_SOLICITADO = new HashSet<EQUIPO_SOLICITADO>();
            this.ACTIVOes = new HashSet<ACTIVO>();
        }

        

            [Display(Name = "Id de la boleta")]
            public string ID { get; set; }

            [Display(Name = "N�mero de boleta")]
            public Nullable<long> NUMERO_BOLETA { get; set; }

            [Display(Name = "Motivo")]
            [Required(ErrorMessage = "Por favor ingrese un motivo v�lido")]
            public string MOTIVO { get; set; }

            [Display(Name = "Fecha de solicitud")]
            public Nullable<System.DateTime> FECHA_SOLICITUD { get; set; }

            [Display(Name = "Fecha de retiro")]
            [Required(ErrorMessage = "Por favor ingrese una rango v�lido")]
            public Nullable<System.DateTime> FECHA_RETIRO { get; set; }

            [Display(Name = "Periodo de uso")]
            [Required(ErrorMessage = "Por favor ingrese una rango v�lido")]
            public int PERIODO_USO { get; set; }

            [Display(Name = "Software requerido")]
            public string SOFTWARE_REQUERIDO { get; set; }

            [Display(Name = "Observaciones del solicitante")]
            public string OBSERVACIONES_SOLICITANTE { get; set; }

            [Display(Name = "Observaciones de qui�n aprueba")]
            public string OBSERVACIONES_APROBADO { get; set; }

            [Display(Name = "Observaciones al recibir el pr�stamo")]
            public string OBSERVACIONES_RECIBIDO { get; set; }

            [Display(Name = "Sigla del Curso")]
            public string SIGLA_CURSO { get; set; }

            [Display(Name = "Estado")]
            public short Estado { get; set; }

            [Display(Name = "C�dula del solicitante")]
            public string CED_SOLICITA { get; set; }

            [Display(Name = "C�dula de qui�n aprueba")]
            public string CED_APRUEBA { get; set; }

    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EQUIPO_SOLICITADO> EQUIPO_SOLICITADO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACTIVO> ACTIVOes { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }
    }
}
