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

    using Microsoft.AspNet.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    public partial class ACTIVO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ACTIVO()
        {

            this.PRESTAMOes = new HashSet<PRESTAMO>();
            this.TRANSACCIONES = new HashSet<TRANSACCION>();
            
            // Se genera el ID a como lo pide la OET.
            this.ID = DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString("D2") + DateTime.Now.Minute.ToString("D2") + DateTime.Now.Second.ToString("D2") + DateTime.Now.Millisecond.ToString("D3"); /// Se genera el ID con el estandar de la OET.
            // A peticion del grupo de prestamos
            this.ESTADO_PRESTADO = 0;
        }

        public string ID { get; set; }

        [Display(Name = "N�mero de serie")]
        public string NUMERO_SERIE { get; set; }

        [Display(Name = "Fecha de compra")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Fecha de compra es requerido")]
        public System.DateTime FECHA_COMPRA { get; set; }

        [Display(Name = "Inicio de servicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> INICIO_SERVICIO { get; set; }
        [Display(Name = "Fecha de ingreso")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]

        public System.DateTime FECHA_INGRESO { get; set; }
        [Display(Name = "Fabricante")]
        [Required(ErrorMessage = "Fabricante es requerido")]
        public string FABRICANTE { get; set; }
        [Display(Name = "Precio")]
        [DataType(DataType.Currency)]
        [Range(1, int.MaxValue, ErrorMessage = "El precio debe tener un valor positivo")]
        [Required(ErrorMessage = "Precio es requerido")]
        public decimal PRECIO { get; set; }
        [Display(Name = "Descripci�n")]
        [DataType(DataType.MultilineText)]
        [StringLength(256, MinimumLength = 6, ErrorMessage = "El campo tiene que tener menos de 256 caracteres y m�s de 6.")]
        [Required(ErrorMessage = "Es necesario que ingrese una descripci�n del activo")]

        public string DESCRIPCION { get; set; }
        [Display(Name = "Exento de impuestos")]
        public bool EXENTO { get; set; }
        [Display(Name = "Sujeto a pr�stamos")]
        public bool PRESTABLE { get; set; }
        [Display(Name = "Tipo de capital")]
        public bool TIPO_CAPITAL { get; set; }
        [Display(Name = "Ingresado por")]
        public string INGRESADO_POR { get; set; }
        [Display(Name = "N�mero de factura")]
        [Required(ErrorMessage = "N�mero de documento es requerido")]
        public string NUMERO_DOCUMENTO { get; set; }
        [Display(Name = "N�mero de lote")]
        public string NUMERO_LOTE { get; set; }
        [Display(Name = "Tipo de transacci�n")]
        [Required(ErrorMessage = "Tipo de transacci�n es requerido")]
        public int TIPO_TRANSACCIONID { get; set; }
        [Display(Name = "Estado")]
        public int ESTADO_ACTIVOID { get; set; }
        [Display(Name = "Tipo de activo")]
        [Required(ErrorMessage = "Tipo de activo es requerido")]
        public int TIPO_ACTIVOID { get; set; }
        [Display(Name = "Comentarios")]
        [DataType(DataType.MultilineText)]
        public string COMENTARIO { get; set; }
        [Display(Name = "Desechado")]
        public bool DESECHADO { get; set; }
        [Display(Name = "Modelo")]
        public string MODELO { get; set; }
        [Display(Name = "Estaci�n")]
        public string V_ESTACIONID { get; set; }
        [Display(Name = "Compa��a")]
        [Required(ErrorMessage = "Compa��a es requerido")]
        public string V_ANFITRIONAID { get; set; }
        [Display(Name = "Proveedor")]
        [Required(ErrorMessage = "El proveedor es requerido")]
        public string V_PROVEEDORIDPROVEEDOR { get; set; }
        [Display(Name = "Tipo de moneda")]
        [Required(ErrorMessage = "El tipo de moneda es requerido")]
        public string V_MONEDAID { get; set; }
        [Display(Name = "Centro de costo")]
        public Nullable<int> CENTRO_DE_COSTOId { get; set; }
        [Display(Name = "N�mero de placa")]
        [Required(ErrorMessage = "El n�mero de placa es requerido")]
        public string PLACA { get; set; }
        [Display(Name = "Estado del prestamo")]
        public Nullable<int> ESTADO_PRESTADO { get; set; }
        [Display(Name = "Responsable")]
        public string V_EMPLEADOSIDEMPLEADO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRESTAMO> PRESTAMOes { get; set; }
        public virtual TIPOS_ACTIVOS TIPOS_ACTIVOS { get; set; }
        public virtual ESTADOS_ACTIVOS ESTADOS_ACTIVOS { get; set; }
        public virtual V_MONEDA V_MONEDA { get; set; }
        public virtual CENTROS_DE_COSTOS CENTROS_DE_COSTOS { get; set; }
        public virtual V_PROVEEDOR V_PROVEEDOR { get; set; }
        public virtual V_ANFITRIONA V_ANFITRIONA { get; set; }
        public virtual TIPOS_TRANSACCIONES TIPOS_TRANSACCIONES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRANSACCION> TRANSACCIONES { get; set; }
        public virtual V_ESTACION V_ESTACION { get; set; }
        public virtual V_EMPLEADOS V_EMPLEADOS { get; set; }

        /**
         * Metodo que se encarga de generar la descripci�n del activo para que se guarde en la bit�cora.
         * @params: "proveedor" del activo
         * @params: "transaccion" el tipo de transaccion que se esta realizando
         * @params: "anfitriona" que es la organizacion a la que pertenece al activo
         * @return: Un string con la descripci�n completa del activo en el formato que se quiere para guardar en la bit�cora.
         */
        public string descripcion(string proveedor, string transaccion, string anfitriona)
        {
            string esta_exento, capital, prestable;
            esta_exento = this.EXENTO ? "Exento" : "Gravado";
            capital = this.TIPO_CAPITAL ? "Capital mayor" : "Capital menor";
            prestable = this.PRESTABLE ? "Prestable" : "No prestable";

            string atributos = this.DESCRIPCION + "-";
            if (this.NUMERO_SERIE != null) atributos += this.NUMERO_SERIE + "-";
            if (this.MODELO != null) atributos += this.MODELO + "-";
            atributos += this.FABRICANTE + "-";
            if (this.NUMERO_LOTE != null) atributos += this.NUMERO_LOTE + "-";
            atributos += this.NUMERO_DOCUMENTO + "-" + this.PRECIO + "-" + esta_exento + "-" + capital + "-" +
                         proveedor + "-" + this.FECHA_COMPRA.Date + "-";
            if (this.INICIO_SERVICIO != null) atributos += this.INICIO_SERVICIO + "-";
            atributos += prestable + "-";
            if (this.V_ESTACION != null) atributos += this.V_ESTACION.NOMBRE + "-";
            if (this.CENTROS_DE_COSTOS != null) atributos += this.CENTROS_DE_COSTOS.Nombre + "-";
            if (this.V_EMPLEADOS != null) atributos += "Responsable: " + this.V_EMPLEADOS.NOMBRE + "-";
            if (this.V_ESTACION != null) atributos += this.V_ESTACION.NOMBRE + "-";
            atributos += transaccion + "-" + anfitriona + "-";
            if (this.INGRESADO_POR != null) atributos += this.INGRESADO_POR + "-";
            atributos += this.FECHA_INGRESO;
            return atributos;
        }

        /**
         * Metodo que realiza una busqueda en todos los activos.
         * @params: busqueda que es un string con la consulta que realiza el usuario
         * @return: IQueryable que contiene los activos que coinciden con la busqueda.
         */
        public static IQueryable<ACTIVO> busquedaSimple(string busqueda, string estacionID, Boolean isAdmin)
        {
            PrestamosEntities db = new PrestamosEntities();

            var result = from a
                         in db.ACTIVOS
                         select a;
            if (!isAdmin)
            {
                result = result.Where(a => a.V_ESTACIONID == estacionID || a.V_ESTACIONID == null);
                result = result.Where(a => a.DESECHADO == false);
            }

            if (!String.IsNullOrEmpty(busqueda))
            {

                result = result.Where(a => a.ESTADOS_ACTIVOS.NOMBRE.Contains(busqueda)
                                            || a.NUMERO_SERIE.Contains(busqueda)
                                            || a.V_ANFITRIONA.NOMBRE.Contains(busqueda)
                                            || a.V_ESTACION.NOMBRE.Contains(busqueda)
                                            || a.TIPOS_ACTIVOS.NOMBRE.Contains(busqueda)
                                            || a.FABRICANTE.Contains(busqueda)
                                            || a.CENTROS_DE_COSTOS.Nombre.Contains(busqueda)
                                            || a.PLACA.Contains(busqueda));
            }
            return result;
        }


        /** 
         * Metodo que se encarga de realizar la busqueda avanzada en los activos
         * @params: params_busqueda que es un diccionario con los parametros de la busqueda avanzada.
         * @params: activos_previos que es un IQueryable en donde vienen los activos que ya pasaron por una consulta previa
         * @return: IQueryable que contiene los activos que coincidieron con la busqueda que se realizo
         */
        public static IQueryable<ACTIVO> busquedaAvanzada(Dictionary<string, string> params_busqueda, IQueryable<ACTIVO> activos_previos)
        {
            IQueryable<ACTIVO> result = activos_previos;
            if (params_busqueda.Count > 0)
            {
                if (!String.IsNullOrEmpty(params_busqueda["proveedor"]))
                {
                    string proveedor = params_busqueda["proveedor"];
                    result = result.Where(a => a.V_PROVEEDORIDPROVEEDOR.Equals(proveedor));
                }
                if (!String.IsNullOrEmpty(params_busqueda["tipo_activo"]))
                {
                    Int32 id = Convert.ToInt32(params_busqueda["tipo_activo"]);
                    result = result.Where(a => a.TIPO_ACTIVOID.Equals(id));
                }
                if (!String.IsNullOrEmpty(params_busqueda["anfitriona"]))
                {
                    string anfitriona = params_busqueda["anfitriona"];
                    result = result.Where(a => a.V_ANFITRIONAID.Equals(anfitriona));
                }
                if (!String.IsNullOrEmpty(params_busqueda["tipo_transaccion"]))
                {
                    Int32 id = Convert.ToInt32(params_busqueda["tipo_transaccion"]);
                    result = result.Where(a => a.TIPO_TRANSACCIONID.Equals(id));
                }
                if (!String.IsNullOrEmpty(params_busqueda["fecha_antes"]))
                {
                    DateTime fecha = Convert.ToDateTime(params_busqueda["fecha_antes"]);
                    result = result.Where(a => a.FECHA_COMPRA.CompareTo(fecha) < 0);
                }
                if (!String.IsNullOrEmpty(params_busqueda["fecha_despues"]))
                {
                    DateTime fecha = Convert.ToDateTime(params_busqueda["fecha_despues"]);
                    result = result.Where(a => a.FECHA_COMPRA.CompareTo(fecha) > 0);
                }
                if (!String.IsNullOrEmpty(params_busqueda["usuario"]))
                {
                    string usuario = params_busqueda["usuario"];
                    result = result.Where(a => a.INGRESADO_POR.Contains(usuario));
                }
                if (!String.IsNullOrEmpty(params_busqueda["estado_activo"]))
                {
                    Int32 id = Convert.ToInt32(params_busqueda["estado_activo"]);
                    result = result.Where(a => a.ESTADO_ACTIVOID.Equals(id));
                }
                if (!String.IsNullOrEmpty(params_busqueda["estacion"]))
                {
                    string estacion = params_busqueda["estacion"];
                    result = result.Where(a => a.V_ESTACIONID.Equals(estacion));
                }
                if (!String.IsNullOrEmpty(params_busqueda["fabricante"]))
                {
                    string fabricante = params_busqueda["fabricante"];
                    result = result.Where(a => a.FABRICANTE.Equals(fabricante));
                }
            }
            return result;
        }
    }
}