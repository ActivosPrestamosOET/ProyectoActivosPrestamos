using System.Collections.Generic;
using System.Web.Mvc;

namespace Inventario.Models
{
    public class ModeloInventario
    {
        public Activos_PrestamosOET.Models.PRESTAMO  Prestamos { get; set; }
        public Activos_PrestamosOET.Models.USUARIO Usuarios { get; set; }
        public Activos_PrestamosOET.Models.TIPOS_ACTIVOS Tipos_Activos { get; set; }
        public Activos_PrestamosOET.Models.ACTIVO Activos { get; set; }
    }
}
