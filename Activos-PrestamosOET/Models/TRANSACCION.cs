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
    public partial class TRANSACCION
    {
        public int ID { get; set; }
        [Display(Name = "Fecha")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Tiene que asignarle una fecha a la transacción.")]
        public System.DateTime FECHA { get; set; }
        [Display(Name = "Responsable")]
        [Required(ErrorMessage = "Tiene que asignarle un responsable a la transacción.")]
        public string RESPONSABLE { get; set; }
        [Display(Name = "Estado")]
        [Required(ErrorMessage = "La transacción tiene que tener el estado del activo.")]
        public string ESTADO { get; set; }
        [Display(Name = "Descripción")]
        [MaxLength(1024, ErrorMessage = "La descripción no puede superar los 1024 caracteres.")]
        [Required(ErrorMessage = "La transacción tiene que tener una descripción.")]
        public string DESCRIPCION { get; set; }
        [Display(Name = "Activo asociado")]
        [Required(ErrorMessage = "La transacción se le tiene que asignar a un activo.")]
        public string ACTIVOID { get; set; }
        public Nullable<long> NUMERO_BOLETA { get; set; }
        public Nullable<System.DateTime> FECHA_RETIRO { get; set; }
        public Nullable<System.DateTime> FECHA_DEVOLUCION { get; set; }
        public string OBSERVACIONES_RECIBO { get; set; }
        public string NOMBRE_SOLICITANTE { get; set; }
        [Display(Name ="Asignado a")]
        public string V_EMPLEADOSIDEMPLEADO { get; set; }
    
        public virtual ACTIVO ACTIVO { get; set; }
        public virtual V_EMPLEADOS V_EMPLEADOS { get; set; }
    }
}
